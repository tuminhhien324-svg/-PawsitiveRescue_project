using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VolunteerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<NguoiDung> _passwordHasher;

        public VolunteerController(ApplicationDbContext context, PasswordHasher<NguoiDung> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // POST: api/volunteer-applications
        [HttpPost("volunteer-applications")]
        public async Task<IActionResult> CreateApplication([FromBody] VolunteerApplicationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(ApiResponse.Fail("Dữ liệu không hợp lệ", ModelState));
            }

            var app = new DonDangKyTinhNguyen
            {
                HoTen = model.HoTen,
                Email = model.Email.Trim().ToLower(),
                SoDienThoai = model.SoDienThoai,
                NamSinh = model.NamSinh,
                NgheNghiep = model.NgheNghiep,
                KyNang = model.KyNang,
                TrangThai = "Chờ xử lý",
                NgayTao = DateTime.Now
            };

            _context.DonDangKyTinhNguyens.Add(app);
            await _context.SaveChangesAsync();

            // Log activity
            _context.NhatKyHeThongs.Add(new NhatKyHeThong
            {
                MoTaHoatDong = $"Đăng ký tham gia tình nguyện viên mới: {app.HoTen} ({app.Email})",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            });
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok(null, "Gửi đơn đăng ký tình nguyện viên thành công!"));
        }

        // GET: api/volunteer/admin/volunteers (Admin only)
        [HttpGet("admin/volunteers")]
        public async Task<IActionResult> GetVolunteers([FromQuery] string? trangthai, [FromQuery] string? kynang)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            var query = _context.TinhNguyenViens
                .Include(v => v.NguoiDung)
                .AsQueryable();

            if (!string.IsNullOrEmpty(trangthai))
            {
                query = query.Where(v => v.TrangThai == trangthai);
            }

            if (!string.IsNullOrEmpty(kynang))
            {
                query = query.Where(v => v.KyNangHienTai != null && v.KyNangHienTai.Contains(kynang));
            }

            var list = await query.ToListAsync();
            return Ok(ApiResponse.Ok(list));
        }

        // GET: api/volunteer/admin/non-volunteers (Admin only)
        [HttpGet("admin/non-volunteers")]
        public async Task<IActionResult> GetNonVolunteers()
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            var volunteerUserIds = await _context.TinhNguyenViens.Select(v => v.MaNguoiDung).ToListAsync();
            var nonVolunteers = await _context.NguoiDungs
                .Where(u => !volunteerUserIds.Contains(u.MaNguoiDung))
                .Select(u => new { u.MaNguoiDung, u.HoTen, u.Email })
                .ToListAsync();

            return Ok(ApiResponse.Ok(nonVolunteers));
        }

        // POST: api/volunteer/admin/volunteers (Admin only - promote user to volunteer)
        [HttpPost("admin/volunteers")]
        public async Task<IActionResult> PromoteUserToVolunteer([FromBody] PromoteVolunteerModel model)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            var user = await _context.NguoiDungs.FindAsync(model.UserId);
            if (user == null)
            {
                return Ok(ApiResponse.Fail("Người dùng không tồn tại"));
            }

            var isAlreadyVol = await _context.TinhNguyenViens.AnyAsync(v => v.MaNguoiDung == model.UserId);
            if (isAlreadyVol)
            {
                return Ok(ApiResponse.Fail("Người dùng đã là tình nguyện viên"));
            }

            // Change role in NguoiDung
            var volRole = await _context.VaiTros.FirstOrDefaultAsync(r => r.TenVaiTro == "Tình nguyện viên");
            if (volRole != null)
            {
                user.MaVaiTro = volRole.MaVaiTro;
            }

            var tnv = new TinhNguyenVien
            {
                MaNguoiDung = model.UserId,
                KyNangHienTai = model.KyNang,
                TrangThai = "Đang hoạt động",
                TyLeChuyenCan = 100
            };

            _context.TinhNguyenViens.Add(tnv);

            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            _context.NhatKyHeThongs.Add(new NhatKyHeThong
            {
                MaNguoiDung = adminId,
                MoTaHoatDong = $"Thiết lập người dùng {user.HoTen} (ID: {user.MaNguoiDung}) làm Tình nguyện viên",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            await _context.SaveChangesAsync();
            return Ok(ApiResponse.Ok(tnv, "Thiết lập tình nguyện viên thành công!"));
        }

        // PATCH: api/volunteer/admin/volunteers/{id}/status (Admin only)
        [HttpPatch("admin/volunteers/{id}/status")]
        public async Task<IActionResult> UpdateVolunteerStatus(int id, [FromBody] StatusUpdateModel model)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            var tnv = await _context.TinhNguyenViens.Include(v => v.NguoiDung).FirstOrDefaultAsync(v => v.MaTinhNguyenVien == id);
            if (tnv == null)
            {
                return Ok(ApiResponse.Fail("Tình nguyện viên không tồn tại"));
            }

            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var prevStatus = tnv.TrangThai;
            tnv.TrangThai = model.Status; // Đang hoạt động, Nghỉ phép, Đang đào tạo

            _context.NhatKyHeThongs.Add(new NhatKyHeThong
            {
                MaNguoiDung = adminId,
                MoTaHoatDong = $"Cập nhật trạng thái TNV {tnv.NguoiDung?.HoTen} (ID: {tnv.MaTinhNguyenVien}) từ '{prevStatus}' sang '{model.Status}'",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            await _context.SaveChangesAsync();
            return Ok(ApiResponse.Ok(null, "Cập nhật trạng thái thành công!"));
        }

        // PATCH: api/volunteer/admin/volunteers/{id} (Admin only)
        [HttpPatch("admin/volunteers/{id}")]
        public async Task<IActionResult> UpdateVolunteerDetails(int id, [FromBody] TinhNguyenVien model)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            var tnv = await _context.TinhNguyenViens.FindAsync(id);
            if (tnv == null)
            {
                return Ok(ApiResponse.Fail("Tình nguyện viên không tồn tại"));
            }

            tnv.KyNangHienTai = model.KyNangHienTai;
            tnv.TyLeChuyenCan = model.TyLeChuyenCan;

            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            _context.NhatKyHeThongs.Add(new NhatKyHeThong
            {
                MaNguoiDung = adminId,
                MoTaHoatDong = $"Cập nhật thông tin kỹ năng/chuyên cần TNV ID: {id}",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            await _context.SaveChangesAsync();
            return Ok(ApiResponse.Ok(tnv, "Cập nhật thông tin tình nguyện viên thành công!"));
        }

        // GET: api/volunteer/admin/tasks (Admin only)
        [HttpGet("admin/tasks")]
        public async Task<IActionResult> GetTasks()
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            try
            {
                var tasks = await _context.CongViecTaiTrams
                    .OrderByDescending(t => t.NgayLamViec)
                    .Select(t => new
                    {
                        t.MaCongViec,
                        t.TenCongViec,
                        t.ThoiGianBatDau,
                        t.ThoiGianKetThuc,
                        t.SoLuongTNVYeuCau,
                        t.DaHoanThanh,
                        t.NgayLamViec,
                        DiemDanhCongViecs = t.DiemDanhCongViecs.Select(dd => new
                        {
                            dd.MaChiTietPhanCong,
                            dd.MaCongViec,
                            dd.MaTinhNguyenVien,
                            dd.CoMat,
                            dd.TrangThai,
                            TinhNguyenVien = dd.TinhNguyenVien != null ? new
                            {
                                dd.TinhNguyenVien.MaTinhNguyenVien,
                                NguoiDung = dd.TinhNguyenVien.NguoiDung != null ? new
                                {
                                    dd.TinhNguyenVien.NguoiDung.HoTen,
                                    dd.TinhNguyenVien.NguoiDung.Email,
                                    dd.TinhNguyenVien.NguoiDung.SoDienThoai
                                } : null
                            } : null
                        }).ToList()
                    })
                    .ToListAsync();

                return Ok(ApiResponse.Ok(tasks));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse.Fail($"Lỗi tải danh sách công việc: {ex.Message}"));
            }
        }

        // POST: api/volunteer/admin/tasks (Admin only)
        [HttpPost("admin/tasks")]
        public async Task<IActionResult> CreateTask([FromBody] CongViecTaiTram model)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            _context.CongViecTaiTrams.Add(model);
            await _context.SaveChangesAsync();

            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            _context.NhatKyHeThongs.Add(new NhatKyHeThong
            {
                MaNguoiDung = adminId,
                MoTaHoatDong = $"Tạo công việc tại trạm: {model.TenCongViec} (ID: {model.MaCongViec})",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            });
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok(model, "Tạo công việc thành công!"));
        }

        // PATCH: api/volunteer/admin/tasks/{id} (Admin only)
        [HttpPatch("admin/tasks/{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskUpdateModel model)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            var task = await _context.CongViecTaiTrams.FindAsync(id);
            if (task == null)
            {
                return Ok(ApiResponse.Fail("Không tìm thấy công việc"));
            }

            task.TenCongViec = model.TenCongViec;
            task.ThoiGianBatDau = model.ThoiGianBatDau;
            task.ThoiGianKetThuc = model.ThoiGianKetThuc;
            task.SoLuongTNVYeuCau = model.SoLuongTNVYeuCau;
            task.DaHoanThanh = model.DaHoanThanh;
            task.NgayLamViec = model.NgayLamViec;

            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            _context.NhatKyHeThongs.Add(new NhatKyHeThong
            {
                MaNguoiDung = adminId,
                MoTaHoatDong = $"Cập nhật công việc tại trạm ID: {id}",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            await _context.SaveChangesAsync();
            return Ok(ApiResponse.Ok(task, "Cập nhật công việc thành công!"));
        }

        // POST: api/volunteer/admin/tasks/{id}/assign (Admin only)
        [HttpPost("admin/tasks/{id}/assign")]
        public async Task<IActionResult> AssignVolunteer(int id, [FromBody] TaskAssignModel model)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            var exists = await _context.DiemDanhCongViecs.AnyAsync(dd => dd.MaCongViec == id && dd.MaTinhNguyenVien == model.VolunteerId);
            if (exists)
            {
                return Ok(ApiResponse.Ok(null, "Tình nguyện viên đã được phân công công việc này!"));
            }

            var dd = new DiemDanhCongViec
            {
                MaCongViec = id,
                MaTinhNguyenVien = model.VolunteerId,
                CoMat = true
            };

            _context.DiemDanhCongViecs.Add(dd);

            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            _context.NhatKyHeThongs.Add(new NhatKyHeThong
            {
                MaNguoiDung = adminId,
                MoTaHoatDong = $"Phân công TNV ID {model.VolunteerId} vào công việc trạm ID {id}",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            await _context.SaveChangesAsync();
            return Ok(ApiResponse.Ok(null, "Phân công công việc thành công!"));
        }

        // POST: api/volunteer/admin/tasks/{id}/attendance (Admin only)
        [HttpPost("admin/tasks/{id}/attendance")]
        public async Task<IActionResult> MarkAttendance(int id, [FromBody] TaskAttendanceModel model)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            var dd = await _context.DiemDanhCongViecs.FirstOrDefaultAsync(x => x.MaCongViec == id && x.MaTinhNguyenVien == model.VolunteerId);
            if (dd == null)
            {
                // Create check-in record
                dd = new DiemDanhCongViec
                {
                    MaCongViec = id,
                    MaTinhNguyenVien = model.VolunteerId,
                    CoMat = model.CoMat
                };
                _context.DiemDanhCongViecs.Add(dd);
            }
            else
            {
                dd.CoMat = model.CoMat;
            }

            await _context.SaveChangesAsync();

            // Recompute TyLeChuyenCan for this volunteer: (Number of present check-ins / Number of total check-ins) * 100
            var tnv = await _context.TinhNguyenViens.FindAsync(model.VolunteerId);
            if (tnv != null)
            {
                var totalShifts = await _context.DiemDanhCongViecs.CountAsync(x => x.MaTinhNguyenVien == tnv.MaTinhNguyenVien);
                var presentShifts = await _context.DiemDanhCongViecs.CountAsync(x => x.MaTinhNguyenVien == tnv.MaTinhNguyenVien && x.CoMat);
                if (totalShifts > 0)
                {
                    tnv.TyLeChuyenCan = (int)(((double)presentShifts / totalShifts) * 100);
                }
            }

            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            _context.NhatKyHeThongs.Add(new NhatKyHeThong
            {
                MaNguoiDung = adminId,
                MoTaHoatDong = $"Điểm danh TNV ID {model.VolunteerId} cho công việc ID {id}: {(model.CoMat ? "Có mặt" : "Vắng mặt")}",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            await _context.SaveChangesAsync();
            return Ok(ApiResponse.Ok(null, "Điểm danh thành công!"));
        }

        // GET: api/Volunteer/admin/applications (Admin only)
        [HttpGet("admin/applications")]
        public async Task<IActionResult> GetApplications([FromQuery] string? trangthai)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            var query = _context.DonDangKyTinhNguyens.AsQueryable();

            if (!string.IsNullOrEmpty(trangthai))
            {
                query = query.Where(a => a.TrangThai == trangthai);
            }

            var list = await query.OrderByDescending(a => a.NgayTao).ToListAsync();
            return Ok(ApiResponse.Ok(list));
        }

        // POST: api/Volunteer/admin/applications/{id}/approve (Admin only)
        [HttpPost("admin/applications/{id}/approve")]
        public async Task<IActionResult> ApproveApplication(int id)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            var app = await _context.DonDangKyTinhNguyens.FindAsync(id);
            if (app == null)
            {
                return Ok(ApiResponse.Fail("Đơn đăng ký không tồn tại."));
            }

            if (app.TrangThai != "Chờ xử lý")
            {
                return Ok(ApiResponse.Fail("Đơn đăng ký đã được xử lý rồi."));
            }

            app.TrangThai = "Đã duyệt";

            // Find or create user
            var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.Email == app.Email);
            var volRole = await _context.VaiTros.FirstOrDefaultAsync(r => r.TenVaiTro == "Tình nguyện viên");

            if (user == null)
            {
                user = new NguoiDung
                {
                    HoTen = app.HoTen,
                    Email = app.Email,
                    SoDienThoai = app.SoDienThoai,
                    NamSinh = app.NamSinh,
                    DiaChi = "Chưa cập nhật",
                    DiemTichLuy = 0,
                    TenHang = "Thành viên mới",
                    MaVaiTro = volRole?.MaVaiTro ?? 2,
                    NgayTao = DateTime.Now
                };
                user.MatKhauMaHoa = _passwordHasher.HashPassword(user, "pawsitive123");
                _context.NguoiDungs.Add(user);
                await _context.SaveChangesAsync(); // Generates MaNguoiDung
            }
            else
            {
                // Promote existing user to volunteer role
                user.MaVaiTro = volRole?.MaVaiTro ?? 2;
                if (string.IsNullOrEmpty(user.SoDienThoai)) user.SoDienThoai = app.SoDienThoai;
                if (!user.NamSinh.HasValue) user.NamSinh = app.NamSinh;
            }

            // Create TinhNguyenVien profile if not exists
            var tnv = await _context.TinhNguyenViens.FirstOrDefaultAsync(v => v.MaNguoiDung == user.MaNguoiDung);
            if (tnv == null)
            {
                tnv = new TinhNguyenVien
                {
                    MaNguoiDung = user.MaNguoiDung,
                    KyNangHienTai = app.KyNang,
                    TrangThai = "Đang hoạt động",
                    TyLeChuyenCan = 100
                };
                _context.TinhNguyenViens.Add(tnv);
            }
            else
            {
                tnv.TrangThai = "Đang hoạt động";
                if (!string.IsNullOrEmpty(app.KyNang)) tnv.KyNangHienTai = app.KyNang;
            }

            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            _context.NhatKyHeThongs.Add(new NhatKyHeThong
            {
                MaNguoiDung = adminId,
                MoTaHoatDong = $"Duyệt đơn đăng ký TNV ID {app.MaDonDangKy} cho {app.HoTen}",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            await _context.SaveChangesAsync();
            return Ok(ApiResponse.Ok(null, "Duyệt đơn đăng ký và kích hoạt tình nguyện viên thành công!"));
        }

        // POST: api/Volunteer/admin/applications/{id}/reject (Admin only)
        [HttpPost("admin/applications/{id}/reject")]
        public async Task<IActionResult> RejectApplication(int id)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            var app = await _context.DonDangKyTinhNguyens.FindAsync(id);
            if (app == null)
            {
                return Ok(ApiResponse.Fail("Đơn đăng ký không tồn tại."));
            }

            if (app.TrangThai != "Chờ xử lý")
            {
                return Ok(ApiResponse.Fail("Đơn đăng ký đã được xử lý rồi."));
            }

            app.TrangThai = "Từ chối";

            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            _context.NhatKyHeThongs.Add(new NhatKyHeThong
            {
                MaNguoiDung = adminId,
                MoTaHoatDong = $"Từ chối đơn đăng ký TNV ID {app.MaDonDangKy} của {app.HoTen}",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            await _context.SaveChangesAsync();
            return Ok(ApiResponse.Ok(null, "Đã từ chối đơn đăng ký tình nguyện viên."));
        }

        // GET: api/Volunteer/my-tasks
        [HttpGet("my-tasks")]
        public async Task<IActionResult> GetMyTasks()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Ok(ApiResponse.Fail("Chưa đăng nhập"));
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var tnv = await _context.TinhNguyenViens.FirstOrDefaultAsync(v => v.MaNguoiDung == userId);
            if (tnv == null)
            {
                return Ok(ApiResponse.Fail("Bạn không phải là tình nguyện viên."));
            }

            if (tnv.TrangThai == "Bị khóa")
            {
                return Ok(ApiResponse.Fail("Tài khoản tình nguyện viên của bạn đã bị khóa."));
            }

            var tasks = await _context.DiemDanhCongViecs
                .Where(dd => dd.MaTinhNguyenVien == tnv.MaTinhNguyenVien)
                .Include(dd => dd.CongViecTaiTram)
                .OrderByDescending(dd => dd.CongViecTaiTram!.NgayLamViec)
                .Select(dd => new
                {
                    maChiTietPhanCong = dd.MaChiTietPhanCong,
                    maCongViec = dd.MaCongViec,
                    tenCongViec = dd.CongViecTaiTram!.TenCongViec,
                    ngayLamViec = dd.CongViecTaiTram.NgayLamViec,
                    thoiGianBatDau = dd.CongViecTaiTram.ThoiGianBatDau,
                    thoiGianKetThuc = dd.CongViecTaiTram.ThoiGianKetThuc,
                    trangThai = dd.TrangThai
                })
                .ToListAsync();

            return Ok(ApiResponse.Ok(tasks));
        }

        // POST: api/Volunteer/tasks/{assignmentId}/accept
        [HttpPost("tasks/{assignmentId}/accept")]
        public async Task<IActionResult> AcceptTask(int assignmentId)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Ok(ApiResponse.Fail("Chưa đăng nhập"));
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var tnv = await _context.TinhNguyenViens.FirstOrDefaultAsync(v => v.MaNguoiDung == userId);
            if (tnv == null || tnv.TrangThai == "Bị khóa")
            {
                return Ok(ApiResponse.Fail("Bạn không có quyền thực hiện hành động này."));
            }

            var assignment = await _context.DiemDanhCongViecs.FindAsync(assignmentId);
            if (assignment == null || assignment.MaTinhNguyenVien != tnv.MaTinhNguyenVien)
            {
                return Ok(ApiResponse.Fail("Không tìm thấy thông tin phân công công việc."));
            }

            assignment.TrangThai = "Đã nhận";
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok(null, "Nhận công việc thành công!"));
        }

        // POST: api/Volunteer/tasks/{assignmentId}/complete
        [HttpPost("tasks/{assignmentId}/complete")]
        public async Task<IActionResult> CompleteTask(int assignmentId)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Ok(ApiResponse.Fail("Chưa đăng nhập"));
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var tnv = await _context.TinhNguyenViens.FirstOrDefaultAsync(v => v.MaNguoiDung == userId);
            if (tnv == null || tnv.TrangThai == "Bị khóa")
            {
                return Ok(ApiResponse.Fail("Bạn không có quyền thực hiện hành động này."));
            }

            var assignment = await _context.DiemDanhCongViecs
                .Include(dd => dd.CongViecTaiTram)
                .FirstOrDefaultAsync(dd => dd.MaChiTietPhanCong == assignmentId);
            
            if (assignment == null || assignment.MaTinhNguyenVien != tnv.MaTinhNguyenVien)
            {
                return Ok(ApiResponse.Fail("Không tìm thấy thông tin phân công công việc."));
            }

            assignment.TrangThai = "Hoàn thành";
            assignment.CoMat = true; // Mark present

            // Check if all assigned volunteers for this task are completed
            if (assignment.CongViecTaiTram != null)
            {
                var allAssigned = await _context.DiemDanhCongViecs
                    .Where(dd => dd.MaCongViec == assignment.MaCongViec)
                    .ToListAsync();
                
                // If this is the last one or all are completed
                bool allCompleted = allAssigned.All(dd => dd.MaChiTietPhanCong == assignmentId || dd.TrangThai == "Hoàn thành");
                if (allCompleted)
                {
                    assignment.CongViecTaiTram.DaHoanThanh = true;
                }
            }

            // Recalculate attendance rate
            var totalShifts = await _context.DiemDanhCongViecs.CountAsync(x => x.MaTinhNguyenVien == tnv.MaTinhNguyenVien);
            var presentShifts = await _context.DiemDanhCongViecs.CountAsync(x => x.MaTinhNguyenVien == tnv.MaTinhNguyenVien && x.CoMat);
            if (totalShifts > 0)
            {
                tnv.TyLeChuyenCan = (int)(((double)presentShifts / totalShifts) * 100);
            }

            await _context.SaveChangesAsync();
            return Ok(ApiResponse.Ok(null, "Xác nhận hoàn thành công việc thành công!"));
        }
    }

    public class PromoteVolunteerModel
    {
        public int UserId { get; set; }
        public string? KyNang { get; set; }
    }

    public class TaskAssignModel
    {
        public int VolunteerId { get; set; }
    }

    public class TaskAttendanceModel
    {
        public int VolunteerId { get; set; }
        public bool CoMat { get; set; }
    }

    public class TaskUpdateModel
    {
        public string TenCongViec { get; set; } = string.Empty;
        public TimeSpan ThoiGianBatDau { get; set; }
        public TimeSpan ThoiGianKetThuc { get; set; }
        public int SoLuongTNVYeuCau { get; set; }
        public bool DaHoanThanh { get; set; }
        public DateTime NgayLamViec { get; set; }
    }
}

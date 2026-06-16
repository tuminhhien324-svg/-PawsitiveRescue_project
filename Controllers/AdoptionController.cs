using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdoptionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<NguoiDung> _passwordHasher;

        public AdoptionController(ApplicationDbContext context, PasswordHasher<NguoiDung> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // POST: api/adoption-applications
        [HttpPost]
        public async Task<IActionResult> CreateApplication([FromBody] AdoptionApplicationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(ApiResponse.Fail("Dữ liệu không hợp lệ", ModelState));
            }

            var pet = await _context.ThuCungs.FindAsync(model.MaThuCung);
            if (pet == null)
            {
                return Ok(ApiResponse.Fail("Thú cưng không tồn tại"));
            }

            // Get or create user
            int userId;
            if (User.Identity?.IsAuthenticated == true)
            {
                userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            }
            else
            {
                var email = model.Email.Trim().ToLower();
                var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    var userRole = await _context.VaiTros.FirstOrDefaultAsync(r => r.TenVaiTro == "Người dùng phổ thông");
                    user = new NguoiDung
                    {
                        HoTen = model.HoTen,
                        Email = email,
                        SoDienThoai = model.SoDienThoai,
                        NamSinh = model.NamSinh,
                        DiaChi = model.DiaChi,
                        DiemTichLuy = 0,
                        TenHang = "Thành viên mới",
                        MaVaiTro = userRole?.MaVaiTro ?? 3,
                        NgayTao = DateTime.Now
                    };
                    user.MatKhauMaHoa = _passwordHasher.HashPassword(user, "pawsitive123");
                    _context.NguoiDungs.Add(user);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // Update user info if missing
                    if (string.IsNullOrEmpty(user.SoDienThoai)) user.SoDienThoai = model.SoDienThoai;
                    if (string.IsNullOrEmpty(user.DiaChi)) user.DiaChi = model.DiaChi;
                    if (!user.NamSinh.HasValue) user.NamSinh = model.NamSinh;
                    await _context.SaveChangesAsync();
                }
                userId = user.MaNguoiDung;
            }

            // Create Adoption application
            var app = new DonNhanNuoi
            {
                MaNguoiDung = userId,
                MaThuCung = model.MaThuCung,
                TrangThaiDon = "Đang xét duyệt",
                NgayTao = DateTime.Now,
                NgayCapNhat = DateTime.Now
            };

            _context.DonNhanNuois.Add(app);
            await _context.SaveChangesAsync(); // Generates MaDonNhanNuoi

            // Create Survey record
            var survey = new KhaoSatDieuKienSong
            {
                MaDonNhanNuoi = app.MaDonNhanNuoi,
                KinhNghiemNuoi = model.KinhNghiemNuoi,
                ThoiGianRanhMoiNgay = model.ThoiGianRanhMoiNgay,
                SuDongYCoGiaDinh = model.SuDongYCoGiaDinh,
                LoaiNhaO = model.LoaiNhaO,
                CoSanVuon = model.CoSanVuon,
                CoLuoiBanCong = model.CoLuoiBanCong,
                CoHangRaoCao = model.CoHangRaoCao
            };
            _context.KhaoSatDieuKienSongs.Add(survey);

            // Update pet status
            pet.TrangThaiNhanNuoi = "Đang xử lý";

            // Add status transition log
            _context.LichSuTrangThais.Add(new LichSuTrangThai
            {
                MaThuCung = pet.MaThuCung,
                TrangThaiCuuHoTruoc = pet.TrangThaiCuuHo,
                TrangThaiCuuHoSau = pet.TrangThaiCuuHo,
                TrangThaiNhanNuoiTruoc = "Chưa nhận nuôi",
                TrangThaiNhanNuoiSau = "Đang xử lý",
                NgayCapNhat = DateTime.Now,
                GhiChuThayDoi = $"Gửi đơn nhận nuôi (Đơn ID: {app.MaDonNhanNuoi})",
                NguoiThucHien = userId
            });

            // Log activity
            _context.NhatKyHeThongs.Add(new NhatKyHeThong
            {
                MaNguoiDung = userId,
                MoTaHoatDong = $"Gửi đơn đăng ký nhận nuôi thú cưng {pet.TenThuCung} (Đơn ID: {app.MaDonNhanNuoi})",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok(new { id = app.MaDonNhanNuoi }, "Gửi hồ sơ nhận nuôi thành công!"));
        }

        // GET: api/adoption-applications/me
        [HttpGet("me")]
        public async Task<IActionResult> GetMyApplications()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Ok(ApiResponse.Fail("Chưa đăng nhập"));
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var apps = await _context.DonNhanNuois
                .Where(a => a.MaNguoiDung == userId)
                .Include(a => a.ThuCung)
                .Include(a => a.KhaoSatDieuKienSong)
                .OrderByDescending(a => a.NgayTao)
                .ToListAsync();

            return Ok(ApiResponse.Ok(apps));
        }

        // GET: api/adoption-applications (Admin only)
        [HttpGet("admin")]
        public async Task<IActionResult> GetAdminApplications([FromQuery] string? trangthai, [FromQuery] string? q)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            var query = _context.DonNhanNuois
                .Include(a => a.NguoiDung)
                .Include(a => a.ThuCung)
                .Include(a => a.KhaoSatDieuKienSong)
                .AsQueryable();

            if (!string.IsNullOrEmpty(trangthai))
            {
                query = query.Where(a => a.TrangThaiDon == trangthai);
            }

            if (!string.IsNullOrEmpty(q))
            {
                var search = q.ToLower().Trim();
                query = query.Where(a => a.NguoiDung!.HoTen.ToLower().Contains(search) ||
                                         a.ThuCung!.TenThuCung.ToLower().Contains(search));
            }

            var apps = await query.OrderByDescending(a => a.NgayTao).ToListAsync();
            return Ok(ApiResponse.Ok(apps));
        }

        // GET: api/adoption-applications/{id} (Admin only)
        [HttpGet("admin/{id}")]
        public async Task<IActionResult> GetApplicationDetails(int id)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            var app = await _context.DonNhanNuois
                .Include(a => a.NguoiDung)
                .Include(a => a.ThuCung)
                .Include(a => a.KhaoSatDieuKienSong)
                .FirstOrDefaultAsync(a => a.MaDonNhanNuoi == id);

            if (app == null)
            {
                return Ok(ApiResponse.Fail("Không tìm thấy đơn nhận nuôi"));
            }

            return Ok(ApiResponse.Ok(app));
        }

        // PATCH: api/adoption-applications/{id}/status (Admin only)
        [HttpPatch("admin/{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] StatusUpdateModel model)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            var app = await _context.DonNhanNuois
                .Include(a => a.ThuCung)
                .FirstOrDefaultAsync(a => a.MaDonNhanNuoi == id);

            if (app == null)
            {
                return Ok(ApiResponse.Fail("Không tìm thấy đơn nhận nuôi"));
            }

            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var prevStatus = app.TrangThaiDon;
            app.TrangThaiDon = model.Status; // Đang xét duyệt, Phỏng vấn, Hoàn tất, Từ chối
            app.NgayCapNhat = DateTime.Now;

            // If completed (Hoàn tất)
            if (model.Status == "Hoàn tất" && app.ThuCung != null)
            {
                app.ThuCung.TrangThaiNhanNuoi = "Đã nhận nuôi";
                _context.LichSuTrangThais.Add(new LichSuTrangThai
                {
                    MaThuCung = app.MaThuCung,
                    TrangThaiCuuHoTruoc = app.ThuCung.TrangThaiCuuHo,
                    TrangThaiCuuHoSau = app.ThuCung.TrangThaiCuuHo,
                    TrangThaiNhanNuoiTruoc = "Đang xử lý",
                    TrangThaiNhanNuoiSau = "Đã nhận nuôi",
                    NgayCapNhat = DateTime.Now,
                    GhiChuThayDoi = $"Đơn nhận nuôi ID {app.MaDonNhanNuoi} được duyệt hoàn tất.",
                    NguoiThucHien = adminId
                });

                // Add points to adopter
                var adopter = await _context.NguoiDungs.FindAsync(app.MaNguoiDung);
                if (adopter != null)
                {
                    adopter.CapNhatDiemTichLuy(100);
                }
            }
            else if (model.Status == "Từ chối" && app.ThuCung != null)
            {
                app.ThuCung.TrangThaiNhanNuoi = "Chưa nhận nuôi";
                _context.LichSuTrangThais.Add(new LichSuTrangThai
                {
                    MaThuCung = app.MaThuCung,
                    TrangThaiCuuHoTruoc = app.ThuCung.TrangThaiCuuHo,
                    TrangThaiCuuHoSau = app.ThuCung.TrangThaiCuuHo,
                    TrangThaiNhanNuoiTruoc = "Đang xử lý",
                    TrangThaiNhanNuoiSau = "Chưa nhận nuôi",
                    NgayCapNhat = DateTime.Now,
                    GhiChuThayDoi = $"Đơn nhận nuôi ID {app.MaDonNhanNuoi} bị từ chối.",
                    NguoiThucHien = adminId
                });
            }

            _context.NhatKyHeThongs.Add(new NhatKyHeThong
            {
                MaNguoiDung = adminId,
                MoTaHoatDong = $"Cập nhật trạng thái đơn nhận nuôi ID: {app.MaDonNhanNuoi} từ '{prevStatus}' sang '{model.Status}'",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok(null, "Cập nhật trạng thái đơn thành công!"));
        }

        // PATCH: api/adoption-applications/{id}/interview (Admin only)
        [HttpPatch("admin/{id}/interview")]
        public async Task<IActionResult> ScheduleInterview(int id, [FromBody] InterviewScheduleModel model)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            var app = await _context.DonNhanNuois.FindAsync(id);
            if (app == null)
            {
                return Ok(ApiResponse.Fail("Không tìm thấy đơn nhận nuôi"));
            }

            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            app.NgayHenPhongVan = model.InterviewDate;
            app.TrangThaiDon = "Phỏng vấn";
            app.NgayCapNhat = DateTime.Now;

            _context.NhatKyHeThongs.Add(new NhatKyHeThong
            {
                MaNguoiDung = adminId,
                MoTaHoatDong = $"Đặt lịch hẹn phỏng vấn cho đơn ID {app.MaDonNhanNuoi} vào {model.InterviewDate}",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok(null, "Đặt lịch hẹn phỏng vấn thành công!"));
        }
    }

    public class StatusUpdateModel
    {
        public string Status { get; set; } = string.Empty;
    }

    public class InterviewScheduleModel
    {
        public DateTime InterviewDate { get; set; }
    }
}

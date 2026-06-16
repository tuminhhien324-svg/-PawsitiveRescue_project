using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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
    public class RescueController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RescueController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/rescue-reports
        [HttpPost("rescue-reports")]
        public async Task<IActionResult> CreateReport([FromBody] RescueReportViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(ApiResponse.Fail("Dữ liệu không hợp lệ", ModelState));
            }

            var report = new BaoCaoCuuHo
            {
                TenNguoiBaoCao = model.TenNguoiBaoCao ?? "Vô Danh",
                SoDienThoai = model.SoDienThoai,
                DiaDiem = model.DiaDiem,
                QuanHuyen = model.QuanHuyen,
                MoTaChiTiet = model.MoTaChiTiet,
                MucDoKhanCap = model.MucDoKhanCap,
                AnhBaoCao = model.AnhBaoCao,
                TrangThai = "Chờ xử lý",
                NgayBaoCao = DateTime.Now
            };

            _context.BaoCaoCuHos.Add(report);
            await _context.SaveChangesAsync();

            // Log activity
            int? userId = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            }
            _context.NhatKyHeThongs.Add(new NhatKyHeThong
            {
                MaNguoiDung = userId,
                MoTaHoatDong = $"Gửi báo cáo SOS cứu hộ mới tại: {model.DiaDiem} (Báo cáo ID: SOS-{report.MaBaoCao})",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            });
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok(new { id = report.MaBaoCao, code = $"SOS-{report.MaBaoCao}" }, "Gửi báo cáo SOS thành công!"));
        }

        // GET: api/rescue/admin/rescue-reports
        [HttpGet("admin/rescue-reports")]
        public async Task<IActionResult> GetAdminReports([FromQuery] string? trangthai, [FromQuery] string? khancep, [FromQuery] string? quanhuyen)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            var query = _context.BaoCaoCuHos
                .Include(r => r.PhanCongCuuHos)
                    .ThenInclude(pc => pc.TinhNguyenVien)
                        .ThenInclude(tnv => tnv!.NguoiDung)
                .AsQueryable();

            if (!string.IsNullOrEmpty(trangthai))
            {
                query = query.Where(r => r.TrangThai == trangthai);
            }

            if (!string.IsNullOrEmpty(khancep))
            {
                query = query.Where(r => r.MucDoKhanCap == khancep);
            }

            if (!string.IsNullOrEmpty(quanhuyen))
            {
                query = query.Where(r => r.QuanHuyen == quanhuyen);
            }

            var reports = await query.OrderByDescending(r => r.NgayBaoCao).ToListAsync();
            return Ok(ApiResponse.Ok(reports));
        }

        // GET: api/rescue/admin/rescue-reports/{id}
        [HttpGet("admin/rescue-reports/{id}")]
        public async Task<IActionResult> GetReportDetails(int id)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            var report = await _context.BaoCaoCuHos
                .Include(r => r.PhanCongCuuHos)
                    .ThenInclude(pc => pc.TinhNguyenVien)
                        .ThenInclude(tnv => tnv!.NguoiDung)
                .FirstOrDefaultAsync(r => r.MaBaoCao == id);

            if (report == null)
            {
                return Ok(ApiResponse.Fail("Không tìm thấy báo cáo SOS"));
            }

            return Ok(ApiResponse.Ok(report));
        }

        // PATCH: api/rescue/admin/rescue-reports/{id}/status
        [HttpPatch("admin/rescue-reports/{id}/status")]
        public async Task<IActionResult> UpdateReportStatus(int id, [FromBody] StatusUpdateModel model)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            var report = await _context.BaoCaoCuHos.FindAsync(id);
            if (report == null)
            {
                return Ok(ApiResponse.Fail("Không tìm thấy báo cáo SOS"));
            }

            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var oldStatus = report.TrangThai;
            report.TrangThai = model.Status; // Chờ xử lý, Đang thực hiện, Đã hoàn thành

            _context.NhatKyHeThongs.Add(new NhatKyHeThong
            {
                MaNguoiDung = adminId,
                MoTaHoatDong = $"Cập nhật trạng thái báo cáo cứu hộ ID: {report.MaBaoCao} từ '{oldStatus}' sang '{model.Status}'",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            await _context.SaveChangesAsync();
            return Ok(ApiResponse.Ok(null, "Cập nhật trạng thái thành công!"));
        }

        // POST: api/rescue/admin/rescue-reports/{id}/assignments
        [HttpPost("admin/rescue-reports/{id}/assignments")]
        public async Task<IActionResult> AddAssignment(int id, [FromBody] AssignmentModel model)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            var report = await _context.BaoCaoCuHos.FindAsync(id);
            if (report == null)
            {
                return Ok(ApiResponse.Fail("Không tìm thấy báo cáo SOS"));
            }

            var tnv = await _context.TinhNguyenViens.FindAsync(model.VolunteerId);
            if (tnv == null)
            {
                return Ok(ApiResponse.Fail("Tình nguyện viên không tồn tại"));
            }

            var exists = await _context.PhanCongCuHos.AnyAsync(pc => pc.MaBaoCao == id && pc.MaTinhNguyenVien == model.VolunteerId);
            if (exists)
            {
                return Ok(ApiResponse.Ok(null, "Tình nguyện viên đã được phân công cho ca cứu hộ này!"));
            }

            var assignment = new PhanCongCuuHo
            {
                MaBaoCao = id,
                MaTinhNguyenVien = model.VolunteerId,
                GhiChu = model.GhiChu,
                NgayPhanCong = DateTime.Now
            };

            _context.PhanCongCuHos.Add(assignment);

            // Auto change status to "Đã phân công"
            if (report.TrangThai == "Chờ xử lý")
            {
                report.TrangThai = "Đã phân công";
            }

            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            _context.NhatKyHeThongs.Add(new NhatKyHeThong
            {
                MaNguoiDung = adminId,
                MoTaHoatDong = $"Phân công TNV (ID: {model.VolunteerId}) cho ca cứu hộ ID: {id}",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            await _context.SaveChangesAsync();
            return Ok(ApiResponse.Ok(null, "Phân công tình nguyện viên thành công!"));
        }

        // DELETE: api/rescue/admin/rescue-reports/{id}/assignments/{volunteerId}
        [HttpDelete("admin/rescue-reports/{id}/assignments/{volunteerId}")]
        public async Task<IActionResult> RemoveAssignment(int id, int volunteerId)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            var assignment = await _context.PhanCongCuHos.FirstOrDefaultAsync(pc => pc.MaBaoCao == id && pc.MaTinhNguyenVien == volunteerId);
            if (assignment == null)
            {
                return Ok(ApiResponse.Fail("Không tìm thấy thông tin phân công"));
            }

            _context.PhanCongCuHos.Remove(assignment);

            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            _context.NhatKyHeThongs.Add(new NhatKyHeThong
            {
                MaNguoiDung = adminId,
                MoTaHoatDong = $"Hủy phân công TNV (ID: {volunteerId}) cho ca cứu hộ ID: {id}",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            await _context.SaveChangesAsync();
            return Ok(ApiResponse.Ok(null, "Hủy phân công thành công!"));
        }

        // GET: api/Rescue/rescue-reports/track/{id}
        [HttpGet("rescue-reports/track/{id}")]
        public async Task<IActionResult> TrackReport(int id)
        {
            var report = await _context.BaoCaoCuHos
                .Select(r => new {
                    r.MaBaoCao,
                    r.TrangThai,
                    r.NgayBaoCao,
                    r.MucDoKhanCap,
                    r.DiaDiem,
                    r.AnhBaoCao
                })
                .FirstOrDefaultAsync(r => r.MaBaoCao == id);

            if (report == null)
            {
                return Ok(ApiResponse.Fail("Không tìm thấy báo cáo SOS"));
            }

            return Ok(ApiResponse.Ok(report));
        }

        // GET: api/Rescue/my-rescues
        [HttpGet("my-rescues")]
        public async Task<IActionResult> GetMyRescues()
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

            var rescues = await _context.PhanCongCuHos
                .Where(pc => pc.MaTinhNguyenVien == tnv.MaTinhNguyenVien)
                .Include(pc => pc.BaoCaoCuuHo)
                .OrderByDescending(pc => pc.NgayPhanCong)
                .Select(pc => new
                {
                    maPhanCong = pc.MaPhanCong,
                    maBaoCao = pc.MaBaoCao,
                    code = "SOS-" + pc.MaBaoCao,
                    tenNguoiBaoCao = pc.BaoCaoCuuHo!.TenNguoiBaoCao,
                    soDienThoai = pc.BaoCaoCuuHo.SoDienThoai,
                    diaDiem = pc.BaoCaoCuuHo.DiaDiem,
                    moTaChiTiet = pc.BaoCaoCuuHo.MoTaChiTiet,
                    mucDoKhanCap = pc.BaoCaoCuuHo.MucDoKhanCap,
                    trangThai = pc.BaoCaoCuuHo.TrangThai,
                    ngayPhanCong = pc.NgayPhanCong,
                    ghiChu = pc.GhiChu
                })
                .ToListAsync();

            return Ok(ApiResponse.Ok(rescues));
        }

        // POST: api/Rescue/my-rescues/{assignmentId}/accept
        [HttpPost("my-rescues/{assignmentId}/accept")]
        public async Task<IActionResult> AcceptRescue(int assignmentId)
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

            var assignment = await _context.PhanCongCuHos
                .Include(pc => pc.BaoCaoCuuHo)
                .FirstOrDefaultAsync(pc => pc.MaPhanCong == assignmentId);

            if (assignment == null || assignment.MaTinhNguyenVien != tnv.MaTinhNguyenVien)
            {
                return Ok(ApiResponse.Fail("Không tìm thấy thông tin phân công cứu hộ."));
            }

            if (assignment.BaoCaoCuuHo != null)
            {
                assignment.BaoCaoCuuHo.TrangThai = "Đang thực hiện";
            }

            _context.NhatKyHeThongs.Add(new NhatKyHeThong
            {
                MaNguoiDung = userId,
                MoTaHoatDong = $"Tình nguyện viên chấp nhận ca cứu hộ SOS-{assignment.MaBaoCao}",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            await _context.SaveChangesAsync();
            return Ok(ApiResponse.Ok(null, "Chấp nhận ca cứu hộ thành công!"));
        }

        // POST: api/Rescue/my-rescues/{assignmentId}/complete
        [HttpPost("my-rescues/{assignmentId}/complete")]
        public async Task<IActionResult> CompleteRescue(int assignmentId)
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

            var assignment = await _context.PhanCongCuHos
                .Include(pc => pc.BaoCaoCuuHo)
                .FirstOrDefaultAsync(pc => pc.MaPhanCong == assignmentId);

            if (assignment == null || assignment.MaTinhNguyenVien != tnv.MaTinhNguyenVien)
            {
                return Ok(ApiResponse.Fail("Không tìm thấy thông tin phân công cứu hộ."));
            }

            if (assignment.BaoCaoCuuHo != null)
            {
                assignment.BaoCaoCuuHo.TrangThai = "Đã hoàn thành";
            }

            _context.NhatKyHeThongs.Add(new NhatKyHeThong
            {
                MaNguoiDung = userId,
                MoTaHoatDong = $"Tình nguyện viên báo cáo hoàn thành ca cứu hộ SOS-{assignment.MaBaoCao}",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            await _context.SaveChangesAsync();
            return Ok(ApiResponse.Ok(null, "Báo cáo hoàn thành ca cứu hộ thành công!"));
        }
    }

    public class AssignmentModel
    {
        public int VolunteerId { get; set; }
        public string? GhiChu { get; set; }
    }
}

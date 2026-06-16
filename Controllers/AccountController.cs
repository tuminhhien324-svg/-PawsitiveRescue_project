using Microsoft.AspNetCore.Authorization;
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
    [Route("api/users/me")]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/users/me/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Ok(ApiResponse.Fail("Chưa đăng nhập"));
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = await _context.NguoiDungs
                .Include(u => u.VaiTro)
                .FirstOrDefaultAsync(u => u.MaNguoiDung == userId);

            if (user == null)
            {
                return Ok(ApiResponse.Fail("Người dùng không tồn tại"));
            }

            string clientRole = "user";
            if (user.VaiTro?.TenVaiTro == "Quản trị viên") clientRole = "admin";
            else if (user.VaiTro?.TenVaiTro == "Tình nguyện viên")
            {
                var tnv = await _context.TinhNguyenViens.FirstOrDefaultAsync(v => v.MaNguoiDung == user.MaNguoiDung);
                if (tnv != null && tnv.TrangThai == "Bị khóa")
                {
                    clientRole = "user";
                }
                else
                {
                    clientRole = "volunteer";
                }
            }

            var profile = new
            {
                maNguoiDung = user.MaNguoiDung,
                hoTen = user.HoTen,
                soDienThoai = user.SoDienThoai,
                email = user.Email,
                namSinh = user.NamSinh,
                diaChi = user.DiaChi,
                diemTichLuy = user.DiemTichLuy,
                tenHang = user.TenHang,
                ngayTao = user.NgayTao,
                role = clientRole
            };

            return Ok(ApiResponse.Ok(profile));
        }

        // PUT: api/users/me/profile
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] ProfileUpdateModel model)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Ok(ApiResponse.Fail("Chưa đăng nhập"));
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = await _context.NguoiDungs.FindAsync(userId);

            if (user == null)
            {
                return Ok(ApiResponse.Fail("Người dùng không tồn tại"));
            }

            user.HoTen = model.HoTen;
            user.SoDienThoai = model.SoDienThoai;
            user.NamSinh = model.NamSinh;
            user.DiaChi = model.DiaChi;

            _context.NhatKyHeThongs.Add(new NhatKyHeThong
            {
                MaNguoiDung = userId,
                MoTaHoatDong = "Cập nhật thông tin cá nhân",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            await _context.SaveChangesAsync();
            return Ok(ApiResponse.Ok(user, "Cập nhật thông tin cá nhân thành công!"));
        }

        // GET: api/users/me/activity
        [HttpGet("activity")]
        public async Task<IActionResult> GetActivity()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Ok(ApiResponse.Fail("Chưa đăng nhập"));
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var donations = await _context.QuyenGops
                .Where(d => d.MaNguoiDung == userId)
                .OrderByDescending(d => d.NgayQuyenGop)
                .Take(5)
                .ToListAsync();

            var adoptions = await _context.DonNhanNuois
                .Where(a => a.MaNguoiDung == userId)
                .Include(a => a.ThuCung)
                .OrderByDescending(a => a.NgayTao)
                .Take(5)
                .ToListAsync();

            var favorites = await _context.ThuCungYeuThichs
                .Where(f => f.MaNguoiDung == userId)
                .Include(f => f.ThuCung)
                .Select(f => f.ThuCung)
                .Take(5)
                .ToListAsync();

            var activity = new
            {
                donations = donations,
                adoptions = adoptions,
                favorites = favorites
            };

            return Ok(ApiResponse.Ok(activity));
        }

        // GET: api/users/me/dashboard
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Ok(ApiResponse.Fail("Chưa đăng nhập"));
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var user = await _context.NguoiDungs
                .Include(u => u.VaiTro)
                .FirstOrDefaultAsync(u => u.MaNguoiDung == userId);

            if (user == null)
            {
                return Ok(ApiResponse.Fail("Người dùng không tồn tại"));
            }

            string clientRole = "user";
            if (user.VaiTro?.TenVaiTro == "Quản trị viên") clientRole = "admin";
            else if (user.VaiTro?.TenVaiTro == "Tình nguyện viên")
            {
                var tnv = await _context.TinhNguyenViens.FirstOrDefaultAsync(v => v.MaNguoiDung == user.MaNguoiDung);
                if (tnv == null || tnv.TrangThai == "Bị khóa")
                {
                    clientRole = "user";
                }
                else
                {
                    clientRole = "volunteer";
                }
            }

            var profile = new
            {
                maNguoiDung = user.MaNguoiDung,
                hoTen = user.HoTen,
                soDienThoai = user.SoDienThoai,
                email = user.Email,
                namSinh = user.NamSinh,
                diaChi = user.DiaChi,
                diemTichLuy = user.DiemTichLuy,
                tenHang = user.TenHang,
                ngayTao = user.NgayTao,
                role = clientRole
            };

            var donations = await _context.QuyenGops
                .Where(d => d.MaNguoiDung == userId)
                .OrderByDescending(d => d.NgayQuyenGop)
                .ToListAsync();

            var adoptions = await _context.DonNhanNuois
                .Where(a => a.MaNguoiDung == userId)
                .Include(a => a.ThuCung)
                .OrderByDescending(a => a.NgayTao)
                .ToListAsync();

            var favorites = await _context.ThuCungYeuThichs
                .Where(f => f.MaNguoiDung == userId)
                .Include(f => f.ThuCung)
                .Select(f => f.ThuCung)
                .ToListAsync();

            var dashboardData = new
            {
                profile = profile,
                donations = donations,
                adoptions = adoptions,
                favorites = favorites
            };

            return Ok(ApiResponse.Ok(dashboardData));
        }

        // ==========================================
        // Razor View Page Actions
        // ==========================================

        [HttpGet("/Account/Login")]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet("/Account/Profile")]
        [AllowAnonymous]
        public IActionResult Profile()
        {
            return View();
        }

        [HttpGet("/Account/AccessDenied")]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            TempData["ErrorMessage"] = "Bạn không có quyền truy cập trang quản trị!";
            return RedirectToAction("Profile");
        }

        [HttpGet("/Account/ForgotPassword")]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }
    }

    public class ProfileUpdateModel
    {
        public string HoTen { get; set; } = string.Empty;
        public string? SoDienThoai { get; set; }
        public int? NamSinh { get; set; }
        public string? DiaChi { get; set; }
    }
}

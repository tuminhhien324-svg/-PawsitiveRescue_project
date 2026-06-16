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
    [Authorize(Roles = "Quản trị viên")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // Razor View Page Actions
        // ==========================================

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ManageAnimals()
        {
            return View();
        }

        public IActionResult ManageAdopters()
        {
            return View();
        }

        public IActionResult ManageDonations()
        {
            return View();
        }

        public IActionResult ManageRescues()
        {
            return View();
        }

        public IActionResult ManageVolunteers()
        {
            return View();
        }

        // Keep aliases so existing links in views don't break
        public IActionResult Pets() => RedirectToAction("ManageAnimals");
        public IActionResult Adopters() => RedirectToAction("ManageAdopters");
        public IActionResult Donations() => RedirectToAction("ManageDonations");

        // ==========================================
        // Dashboard Stats API Endpoints (JSON)
        // ==========================================

        [HttpGet("/api/admin/dashboard/overview")]
        [AllowAnonymous] // Allow AJAX widgets to call if route guard handled in frontend
        public async Task<IActionResult> GetOverview()
        {
            var totalPets = await _context.ThuCungs.CountAsync();
            var totalAdoptions = await _context.DonNhanNuois.CountAsync();
            var totalRescues = await _context.BaoCaoCuHos.CountAsync();
            var totalVolunteers = await _context.TinhNguyenViens.CountAsync();
            var totalDonations = await _context.QuyenGops.SumAsync(d => d.SoTien);
            var completedTasks = await _context.CongViecTaiTrams.CountAsync(t => t.DaHoanThanh);
            var medicalCost = await _context.ChiPhiCuHos.SumAsync(c => c.SoTien);
            var checkups = await _context.LichSuSucKhoes.CountAsync();

            var overview = new
            {
                tongThuCung = totalPets,
                tongDonNhanNuoi = totalAdoptions,
                tongBaoCaoCuuHo = totalRescues,
                tongTinhNguyenVien = totalVolunteers,
                tongTienQuyenGop = totalDonations,
                soCongViecHoanThanh = completedTasks,
                tongChiPhiYTe = medicalCost,
                tongLuotKhamBenh = checkups
            };

            return Ok(ApiResponse.Ok(overview));
        }

        [HttpGet("/api/admin/dashboard/adoptions-by-month")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAdoptionsByMonth([FromQuery] int year = 2026)
        {
            var adoptions = await _context.DonNhanNuois
                .Where(d => d.TrangThaiDon == "Hoàn tất" && d.NgayCapNhat.Year == year)
                .GroupBy(d => d.NgayCapNhat.Month)
                .Select(g => new
                {
                    thang = g.Key,
                    soLuong = g.Count()
                })
                .ToListAsync();

            // Populate all 12 months
            var fullYear = Enumerable.Range(1, 12).Select(m => new
            {
                thang = m,
                soLuong = adoptions.FirstOrDefault(a => a.thang == m)?.soLuong ?? 0
            }).ToList();

            return Ok(ApiResponse.Ok(fullYear));
        }

        [HttpGet("/api/admin/dashboard/donations-by-month")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDonationsByMonth([FromQuery] int year = 2026)
        {
            var donations = await _context.QuyenGops
                .Where(d => d.NgayQuyenGop.Year == year)
                .GroupBy(d => d.NgayQuyenGop.Month)
                .Select(g => new
                {
                    thang = g.Key,
                    soTien = g.Sum(d => d.SoTien)
                })
                .ToListAsync();

            var fullYear = Enumerable.Range(1, 12).Select(m => new
            {
                thang = m,
                soTien = donations.FirstOrDefault(d => d.thang == m)?.soTien ?? 0
            }).ToList();

            return Ok(ApiResponse.Ok(fullYear));
        }

        [HttpGet("/api/admin/dashboard/rescue-ratio")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRescueRatio()
        {
            var total = await _context.BaoCaoCuHos.CountAsync();
            if (total == 0)
            {
                return Ok(ApiResponse.Ok(new { choXuLy = 0, dangThucHien = 0, daHoanThanh = 0 }));
            }

            var pending = await _context.BaoCaoCuHos.CountAsync(r => r.TrangThai == "Chờ xử lý");
            var ongoing = await _context.BaoCaoCuHos.CountAsync(r => r.TrangThai == "Đang thực hiện");
            var completed = await _context.BaoCaoCuHos.CountAsync(r => r.TrangThai == "Đã hoàn thành");

            var ratio = new
            {
                choXuLy = Math.Round((double)pending / total * 100, 2),
                dangThucHien = Math.Round((double)ongoing / total * 100, 2),
                daHoanThanh = Math.Round((double)completed / total * 100, 2)
            };

            return Ok(ApiResponse.Ok(ratio));
        }

        [HttpGet("/api/admin/dashboard/top-breeds")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTopBreeds()
        {
            var top = await _context.DonNhanNuois
                .Where(d => d.TrangThaiDon == "Hoàn tất")
                .Include(d => d.ThuCung)
                .GroupBy(d => d.ThuCung!.GiongThuCung)
                .Select(g => new
                {
                    giong = g.Key ?? "Khác",
                    soLuong = g.Count()
                })
                .OrderByDescending(x => x.soLuong)
                .Take(5)
                .ToListAsync();

            return Ok(ApiResponse.Ok(top));
        }

        [HttpGet("/api/admin/dashboard/top-donors")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTopDonors()
        {
            var donors = await _context.QuyenGops
                .GroupBy(d => d.TenNguoiQuyenGop)
                .Select(g => new
                {
                    ten = g.Key,
                    tongTien = g.Sum(d => d.SoTien),
                    soLuot = g.Count()
                })
                .OrderByDescending(x => x.tongTien)
                .Take(10)
                .ToListAsync();

            return Ok(ApiResponse.Ok(donors));
        }

        [HttpGet("/api/admin/dashboard/top-volunteers")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTopVolunteers()
        {
            var volunteers = await _context.TinhNguyenViens
                .Where(v => v.TrangThai == "Đang hoạt động")
                .Include(v => v.NguoiDung)
                .Select(v => new
                {
                    ten = v.NguoiDung!.HoTen,
                    kyNang = v.KyNangHienTai,
                    chuyenCan = v.TyLeChuyenCan
                })
                .OrderByDescending(v => v.chuyenCan)
                .Take(10)
                .ToListAsync();

            return Ok(ApiResponse.Ok(volunteers));
        }

        [HttpGet("/api/admin/audit-logs")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAuditLogs()
        {
            var logs = await _context.NhatKyHeThongs
                .Include(l => l.NguoiDung)
                .OrderByDescending(l => l.NgayGhiNhan)
                .Select(l => new
                {
                    maNhatKy = l.MaNhatKy,
                    hoTen = l.NguoiDung != null ? l.NguoiDung.HoTen : "Hệ thống / Khách",
                    moTaHoatDong = l.MoTaHoatDong,
                    diaChiIP = l.DiaChiIP,
                    ngayGhiNhan = l.NgayGhiNhan
                })
                .ToListAsync();

            return Ok(ApiResponse.Ok(logs));
        }
    }
}

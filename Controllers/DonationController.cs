using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DonationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DonationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/donations
        [HttpPost]
        public async Task<IActionResult> CreateDonation([FromBody] DonationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(ApiResponse.Fail("Dữ liệu không hợp lệ", ModelState));
            }

            int? userId = null;
            string donorName = model.TenNguoiQuyenGop ?? "Ẩn danh";
            if (User.Identity?.IsAuthenticated == true && !model.AnDanh)
            {
                userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var user = await _context.NguoiDungs.FindAsync(userId);
                if (user != null)
                {
                    donorName = user.HoTen;
                    // Add points to user: 1 point per 10,000 VND donated
                    int addedPoints = (int)(model.SoTien / 10000);
                    if (addedPoints > 0)
                    {
                        user.CapNhatDiemTichLuy(addedPoints);
                    }
                }
            }
            else if (model.AnDanh)
            {
                donorName = "Ẩn danh";
            }

            var donation = new QuyenGop
            {
                MaNguoiDung = userId,
                TenNguoiQuyenGop = donorName,
                SoTien = model.SoTien,
                TenQuyQuyenGop = model.TenQuyQuyenGop,
                LoiNhan = model.LoiNhan,
                NgayQuyenGop = DateTime.Now
            };

            _context.QuyenGops.Add(donation);

            // Update ChienDichQuyenGop raised money
            var campaign = await _context.ChienDichQuyenGops.FirstOrDefaultAsync(c => c.TenChienDich == model.TenQuyQuyenGop);
            if (campaign != null)
            {
                campaign.SoTienDaQuyenGop += model.SoTien;
            }

            // Log activity
            _context.NhatKyHeThongs.Add(new NhatKyHeThong
            {
                MaNguoiDung = userId,
                MoTaHoatDong = $"Quyên góp {model.SoTien:N0} VND vào {model.TenQuyQuyenGop} (Người quyên góp: {donorName})",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok(donation, "Quyên góp thành công! Cảm ơn sự đồng hành của bạn."));
        }

        // GET: api/donations/campaigns
        [HttpGet("campaigns")]
        public async Task<IActionResult> GetCampaigns()
        {
            var campaigns = await _context.ChienDichQuyenGops.ToListAsync();
            return Ok(ApiResponse.Ok(campaigns));
        }

        // GET: api/donations/me
        [HttpGet("me")]
        public async Task<IActionResult> GetMyDonations()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Ok(ApiResponse.Fail("Chưa đăng nhập"));
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var donations = await _context.QuyenGops
                .Where(d => d.MaNguoiDung == userId)
                .OrderByDescending(d => d.NgayQuyenGop)
                .ToListAsync();

            return Ok(ApiResponse.Ok(donations));
        }

        // GET: api/donations/admin
        [HttpGet("admin")]
        public async Task<IActionResult> GetAdminDonations([FromQuery] string? quy, [FromQuery] string? q)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            var query = _context.QuyenGops.AsQueryable();

            if (!string.IsNullOrEmpty(quy))
            {
                query = query.Where(d => d.TenQuyQuyenGop == quy);
            }

            if (!string.IsNullOrEmpty(q))
            {
                var search = q.ToLower().Trim();
                query = query.Where(d => d.TenNguoiQuyenGop.ToLower().Contains(search) ||
                                         (d.LoiNhan != null && d.LoiNhan.ToLower().Contains(search)));
            }

            var donations = await query.OrderByDescending(d => d.NgayQuyenGop).ToListAsync();
            return Ok(ApiResponse.Ok(donations));
        }

        // GET: api/donations/admin/stats
        [HttpGet("admin/stats")]
        public async Task<IActionResult> GetStats()
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            var totalAmount = await _context.QuyenGops.SumAsync(d => d.SoTien);

            var topDonors = await _context.QuyenGops
                .GroupBy(d => d.TenNguoiQuyenGop)
                .Select(g => new
                {
                    TenNguoiQuyenGop = g.Key,
                    TongTien = g.Sum(d => d.SoTien),
                    SoLuot = g.Count()
                })
                .OrderByDescending(x => x.TongTien)
                .Take(5)
                .ToListAsync();

            var monthlyStats = await _context.QuyenGops
                .GroupBy(d => new { Year = d.NgayQuyenGop.Year, Month = d.NgayQuyenGop.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TongTien = g.Sum(d => d.SoTien)
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            var stats = new
            {
                totalAmount = totalAmount,
                topDonors = topDonors,
                monthlyStats = monthlyStats
            };

            return Ok(ApiResponse.Ok(stats));
        }

        // GET: api/donations/admin/export
        [HttpGet("admin/export")]
        public async Task<IActionResult> ExportCSV()
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Unauthorized();
            }

            var donations = await _context.QuyenGops.OrderByDescending(d => d.NgayQuyenGop).ToListAsync();

            var builder = new StringBuilder();
            builder.AppendLine("Mã Quyên Góp,Người Quyên Góp,Số Tiền,Quỹ Nhận,Lời Nhắn,Ngày Quyên Góp");

            foreach (var d in donations)
            {
                builder.AppendLine($"{d.MaQuyenGop},{d.TenNguoiQuyenGop},{d.SoTien},{d.TenQuyQuyenGop},\"{d.LoiNhan}\",{d.NgayQuyenGop}");
            }

            var csvBytes = Encoding.UTF8.GetBytes(builder.ToString());
            var bom = Encoding.UTF8.GetPreamble();
            var fileBytes = new byte[bom.Length + csvBytes.Length];
            Buffer.BlockCopy(bom, 0, fileBytes, 0, bom.Length);
            Buffer.BlockCopy(csvBytes, 0, fileBytes, bom.Length, csvBytes.Length);

            return File(fileBytes, "text/csv", $"donations_export_{DateTime.Now:yyyyMMddHHmmss}.csv");
        }

        // POST: api/Donation/admin/campaigns
        [HttpPost("admin/campaigns")]
        public async Task<IActionResult> CreateCampaign([FromBody] ChienDichQuyenGop model)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            if (!ModelState.IsValid)
            {
                return Ok(ApiResponse.Fail("Dữ liệu không hợp lệ", ModelState));
            }

            model.NgayTao = DateTime.Now;
            _context.ChienDichQuyenGops.Add(model);
            await _context.SaveChangesAsync();

            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            _context.NhatKyHeThongs.Add(new NhatKyHeThong
            {
                MaNguoiDung = adminId,
                MoTaHoatDong = $"Tạo chiến dịch quyên góp mới: '{model.TenChienDich}' (ID: {model.MaChienDich})",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            });
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok(model, "Tạo chiến dịch thành công!"));
        }

        // PUT: api/Donation/admin/campaigns/{id}
        [HttpPut("admin/campaigns/{id}")]
        public async Task<IActionResult> UpdateCampaign(int id, [FromBody] ChienDichQuyenGop model)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            var campaign = await _context.ChienDichQuyenGops.FindAsync(id);
            if (campaign == null)
            {
                return Ok(ApiResponse.Fail("Không tìm thấy chiến dịch"));
            }

            campaign.TenChienDich = model.TenChienDich;
            campaign.MoTa = model.MoTa;
            campaign.SoTienMucTieu = model.SoTienMucTieu;
            if (!string.IsNullOrEmpty(model.AnhChienDich))
            {
                campaign.AnhChienDich = model.AnhChienDich;
            }

            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            _context.NhatKyHeThongs.Add(new NhatKyHeThong
            {
                MaNguoiDung = adminId,
                MoTaHoatDong = $"Cập nhật chiến dịch quyên góp: '{campaign.TenChienDich}' (ID: {campaign.MaChienDich})",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            await _context.SaveChangesAsync();
            return Ok(ApiResponse.Ok(campaign, "Cập nhật chiến dịch thành công!"));
        }

        // DELETE: api/Donation/admin/campaigns/{id}
        [HttpDelete("admin/campaigns/{id}")]
        public async Task<IActionResult> DeleteCampaign(int id)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            var campaign = await _context.ChienDichQuyenGops.FindAsync(id);
            if (campaign == null)
            {
                return Ok(ApiResponse.Fail("Không tìm thấy chiến dịch"));
            }

            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            _context.NhatKyHeThongs.Add(new NhatKyHeThong
            {
                MaNguoiDung = adminId,
                MoTaHoatDong = $"Xóa chiến dịch quyên góp: '{campaign.TenChienDich}' (ID: {campaign.MaChienDich})",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            _context.ChienDichQuyenGops.Remove(campaign);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok(null, "Xóa chiến dịch thành công!"));
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Adopt()
        {
            return View();
        }

        [Authorize]
        public IActionResult AdoptFlow()
        {
            return View();
        }

        public IActionResult Blogs()
        {
            return View();
        }

        public IActionResult BlogDetail()
        {
            return View();
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.ThuCungs
                .Include(p => p.AnhThuCungs)
                .FirstOrDefaultAsync(p => p.MaThuCung == id);

            if (pet == null)
            {
                return NotFound();
            }

            return View(pet);
        }

        [Authorize]
        public IActionResult Donate()
        {
            return View();
        }

        public IActionResult Rescue()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Volunteer()
        {
            var emailClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            if (!string.IsNullOrEmpty(emailClaim))
            {
                var user = await _context.NguoiDungs
                    .Include(u => u.VaiTro)
                    .FirstOrDefaultAsync(u => u.Email == emailClaim);
                if (user != null && user.VaiTro?.TenVaiTro == "Tình nguyện viên")
                {
                    var tnv = await _context.TinhNguyenViens.FirstOrDefaultAsync(v => v.MaNguoiDung == user.MaNguoiDung);
                    if (tnv != null && tnv.TrangThai != "Bị khóa")
                    {
                        return RedirectToAction("Profile", "Account");
                    }
                }
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

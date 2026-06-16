using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.ViewModels;
using System.Net.Http;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BlogController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/blogs
        [HttpGet]
        public async Task<IActionResult> GetBlogs([FromQuery] bool? successStories)
        {
            var query = _context.BaiViets
                .Include(b => b.TacGia)
                .Include(b => b.ThuCung)
                .AsQueryable();

            if (successStories.HasValue)
            {
                query = query.Where(b => b.LaCauChuyenThanhCong == successStories.Value);
            }

            var blogs = await query.OrderByDescending(b => b.NgayTao).ToListAsync();
            return Ok(ApiResponse.Ok(blogs));
        }

        // GET: api/blogs/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogById(int id)
        {
            var blog = await _context.BaiViets
                .Include(b => b.TacGia)
                .Include(b => b.ThuCung)
                .FirstOrDefaultAsync(b => b.MaBaiViet == id);

            if (blog == null)
            {
                return Ok(ApiResponse.Fail("Không tìm thấy bài viết"));
            }

            return Ok(ApiResponse.Ok(blog));
        }

        // POST: api/blog/admin/blogs (Admin only)
        [HttpPost("admin/blogs")]
        public async Task<IActionResult> CreateBlog([FromBody] BaiViet model)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            if (!ModelState.IsValid)
            {
                return Ok(ApiResponse.Fail("Dữ liệu không hợp lệ", ModelState));
            }

            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            model.MaTacGia = adminId;
            model.NgayTao = DateTime.Now;

            _context.BaiViets.Add(model);
            await _context.SaveChangesAsync();

            _context.NhatKyHeThongs.Add(new NhatKyHeThong
            {
                MaNguoiDung = adminId,
                MoTaHoatDong = $"Tạo bài viết mới: '{model.TieuDe}' (ID: {model.MaBaiViet})",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            });
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok(model, "Đăng bài viết thành công!"));
        }

        // PUT: api/blog/admin/blogs/{id} (Admin only)
        [HttpPut("admin/blogs/{id}")]
        public async Task<IActionResult> UpdateBlog(int id, [FromBody] BaiViet model)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            var blog = await _context.BaiViets.FindAsync(id);
            if (blog == null)
            {
                return Ok(ApiResponse.Fail("Không tìm thấy bài viết"));
            }

            blog.TieuDe = model.TieuDe;
            blog.NoiDung = model.NoiDung;
            if (!string.IsNullOrEmpty(model.DuongDanAnhDaiDien))
            {
                blog.DuongDanAnhDaiDien = model.DuongDanAnhDaiDien;
            }
            blog.LaCauChuyenThanhCong = model.LaCauChuyenThanhCong;
            blog.MaThuCung = model.MaThuCung;

            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            _context.NhatKyHeThongs.Add(new NhatKyHeThong
            {
                MaNguoiDung = adminId,
                MoTaHoatDong = $"Cập nhật bài viết: '{blog.TieuDe}' (ID: {blog.MaBaiViet})",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            await _context.SaveChangesAsync();
            return Ok(ApiResponse.Ok(blog, "Cập nhật bài viết thành công!"));
        }

        // DELETE: api/blog/admin/blogs/{id} (Admin only)
        [HttpDelete("admin/blogs/{id}")]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            var blog = await _context.BaiViets.FindAsync(id);
            if (blog == null)
            {
                return Ok(ApiResponse.Fail("Không tìm thấy bài viết"));
            }

            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            _context.NhatKyHeThongs.Add(new NhatKyHeThong
            {
                MaNguoiDung = adminId,
                MoTaHoatDong = $"Xóa bài viết: '{blog.TieuDe}' (ID: {blog.MaBaiViet})",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            _context.BaiViets.Remove(blog);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok(null, "Xóa bài viết thành công!"));
        }

        // POST: api/newsletter/subscribe
        [HttpPost("newsletter/subscribe")]
        public async Task<IActionResult> SubscribeNewsletter([FromBody] NewsletterSubscribeModel model)
        {
            if (string.IsNullOrEmpty(model.Email))
            {
                return Ok(ApiResponse.Fail("Email không hợp lệ"));
            }

            var email = model.Email.Trim().ToLower();
            var exists = await _context.Newsletters.FindAsync(email);
            if (exists != null)
            {
                return Ok(ApiResponse.Ok(null, "Bạn đã đăng ký bản tin này rồi!"));
            }

            var sub = new Newsletter
            {
                Email = email,
                NgayDangKy = DateTime.Now,
                TrangThai = "Hoạt động"
            };

            _context.Newsletters.Add(sub);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok(null, "Đăng ký nhận tin thành công!"));
        }

        // POST: api/Blog/admin/sync-news (Admin only)
        [HttpPost("admin/sync-news")]
        public async Task<IActionResult> SyncNews()
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            string feedUrl = "https://kingspet.vn/feed/";
            int countAdded = 0;

            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
                    var xmlContent = await httpClient.GetStringAsync(feedUrl);
                    var doc = XDocument.Parse(xmlContent);
                    var items = doc.Descendants("item");

                    foreach (var item in items)
                    {
                        var title = item.Element("title")?.Value ?? "";
                        if (string.IsNullOrEmpty(title)) continue;

                        // Check if article with same title already exists
                        bool exists = await _context.BaiViets.AnyAsync(b => b.TieuDe == title);
                        if (exists) continue;

                        var description = item.Element("description")?.Value ?? "";
                        
                        // Try to get content:encoded
                        XNamespace contentNs = "http://purl.org/rss/1.0/modules/content/";
                        var content = item.Element(contentNs + "encoded")?.Value ?? description;

                        // Try to extract first image URL from content or description
                        string? imageUrl = null;
                        var imgMatch = Regex.Match(content, @"<img[^>]+src=""([^""]+)""");
                        if (imgMatch.Success)
                        {
                            imageUrl = imgMatch.Groups[1].Value;
                        }
                        else
                        {
                            // fallback from description
                            var imgMatchDesc = Regex.Match(description, @"<img[^>]+src=""([^""]+)""");
                            if (imgMatchDesc.Success)
                            {
                                imageUrl = imgMatchDesc.Groups[1].Value;
                            }
                        }

                        // Parse pubDate
                        DateTime pubDate = DateTime.Now;
                        var pubDateStr = item.Element("pubDate")?.Value;
                        if (!string.IsNullOrEmpty(pubDateStr) && DateTime.TryParse(pubDateStr, out var parsedDate))
                        {
                            pubDate = parsedDate;
                        }

                        if (title.Length > 255) title = title.Substring(0, 250) + "...";

                        // Create blog post
                        var newBlog = new BaiViet
                        {
                            MaTacGia = adminId,
                            TieuDe = title,
                            NoiDung = content,
                            DuongDanAnhDaiDien = imageUrl ?? "https://images.unsplash.com/photo-1543466835-00a7907e9de1?auto=format&fit=crop&q=80&w=600",
                            LaCauChuyenThanhCong = title.ToLower().Contains("cứu hộ") || title.ToLower().Contains("tìm lại") || title.ToLower().Contains("mái ấm"),
                            NgayTao = pubDate
                        };

                        _context.BaiViets.Add(newBlog);
                        countAdded++;
                    }

                    if (countAdded > 0)
                    {
                        await _context.SaveChangesAsync();
                        _context.NhatKyHeThongs.Add(new NhatKyHeThong
                        {
                            MaNguoiDung = adminId,
                            MoTaHoatDong = $"Đồng bộ bài viết từ mạng: Thêm thành công {countAdded} bài viết mới.",
                            DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
                        });
                        await _context.SaveChangesAsync();
                    }
                }

                return Ok(ApiResponse.Ok(countAdded, $"Đồng bộ thành công! Đã thêm {countAdded} bài viết mới từ mạng."));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse.Fail($"Lỗi khi đồng bộ bài viết: {ex.Message}"));
            }
        }
    }

    public class NewsletterSubscribeModel
    {
        public string Email { get; set; } = string.Empty;
    }
}

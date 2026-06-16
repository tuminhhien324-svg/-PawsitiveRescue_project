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
    [Route("api/[controller]")]
    public class PetsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PetsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/pets
        [HttpGet]
        public async Task<IActionResult> GetPets(
            [FromQuery] string? loai,
            [FromQuery] string? tuoi,
            [FromQuery] string? gioitinh,
            [FromQuery] string? kichthuoc,
            [FromQuery] string? trangthai,
            [FromQuery] string? q)
        {
            var query = _context.ThuCungs
                .Include(p => p.AnhThuCungs)
                .AsQueryable();

            // Filter species (LoaiNguonGoc)
            if (!string.IsNullOrEmpty(loai))
            {
                var normLoai = loai.Trim().ToLower();
                if (normLoai == "dog" || normLoai == "chó")
                    query = query.Where(p => p.LoaiNguonGoc == "Chó");
                else if (normLoai == "cat" || normLoai == "mèo")
                    query = query.Where(p => p.LoaiNguonGoc == "Mèo");
                else
                    query = query.Where(p => p.LoaiNguonGoc != "Chó" && p.LoaiNguonGoc != "Mèo");
            }

            // Filter gender
            if (!string.IsNullOrEmpty(gioitinh))
            {
                var normGender = gioitinh.Trim().ToLower();
                if (normGender == "male" || normGender == "đực")
                    query = query.Where(p => p.GioiTinh == "Đực");
                else if (normGender == "female" || normGender == "cái")
                    query = query.Where(p => p.GioiTinh == "Cái");
            }

            // Filter age (tuoi)
            if (!string.IsNullOrEmpty(tuoi))
            {
                var normAge = tuoi.Trim().ToLower();
                if (normAge == "baby")
                {
                    // 0-6 months
                    query = query.Where(p => (p.DonViTuoi == "Tháng" && p.GiaTriTuoi <= 6) || (p.DonViTuoi == "Tuổi" && p.GiaTriTuoi <= 0.5m));
                }
                else if (normAge == "young")
                {
                    // 6-18 months
                    query = query.Where(p => (p.DonViTuoi == "Tháng" && p.GiaTriTuoi > 6 && p.GiaTriTuoi <= 18) || (p.DonViTuoi == "Tuổi" && p.GiaTriTuoi > 0.5m && p.GiaTriTuoi <= 1.5m));
                }
                else if (normAge == "adult")
                {
                    // >18 months (>1.5 years)
                    query = query.Where(p => (p.DonViTuoi == "Tháng" && p.GiaTriTuoi > 18) || (p.DonViTuoi == "Tuổi" && p.GiaTriTuoi > 1.5m));
                }
            }

            // Filter size
            if (!string.IsNullOrEmpty(kichthuoc))
            {
                // Simple representation since size is descriptive in database
                // Small (< 5kg), Medium (5-15kg), Large (> 15kg) - we check Giong or description, or just ignore for simplicity if not in original table.
                // We'll keep size filter matching as a placeholder/contains search
                var sizeText = kichthuoc.ToLower().Trim();
                if (sizeText == "small") sizeText = "nhỏ";
                else if (sizeText == "medium") sizeText = "vừa";
                else if (sizeText == "large") sizeText = "lớn";

                query = query.Where(p => p.MoTa != null && p.MoTa.ToLower().Contains(sizeText));
            }

            // Filter adoption status
            if (!string.IsNullOrEmpty(trangthai))
            {
                query = query.Where(p => p.TrangThaiNhanNuoi.ToLower() == trangthai.ToLower().Trim());
            }
            else
            {
                var isAdmin = User.Identity?.IsAuthenticated == true && User.IsInRole("Quản trị viên");
                if (!isAdmin)
                {
                    query = query.Where(p => p.TrangThaiNhanNuoi != "Đã nhận nuôi");
                }
            }

            // Filter search query
            if (!string.IsNullOrEmpty(q))
            {
                var search = q.ToLower().Trim();
                query = query.Where(p => p.TenThuCung.ToLower().Contains(search) ||
                                         (p.GiongThuCung != null && p.GiongThuCung.ToLower().Contains(search)) ||
                                         (p.MoTa != null && p.MoTa.ToLower().Contains(search)));
            }

            var pets = await query.ToListAsync();
            return Ok(ApiResponse.Ok(pets));
        }

        // GET: api/pets/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPetById(int id)
        {
            var pet = await _context.ThuCungs
                .Include(p => p.AnhThuCungs)
                .Include(p => p.LichSuSucKhoes)
                .Include(p => p.ChiPhiCuuHos)
                .Include(p => p.LichSuTrangThais)
                .FirstOrDefaultAsync(p => p.MaThuCung == id);

            if (pet == null)
            {
                return Ok(ApiResponse.Fail("Không tìm thấy thú cưng"));
            }

            return Ok(ApiResponse.Ok(pet));
        }

        // POST: api/pets (Admin only)
        [HttpPost]
        public async Task<IActionResult> CreatePet([FromBody] ThuCung model)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            if (!ModelState.IsValid)
            {
                return Ok(ApiResponse.Fail("Dữ liệu không hợp lệ", ModelState));
            }

            _context.ThuCungs.Add(model);
            await _context.SaveChangesAsync();

            // Log activity
            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            _context.NhatKyHeThongs.Add(new NhatKyHeThong
            {
                MaNguoiDung = adminId,
                MoTaHoatDong = $"Thêm hồ sơ thú cưng mới: {model.TenThuCung} (ID: {model.MaThuCung})",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            // Add status history log
            _context.LichSuTrangThais.Add(new LichSuTrangThai
            {
                MaThuCung = model.MaThuCung,
                TrangThaiCuuHoTruoc = null,
                TrangThaiCuuHoSau = model.TrangThaiCuuHo,
                TrangThaiNhanNuoiTruoc = null,
                TrangThaiNhanNuoiSau = model.TrangThaiNhanNuoi,
                NgayCapNhat = DateTime.Now,
                GhiChuThayDoi = "Tạo hồ sơ thú cưng",
                NguoiThucHien = adminId
            });

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok(model, "Thêm thú cưng thành công!"));
        }

        // PUT: api/pets/{id} (Admin only)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePet(int id, [FromBody] ThuCung model)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            var pet = await _context.ThuCungs.FindAsync(id);
            if (pet == null)
            {
                return Ok(ApiResponse.Fail("Không tìm thấy thú cưng"));
            }

            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            // Log status transitions if changed
            if (pet.TrangThaiCuuHo != model.TrangThaiCuuHo || pet.TrangThaiNhanNuoi != model.TrangThaiNhanNuoi)
            {
                _context.LichSuTrangThais.Add(new LichSuTrangThai
                {
                    MaThuCung = pet.MaThuCung,
                    TrangThaiCuuHoTruoc = pet.TrangThaiCuuHo,
                    TrangThaiCuuHoSau = model.TrangThaiCuuHo,
                    TrangThaiNhanNuoiTruoc = pet.TrangThaiNhanNuoi,
                    TrangThaiNhanNuoiSau = model.TrangThaiNhanNuoi,
                    NgayCapNhat = DateTime.Now,
                    GhiChuThayDoi = "Cập nhật thông tin trạng thái",
                    NguoiThucHien = adminId
                });
            }

            pet.TenThuCung = model.TenThuCung;
            pet.LoaiNguonGoc = model.LoaiNguonGoc;
            pet.GiongThuCung = model.GiongThuCung;
            pet.GiaTriTuoi = model.GiaTriTuoi;
            pet.DonViTuoi = model.DonViTuoi;
            pet.GioiTinh = model.GioiTinh;
            pet.MoTa = model.MoTa;
            if (!string.IsNullOrEmpty(model.AnhChinh))
            {
                pet.AnhChinh = model.AnhChinh;
            }
            pet.TinhTrangSucKhoe = model.TinhTrangSucKhoe;
            pet.TrangThaiCuuHo = model.TrangThaiCuuHo;
            pet.TrangThaiNhanNuoi = model.TrangThaiNhanNuoi;
            pet.NgayCuuHo = model.NgayCuuHo;
            pet.DacDiem = model.DacDiem;
            pet.TiemNgua = model.TiemNgua;
            pet.SoThich = model.SoThich;

            _context.NhatKyHeThongs.Add(new NhatKyHeThong
            {
                MaNguoiDung = adminId,
                MoTaHoatDong = $"Cập nhật hồ sơ thú cưng: {pet.TenThuCung} (ID: {pet.MaThuCung})",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            await _context.SaveChangesAsync();
            return Ok(ApiResponse.Ok(pet, "Cập nhật thú cưng thành công!"));
        }

        // DELETE: api/pets/{id} (Admin only)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePet(int id)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("Quản trị viên"))
            {
                return Ok(ApiResponse.Fail("Không có quyền thực hiện hành động này."));
            }

            var pet = await _context.ThuCungs.FindAsync(id);
            if (pet == null)
            {
                return Ok(ApiResponse.Fail("Không tìm thấy thú cưng"));
            }

            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            _context.NhatKyHeThongs.Add(new NhatKyHeThong
            {
                MaNguoiDung = adminId,
                MoTaHoatDong = $"Xóa hồ sơ thú cưng: {pet.TenThuCung} (ID: {pet.MaThuCung})",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            _context.ThuCungs.Remove(pet);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok(null, "Xóa thú cưng thành công!"));
        }

        // POST: api/pets/{id}/favorite
        [HttpPost("{id}/favorite")]
        public async Task<IActionResult> AddFavorite(int id)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Ok(ApiResponse.Fail("Vui lòng đăng nhập để lưu thú cưng yêu thích."));
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var petExists = await _context.ThuCungs.AnyAsync(p => p.MaThuCung == id);
            if (!petExists)
            {
                return Ok(ApiResponse.Fail("Thú cưng không tồn tại"));
            }

            var favExists = await _context.ThuCungYeuThichs.AnyAsync(f => f.MaNguoiDung == userId && f.MaThuCung == id);
            if (favExists)
            {
                return Ok(ApiResponse.Ok(null, "Đã nằm trong danh sách yêu thích!"));
            }

            var fav = new ThuCungYeuThich
            {
                MaNguoiDung = userId,
                MaThuCung = id,
                NgayLuu = DateTime.Now
            };

            _context.ThuCungYeuThichs.Add(fav);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok(null, "Đã thêm vào danh sách yêu thích!"));
        }

        // DELETE: api/pets/{id}/favorite
        [HttpDelete("{id}/favorite")]
        public async Task<IActionResult> RemoveFavorite(int id)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Ok(ApiResponse.Fail("Chưa đăng nhập."));
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var fav = await _context.ThuCungYeuThichs.FirstOrDefaultAsync(f => f.MaNguoiDung == userId && f.MaThuCung == id);
            if (fav == null)
            {
                return Ok(ApiResponse.Fail("Bé chưa được lưu yêu thích"));
            }

            _context.ThuCungYeuThichs.Remove(fav);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok(null, "Đã xóa khỏi danh sách yêu thích!"));
        }

        // GET: api/users/me/favorites
        [HttpGet("favorites")]
        public async Task<IActionResult> GetMyFavorites()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Ok(ApiResponse.Fail("Chưa đăng nhập."));
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var favorites = await _context.ThuCungYeuThichs
                .Where(f => f.MaNguoiDung == userId)
                .Include(f => f.ThuCung)
                .Select(f => f.ThuCung)
                .ToListAsync();

            return Ok(ApiResponse.Ok(favorites));
        }

        // POST: api/pets/upload-image
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(Microsoft.AspNetCore.Http.IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Ok(ApiResponse.Fail("Vui lòng chọn file ảnh"));
            }

            // Validate extension
            var extension = System.IO.Path.GetExtension(file.FileName).ToLower();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            if (!allowedExtensions.Contains(extension))
            {
                return Ok(ApiResponse.Fail("Định dạng file không hợp lệ! Chỉ chấp nhận các định dạng ảnh: .jpg, .jpeg, .png, .gif, .webp"));
            }

            // Validate MIME type
            if (string.IsNullOrEmpty(file.ContentType) || !file.ContentType.StartsWith("image/"))
            {
                return Ok(ApiResponse.Fail("File tải lên không phải là ảnh hợp lệ!"));
            }

            // Validate file size (5 MB limit)
            if (file.Length > 5 * 1024 * 1024)
            {
                return Ok(ApiResponse.Fail("Dung lượng file vượt quá giới hạn cho phép (Tối đa 5MB)!"));
            }

            var uploadsFolder = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!System.IO.Directory.Exists(uploadsFolder))
            {
                System.IO.Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + System.IO.Path.GetFileName(file.FileName);
            var filePath = System.IO.Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var relativePath = "/uploads/" + uniqueFileName;
            return Ok(ApiResponse.Ok(new { path = relativePath }, "Tải ảnh lên thành công!"));
        }
    }
}

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<NguoiDung> _passwordHasher;
        private static readonly ConcurrentDictionary<string, (string Otp, DateTime Expiry)> _otpStore = new();

        public AuthController(ApplicationDbContext context, PasswordHasher<NguoiDung> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // POST: api/auth/send-otp
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(ApiResponse.Fail("Email không hợp lệ", ModelState));
            }

            var email = model.Email.Trim().ToLower();
            var existingUser = await _context.NguoiDungs.AnyAsync(u => u.Email.ToLower() == email);
            if (existingUser)
            {
                return Ok(ApiResponse.Fail("Email này đã được sử dụng!"));
            }

            // Generate 6-digit random OTP
            var rand = new Random();
            var otp = rand.Next(100000, 999999).ToString();
            var expiry = DateTime.Now.AddMinutes(5);

            _otpStore[email] = (otp, expiry);

            // Log to console for development verification
            Console.WriteLine($"[OTP Service] Registration OTP for {email} is: {otp} (Expires at {expiry})");

            return Ok(ApiResponse.Ok(new { email = email, otp = otp }, $"Mã OTP đã được gửi về email của bạn (Mã: {otp})"));
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(ApiResponse.Fail("Dữ liệu không hợp lệ", ModelState));
            }

            var existingUser = await _context.NguoiDungs.AnyAsync(u => u.Email.ToLower() == model.Email.ToLower().Trim());
            if (existingUser)
            {
                return Ok(ApiResponse.Fail("Email này đã được sử dụng!"));
            }

            var defaultRole = await _context.VaiTros.FirstOrDefaultAsync(r => r.TenVaiTro == "Người dùng phổ thông");
            if (defaultRole == null)
            {
                return Ok(ApiResponse.Fail("Không tìm thấy vai trò mặc định trong hệ thống."));
            }

            var newUser = new NguoiDung
            {
                HoTen = model.HoTen,
                Email = model.Email.Trim().ToLower(),
                SoDienThoai = model.SoDienThoai,
                NamSinh = model.NamSinh,
                DiaChi = model.DiaChi,
                DiemTichLuy = 0,
                TenHang = "Thành viên mới",
                MaVaiTro = defaultRole.MaVaiTro,
                NgayTao = DateTime.Now
            };

            newUser.MatKhauMaHoa = _passwordHasher.HashPassword(newUser, model.Password);

            _context.NguoiDungs.Add(newUser);
            
            // Clean up verified OTP
            _otpStore.TryRemove(newUser.Email, out _);

            await _context.SaveChangesAsync();

            // Log activity
            _context.NhatKyHeThongs.Add(new NhatKyHeThong
            {
                MaNguoiDung = newUser.MaNguoiDung,
                MoTaHoatDong = $"Đăng ký tài khoản mới: {newUser.Email}",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            });
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok(new { email = newUser.Email, hoTen = newUser.HoTen }, "Đăng ký thành công!"));
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(ApiResponse.Fail("Dữ liệu không hợp lệ", ModelState));
            }

            var user = await _context.NguoiDungs
                .Include(u => u.VaiTro)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower().Trim());

            if (user == null)
            {
                return Ok(ApiResponse.Fail("Email hoặc mật khẩu không đúng!"));
            }

            var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.MatKhauMaHoa, model.Password);
            if (verifyResult == PasswordVerificationResult.Failed)
            {
                return Ok(ApiResponse.Fail("Email hoặc mật khẩu không đúng!"));
            }

            // Map Vietnamese db role to frontend string
            string clientRole = "user";
            if (user.VaiTro?.TenVaiTro == "Quản trị viên")
            {
                clientRole = "admin";
            }
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

            // Issue session cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.MaNguoiDung.ToString()),
                new Claim(ClaimTypes.Name, user.HoTen),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.VaiTro?.TenVaiTro ?? "Người dùng phổ thông")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            // Log activity
            _context.NhatKyHeThongs.Add(new NhatKyHeThong
            {
                MaNguoiDung = user.MaNguoiDung,
                MoTaHoatDong = $"Đăng nhập hệ thống",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            });
            await _context.SaveChangesAsync();

            var token = "mock-jwt-token-for-" + user.Email;

            var userProfile = new
            {
                email = user.Email,
                role = clientRole,
                displayName = user.HoTen,
                avatar = "https://cdn-icons-png.flaticon.com/512/149/149071.png",
                diemTichLuy = user.DiemTichLuy,
                tenHang = user.TenHang,
                token = token
            };

            return Ok(ApiResponse.Ok(userProfile, "Đăng nhập thành công!"));
        }

        // GET: api/auth/me
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Ok(ApiResponse.Fail("Chưa đăng nhập"));
            }

            var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(emailClaim))
            {
                return Ok(ApiResponse.Fail("Không tìm thấy thông tin email trong session"));
            }

            var user = await _context.NguoiDungs
                .Include(u => u.VaiTro)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == emailClaim.ToLower().Trim());

            if (user == null)
            {
                return Ok(ApiResponse.Fail("Người dùng không tồn tại"));
            }

            string clientRole = "user";
            if (user.VaiTro?.TenVaiTro == "Quản trị viên")
            {
                clientRole = "admin";
            }
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

            var userProfile = new
            {
                email = user.Email,
                role = clientRole,
                displayName = user.HoTen,
                avatar = "https://cdn-icons-png.flaticon.com/512/149/149071.png",
                diemTichLuy = user.DiemTichLuy,
                tenHang = user.TenHang
            };

            return Ok(ApiResponse.Ok(userProfile));
        }

        // POST: api/auth/logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
                var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.Email == emailClaim);

                if (user != null)
                {
                    _context.NhatKyHeThongs.Add(new NhatKyHeThong
                    {
                        MaNguoiDung = user.MaNguoiDung,
                        MoTaHoatDong = "Đăng xuất hệ thống",
                        DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
                    });
                    await _context.SaveChangesAsync();
                }
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(ApiResponse.Ok(null, "Đăng xuất thành công!"));
        }

        // POST: api/auth/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordViewModel model)
        {
            var email = model.Email.Trim().ToLower();
            var userExists = await _context.NguoiDungs.AnyAsync(u => u.Email.ToLower() == email);
            if (!userExists)
            {
                return Ok(ApiResponse.Fail("Email không tồn tại trong hệ thống!"));
            }

            // Generate 6-digit random OTP
            var rand = new Random();
            var otp = rand.Next(100000, 999999).ToString();
            var expiry = DateTime.Now.AddMinutes(5);

            _otpStore[email] = (otp, expiry);

            // Log to console for development verification
            Console.WriteLine($"[OTP Service] Password Recovery OTP for {email} is: {otp} (Expires at {expiry})");

            return Ok(ApiResponse.Ok(new { email = email, otp = otp }, $"Mã OTP đã được gửi về email của bạn (Mã: {otp})"));
        }

        // POST: api/auth/verify-otp
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpViewModel model)
        {
            var email = model.Email.Trim().ToLower();
            if (_otpStore.TryGetValue(email, out var storedOtp))
            {
                if (storedOtp.Expiry < DateTime.Now)
                {
                    return Ok(ApiResponse.Fail("Mã OTP đã hết hạn!"));
                }

                if (storedOtp.Otp == model.Otp.Trim())
                {
                    return Ok(ApiResponse.Ok(new { email = email }, "Xác minh OTP thành công!"));
                }
            }
            return Ok(ApiResponse.Fail("Mã OTP không chính xác!"));
        }

        // POST: api/auth/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
        {
            var email = model.Email.Trim().ToLower();
            var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.Email.ToLower() == email);
            if (user == null)
            {
                return Ok(ApiResponse.Fail("Người dùng không tồn tại!"));
            }

            user.MatKhauMaHoa = _passwordHasher.HashPassword(user, model.Password);
            
            // Clean up verified OTP
            _otpStore.TryRemove(email, out _);

            await _context.SaveChangesAsync();

            // Log activity
            _context.NhatKyHeThongs.Add(new NhatKyHeThong
            {
                MaNguoiDung = user.MaNguoiDung,
                MoTaHoatDong = "Đặt lại mật khẩu khôi phục",
                DiaChiIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            });
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok(null, "Đổi mật khẩu mới thành công!"));
        }
    }
}

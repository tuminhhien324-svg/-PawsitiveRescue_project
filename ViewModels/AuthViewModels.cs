using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải từ 6 ký tự")]
        public string Password { get; set; } = string.Empty;

        [StringLength(15)]
        public string? SoDienThoai { get; set; }

        public int? NamSinh { get; set; }

        [StringLength(255)]
        public string? DiaChi { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = string.Empty;
    }

    public class SendOtpViewModel
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = string.Empty;
    }

    public class VerifyOtpViewModel
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã OTP không được để trống")]
        public string Otp { get; set; } = string.Empty;
    }

    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
        [MinLength(6, ErrorMessage = "Mật khẩu mới phải từ 6 ký tự")]
        public string Password { get; set; } = string.Empty;
    }
}

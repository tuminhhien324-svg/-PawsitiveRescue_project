using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("DonDangKyTinhNguyen")]
    public class DonDangKyTinhNguyen
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaDonDangKy { get; set; }

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string SoDienThoai { get; set; } = string.Empty;

        public int NamSinh { get; set; }

        [StringLength(100)]
        public string? NgheNghiep { get; set; }

        [StringLength(255)]
        public string? KyNang { get; set; }

        [Required]
        [StringLength(50)]
        public string TrangThai { get; set; } = "Chờ xử lý"; // Chờ xử lý, Đã duyệt, Từ chối

        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("BaoCaoCuuHo")]
    public class BaoCaoCuuHo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaBaoCao { get; set; }

        [StringLength(100)]
        public string? TenNguoiBaoCao { get; set; }

        [StringLength(15)]
        public string? SoDienThoai { get; set; }

        [Required]
        [StringLength(255)]
        public string DiaDiem { get; set; } = string.Empty;

        [StringLength(50)]
        public string? QuanHuyen { get; set; }

        [Required]
        public string MoTaChiTiet { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string MucDoKhanCap { get; set; } = string.Empty; // Khẩn cấp, Bình thường

        [Required]
        [StringLength(50)]
        public string TrangThai { get; set; } = "Chờ xử lý"; // Chờ xử lý, Đang thực hiện, Đã hoàn thành

        public DateTime NgayBaoCao { get; set; } = DateTime.Now;

        [StringLength(500)]
        public string? AnhBaoCao { get; set; }

        // Navigations
        public ICollection<PhanCongCuuHo> PhanCongCuuHos { get; set; } = new List<PhanCongCuuHo>();
    }
}

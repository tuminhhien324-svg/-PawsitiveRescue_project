using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("QuyenGop")]
    public class QuyenGop
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaQuyenGop { get; set; }

        public int? MaNguoiDung { get; set; }

        [ForeignKey("MaNguoiDung")]
        public NguoiDung? NguoiDung { get; set; }

        [Required]
        [StringLength(100)]
        public string TenNguoiQuyenGop { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal SoTien { get; set; }

        [Required]
        [StringLength(100)]
        public string TenQuyQuyenGop { get; set; } = string.Empty;

        [StringLength(255)]
        public string? LoiNhan { get; set; }

        public DateTime NgayQuyenGop { get; set; } = DateTime.Now;
    }
}

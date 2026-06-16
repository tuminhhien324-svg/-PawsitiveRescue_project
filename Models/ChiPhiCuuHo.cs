using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("ChiPhiCuuHo")]
    public class ChiPhiCuuHo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaChiPhi { get; set; }

        public int MaThuCung { get; set; }

        [ForeignKey("MaThuCung")]
        public ThuCung? ThuCung { get; set; }

        public DateTime NgayChiTieu { get; set; } = DateTime.Today;

        [Required]
        [StringLength(100)]
        public string LoaiChiPhi { get; set; } = string.Empty; // Viện phí, Thức ăn, Phụ kiện, Vận chuyển

        [Column(TypeName = "decimal(18,2)")]
        public decimal SoTien { get; set; }

        [Required]
        [StringLength(255)]
        public string MucDichChiTiet { get; set; } = string.Empty;

        public int? NguoiXacNhan { get; set; }

        [ForeignKey("NguoiXacNhan")]
        public NguoiDung? NguoiDungXacNhan { get; set; }
    }
}

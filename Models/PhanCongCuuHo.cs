using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("PhanCongCuuHo")]
    public class PhanCongCuuHo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaPhanCong { get; set; }

        public int MaBaoCao { get; set; }

        [ForeignKey("MaBaoCao")]
        public BaoCaoCuuHo? BaoCaoCuuHo { get; set; }

        public int MaTinhNguyenVien { get; set; }

        [ForeignKey("MaTinhNguyenVien")]
        public TinhNguyenVien? TinhNguyenVien { get; set; }

        public DateTime NgayPhanCong { get; set; } = DateTime.Now;

        [StringLength(255)]
        public string? GhiChu { get; set; }
    }
}

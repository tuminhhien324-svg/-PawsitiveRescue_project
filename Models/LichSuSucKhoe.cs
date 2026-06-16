using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("LichSuSucKhoe")]
    public class LichSuSucKhoe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaLichSuSK { get; set; }

        public int MaThuCung { get; set; }

        [ForeignKey("MaThuCung")]
        public ThuCung? ThuCung { get; set; }

        public DateTime NgayKham { get; set; } = DateTime.Today;

        [Required]
        [StringLength(50)]
        public string LoaiHinh { get; set; } = string.Empty; // Tiêm phòng, Khám bệnh, Triệt sản, Xổ giun

        [Required]
        [StringLength(255)]
        public string ChuanDoan { get; set; } = string.Empty;

        public string? GhiChuBenhAn { get; set; }

        [StringLength(100)]
        public string? BacSiThuY { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("LichSuTrangThai")]
    public class LichSuTrangThai
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaLichSuTT { get; set; }

        public int MaThuCung { get; set; }

        [ForeignKey("MaThuCung")]
        public ThuCung? ThuCung { get; set; }

        [StringLength(50)]
        public string? TrangThaiCuuHoTruoc { get; set; }

        [Required]
        [StringLength(50)]
        public string TrangThaiCuuHoSau { get; set; } = string.Empty;

        [StringLength(50)]
        public string? TrangThaiNhanNuoiTruoc { get; set; }

        [Required]
        [StringLength(50)]
        public string TrangThaiNhanNuoiSau { get; set; } = string.Empty;

        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        [StringLength(255)]
        public string? GhiChuThayDoi { get; set; }

        public int? NguoiThucHien { get; set; }

        [ForeignKey("NguoiThucHien")]
        public NguoiDung? NguoiDungThucHien { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("KhaoSatDieuKienSong")]
    public class KhaoSatDieuKienSong
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaKhaoSat { get; set; }

        public int MaDonNhanNuoi { get; set; }

        [ForeignKey("MaDonNhanNuoi")]
        public DonNhanNuoi? DonNhanNuoi { get; set; }

        [Required]
        [StringLength(50)]
        public string KinhNghiemNuoi { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string ThoiGianRanhMoiNgay { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string SuDongYCoGiaDinh { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LoaiNhaO { get; set; } = string.Empty;

        public bool CoSanVuon { get; set; }

        public bool CoLuoiBanCong { get; set; }

        public bool CoHangRaoCao { get; set; }
    }
}

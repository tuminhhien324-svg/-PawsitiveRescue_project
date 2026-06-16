using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("ThuCung")]
    public class ThuCung
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaThuCung { get; set; }

        [Required]
        [StringLength(100)]
        public string TenThuCung { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LoaiNguonGoc { get; set; } = string.Empty; // Chó, Mèo, Khác

        [StringLength(100)]
        public string? GiongThuCung { get; set; }

        [Column(TypeName = "decimal(3,1)")]
        public decimal? GiaTriTuoi { get; set; }

        [StringLength(10)]
        public string? DonViTuoi { get; set; } // Tuổi, Tháng

        [StringLength(10)]
        public string? GioiTinh { get; set; } // Đực, Cái

        public string? MoTa { get; set; }

        [StringLength(255)]
        public string? AnhChinh { get; set; }

        [StringLength(100)]
        public string? TinhTrangSucKhoe { get; set; }

        [Required]
        [StringLength(50)]
        public string TrangThaiCuuHo { get; set; } = string.Empty; // ĐANG CHỜ, ĐÃ NHẬN, ĐANG CỨU HỘ

        [Required]
        [StringLength(50)]
        public string TrangThaiNhanNuoi { get; set; } = "Chưa nhận nuôi"; // Chưa nuôi, Đang xử lý, Đã nhận nuôi

        public DateTime? NgayCuuHo { get; set; }

        [StringLength(255)]
        public string? DacDiem { get; set; }

        [StringLength(100)]
        public string? TiemNgua { get; set; }

        [StringLength(255)]
        public string? SoThich { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigations
        public ICollection<AnhThuCung> AnhThuCungs { get; set; } = new List<AnhThuCung>();
        public ICollection<LichSuSucKhoe> LichSuSucKhoes { get; set; } = new List<LichSuSucKhoe>();
        public ICollection<ChiPhiCuuHo> ChiPhiCuuHos { get; set; } = new List<ChiPhiCuuHo>();
        public ICollection<LichSuTrangThai> LichSuTrangThais { get; set; } = new List<LichSuTrangThai>();
        public ICollection<DonNhanNuoi> DonNhanNuois { get; set; } = new List<DonNhanNuoi>();
        public ICollection<ThuCungYeuThich> ThuCungYeuThichs { get; set; } = new List<ThuCungYeuThich>();
        public ICollection<BaiViet> BaiViets { get; set; } = new List<BaiViet>();
    }
}

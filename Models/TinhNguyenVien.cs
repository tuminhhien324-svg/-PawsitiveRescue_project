using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("TinhNguyenVien")]
    public class TinhNguyenVien
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaTinhNguyenVien { get; set; }

        public int MaNguoiDung { get; set; }

        [ForeignKey("MaNguoiDung")]
        public NguoiDung? NguoiDung { get; set; }

        [StringLength(100)]
        public string? KyNangHienTai { get; set; }

        [Required]
        [StringLength(50)]
        public string TrangThai { get; set; } = "Đang hoạt động"; // Đang hoạt động, Nghỉ phép, Đang đào tạo

        public int TyLeChuyenCan { get; set; } = 100;

        // Navigations
        public ICollection<PhanCongCuuHo> PhanCongCuuHos { get; set; } = new List<PhanCongCuuHo>();
        public ICollection<DiemDanhCongViec> DiemDanhCongViecs { get; set; } = new List<DiemDanhCongViec>();
    }
}

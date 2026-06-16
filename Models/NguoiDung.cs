using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("NguoiDung")]
    public class NguoiDung
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaNguoiDung { get; set; }

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [StringLength(15)]
        public string? SoDienThoai { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string MatKhauMaHoa { get; set; } = string.Empty;

        public int? NamSinh { get; set; }

        [StringLength(255)]
        public string? DiaChi { get; set; }

        public int DiemTichLuy { get; set; } = 0;

        [StringLength(50)]
        public string TenHang { get; set; } = "Thành viên mới";

        public int MaVaiTro { get; set; }

        [ForeignKey("MaVaiTro")]
        public VaiTro? VaiTro { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public void CapNhatDiemTichLuy(int diemCong)
        {
            DiemTichLuy += diemCong;
            if (DiemTichLuy >= 500) TenHang = "Thành viên Kim Cương";
            else if (DiemTichLuy >= 200) TenHang = "Thành viên Vàng";
            else TenHang = "Thành viên tích cực";
        }

        // Navigations
        public TinhNguyenVien? TinhNguyenVien { get; set; }
        public ICollection<DonNhanNuoi> DonNhanNuois { get; set; } = new List<DonNhanNuoi>();
        public ICollection<ThuCungYeuThich> ThuCungYeuThichs { get; set; } = new List<ThuCungYeuThich>();
        public ICollection<QuyenGop> QuyenGops { get; set; } = new List<QuyenGop>();
        public ICollection<BaiViet> BaiViets { get; set; } = new List<BaiViet>();
        public ICollection<NhatKyHeThong> NhatKyHeThongs { get; set; } = new List<NhatKyHeThong>();
        public ICollection<ChiPhiCuuHo> ChiPhiCuHos { get; set; } = new List<ChiPhiCuuHo>();
        public ICollection<LichSuTrangThai> LichSuTrangThais { get; set; } = new List<LichSuTrangThai>();
    }
}

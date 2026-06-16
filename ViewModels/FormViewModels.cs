using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels
{
    public class RescueReportViewModel
    {
        [StringLength(100)]
        public string? TenNguoiBaoCao { get; set; }

        [StringLength(15)]
        public string? SoDienThoai { get; set; }

        [Required(ErrorMessage = "Địa điểm không được để trống")]
        [StringLength(255)]
        public string DiaDiem { get; set; } = string.Empty;

        [StringLength(50)]
        public string? QuanHuyen { get; set; }

        [Required(ErrorMessage = "Mô tả chi tiết không được để trống")]
        public string MoTaChiTiet { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mức độ khẩn cấp không được để trống")]
        [StringLength(20)]
        public string MucDoKhanCap { get; set; } = "Bình thường"; // Khẩn cấp, Bình thường

        [StringLength(500)]
        public string? AnhBaoCao { get; set; }
    }

    public class VolunteerApplicationViewModel
    {
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [StringLength(15)]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Năm sinh không được để trống")]
        public int NamSinh { get; set; }

        [StringLength(100)]
        public string? NgheNghiep { get; set; }

        [StringLength(255)]
        public string? KyNang { get; set; }
    }

    public class DonationViewModel
    {
        [Required(ErrorMessage = "Số tiền không được để trống")]
        [Range(1000, 1000000000, ErrorMessage = "Số tiền phải tối thiểu từ 1,000 VND")]
        public decimal SoTien { get; set; }

        [Required(ErrorMessage = "Tên quỹ không được để trống")]
        [StringLength(100)]
        public string TenQuyQuyenGop { get; set; } = string.Empty;

        [StringLength(255)]
        public string? LoiNhan { get; set; }

        [StringLength(100)]
        public string? TenNguoiQuyenGop { get; set; }

        public bool AnDanh { get; set; }
    }

    public class AdoptionApplicationViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn thú cưng")]
        public int MaThuCung { get; set; }

        // Step 1: Personal Info
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [StringLength(15)]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Năm sinh không được để trống")]
        public int NamSinh { get; set; }

        [Required(ErrorMessage = "Địa chi không được để trống")]
        [StringLength(255)]
        public string DiaChi { get; set; } = string.Empty;

        // Step 2: Experience & Time
        [Required(ErrorMessage = "Kinh nghiệm nuôi không được để trống")]
        [StringLength(50)]
        public string KinhNghiemNuoi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Thời gian rảnh không được để trống")]
        [StringLength(50)]
        public string ThoiGianRanhMoiNgay { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sự đồng ý của gia đình không được để trống")]
        [StringLength(50)]
        public string SuDongYCoGiaDinh { get; set; } = string.Empty;

        // Step 3: Living Conditions
        [Required(ErrorMessage = "Loại nhà ở không được để trống")]
        [StringLength(100)]
        public string LoaiNhaO { get; set; } = string.Empty;

        public bool CoSanVuon { get; set; }
        public bool CoLuoiBanCong { get; set; }
        public bool CoHangRaoCao { get; set; }
    }
}

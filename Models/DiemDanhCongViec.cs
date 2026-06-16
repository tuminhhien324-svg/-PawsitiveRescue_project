using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("DiemDanhCongViec")]
    public class DiemDanhCongViec
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaChiTietPhanCong { get; set; }

        public int MaCongViec { get; set; }

        [ForeignKey("MaCongViec")]
        public CongViecTaiTram? CongViecTaiTram { get; set; }

        public int MaTinhNguyenVien { get; set; }

        [ForeignKey("MaTinhNguyenVien")]
        public TinhNguyenVien? TinhNguyenVien { get; set; }

        public bool CoMat { get; set; } = true;

        [Required]
        [StringLength(50)]
        public string TrangThai { get; set; } = "Đang chờ"; // Đang chờ, Đã nhận, Hoàn thành
    }
}

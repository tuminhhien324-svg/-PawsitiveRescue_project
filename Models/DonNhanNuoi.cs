using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("DonNhanNuoi")]
    public class DonNhanNuoi
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaDonNhanNuoi { get; set; }

        public int MaNguoiDung { get; set; }

        [ForeignKey("MaNguoiDung")]
        public NguoiDung? NguoiDung { get; set; }

        public int MaThuCung { get; set; }

        [ForeignKey("MaThuCung")]
        public ThuCung? ThuCung { get; set; }

        [Required]
        [StringLength(50)]
        public string TrangThaiDon { get; set; } = "Đang xét duyệt"; // Đang xét duyệt, Phỏng vấn, Hoàn tất, Từ chối

        public DateTime? NgayHenPhongVan { get; set; }

        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        public DateTime NgayTao { get; set; } = DateTime.Now;

        // One-to-one Survey relation
        public KhaoSatDieuKienSong? KhaoSatDieuKienSong { get; set; }
    }
}

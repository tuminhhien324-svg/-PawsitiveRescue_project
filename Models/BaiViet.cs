using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("BaiViet")]
    public class BaiViet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaBaiViet { get; set; }

        public int MaTacGia { get; set; }

        [ForeignKey("MaTacGia")]
        public NguoiDung? TacGia { get; set; }

        [Required]
        [StringLength(255)]
        public string TieuDe { get; set; } = string.Empty;

        [Required]
        public string NoiDung { get; set; } = string.Empty;

        [StringLength(255)]
        public string? DuongDanAnhDaiDien { get; set; }

        public bool LaCauChuyenThanhCong { get; set; } = false;

        public int? MaThuCung { get; set; }

        [ForeignKey("MaThuCung")]
        public ThuCung? ThuCung { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
}

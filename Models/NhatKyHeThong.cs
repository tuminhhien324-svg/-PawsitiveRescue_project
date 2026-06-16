using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("NhatKyHeThong")]
    public class NhatKyHeThong
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaNhatKy { get; set; }

        public int? MaNguoiDung { get; set; }

        [ForeignKey("MaNguoiDung")]
        public NguoiDung? NguoiDung { get; set; }

        [Required]
        public string MoTaHoatDong { get; set; } = string.Empty;

        [StringLength(50)]
        public string? DiaChiIP { get; set; }

        public DateTime NgayGhiNhan { get; set; } = DateTime.Now;
    }
}

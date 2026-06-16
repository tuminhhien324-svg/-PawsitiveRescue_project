using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("AnhThuCung")]
    public class AnhThuCung
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaAnh { get; set; }

        public int MaThuCung { get; set; }

        [ForeignKey("MaThuCung")]
        public ThuCung? ThuCung { get; set; }

        [Required]
        [StringLength(255)]
        public string DuongDanAnh { get; set; } = string.Empty;

        [StringLength(100)]
        public string? GhiChu { get; set; }

        public DateTime NgayTaiLen { get; set; } = DateTime.Now;
    }
}

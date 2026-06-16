using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("ChienDichQuyenGop")]
    public class ChienDichQuyenGop
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaChienDich { get; set; }

        [Required]
        [StringLength(100)]
        public string TenChienDich { get; set; } = string.Empty;

        public string? MoTa { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SoTienMucTieu { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SoTienDaQuyenGop { get; set; } = 0;

        [StringLength(255)]
        public string? AnhChienDich { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
}

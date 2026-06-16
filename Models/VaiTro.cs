using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("VaiTro")]
    public class VaiTro
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaVaiTro { get; set; }

        [Required]
        [StringLength(50)]
        public string TenVaiTro { get; set; } = string.Empty;

        public ICollection<NguoiDung> NguoiDungs { get; set; } = new List<NguoiDung>();
    }
}

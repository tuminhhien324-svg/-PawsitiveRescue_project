using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("ThuCungYeuThich")]
    public class ThuCungYeuThich
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaYeuThich { get; set; }

        public int MaNguoiDung { get; set; }

        [ForeignKey("MaNguoiDung")]
        public NguoiDung? NguoiDung { get; set; }

        public int MaThuCung { get; set; }

        [ForeignKey("MaThuCung")]
        public ThuCung? ThuCung { get; set; }

        public DateTime NgayLuu { get; set; } = DateTime.Now;
    }
}

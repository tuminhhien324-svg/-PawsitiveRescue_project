using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("Newsletter")]
    public class Newsletter
    {
        [Key]
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        public DateTime NgayDangKy { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        public string TrangThai { get; set; } = "Hoạt động";
    }
}

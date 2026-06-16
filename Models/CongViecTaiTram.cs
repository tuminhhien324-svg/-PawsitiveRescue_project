using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("CongViecTaiTram")]
    public class CongViecTaiTram
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaCongViec { get; set; }

        [Required]
        [StringLength(100)]
        public string TenCongViec { get; set; } = string.Empty;

        public TimeSpan ThoiGianBatDau { get; set; }

        public TimeSpan ThoiGianKetThuc { get; set; }

        public int SoLuongTNVYeuCau { get; set; }

        public bool DaHoanThanh { get; set; } = false;

        public DateTime NgayLamViec { get; set; } = DateTime.Today;

        // Navigations
        public ICollection<DiemDanhCongViec> DiemDanhCongViecs { get; set; } = new List<DiemDanhCongViec>();
    }
}

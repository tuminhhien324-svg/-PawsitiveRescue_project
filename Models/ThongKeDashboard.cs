using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("ThongKeDashboard")]
    public class ThongKeDashboard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaThongKe { get; set; }

        public DateTime NgayThongKe { get; set; } = DateTime.Today;

        public int ThangThongKe { get; set; }

        public int NamThongKe { get; set; }

        public int TongThuCung { get; set; } = 0;

        public int TongDonNhanNuoi { get; set; } = 0;

        public int TongBaoCaoCuuHo { get; set; } = 0;

        public int TongTinhNguyenVien { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TongTienQuyenGop { get; set; } = 0;

        public int SoCongViecHoanThanh { get; set; } = 0;

        public int TyLeThamGiaTrungBinh { get; set; } = 0;

        [Column(TypeName = "decimal(5,2)")]
        public decimal TyLeDuyetDon { get; set; } = 0;

        [Column(TypeName = "decimal(5,2)")]
        public decimal TyLeTuChoiDon { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TongChiPhiYTe { get; set; } = 0;

        public int TongLuotKhamBenh { get; set; } = 0;
    }
}

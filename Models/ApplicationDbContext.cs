using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<AnhThuCung> AnhThuCungs { get; set; }
        public DbSet<BaiViet> BaiViets { get; set; }
        public DbSet<BaoCaoCuuHo> BaoCaoCuHos { get; set; }
        public DbSet<ChiPhiCuuHo> ChiPhiCuHos { get; set; }
        public DbSet<ChienDichQuyenGop> ChienDichQuyenGops { get; set; }
        public DbSet<CongViecTaiTram> CongViecTaiTrams { get; set; }
        public DbSet<DiemDanhCongViec> DiemDanhCongViecs { get; set; }
        public DbSet<DonDangKyTinhNguyen> DonDangKyTinhNguyens { get; set; }
        public DbSet<DonNhanNuoi> DonNhanNuois { get; set; }
        public DbSet<KhaoSatDieuKienSong> KhaoSatDieuKienSongs { get; set; }
        public DbSet<LichSuSucKhoe> LichSuSucKhoes { get; set; }
        public DbSet<LichSuTrangThai> LichSuTrangThais { get; set; }
        public DbSet<Newsletter> Newsletters { get; set; }
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<NhatKyHeThong> NhatKyHeThongs { get; set; }
        public DbSet<PhanCongCuuHo> PhanCongCuHos { get; set; }
        public DbSet<QuyenGop> QuyenGops { get; set; }
        public DbSet<ThongKeDashboard> ThongKeDashboards { get; set; }
        public DbSet<ThuCung> ThuCungs { get; set; }
        public DbSet<ThuCungYeuThich> ThuCungYeuThichs { get; set; }
        public DbSet<TinhNguyenVien> TinhNguyenViens { get; set; }
        public DbSet<VaiTro> VaiTros { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // AnhThuCung -> ThuCung
            modelBuilder.Entity<AnhThuCung>()
                .HasOne(a => a.ThuCung)
                .WithMany(p => p.AnhThuCungs)
                .HasForeignKey(a => a.MaThuCung)
                .OnDelete(DeleteBehavior.Cascade);

            // BaiViet -> TacGia (NguoiDung)
            modelBuilder.Entity<BaiViet>()
                .HasOne(b => b.TacGia)
                .WithMany(u => u.BaiViets)
                .HasForeignKey(b => b.MaTacGia)
                .OnDelete(DeleteBehavior.Restrict);

            // BaiViet -> ThuCung
            modelBuilder.Entity<BaiViet>()
                .HasOne(b => b.ThuCung)
                .WithMany(p => p.BaiViets)
                .HasForeignKey(b => b.MaThuCung)
                .OnDelete(DeleteBehavior.SetNull);

            // ChiPhiCuuHo -> ThuCung
            modelBuilder.Entity<ChiPhiCuuHo>()
                .HasOne(c => c.ThuCung)
                .WithMany(p => p.ChiPhiCuuHos)
                .HasForeignKey(c => c.MaThuCung)
                .OnDelete(DeleteBehavior.Cascade);

            // ChiPhiCuuHo -> NguoiDungXacNhan (NguoiDung)
            modelBuilder.Entity<ChiPhiCuuHo>()
                .HasOne(c => c.NguoiDungXacNhan)
                .WithMany(u => u.ChiPhiCuHos)
                .HasForeignKey(c => c.NguoiXacNhan)
                .OnDelete(DeleteBehavior.SetNull);

            // DiemDanhCongViec -> CongViecTaiTram
            modelBuilder.Entity<DiemDanhCongViec>()
                .HasOne(d => d.CongViecTaiTram)
                .WithMany(w => w.DiemDanhCongViecs)
                .HasForeignKey(d => d.MaCongViec)
                .OnDelete(DeleteBehavior.Cascade);

            // DiemDanhCongViec -> TinhNguyenVien
            modelBuilder.Entity<DiemDanhCongViec>()
                .HasOne(d => d.TinhNguyenVien)
                .WithMany(v => v.DiemDanhCongViecs)
                .HasForeignKey(d => d.MaTinhNguyenVien)
                .OnDelete(DeleteBehavior.Cascade);

            // DiemDanhCongViec Unique Constraint
            modelBuilder.Entity<DiemDanhCongViec>()
                .HasIndex(d => new { d.MaCongViec, d.MaTinhNguyenVien })
                .IsUnique();

            // DonNhanNuoi -> NguoiDung
            modelBuilder.Entity<DonNhanNuoi>()
                .HasOne(d => d.NguoiDung)
                .WithMany(u => u.DonNhanNuois)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.Restrict);

            // DonNhanNuoi -> ThuCung
            modelBuilder.Entity<DonNhanNuoi>()
                .HasOne(d => d.ThuCung)
                .WithMany(p => p.DonNhanNuois)
                .HasForeignKey(d => d.MaThuCung)
                .OnDelete(DeleteBehavior.Restrict);

            // KhaoSatDieuKienSong -> DonNhanNuoi
            modelBuilder.Entity<KhaoSatDieuKienSong>()
                .HasOne(k => k.DonNhanNuoi)
                .WithOne(d => d.KhaoSatDieuKienSong)
                .HasForeignKey<KhaoSatDieuKienSong>(k => k.MaDonNhanNuoi)
                .OnDelete(DeleteBehavior.Cascade);

            // LichSuSucKhoe -> ThuCung
            modelBuilder.Entity<LichSuSucKhoe>()
                .HasOne(l => l.ThuCung)
                .WithMany(p => p.LichSuSucKhoes)
                .HasForeignKey(l => l.MaThuCung)
                .OnDelete(DeleteBehavior.Cascade);

            // LichSuTrangThai -> ThuCung
            modelBuilder.Entity<LichSuTrangThai>()
                .HasOne(l => l.ThuCung)
                .WithMany(p => p.LichSuTrangThais)
                .HasForeignKey(l => l.MaThuCung)
                .OnDelete(DeleteBehavior.Cascade);

            // LichSuTrangThai -> NguoiDungThucHien
            modelBuilder.Entity<LichSuTrangThai>()
                .HasOne(l => l.NguoiDungThucHien)
                .WithMany(u => u.LichSuTrangThais)
                .HasForeignKey(l => l.NguoiThucHien)
                .OnDelete(DeleteBehavior.SetNull);

            // NguoiDung -> VaiTro
            modelBuilder.Entity<NguoiDung>()
                .HasOne(n => n.VaiTro)
                .WithMany(v => v.NguoiDungs)
                .HasForeignKey(n => n.MaVaiTro)
                .OnDelete(DeleteBehavior.Restrict);

            // NhatKyHeThong -> NguoiDung
            modelBuilder.Entity<NhatKyHeThong>()
                .HasOne(n => n.NguoiDung)
                .WithMany(u => u.NhatKyHeThongs)
                .HasForeignKey(n => n.MaNguoiDung)
                .OnDelete(DeleteBehavior.SetNull);

            // PhanCongCuuHo -> BaoCaoCuuHo
            modelBuilder.Entity<PhanCongCuuHo>()
                .HasOne(p => p.BaoCaoCuuHo)
                .WithMany(b => b.PhanCongCuuHos)
                .HasForeignKey(p => p.MaBaoCao)
                .OnDelete(DeleteBehavior.Cascade);

            // PhanCongCuuHo -> TinhNguyenVien
            modelBuilder.Entity<PhanCongCuuHo>()
                .HasOne(p => p.TinhNguyenVien)
                .WithMany(v => v.PhanCongCuuHos)
                .HasForeignKey(p => p.MaTinhNguyenVien)
                .OnDelete(DeleteBehavior.Restrict);

            // PhanCongCuuHo Unique Constraint
            modelBuilder.Entity<PhanCongCuuHo>()
                .HasIndex(p => new { p.MaBaoCao, p.MaTinhNguyenVien })
                .IsUnique();

            // QuyenGop -> NguoiDung
            modelBuilder.Entity<QuyenGop>()
                .HasOne(q => q.NguoiDung)
                .WithMany(u => u.QuyenGops)
                .HasForeignKey(q => q.MaNguoiDung)
                .OnDelete(DeleteBehavior.SetNull);

            // ThuCungYeuThich -> NguoiDung
            modelBuilder.Entity<ThuCungYeuThich>()
                .HasOne(t => t.NguoiDung)
                .WithMany(u => u.ThuCungYeuThichs)
                .HasForeignKey(t => t.MaNguoiDung)
                .OnDelete(DeleteBehavior.Cascade);

            // ThuCungYeuThich -> ThuCung
            modelBuilder.Entity<ThuCungYeuThich>()
                .HasOne(t => t.ThuCung)
                .WithMany(p => p.ThuCungYeuThichs)
                .HasForeignKey(t => t.MaThuCung)
                .OnDelete(DeleteBehavior.Cascade);

            // ThuCungYeuThich Unique Constraint
            modelBuilder.Entity<ThuCungYeuThich>()
                .HasIndex(t => new { t.MaNguoiDung, t.MaThuCung })
                .IsUnique();

            // TinhNguyenVien -> NguoiDung
            modelBuilder.Entity<TinhNguyenVien>()
                .HasOne(t => t.NguoiDung)
                .WithOne(u => u.TinhNguyenVien)
                .HasForeignKey<TinhNguyenVien>(t => t.MaNguoiDung)
                .OnDelete(DeleteBehavior.Cascade);

            // ThongKeDashboard Index
            modelBuilder.Entity<ThongKeDashboard>()
                .HasIndex(t => t.NgayThongKe)
                .IsUnique();
        }
    }
}
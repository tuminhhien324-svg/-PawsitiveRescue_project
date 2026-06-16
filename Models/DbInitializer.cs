using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Models
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context, PasswordHasher<NguoiDung> passwordHasher)
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // Ensure new columns exist in ThuCung table
            try
            {
                context.Database.ExecuteSqlRaw(@"
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[ThuCung]') AND name = 'DacDiem')
                    BEGIN
                        ALTER TABLE [ThuCung] ADD [DacDiem] nvarchar(255) NULL;
                    END
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[ThuCung]') AND name = 'TiemNgua')
                    BEGIN
                        ALTER TABLE [ThuCung] ADD [TiemNgua] nvarchar(100) NULL;
                    END
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[ThuCung]') AND name = 'SoThich')
                    BEGIN
                        ALTER TABLE [ThuCung] ADD [SoThich] nvarchar(255) NULL;
                    END
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[DiemDanhCongViec]') AND name = 'TrangThai')
                    BEGIN
                        ALTER TABLE [DiemDanhCongViec] ADD [TrangThai] nvarchar(50) NOT NULL DEFAULT N'Đang chờ';
                    END
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[BaoCaoCuuHo]') AND name = 'AnhBaoCao')
                    BEGIN
                        ALTER TABLE [BaoCaoCuuHo] ADD [AnhBaoCao] nvarchar(500) NULL;
                    END
                ");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error altering ThuCung / DiemDanhCongViec table: " + ex.Message);
            }

            // 1. Seed Roles if missing
            if (!context.VaiTros.Any())
            {
                context.VaiTros.AddRange(
                    new VaiTro { TenVaiTro = "Quản trị viên" },
                    new VaiTro { TenVaiTro = "Tình nguyện viên" },
                    new VaiTro { TenVaiTro = "Người dùng phổ thông" }
                );
                context.SaveChanges();
            }

            var adminRole = context.VaiTros.FirstOrDefault(r => r.TenVaiTro == "Quản trị viên");
            var userRole = context.VaiTros.FirstOrDefault(r => r.TenVaiTro == "Người dùng phổ thông");

            // 2. Seed Admin User if missing
            if (!context.NguoiDungs.Any(u => u.Email == "admin@pawsitive.vn"))
            {
                var admin = new NguoiDung
                {
                    HoTen = "Quản trị viên Hệ thống",
                    Email = "admin@pawsitive.vn",
                    SoDienThoai = "0123456789",
                    NamSinh = 1990,
                    DiaChi = "Trạm Cứu Hộ Pawsitive, Hà Nội",
                    DiemTichLuy = 999,
                    TenHang = "Quản trị viên",
                    MaVaiTro = adminRole?.MaVaiTro ?? 1,
                    NgayTao = DateTime.Now
                };
                admin.MatKhauMaHoa = passwordHasher.HashPassword(admin, "admin123");
                context.NguoiDungs.Add(admin);
                context.SaveChanges();
            }
        }
    }
}

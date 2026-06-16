using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BaoCaoCuuHo",
                columns: table => new
                {
                    MaBaoCao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenNguoiBaoCao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    DiaDiem = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    QuanHuyen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MoTaChiTiet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MucDoKhanCap = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayBaoCao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaoCaoCuuHo", x => x.MaBaoCao);
                });

            migrationBuilder.CreateTable(
                name: "ChienDichQuyenGop",
                columns: table => new
                {
                    MaChienDich = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenChienDich = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoTienMucTieu = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SoTienDaQuyenGop = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AnhChienDich = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChienDichQuyenGop", x => x.MaChienDich);
                });

            migrationBuilder.CreateTable(
                name: "CongViecTaiTram",
                columns: table => new
                {
                    MaCongViec = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenCongViec = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ThoiGianBatDau = table.Column<TimeSpan>(type: "time", nullable: false),
                    ThoiGianKetThuc = table.Column<TimeSpan>(type: "time", nullable: false),
                    SoLuongTNVYeuCau = table.Column<int>(type: "int", nullable: false),
                    DaHoanThanh = table.Column<bool>(type: "bit", nullable: false),
                    NgayLamViec = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CongViecTaiTram", x => x.MaCongViec);
                });

            migrationBuilder.CreateTable(
                name: "DonDangKyTinhNguyen",
                columns: table => new
                {
                    MaDonDangKy = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    NamSinh = table.Column<int>(type: "int", nullable: false),
                    NgheNghiep = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    KyNang = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonDangKyTinhNguyen", x => x.MaDonDangKy);
                });

            migrationBuilder.CreateTable(
                name: "Newsletter",
                columns: table => new
                {
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgayDangKy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Newsletter", x => x.Email);
                });

            migrationBuilder.CreateTable(
                name: "ThongKeDashboard",
                columns: table => new
                {
                    MaThongKe = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NgayThongKe = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThangThongKe = table.Column<int>(type: "int", nullable: false),
                    NamThongKe = table.Column<int>(type: "int", nullable: false),
                    TongThuCung = table.Column<int>(type: "int", nullable: false),
                    TongDonNhanNuoi = table.Column<int>(type: "int", nullable: false),
                    TongBaoCaoCuuHo = table.Column<int>(type: "int", nullable: false),
                    TongTinhNguyenVien = table.Column<int>(type: "int", nullable: false),
                    TongTienQuyenGop = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SoCongViecHoanThanh = table.Column<int>(type: "int", nullable: false),
                    TyLeThamGiaTrungBinh = table.Column<int>(type: "int", nullable: false),
                    TyLeDuyetDon = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    TyLeTuChoiDon = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    TongChiPhiYTe = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TongLuotKhamBenh = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongKeDashboard", x => x.MaThongKe);
                });

            migrationBuilder.CreateTable(
                name: "ThuCung",
                columns: table => new
                {
                    MaThuCung = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenThuCung = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LoaiNguonGoc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GiongThuCung = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GiaTriTuoi = table.Column<decimal>(type: "decimal(3,1)", nullable: true),
                    DonViTuoi = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnhChinh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TinhTrangSucKhoe = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TrangThaiCuuHo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TrangThaiNhanNuoi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayCuuHo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThuCung", x => x.MaThuCung);
                });

            migrationBuilder.CreateTable(
                name: "VaiTro",
                columns: table => new
                {
                    MaVaiTro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenVaiTro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaiTro", x => x.MaVaiTro);
                });

            migrationBuilder.CreateTable(
                name: "AnhThuCung",
                columns: table => new
                {
                    MaAnh = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaThuCung = table.Column<int>(type: "int", nullable: false),
                    DuongDanAnh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NgayTaiLen = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnhThuCung", x => x.MaAnh);
                    table.ForeignKey(
                        name: "FK_AnhThuCung_ThuCung_MaThuCung",
                        column: x => x.MaThuCung,
                        principalTable: "ThuCung",
                        principalColumn: "MaThuCung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LichSuSucKhoe",
                columns: table => new
                {
                    MaLichSuSK = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaThuCung = table.Column<int>(type: "int", nullable: false),
                    NgayKham = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoaiHinh = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ChuanDoan = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    GhiChuBenhAn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BacSiThuY = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuSucKhoe", x => x.MaLichSuSK);
                    table.ForeignKey(
                        name: "FK_LichSuSucKhoe_ThuCung_MaThuCung",
                        column: x => x.MaThuCung,
                        principalTable: "ThuCung",
                        principalColumn: "MaThuCung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDung",
                columns: table => new
                {
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MatKhauMaHoa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NamSinh = table.Column<int>(type: "int", nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DiemTichLuy = table.Column<int>(type: "int", nullable: false),
                    TenHang = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaVaiTro = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDung", x => x.MaNguoiDung);
                    table.ForeignKey(
                        name: "FK_NguoiDung_VaiTro_MaVaiTro",
                        column: x => x.MaVaiTro,
                        principalTable: "VaiTro",
                        principalColumn: "MaVaiTro",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BaiViet",
                columns: table => new
                {
                    MaBaiViet = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTacGia = table.Column<int>(type: "int", nullable: false),
                    TieuDe = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DuongDanAnhDaiDien = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    LaCauChuyenThanhCong = table.Column<bool>(type: "bit", nullable: false),
                    MaThuCung = table.Column<int>(type: "int", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaiViet", x => x.MaBaiViet);
                    table.ForeignKey(
                        name: "FK_BaiViet_NguoiDung_MaTacGia",
                        column: x => x.MaTacGia,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BaiViet_ThuCung_MaThuCung",
                        column: x => x.MaThuCung,
                        principalTable: "ThuCung",
                        principalColumn: "MaThuCung",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ChiPhiCuuHo",
                columns: table => new
                {
                    MaChiPhi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaThuCung = table.Column<int>(type: "int", nullable: false),
                    NgayChiTieu = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoaiChiPhi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MucDichChiTiet = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NguoiXacNhan = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiPhiCuuHo", x => x.MaChiPhi);
                    table.ForeignKey(
                        name: "FK_ChiPhiCuuHo_NguoiDung_NguoiXacNhan",
                        column: x => x.NguoiXacNhan,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ChiPhiCuuHo_ThuCung_MaThuCung",
                        column: x => x.MaThuCung,
                        principalTable: "ThuCung",
                        principalColumn: "MaThuCung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DonNhanNuoi",
                columns: table => new
                {
                    MaDonNhanNuoi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    MaThuCung = table.Column<int>(type: "int", nullable: false),
                    TrangThaiDon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayHenPhongVan = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonNhanNuoi", x => x.MaDonNhanNuoi);
                    table.ForeignKey(
                        name: "FK_DonNhanNuoi_NguoiDung_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DonNhanNuoi_ThuCung_MaThuCung",
                        column: x => x.MaThuCung,
                        principalTable: "ThuCung",
                        principalColumn: "MaThuCung",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LichSuTrangThai",
                columns: table => new
                {
                    MaLichSuTT = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaThuCung = table.Column<int>(type: "int", nullable: false),
                    TrangThaiCuuHoTruoc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TrangThaiCuuHoSau = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TrangThaiNhanNuoiTruoc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TrangThaiNhanNuoiSau = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GhiChuThayDoi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NguoiThucHien = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuTrangThai", x => x.MaLichSuTT);
                    table.ForeignKey(
                        name: "FK_LichSuTrangThai_NguoiDung_NguoiThucHien",
                        column: x => x.NguoiThucHien,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LichSuTrangThai_ThuCung_MaThuCung",
                        column: x => x.MaThuCung,
                        principalTable: "ThuCung",
                        principalColumn: "MaThuCung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NhatKyHeThong",
                columns: table => new
                {
                    MaNhatKy = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: true),
                    MoTaHoatDong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiaChiIP = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NgayGhiNhan = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhatKyHeThong", x => x.MaNhatKy);
                    table.ForeignKey(
                        name: "FK_NhatKyHeThong_NguoiDung_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "QuyenGop",
                columns: table => new
                {
                    MaQuyenGop = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: true),
                    TenNguoiQuyenGop = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TenQuyQuyenGop = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LoiNhan = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NgayQuyenGop = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuyenGop", x => x.MaQuyenGop);
                    table.ForeignKey(
                        name: "FK_QuyenGop_NguoiDung_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ThuCungYeuThich",
                columns: table => new
                {
                    MaYeuThich = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    MaThuCung = table.Column<int>(type: "int", nullable: false),
                    NgayLuu = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThuCungYeuThich", x => x.MaYeuThich);
                    table.ForeignKey(
                        name: "FK_ThuCungYeuThich_NguoiDung_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThuCungYeuThich_ThuCung_MaThuCung",
                        column: x => x.MaThuCung,
                        principalTable: "ThuCung",
                        principalColumn: "MaThuCung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TinhNguyenVien",
                columns: table => new
                {
                    MaTinhNguyenVien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    KyNangHienTai = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TyLeChuyenCan = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TinhNguyenVien", x => x.MaTinhNguyenVien);
                    table.ForeignKey(
                        name: "FK_TinhNguyenVien_NguoiDung_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KhaoSatDieuKienSong",
                columns: table => new
                {
                    MaKhaoSat = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDonNhanNuoi = table.Column<int>(type: "int", nullable: false),
                    KinhNghiemNuoi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ThoiGianRanhMoiNgay = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SuDongYCoGiaDinh = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LoaiNhaO = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CoSanVuon = table.Column<bool>(type: "bit", nullable: false),
                    CoLuoiBanCong = table.Column<bool>(type: "bit", nullable: false),
                    CoHangRaoCao = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhaoSatDieuKienSong", x => x.MaKhaoSat);
                    table.ForeignKey(
                        name: "FK_KhaoSatDieuKienSong_DonNhanNuoi_MaDonNhanNuoi",
                        column: x => x.MaDonNhanNuoi,
                        principalTable: "DonNhanNuoi",
                        principalColumn: "MaDonNhanNuoi",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiemDanhCongViec",
                columns: table => new
                {
                    MaChiTietPhanCong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaCongViec = table.Column<int>(type: "int", nullable: false),
                    MaTinhNguyenVien = table.Column<int>(type: "int", nullable: false),
                    CoMat = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiemDanhCongViec", x => x.MaChiTietPhanCong);
                    table.ForeignKey(
                        name: "FK_DiemDanhCongViec_CongViecTaiTram_MaCongViec",
                        column: x => x.MaCongViec,
                        principalTable: "CongViecTaiTram",
                        principalColumn: "MaCongViec",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiemDanhCongViec_TinhNguyenVien_MaTinhNguyenVien",
                        column: x => x.MaTinhNguyenVien,
                        principalTable: "TinhNguyenVien",
                        principalColumn: "MaTinhNguyenVien",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhanCongCuuHo",
                columns: table => new
                {
                    MaPhanCong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaBaoCao = table.Column<int>(type: "int", nullable: false),
                    MaTinhNguyenVien = table.Column<int>(type: "int", nullable: false),
                    NgayPhanCong = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanCongCuuHo", x => x.MaPhanCong);
                    table.ForeignKey(
                        name: "FK_PhanCongCuuHo_BaoCaoCuuHo_MaBaoCao",
                        column: x => x.MaBaoCao,
                        principalTable: "BaoCaoCuuHo",
                        principalColumn: "MaBaoCao",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhanCongCuuHo_TinhNguyenVien_MaTinhNguyenVien",
                        column: x => x.MaTinhNguyenVien,
                        principalTable: "TinhNguyenVien",
                        principalColumn: "MaTinhNguyenVien",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnhThuCung_MaThuCung",
                table: "AnhThuCung",
                column: "MaThuCung");

            migrationBuilder.CreateIndex(
                name: "IX_BaiViet_MaTacGia",
                table: "BaiViet",
                column: "MaTacGia");

            migrationBuilder.CreateIndex(
                name: "IX_BaiViet_MaThuCung",
                table: "BaiViet",
                column: "MaThuCung");

            migrationBuilder.CreateIndex(
                name: "IX_ChiPhiCuuHo_MaThuCung",
                table: "ChiPhiCuuHo",
                column: "MaThuCung");

            migrationBuilder.CreateIndex(
                name: "IX_ChiPhiCuuHo_NguoiXacNhan",
                table: "ChiPhiCuuHo",
                column: "NguoiXacNhan");

            migrationBuilder.CreateIndex(
                name: "IX_DiemDanhCongViec_MaCongViec_MaTinhNguyenVien",
                table: "DiemDanhCongViec",
                columns: new[] { "MaCongViec", "MaTinhNguyenVien" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiemDanhCongViec_MaTinhNguyenVien",
                table: "DiemDanhCongViec",
                column: "MaTinhNguyenVien");

            migrationBuilder.CreateIndex(
                name: "IX_DonNhanNuoi_MaNguoiDung",
                table: "DonNhanNuoi",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_DonNhanNuoi_MaThuCung",
                table: "DonNhanNuoi",
                column: "MaThuCung");

            migrationBuilder.CreateIndex(
                name: "IX_KhaoSatDieuKienSong_MaDonNhanNuoi",
                table: "KhaoSatDieuKienSong",
                column: "MaDonNhanNuoi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LichSuSucKhoe_MaThuCung",
                table: "LichSuSucKhoe",
                column: "MaThuCung");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuTrangThai_MaThuCung",
                table: "LichSuTrangThai",
                column: "MaThuCung");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuTrangThai_NguoiThucHien",
                table: "LichSuTrangThai",
                column: "NguoiThucHien");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_MaVaiTro",
                table: "NguoiDung",
                column: "MaVaiTro");

            migrationBuilder.CreateIndex(
                name: "IX_NhatKyHeThong_MaNguoiDung",
                table: "NhatKyHeThong",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_PhanCongCuuHo_MaBaoCao_MaTinhNguyenVien",
                table: "PhanCongCuuHo",
                columns: new[] { "MaBaoCao", "MaTinhNguyenVien" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhanCongCuuHo_MaTinhNguyenVien",
                table: "PhanCongCuuHo",
                column: "MaTinhNguyenVien");

            migrationBuilder.CreateIndex(
                name: "IX_QuyenGop_MaNguoiDung",
                table: "QuyenGop",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_ThongKeDashboard_NgayThongKe",
                table: "ThongKeDashboard",
                column: "NgayThongKe",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThuCungYeuThich_MaNguoiDung_MaThuCung",
                table: "ThuCungYeuThich",
                columns: new[] { "MaNguoiDung", "MaThuCung" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThuCungYeuThich_MaThuCung",
                table: "ThuCungYeuThich",
                column: "MaThuCung");

            migrationBuilder.CreateIndex(
                name: "IX_TinhNguyenVien_MaNguoiDung",
                table: "TinhNguyenVien",
                column: "MaNguoiDung",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnhThuCung");

            migrationBuilder.DropTable(
                name: "BaiViet");

            migrationBuilder.DropTable(
                name: "ChienDichQuyenGop");

            migrationBuilder.DropTable(
                name: "ChiPhiCuuHo");

            migrationBuilder.DropTable(
                name: "DiemDanhCongViec");

            migrationBuilder.DropTable(
                name: "DonDangKyTinhNguyen");

            migrationBuilder.DropTable(
                name: "KhaoSatDieuKienSong");

            migrationBuilder.DropTable(
                name: "LichSuSucKhoe");

            migrationBuilder.DropTable(
                name: "LichSuTrangThai");

            migrationBuilder.DropTable(
                name: "Newsletter");

            migrationBuilder.DropTable(
                name: "NhatKyHeThong");

            migrationBuilder.DropTable(
                name: "PhanCongCuuHo");

            migrationBuilder.DropTable(
                name: "QuyenGop");

            migrationBuilder.DropTable(
                name: "ThongKeDashboard");

            migrationBuilder.DropTable(
                name: "ThuCungYeuThich");

            migrationBuilder.DropTable(
                name: "CongViecTaiTram");

            migrationBuilder.DropTable(
                name: "DonNhanNuoi");

            migrationBuilder.DropTable(
                name: "BaoCaoCuuHo");

            migrationBuilder.DropTable(
                name: "TinhNguyenVien");

            migrationBuilder.DropTable(
                name: "ThuCung");

            migrationBuilder.DropTable(
                name: "NguoiDung");

            migrationBuilder.DropTable(
                name: "VaiTro");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DacDiem",
                table: "ThuCung",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SoThich",
                table: "ThuCung",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TiemNgua",
                table: "ThuCung",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrangThai",
                table: "DiemDanhCongViec",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AnhBaoCao",
                table: "BaoCaoCuuHo",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DacDiem",
                table: "ThuCung");

            migrationBuilder.DropColumn(
                name: "SoThich",
                table: "ThuCung");

            migrationBuilder.DropColumn(
                name: "TiemNgua",
                table: "ThuCung");

            migrationBuilder.DropColumn(
                name: "TrangThai",
                table: "DiemDanhCongViec");

            migrationBuilder.DropColumn(
                name: "AnhBaoCao",
                table: "BaoCaoCuuHo");
        }
    }
}

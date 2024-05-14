using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gest_hotel.web.Migrations
{
    /// <inheritdoc />
    public partial class daba : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageFile",
                table: "Chambres");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Chambres",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Chambres");

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageFile",
                table: "Chambres",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}

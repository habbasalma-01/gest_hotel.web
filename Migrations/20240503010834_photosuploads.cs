using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gest_hotel.web.Migrations
{
    /// <inheritdoc />
    public partial class photosuploads : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "Chambres",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "Chambres");
        }
    }
}

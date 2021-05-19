using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseProject.Migrations
{
    public partial class DealersBrands : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BrandId",
                table: "Dealers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Dealers_BrandId",
                table: "Dealers",
                column: "BrandId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dealers_Brand_BrandId",
                table: "Dealers",
                column: "BrandId",
                principalTable: "Brand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dealers_Brand_BrandId",
                table: "Dealers");

            migrationBuilder.DropIndex(
                name: "IX_Dealers_BrandId",
                table: "Dealers");

            migrationBuilder.DropColumn(
                name: "BrandId",
                table: "Dealers");
        }
    }
}

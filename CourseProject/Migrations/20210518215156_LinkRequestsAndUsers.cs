using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseProject.Migrations
{
    public partial class LinkRequestsAndUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "PurchaseRequest",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagerId",
                table: "PurchaseRequest",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequest_ClientId",
                table: "PurchaseRequest",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequest_ManagerId",
                table: "PurchaseRequest",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseRequest_AspNetUsers_ClientId",
                table: "PurchaseRequest",
                column: "ClientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseRequest_AspNetUsers_ManagerId",
                table: "PurchaseRequest",
                column: "ManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseRequest_AspNetUsers_ClientId",
                table: "PurchaseRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseRequest_AspNetUsers_ManagerId",
                table: "PurchaseRequest");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseRequest_ClientId",
                table: "PurchaseRequest");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseRequest_ManagerId",
                table: "PurchaseRequest");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "PurchaseRequest");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "PurchaseRequest");
        }
    }
}

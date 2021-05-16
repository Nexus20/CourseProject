using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseProject.Migrations
{
    public partial class OptionalTransmission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Car_TransmissionType_TransmissionTypeId",
                table: "Car");

            migrationBuilder.AlterColumn<int>(
                name: "TransmissionTypeId",
                table: "Car",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Car_TransmissionType_TransmissionTypeId",
                table: "Car",
                column: "TransmissionTypeId",
                principalTable: "TransmissionType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Car_TransmissionType_TransmissionTypeId",
                table: "Car");

            migrationBuilder.AlterColumn<int>(
                name: "TransmissionTypeId",
                table: "Car",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Car_TransmissionType_TransmissionTypeId",
                table: "Car",
                column: "TransmissionTypeId",
                principalTable: "TransmissionType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

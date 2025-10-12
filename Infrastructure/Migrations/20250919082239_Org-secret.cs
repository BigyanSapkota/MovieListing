using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Orgsecret : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "PaymentTransaction",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId",
                table: "PaymentTransaction",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecretKey",
                table: "Organizations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransaction_OrganizationId",
                table: "PaymentTransaction",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransaction_UserId",
                table: "PaymentTransaction",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTransaction_Organizations_OrganizationId",
                table: "PaymentTransaction",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTransaction_Users_UserId",
                table: "PaymentTransaction",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransaction_Organizations_OrganizationId",
                table: "PaymentTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransaction_Users_UserId",
                table: "PaymentTransaction");

            migrationBuilder.DropIndex(
                name: "IX_PaymentTransaction_OrganizationId",
                table: "PaymentTransaction");

            migrationBuilder.DropIndex(
                name: "IX_PaymentTransaction_UserId",
                table: "PaymentTransaction");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "PaymentTransaction");

            migrationBuilder.DropColumn(
                name: "SecretKey",
                table: "Organizations");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "PaymentTransaction",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeleteRequestApprove : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeleteRequest",
                columns: table => new
                {
                    DeleteRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MovieId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestedByAdminId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApprovedCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeleteRequest", x => x.DeleteRequestId);
                });

            migrationBuilder.CreateTable(
                name: "DeleteApproval",
                columns: table => new
                {
                    ApprovalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeleteRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovedByAdminId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeleteApproval", x => x.ApprovalId);
                    table.ForeignKey(
                        name: "FK_DeleteApproval_DeleteRequest_DeleteRequestId",
                        column: x => x.DeleteRequestId,
                        principalTable: "DeleteRequest",
                        principalColumn: "DeleteRequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeleteApproval_DeleteRequestId",
                table: "DeleteApproval",
                column: "DeleteRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeleteApproval");

            migrationBuilder.DropTable(
                name: "DeleteRequest");
        }
    }
}

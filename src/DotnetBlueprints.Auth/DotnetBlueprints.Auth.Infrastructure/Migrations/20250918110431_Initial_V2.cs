using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotnetBlueprints.Auth.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial_V2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Companies_CompanyId",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Roles_CompanyId",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Roles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "Roles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_CompanyId",
                table: "Roles",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Companies_CompanyId",
                table: "Roles",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");
        }
    }
}

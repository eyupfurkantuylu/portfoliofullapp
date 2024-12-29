using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Profiles_ProfileId",
                table: "Contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_NavbarItems_Profiles_ProfileId",
                table: "NavbarItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Skill_Profiles_ProfileId",
                table: "Skill");

            migrationBuilder.DropIndex(
                name: "IX_Skill_ProfileId",
                table: "Skill");

            migrationBuilder.DropIndex(
                name: "IX_NavbarItems_ProfileId",
                table: "NavbarItems");

            migrationBuilder.DropIndex(
                name: "IX_Contacts_ProfileId",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "ProfileId",
                table: "Skill");

            migrationBuilder.DropColumn(
                name: "ProfileId",
                table: "NavbarItems");

            migrationBuilder.DropColumn(
                name: "ProfileId",
                table: "Contacts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfileId",
                table: "Skill",
                type: "nvarchar(36)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProfileId",
                table: "NavbarItems",
                type: "nvarchar(36)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProfileId",
                table: "Contacts",
                type: "nvarchar(36)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Skill_ProfileId",
                table: "Skill",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_NavbarItems_ProfileId",
                table: "NavbarItems",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_ProfileId",
                table: "Contacts",
                column: "ProfileId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Profiles_ProfileId",
                table: "Contacts",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NavbarItems_Profiles_ProfileId",
                table: "NavbarItems",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Skill_Profiles_ProfileId",
                table: "Skill",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

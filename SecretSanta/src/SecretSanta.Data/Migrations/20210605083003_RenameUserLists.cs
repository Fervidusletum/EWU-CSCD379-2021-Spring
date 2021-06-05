using Microsoft.EntityFrameworkCore.Migrations;

namespace SecretSanta.Data.Migrations
{
    public partial class RenameUserLists : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupUser_Groups_UserGroupsId",
                table: "GroupUser");

            migrationBuilder.RenameColumn(
                name: "UserGroupsId",
                table: "GroupUser",
                newName: "GroupsId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupUser_Groups_GroupsId",
                table: "GroupUser",
                column: "GroupsId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupUser_Groups_GroupsId",
                table: "GroupUser");

            migrationBuilder.RenameColumn(
                name: "GroupsId",
                table: "GroupUser",
                newName: "UserGroupsId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupUser_Groups_UserGroupsId",
                table: "GroupUser",
                column: "UserGroupsId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

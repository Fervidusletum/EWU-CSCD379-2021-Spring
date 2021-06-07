using Microsoft.EntityFrameworkCore.Migrations;

namespace SecretSanta.Data.Migrations
{
    public partial class ChangeAltKeysToIndices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Groups_groupId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Users_GiverId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Users_ReceiverId",
                table: "Assignments");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Users_FirstName_LastName",
                table: "Users");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Groups_Name",
                table: "Groups");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Gifts_Title",
                table: "Gifts");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_groupId",
                table: "Assignments");

            migrationBuilder.RenameColumn(
                name: "groupId",
                table: "Assignments",
                newName: "AssignmentGroupId");

            migrationBuilder.AlterColumn<int>(
                name: "ReceiverId",
                table: "Assignments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GiverId",
                table: "Assignments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_FirstName_LastName",
                table: "Users",
                columns: new[] { "FirstName", "LastName" });

            migrationBuilder.CreateIndex(
                name: "IX_Groups_Name",
                table: "Groups",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Gifts_Title_ReceiverId",
                table: "Gifts",
                columns: new[] { "Title", "ReceiverId" });

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_AssignmentGroupId_GiverId_ReceiverId",
                table: "Assignments",
                columns: new[] { "AssignmentGroupId", "GiverId", "ReceiverId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Groups_AssignmentGroupId",
                table: "Assignments",
                column: "AssignmentGroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Users_GiverId",
                table: "Assignments",
                column: "GiverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Users_ReceiverId",
                table: "Assignments",
                column: "ReceiverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Groups_AssignmentGroupId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Users_GiverId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Users_ReceiverId",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Users_FirstName_LastName",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Groups_Name",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Gifts_Title_ReceiverId",
                table: "Gifts");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_AssignmentGroupId_GiverId_ReceiverId",
                table: "Assignments");

            migrationBuilder.RenameColumn(
                name: "AssignmentGroupId",
                table: "Assignments",
                newName: "groupId");

            migrationBuilder.AlterColumn<int>(
                name: "ReceiverId",
                table: "Assignments",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "GiverId",
                table: "Assignments",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Users_FirstName_LastName",
                table: "Users",
                columns: new[] { "FirstName", "LastName" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Groups_Name",
                table: "Groups",
                column: "Name");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Gifts_Title",
                table: "Gifts",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_groupId",
                table: "Assignments",
                column: "groupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Groups_groupId",
                table: "Assignments",
                column: "groupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Users_GiverId",
                table: "Assignments",
                column: "GiverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Users_ReceiverId",
                table: "Assignments",
                column: "ReceiverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

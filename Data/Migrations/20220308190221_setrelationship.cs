using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assignment_QnAWeb.Data.Migrations
{
    public partial class setrelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnswerVote_Answer_AnswerId",
                table: "AnswerVote");

            migrationBuilder.AddColumn<int>(
                name: "QuestionId",
                table: "QuestionVote",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AnswerId",
                table: "AnswerVote",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionVote_QuestionId",
                table: "QuestionVote",
                column: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerVote_Answer_AnswerId",
                table: "AnswerVote",
                column: "AnswerId",
                principalTable: "Answer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionVote_Question_QuestionId",
                table: "QuestionVote",
                column: "QuestionId",
                principalTable: "Question",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnswerVote_Answer_AnswerId",
                table: "AnswerVote");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionVote_Question_QuestionId",
                table: "QuestionVote");

            migrationBuilder.DropIndex(
                name: "IX_QuestionVote_QuestionId",
                table: "QuestionVote");

            migrationBuilder.DropColumn(
                name: "QuestionId",
                table: "QuestionVote");

            migrationBuilder.AlterColumn<int>(
                name: "AnswerId",
                table: "AnswerVote",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerVote_Answer_AnswerId",
                table: "AnswerVote",
                column: "AnswerId",
                principalTable: "Answer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

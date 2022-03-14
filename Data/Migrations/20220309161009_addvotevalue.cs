using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assignment_QnAWeb.Data.Migrations
{
    public partial class addvotevalue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VoteValue",
                table: "QuestionVote",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VoteValue",
                table: "AnswerVote",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VoteValue",
                table: "QuestionVote");

            migrationBuilder.DropColumn(
                name: "VoteValue",
                table: "AnswerVote");
        }
    }
}

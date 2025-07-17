using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog.Migrations
{
    /// <inheritdoc />
    public partial class AddPostLikesRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<string>(
            //    name: "profilepicture",
            //    table: "users",
            //    type: "text",
            //    nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_likes_postid",
                table: "likes",
                column: "postid");

            migrationBuilder.AddForeignKey(
                name: "FK_likes_posts_postid",
                table: "likes",
                column: "postid",
                principalTable: "posts",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_likes_posts_postid",
                table: "likes");

            migrationBuilder.DropIndex(
                name: "IX_likes_postid",
                table: "likes");

            migrationBuilder.DropColumn(
                name: "profilepicture",
                table: "users");
        }
    }
}

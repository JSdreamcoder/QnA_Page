using Assignment_QnAWeb.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Assignment_QnAWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //public DbSet<AppUser> AppUser { get; set; }
        public DbSet<Answer> Answer { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<Tag> Tag { get; set; }
        public DbSet<QuestionTag> QuestionTag { get; set; }

        public DbSet<QuestionVote> QuestionVote { get; set; }
        public DbSet<AnswerVote> AnswerVote { get; set; }

        public DbSet<Comment> Comment { get; set; }
    }
}
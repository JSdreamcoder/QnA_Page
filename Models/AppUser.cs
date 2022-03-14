using Microsoft.AspNetCore.Identity;

namespace Assignment_QnAWeb.Models
{
    public class AppUser : IdentityUser
    {
        public int Reputation { get; set; } = 0;

        public ICollection<Question> Questions { get; set; }
        public ICollection<Answer> Answers { get; set; }
        public ICollection<QuestionVote> QuestionVotes { get; set; }
        public ICollection<AnswerVote> AnswerVotes { get; set; }

        public AppUser()
        {
            Questions = new HashSet<Question>();
            Answers = new HashSet<Answer>();
            AnswerVotes = new HashSet<AnswerVote>();
            QuestionVotes = new HashSet<QuestionVote>();
        }
    }
}

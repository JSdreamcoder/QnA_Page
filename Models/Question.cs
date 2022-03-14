namespace Assignment_QnAWeb.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }

        public string Content { get; set; }

        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public int Vote { get; set; } = 0;
        

        public ICollection<Answer> Answers { get; set; }
        public ICollection<QuestionVote> QuestionVotes { get; set; }
        public ICollection<QuestionTag> QuestiongTags { get; set; }
        public ICollection<Comment> Comments { get; set; }

        public Question()
        {
            Answers = new HashSet<Answer>();
            QuestiongTags = new HashSet<QuestionTag>();
            QuestionVotes = new HashSet<QuestionVote>();
            Comments = new HashSet<Comment>();
            
        }

      
    }
}

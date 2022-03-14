namespace Assignment_QnAWeb.Models
{
    public class Answer
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }

        public int? QuestionId { get; set; }

        public string AppUserId { get; set; }
              

        public AppUser AppUser { get; set; }

        public bool IsPick { get; set; } = false;

        public int vote { get; set; } = 0;

        public ICollection<AnswerVote> AnswerVotes { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public Answer()
        {
            AnswerVotes = new HashSet<AnswerVote>();
            Comments = new HashSet<Comment>();
        }





    }
}

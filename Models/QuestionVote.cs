namespace Assignment_QnAWeb.Models
{
    public class QuestionVote
    {
        public int Id { get; set; }
        public int? QuestionId { get; set; }
        public Question Question { get; set; }
        public string AppUserId { get; set; }

        public AppUser AppUser { get; set; }  

        public int VoteValue { get; set; } = 0;
    }
}

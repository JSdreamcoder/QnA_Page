namespace Assignment_QnAWeb.Models
{
    public class AnswerVote
    {
        public int Id { get; set; }
        public int? AnswerId { get; set; }
        public Answer Answer { get; set; }
        public string? AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public int VoteValue { get; set; } = 0;
    }
}

namespace Assignment_QnAWeb.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? AnswerId { get; set; }
        public Answer? Answer { get; set; }
        public int? QuestionId { get; set; }
        public Question? Question { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }
    }
}

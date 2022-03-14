namespace Assignment_QnAWeb.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<QuestionTag> QuestionTags { get; set; }

        public Tag()
        {
            QuestionTags = new HashSet<QuestionTag>();
        }
        
    }
}

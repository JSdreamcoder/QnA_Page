using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment_QnAWeb.Models
{
    public class Question
    {
        public int Id { get; set; }
        [Display(Name = "Question Title")]
        [StringLength(1000, MinimumLength = 3, ErrorMessage = "Title must be more 3 Letters")]
        public string Title { get; set; }
        public DateTime Date { get; set; }
        [Display(Name = "Question Content")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Content must be more 10 Letters")]
        public string Content { get; set; }

        //[NotMapped]
        //public int testIdForNotMapped { get; set; }

        public string AppUserId { get; set; }
        public AppUser? AppUser { get; set; }

        public int Vote { get; set; } = 0;
        

        public ICollection<Answer> Answers { get; set; }
        public ICollection<QuestionVote> QuestionVotes { get; set; }
        public ICollection<QuestionTag>? QuestiongTags { get; set; }
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

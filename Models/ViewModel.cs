namespace Assignment_QnAWeb.Models
{
    public class ViewModel
    {
        public List<Answer> Answers { get; set; }
        public List<Question> Questions { get; set; }

        public Question Question { get; set; }
        public Answer PickAnswer { get; set; }

        public ViewModel(List<Answer> answers, Question q, Answer a )
        {
            Answers = answers;
            Question = q;
            PickAnswer = a;
        }
    }
}

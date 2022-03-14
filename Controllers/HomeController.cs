using Assignment_QnAWeb.Data;
using Assignment_QnAWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

//for git push test
namespace Assignment_QnAWeb.Controllers
{   [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private UserManager<AppUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private ApplicationDbContext _db;
        public HomeController(ILogger<HomeController> logger,
                              UserManager<AppUser> userManager,
                              RoleManager<IdentityRole> roleManager,
                              ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            _logger = logger;
        }

        //test page using pagemodel in Microsoft
        public async Task<IActionResult> test(string sortOrder,
                                  string currentFilter,
                                  string searchString,
                                  int? pageNumber)
        {
            var question = _db.Question;
            ViewData["CurrentSort"] = sortOrder;
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            int pageSize = 3;
            return View(await PaginatedList<Question>.CreateAsync(question.AsNoTracking(),
                        pageNumber ?? 1, pageSize));

        }

        [AllowAnonymous]
        public IActionResult Index(string tab, int page, string tag)
        {
            if (tab == null)
            {
                tab = "newest";
            }
            if (page == default)
            {
                page = 1;
            }
            if (User.Identity.Name != null)
            {
            ViewBag.UserId = _db.Users.First(u=> u.UserName == User.Identity.Name).Id;
            }
            ViewBag.SelectedPage = page;
            ViewBag.Tag = tag;
            var questiontags = _db.QuestionTag.Include(x => x.Tag).
                               Where(qt=>qt.Tag.Name == tag);
            
            var questions = new List<Question>();
            if (tag != null)
            {
                questions = _db.Question.Include(q => q.Answers).Include(q => q.AppUser).
                            Where(q => questiontags.Select(qt => qt.QuestionId).Contains(q.Id)).
                            ToList();
            }else
            {
                questions = _db.Question.Include(q => q.Answers).Include(q => q.AppUser).ToList();
            }
            
           
            
            var orderedList = new List<Question>();
            if (tab == "newest")
            {
                orderedList = questions.OrderByDescending(q => q.Date).ToList();
            }
            else if (tab == "answer")
            {
                orderedList = questions.OrderByDescending(q => q.Answers.Count).ToList();
            
            }
            ViewBag.Tab = tab;
            // paging (10questions)
            // My own method
            ViewBag.Count = orderedList.Count;
            decimal dQnum = ViewBag.Count;
            //var itemsInPage = new List<Question>();
            
            //if (questions.Count > 0)
            //{
            //    if (Math.Ceiling(dQnum / 10) > page || ViewBag.Count % 10 == 0)
            //     {
            //         itemsInPage = orderedList.GetRange((page - 1) * 10, 10);
            //     }
            //     else
            //     {
            //         itemsInPage = orderedList.GetRange((page - 1) * 10, ViewBag.Count % 10);
            //     }
            //     ViewBag.Page = Math.Ceiling(dQnum / 10);
                 
            //}


            // from Microsoft 
            var numberOfItems = 10;
            ViewBag.TotalPage = Math.Ceiling(ViewBag.Count/(double)numberOfItems);
            var itemsInPage =orderedList.Skip((page-1)*numberOfItems).Take(numberOfItems).ToList();
            ViewBag.CurrentPage = page;



            return View(itemsInPage);
            
        }
       

        [AllowAnonymous]
        public IActionResult Details(int Id)
        {
            if (Id == null)
            {
                return BadRequest();
            }
            if (!_db.Question.Any(q => q.Id == Id))
            {
                return NotFound();
            }
            var question = _db.Question.
                           Include(q => q.AppUser).
                           Include(q=> q.Comments).ThenInclude(c=>c.User).
                           Include(q => q.QuestiongTags).ThenInclude(qc => qc.Tag).
                           First(q => q.Id == Id);
            
            var answers = _db.Answer.Include(a => a.AppUser).
                                     Include(a => a.Comments).ThenInclude(c => c.User).
                                     Where(a => a.QuestionId == Id).ToList();
            ViewBag.Answers = question.Answers.Count;
            var pickAnswer = new Answer();
            // remove pickanswer from the asnwerlist
            if (question.Answers.Any(a => a.IsPick == true))
            {
                pickAnswer = answers.First(a => a.IsPick == true);
                answers.Remove(pickAnswer);
            }
            
            string UserName = User.Identity.Name;
            var CurrentUser = _db.Users.FirstOrDefault(a => a.UserName == UserName);
            if (CurrentUser != null)
            ViewBag.UserId = CurrentUser.Id;
            
            var viewmodel = new ViewModel(answers, question,pickAnswer);
            return View(viewmodel);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Details(int questionId, int questionVote,int pickId,
                                                 int answerId, int answerVote,string answerText)
        {
            try
            {
               
                var CurrentUser = await _userManager.FindByNameAsync(User.Identity.Name);
                
                //For Question Vote
                if (questionId != default && questionVote != default)
                {
                    var Question = _db.Question.Include(q=>q.AppUser).Include(q=>q.QuestionVotes).
                                   First(q => q.Id == questionId);
                    if (Question == null)
                    {
                        return NotFound();
                    }
                    // prevent from voting by same user
                    if (Question.AppUserId == CurrentUser.Id)
                    {
                        return RedirectToAction("Details");
                    }
                    if (questionVote == 1)
                    {
                        if (!_db.QuestionVote.Any(v => v.AppUserId == CurrentUser.Id &&
                                                       v.QuestionId == Question.Id))
                        {
                            var newVote = new QuestionVote();
                            newVote.AppUserId = CurrentUser.Id;
                            newVote.QuestionId = questionId;
                            newVote.VoteValue = 1;
                            Question.Vote += 1;
                            _db.QuestionVote.Add(newVote);
                            Question.AppUser.Reputation += 5;
                        }else
                        {
                            var vote = _db.QuestionVote.First(v => v.AppUserId == CurrentUser.Id &&
                                                                   v.QuestionId == Question.Id);
                            if (vote.VoteValue == -1)
                            {
                                vote.VoteValue = 1;
                                Question.Vote += 2;
                                Question.AppUser.Reputation += 10;
                            }
                        }

                    }
                    else if (questionVote == -1)
                    {
                        if (!_db.QuestionVote.Any(v => v.AppUserId == CurrentUser.Id &&
                                                       v.QuestionId == Question.Id))
                        {
                            var newVote = new QuestionVote();
                            newVote.AppUserId = CurrentUser.Id;
                            newVote.QuestionId = questionId;
                            newVote.VoteValue = -1;
                            Question.Vote -= 1;
                            _db.QuestionVote.Add(newVote);
                            Question.AppUser.Reputation -= 5;
                        }
                        else
                        {
                            var vote = _db.QuestionVote.First(v => v.AppUserId == CurrentUser.Id &&
                                                                   v.QuestionId == Question.Id);
                            if (vote.VoteValue == 1)
                            {
                                vote.VoteValue = -1;
                                Question.Vote -= 2;
                                Question.AppUser.Reputation -= 10;
                            }
                        }
                    }
                   
                }
                //For Answer Vote
                if (answerId != 0)
                {
                    if (_db.Answer.First(q => q.Id == answerId) == null)
                    {
                        return NotFound();
                    }
                    var Answer = _db.Answer.Include(a=>a.AppUser).Include(a=>a.AnswerVotes).
                                First(q => q.Id == answerId);
                    
                    // prevent from voting by same user
                    if (Answer.AppUserId == CurrentUser.Id)
                    {
                        return RedirectToAction("Details");
                    }
                    if (answerVote == 1)
                    {
                        if (!_db.AnswerVote.Any(v => v.AppUserId == CurrentUser.Id &&
                                                       v.AnswerId == Answer.Id))
                        {
                            var newVote = new AnswerVote();
                            newVote.AppUserId = CurrentUser.Id;
                            newVote.AnswerId = answerId;
                            newVote.VoteValue = 1;
                            Answer.vote += 1;
                            _db.AnswerVote.Add(newVote);
                            Answer.AppUser.Reputation += 5;
                        }
                        else
                        {
                            var vote = _db.AnswerVote.First(v => v.AppUserId == CurrentUser.Id &&
                                                                   v.AnswerId == Answer.Id);
                            if (vote.VoteValue == -1)
                            {
                                vote.VoteValue = 1;
                                Answer.vote += 2;
                                Answer.AppUser.Reputation += 10;
                            }
                        }
                    }
                    else if (answerVote == -1)
                    {
                        if (!_db.AnswerVote.Any(v => v.AppUserId == CurrentUser.Id &&
                                                      v.AnswerId == Answer.Id))
                        {
                            var newVote = new AnswerVote();
                            newVote.AppUserId = CurrentUser.Id;
                            newVote.AnswerId = answerId;
                            newVote.VoteValue = -1;
                            Answer.vote -= 1;
                            _db.AnswerVote.Add(newVote);
                            Answer.AppUser.Reputation -= 5;
                        }
                        else
                        {
                            var vote = _db.AnswerVote.First(v => v.AppUserId == CurrentUser.Id &&
                                                                   v.AnswerId == Answer.Id);
                            if (vote.VoteValue == 1)
                            {
                                vote.VoteValue = -1;
                                Answer.vote -= 2;
                                Answer.AppUser.Reputation -= 10;
                            }
                        }
                    }
                    

                }
                
                
                //For Pick Answer
                if (pickId != default)
                {
                    
                    var question = _db.Question.Include(q => q.Answers).First(q => q.Id == questionId);
                    var Answer = _db.Answer.First(a => a.Id == pickId);
                    var previouspicked = new Answer();
                    if (question.Answers.Any(a => a.IsPick == true))
                    {
                        previouspicked = question.Answers.First(a => a.IsPick == true);
                        previouspicked.IsPick = false;
                        Answer.IsPick = true;
                    }else
                    {
                        Answer.IsPick = true;
                    }
                      
                }
                //For Making Answer
                if (answerText != null)
                {
                    var newAnswer = new Answer();
                    newAnswer.Content = answerText;
                    newAnswer.AppUserId = CurrentUser.Id;
                    newAnswer.QuestionId = questionId;
                    newAnswer.Date = DateTime.Now;
                    _db.Answer.Add(newAnswer);
                }

                
                await _userManager.UpdateAsync(CurrentUser);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { Message = ex.Message });
            }
            
            return RedirectToAction("Details");
        }

        public IActionResult Create()
        {
            
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string title,string content,string tag1,
                                                string tag2 , string tag3)
        {
            try
            {
                var tagList = new List<string>();
                if(tag1 != null)
                    tagList.Add(tag1);

                if (tag2 != null)
                    tagList.Add(tag2);

                if (tag3 != null)
                    tagList.Add(tag3);

                if (title == null || content == null || tagList==null)
                    return BadRequest();
                
                var CurrentUser = await _userManager.FindByNameAsync(User.Identity.Name);
                var newQuestion = new Question();
                newQuestion.Title = title;
                newQuestion.Content = content;
                newQuestion.AppUserId = CurrentUser.Id.ToString();
                newQuestion.Date = DateTime.Now;
                _db.Question.Add(newQuestion);
                await _userManager.UpdateAsync(CurrentUser); //db.savechanges
                foreach (var tag in tagList)
                {

                    if (_db.Tag.Any(t => t.Name == tag))
                    {
                        var newQuestionTag = new QuestionTag();
                        newQuestionTag.QuestionId = newQuestion.Id;
                        newQuestionTag.TagId = _db.Tag.First(t => t.Name == tag).Id;
                        _db.QuestionTag.Add(newQuestionTag);
                    }
                    else
                    {
                        var newTag = new Tag();
                        newTag.Name = tag;
                        _db.Tag.Add(newTag);
                        await _userManager.UpdateAsync(CurrentUser);
                        var newQuestionTag = new QuestionTag();
                        newQuestionTag.QuestionId = newQuestion.Id;
                        newQuestionTag.TagId = _db.Tag.First(t => t.Name == tag).Id;
                        _db.QuestionTag.Add(newQuestionTag);
                    }
                }
                await _userManager.UpdateAsync(CurrentUser);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error",new { Message = ex.Message });
            }

            return RedirectToAction("Index");
        }

        public IActionResult EditQuestion(int questionId)
        {
            var question = _db.Question.First(q => q.Id == questionId);

            return View(question);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditQuestion(Question question)
        {
            if (question.Title == null || question.Content == null)
                return BadRequest();
            try
            {

                var editQuestion = _db.Question.Find(question.Id);
             
                editQuestion.Date = DateTime.Now;
                editQuestion.Title = question.Title;
                editQuestion.Content = question.Content;
                _db.Update(editQuestion);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error" , new { Message = ex.Message });
            }
            return RedirectToAction("Details", new {Id=question.Id});
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(string newRole)
        {
            if (newRole == null)
            {
                return BadRequest();
            }
            await _roleManager.CreateAsync(new IdentityRole(newRole));
            _db.SaveChanges();
            
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult SetUserRole()
        {
            SelectList userSelectList = new SelectList(_db.Users, "UserName", "UserName");
            SelectList roleSelectList = new SelectList(_db.Roles, "Name", "Name");
            ViewBag.userSelectList = userSelectList;
            ViewBag.roleSelectList = roleSelectList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SetUserRole(string UserName, string RoleName)
        {
            if (UserName == null || RoleName == null)
            {
                return NotFound();
            }
            var user = await _userManager.FindByNameAsync(UserName);

            if (await _roleManager.RoleExistsAsync(RoleName))
            {

                if (!await _userManager.IsInRoleAsync(user, RoleName))
                {
                    await _userManager.AddToRoleAsync(user, RoleName);
                }
                //_db.SaveChanges();
                await _userManager.UpdateAsync(user);
            }

            return RedirectToAction("SetUserRole");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult QuestionComment(int questionId )
        {
            if(questionId != 0)
            ViewBag.QuestionId = questionId;

            var currentUserName = User.Identity.Name;
            ViewBag.UserId = _db.Users.First(u=> u.UserName == currentUserName).Id;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuestionComment([Bind("QuestionId,Content,UserId")] Comment comment)
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            //try
            //{
            //    if (questionId == 0 || comment == null)
            //        return BadRequest();


            //    var newComment = new Comment();
            //    newComment.QuestionId = questionId;
            //    newComment.Content = comment;
            //    newComment.CreatedDate = DateTime.Now;
            //    newComment.UserId = currentUser.Id;
            //    _db.Comment.Add(newComment);
            //}
            //catch (Exception ex)
            //{
            //    return RedirectToAction("Error",new { Message = ex.Message });
            //}

            ModelState.ClearValidationState("User");

            //if (!TryValidateModel(comment))
            //{
            //    _db.Comment.Add(comment);
            //    await _db.SaveChangesAsync();
            //    return RedirectToAction("Details", new { Id = comment.QuestionId });
            //}

            if (!ModelState.IsValid)
            {

                comment.CreatedDate = DateTime.Now;
                _db.Comment.Add(comment);
                await _userManager.UpdateAsync(currentUser);
                return RedirectToAction("Details", new { Id = comment.QuestionId });
            }

            //await _userManager.UpdateAsync(currentUser);
            return RedirectToAction("Details", new { Id = comment.QuestionId });
        }

        public IActionResult EditQuestionComment(int Id)
        {
            var comment =  _db.Comment.Find(Id);
            
            return View(comment);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditQuestionComment( Comment comment)
        {
            if (comment == null)
            {
                return BadRequest();
            };
            try
            {
                var editcomment = _db.Comment.First(c=>c.Id == comment.Id);
                 editcomment.Content = comment.Content;
                 editcomment.CreatedDate = DateTime.Now;
                _db.Comment.Update(editcomment);
                await _db.SaveChangesAsync();
                return RedirectToAction("Details", new { Id = editcomment.QuestionId });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { message = ex.Message });
            }
            return View();
        }

        public IActionResult AnswerComment(int questionId, int answerId)
        {
            if (questionId != 0 && answerId != 0)
            {
                ViewBag.QuestionId = questionId;
                ViewBag.AnswerId = answerId;
                
            }
                

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AnswerComment(int questionId, int answerId, string comment)
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                if (questionId == 0 || comment == null || answerId == 0)
                    return BadRequest();


                var newComment = new Comment();
                newComment.AnswerId = answerId; 
                
                newComment.Content = comment;
                newComment.CreatedDate = DateTime.Now;
                newComment.UserId = currentUser.Id;
                _db.Comment.Add(newComment);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { Message = ex.Message });
            }
            await _userManager.UpdateAsync(currentUser);
            return RedirectToAction("Details", new { Id = questionId });
        }

        public IActionResult EditAnswerComment(int Id)
        {
            var comment = _db.Comment.Find(Id);

            return View(comment);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAnswerComment(Comment comment)
        {
            if (comment == null)
            {
                return BadRequest();
            };
            try
            {
                var editcomment = _db.Comment.Include(c=>c.Answer)
                                             .First(c => c.Id == comment.Id);
                editcomment.Content = comment.Content;
                editcomment.CreatedDate = DateTime.Now;
                _db.Comment.Update(editcomment);
                _db.SaveChanges();
                return RedirectToAction("Details", new { Id = editcomment.Answer.QuestionId });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { message = ex.Message });
            }
            return View();
        }


        [Authorize(Roles = "Admin")]
        public IActionResult AdminPage(string tab, int page, string tag)
        {
            if (tab == null)
            {
                tab = "newest";
            }
            if (page == default)
            {
                page = 1;
            }

            ViewBag.Tag = tag;
            var questiontags = _db.QuestionTag.Include(x => x.Tag).
                               Where(qt => qt.Tag.Name == tag);

            var questions = new List<Question>();
            if (tag != null)
            {
                questions = _db.Question.Include(q => q.Answers).Include(q => q.AppUser).
                            Where(q => questiontags.Select(qt => qt.QuestionId).Contains(q.Id)).
                            ToList();
            }
            else
            {
                questions = _db.Question.Include(q => q.Answers).Include(q => q.AppUser).ToList();
            }



            var orderedList = new List<Question>();
            if (tab == "newest")
            {
                orderedList = questions.OrderByDescending(q => q.Date).ToList();
            }
            else if (tab == "answer")
            {
                orderedList = questions.OrderByDescending(q => q.Answers.Count).ToList();

            }
            // paging (10queations) 
            ViewBag.Count = orderedList.Count;
            decimal dQnum = ViewBag.Count;
            var itemsInPage = new List<Question>();

            if (questions.Count > 0)
            {
                if (Math.Ceiling(dQnum / 10) > page || ViewBag.Count % 10 == 0)
                {
                    itemsInPage = orderedList.GetRange((page - 1) * 10, 10);
                }
                else
                {
                    itemsInPage = orderedList.GetRange((page - 1) * 10, ViewBag.Count % 10);
                }
                ViewBag.Page = Math.Ceiling(dQnum / 10);
                ViewBag.Tab = tab;




            }


            return View(itemsInPage);

        }
        
        [HttpPost]
        //For delete questions
        public async Task<IActionResult> AdminPage(int questionId)
        {
            var CurrentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            try
            {



                var question = _db.Question.Include(q => q.QuestiongTags).
                                            Include(q => q.QuestionVotes).
                                            Include(q => q.Comments).
                                            Include(q => q.Answers).
                                            First(q => q.Id == questionId);

                foreach (var item in question.QuestiongTags)
                    _db.QuestionTag.Remove(item);

                foreach (var item in question.QuestionVotes)
                    _db.QuestionVote.Remove(item);

                foreach (var item in question.Comments)
                    _db.Comment.Remove(item);

                if (question.Answers.Any())
                {
                    var answers = _db.Answer.Include(a => a.Comments).
                                             Include(a => a.AnswerVotes).
                                             Where(a => a.QuestionId == question.Id);
                    foreach (var answer in answers)
                    {
                        foreach (var item in answer.Comments)
                            _db.Comment.Remove(item);

                        foreach (var item in answer.AnswerVotes)
                            _db.AnswerVote.Remove(item);
                    }
                }


                _db.Question.Remove(question);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { Message = ex.Message });
            }
            await _userManager.UpdateAsync(CurrentUser);
            return RedirectToAction("AdminPage");
        }

   


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string message)
        {
            ViewBag.Message = message;
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
using Assignment_QnAWeb.Data;
using Assignment_QnAWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Assignment_QnAWeb.Controllers
{
    public class TestController : Controller
    {
        public ApplicationDbContext db;

        public TestController(ApplicationDbContext _db)
        {
            db = _db;
        }
        // GET: TestController
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult QuestionList()
        {
            List<Question> Questions = db.Question.ToList();
            return View(Questions);
        }

        public ActionResult QuestionListWithRazerView()
        {
            List<Question> Questions = db.Question.ToList();
            return View(Questions);
        }
        [HttpPost]
        public IActionResult DeleteQuesiton(int questionId)
        {
            
                var question = db.Question.Find(questionId);
                db.Question.Remove(question);
                db.SaveChanges();
            return RedirectToAction("QuestionList");
        }

        // GET: TestController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TestController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TestController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TestController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TestController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TestController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TestController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

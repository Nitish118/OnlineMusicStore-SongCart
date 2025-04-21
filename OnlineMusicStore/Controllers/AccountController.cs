using System.Linq;
using System.Web.Mvc;
using OnlineMusicStore.Data;
using OnlineMusicStore.Models;

namespace OnlineMusicStore.Controllers
{
    public class AccountController : Controller
    {
        private MusicStoreContext db = new MusicStoreContext();

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Login");
            }

            return View(user);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = db.Users.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);

                if (existingUser != null)
                {
                    // Set session variables for User
                    Session["UserId"] = existingUser.Id;
                    Session["Username"] = existingUser.Username;
                    Session["IsAdmin"] = existingUser.IsAdmin;

                    // Redirect based on Admin or User
                    if (existingUser.IsAdmin)
                    {
                        return RedirectToAction("AdminDashboard", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("UserDashboard", "User");
                    }
                }
                else
                {
                    ViewBag.Error = "Invalid email or password.";
                }
            }
            else
            {
                ViewBag.Error = "Form validation failed. Please try again.";
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult UserHome()
        {
            if (Session["UserId"] != null && !(bool)Session["IsAdmin"])
            {
                using (var db = new MusicStoreContext())
                {
                    var songs = db.Songs.ToList();
                    return View(songs);
                }
            }
            return RedirectToAction("Login");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public ActionResult AdminDashboard()
        {
            if (Session["UserId"] != null && Session["IsAdmin"] != null && (bool)Session["IsAdmin"])
            {
                return View();
            }

            return RedirectToAction("Login");
        }
    }
}

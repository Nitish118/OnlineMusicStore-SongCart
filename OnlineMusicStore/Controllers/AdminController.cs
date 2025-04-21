using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using OnlineMusicStore.Data;
using OnlineMusicStore.Models;

namespace OnlineMusicStore.Controllers
{
    public class AdminController : Controller
    {
        private MusicStoreContext db = new MusicStoreContext();

        // GET: Admin/AdminDashboard
        [HttpGet]
        public ActionResult AdminDashboard()
        {
            if (Session["UserId"] != null && (bool)Session["IsAdmin"])
            {
                var totalUsers = db.Users.Count();
                var totalSongs = db.Songs.Count();
                var totalOrders = db.Orders.Count();
                var totalRevenue = db.Orders.Sum(o => (decimal?)o.TotalAmount) ?? 0;
                var pendingFeedback = db.Feedbacks.Count();

                ViewBag.TotalUsers = totalUsers;
                ViewBag.TotalSongs = totalSongs;
                ViewBag.TotalOrders = totalOrders;
                ViewBag.PendingFeedback = pendingFeedback;
                ViewBag.TotalRevenue = totalRevenue;

                var recentOrders = db.Orders
                                     .Include("User")
                                     .Include("OrderDetails.Song")
                                     .OrderByDescending(o => o.OrderDate)
                                     .Take(5)
                                     .ToList();

                ViewBag.RecentOrders = recentOrders;

                var topSongs = db.OrderDetails
                 .GroupBy(od => od.SongId)
                 .Select(g => new
                 {
                     SongId = g.Key,
                     TotalPurchased = g.Sum(od => od.Quantity),
                     Song = g.Select(od => od.Song).FirstOrDefault()
                 })
                 .OrderByDescending(x => x.TotalPurchased)
                 .Take(5)
                 .ToList();

                ViewBag.TopSongs = topSongs;


                return View();
            }
            return RedirectToAction("Login");
        }


        public ActionResult ViewContactMessages()
        {
            var messages = db.ContactMessages.OrderByDescending(m => m.SubmittedAt).ToList();
            return View(messages);
        }





        // View all songs
        public ActionResult ManageSongs()
        {
            var songs = db.Songs.ToList();
            return View(songs);
        }





        // GET: Admin/AddSong
        [HttpGet]
        public ActionResult AddSong()
        {
            if (Session["UserId"] != null && Session["IsAdmin"] != null && (bool)Session["IsAdmin"])
            {
                return View();
            }
            return RedirectToAction("Login", "Account");
        }

        // POST: Admin/AddSong
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSong(Song song)
        {
            if (Session["UserId"] != null && Session["IsAdmin"] != null && (bool)Session["IsAdmin"])
            {
                if (ModelState.IsValid)
                {
                    db.Songs.Add(song);
                    db.SaveChanges();
                    ViewBag.Message = "Song added successfully!";
                    return RedirectToAction("ManageSongs");
                }
                return View(song);
            }
            return RedirectToAction("Login", "Account");
        }

        // GET: Admin/DeleteSong/{id}
        [HttpGet]
        public ActionResult DeleteSong(int id)
        {
            var song = db.Songs.Find(id);

            if (song != null)
            {
                // Remove related cart items
                var cartItems = db.CartItem.Where(c => c.SongId == id).ToList();
                foreach (var item in cartItems)
                {
                    db.CartItem.Remove(item);
                }

                // Remove related wishlist items
                var wishlistItems = db.WishlistItems.Where(w => w.SongId == id).ToList();
                foreach (var item in wishlistItems)
                {
                    db.WishlistItems.Remove(item);
                }

                // Optional: remove from OrderDetails if needed
                var orderDetails = db.OrderDetails.Where(o => o.SongId == id).ToList();
                foreach (var item in orderDetails)
                {
                    db.OrderDetails.Remove(item);
                }

                // Now remove the song itself
                db.Songs.Remove(song);
                db.SaveChanges();
            }

            return RedirectToAction("ManageSongs");
        }




        public ActionResult ManageUsers()
        {
            if (Session["UserId"] != null && (bool)Session["IsAdmin"])
            {
                var users = db.Users.ToList();
                ViewBag.Users = users;  // Passing the list of users to the view
                return View();
            }
            return RedirectToAction("Login");
        }


        [HttpGet]
        public ActionResult DeleteUser(int id)
        {
            var user = db.Users.Find(id);
            if (user != null && !user.IsAdmin)
            {
                db.Users.Remove(user);
                db.SaveChanges();
            }

            return RedirectToAction("ManageUsers");
        }


        // GET: Admin/EditSong/5
        [HttpGet]
        public ActionResult EditSong(int id)
        {
            var song = db.Songs.Find(id);
            if (song == null)
            {
                return HttpNotFound();
            }
            return View(song);
        }

        // POST: Admin/EditSong
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSong(Song updatedSong)
        {
            if (ModelState.IsValid)
            {
                db.Entry(updatedSong).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ManageSongs");
            }
            return View(updatedSong);
        }


        // View all feedback
        [HttpGet]
        public ActionResult ManageFeedback()
        {
            if (Session["UserId"] != null && (bool)Session["IsAdmin"])
            {
                var feedbacks = db.Feedbacks.Include("User").OrderByDescending(f => f.Id).ToList();
                return View(feedbacks);
            }

            return RedirectToAction("Login", "Account");
        }

        // Optionally delete feedback
        [HttpGet]
        public ActionResult DeleteFeedback(int id)
        {
            var feedback = db.Feedbacks.Find(id);
            if (feedback != null)
            {
                db.Feedbacks.Remove(feedback);
                db.SaveChanges();
            }

            return RedirectToAction("ManageFeedback");
        }

        [HttpGet]
        public ActionResult ReplyToFeedback(int id)
        {
            var feedback = db.Feedbacks.Include("User").FirstOrDefault(f => f.Id == id);
            if (feedback == null) return HttpNotFound();
            return View(feedback);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReplyToFeedback(int id, string ReplyMessage)
        {
            var feedback = db.Feedbacks.Find(id);
            if (feedback != null && string.IsNullOrEmpty(feedback.ReplyMessage))
            {
                feedback.ReplyMessage = ReplyMessage;
                db.SaveChanges();
            }
            return RedirectToAction("ManageFeedback");
        }


        [HttpGet]
        public ActionResult ManageOrders()
        {
            if (Session["UserId"] != null && (bool)Session["IsAdmin"])
            {
                var orders = db.Orders
                               .Include("User")
                               .Include("OrderDetails.Song")
                               .OrderByDescending(o => o.OrderDate)
                               .ToList();

                return View(orders);
            }

            return RedirectToAction("Login", "Account");
        }


        public ActionResult ViewAllOrders()
        {
            if (Session["UserId"] != null && (bool)Session["IsAdmin"])
            {
                var orders = db.Orders
                               .Include("User")
                               .Include("OrderDetails.Song")
                               .ToList();

                return View(orders);
            }
            return RedirectToAction("Login", "Account");
        }







    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using OnlineMusicStore.Data;
using OnlineMusicStore.Models;

public class UserController : Controller
{
    private MusicStoreContext db = new MusicStoreContext();

    // User Home Page
    public ActionResult UserHome()
    {
        if (Session["UserId"] == null)
            return RedirectToAction("Login", "Account");

        var songs = db.Songs.ToList();
        return View("~/Views/Account/UserHome.cshtml", songs); // Pass songs to view
    }

    public ActionResult UserProfile()
    {
        if (Session["UserId"] != null)
        {
            int userId = (int)Session["UserId"];
            var user = db.Users.Find(userId);

            if (user != null)
            {
                return View(user);
            }
        }
        return RedirectToAction("Login", "Account");
    }

    public ActionResult About()
    {
        return View();
    }


    public ActionResult EditProfile()
    {
        if (Session["UserId"] != null)
        {
            int userId = (int)Session["UserId"];
            var user = db.Users.Find(userId);

            if (user != null)
            {
                return View(user);
            }
        }
        return RedirectToAction("Login", "Account");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult UpdateProfile(User updatedUser)
    {
        if (Session["UserId"] != null)
        {
            int userId = (int)Session["UserId"];
            var user = db.Users.Find(userId);

            if (user != null)
            {
                // Update the email and password (if provided)
                user.Email = updatedUser.Email;
                user.Username = updatedUser.Username;
                user.Address = updatedUser.Address;
                user.PaymentMethod = updatedUser.PaymentMethod;
                if (!string.IsNullOrEmpty(updatedUser.Password))
                {
                    user.Password = updatedUser.Password; // Implement password hashing if needed
                }

                db.SaveChanges();
                TempData["ProfileUpdated"] = true;
                return RedirectToAction("UserProfile");
            }
        }
        return RedirectToAction("Login", "Account");
    }




    [HttpPost]
    public ActionResult AddToCart(int songId)
    {
        if (Session["UserId"] == null)
            return Json(new { success = false, message = "Please log in first" });

        int userId = (int)Session["UserId"];

        var song = db.Songs.Find(songId);
        if (song == null)
            return Json(new { success = false, message = "Song not found" });

        // Check if the item already exists in user's cart
        var existingItem = db.CartItem.FirstOrDefault(c => c.UserId == userId && c.SongId == songId);
        if (existingItem != null)
        {
            existingItem.Quantity += 1;
        }
        else
        {
            db.CartItem.Add(new CartItem
            {
                UserId = userId,
                SongId = song.SongId,
                Title = song.Title,
                Artist = song.Artist,
                Price = song.Price,
                CoverImageUrl = song.CoverImageUrl,
                Quantity = 1
            });
        }

        db.SaveChanges();

        // Count and total for cart UI
        var userCart = db.CartItem.Where(c => c.UserId == userId).ToList();
        return Json(new
        {
            success = true,
            message = "Added to cart successfully",
            cartCount = userCart.Count,
            cartTotal = userCart.Sum(item => item.Price * item.Quantity)
        });
    }


    // VIEW CART
    public ActionResult ViewCart()
    {
        if (Session["UserId"] == null)
            return RedirectToAction("Login", "Account");

        int userId = (int)Session["UserId"];
        var cart = db.CartItem.Where(c => c.UserId == userId).ToList();

        return View(cart);
    }


    // REMOVE ITEM FROM CART
    public ActionResult RemoveFromCart(int id)
    {
        if (Session["UserId"] == null)
            return RedirectToAction("Login", "Account");

        int userId = (int)Session["UserId"];
        var item = db.CartItem.FirstOrDefault(c => c.SongId == id && c.UserId == userId);
        if (item != null)
        {
            db.CartItem.Remove(item);
            db.SaveChanges();
        }

        return RedirectToAction("ViewCart");
    }


    // GET CART COUNT
    public ActionResult GetCartCount()
    {
        var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();
        return Json(cart.Count, JsonRequestBehavior.AllowGet);
    }

    // CHECKOUT
    public ActionResult Checkout()
    {
        // Get the userId from session
        if (Session["UserId"] == null)
        {
            return RedirectToAction("Login", "Account");
        }

        int userId = (int)Session["UserId"];

        // Fetch the user's cart from the database
        var cart = db.CartItem.Where(c => c.UserId == userId).ToList();

        // Make sure cart exists and has items
        if (cart == null || !cart.Any())
        {
            ViewBag.Message = "Your cart is empty!";
            return View();
        }

        // Calculate total amount
        decimal totalAmount = cart.Sum(item => item.Price * item.Quantity);

        // Create a new order
        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.Now,
            TotalAmount = totalAmount
        };

        // Save order to the database
        db.Orders.Add(order);
        db.SaveChanges();

        // Save order details (each song in the order)
        foreach (var item in cart)
        {
            var orderDetail = new OrderDetail
            {
                OrderId = order.Id,
                SongId = item.SongId,
                Quantity = item.Quantity,
                UnitPrice = item.Price
            };
            db.OrderDetails.Add(orderDetail);
        }
        db.SaveChanges();

        // Clear the user's cart from the database
        var userCartItems = db.CartItem.Where(c => c.UserId == userId).ToList();
        db.CartItem.RemoveRange(userCartItems);
        db.SaveChanges();

        // Clear the cart from session as well
        Session["Cart"] = new List<CartItem>();

        // Pass the order to the view for confirmation
        ViewBag.OrderId = order.Id;
        ViewBag.Message = "Thank you for your purchase! Your order has been placed successfully.";

        return View(order);
    }


    // BROWSE SONGS
    public ActionResult Browse(string search)
    {
        var songs = db.Songs.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            songs = songs.Where(s =>
                s.Title.Contains(search) ||
                s.Artist.Contains(search) ||
                s.Genre.Contains(search));
        }

        return View(songs.ToList());
    }



    public ActionResult BrowseSongsByGenre(string search)
    {
        var songs = db.Songs.ToList();

        if (!string.IsNullOrEmpty(search))
        {
            songs = songs.Where(s =>
                s.Title.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                s.Artist.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                s.Genre.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        }

        var genres = songs.Select(s => s.Genre).Distinct().ToList();
        ViewBag.Genres = genres;

        return View("~/Views/User/BrowseSongsByGenre.cshtml", songs);
    }

    public ActionResult UserDashboard()
    {
        if (Session["UserId"] != null && !(bool)Session["IsAdmin"])
        {
            var songs = db.Songs.ToList(); // You can customize this with filters
            return View("UserDashboard", songs);
        }
        return RedirectToAction("Login", "Account");
    }



    public ActionResult OrderHistory()
    {
        if (Session["UserId"] == null)
        {
            return RedirectToAction("Login", "Account");
        }

        int userId = (int)Session["UserId"];

        var orders = db.Orders
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToList();

        var orderHistory = orders.Select(order => new OrderHistoryViewModel
        {
            OrderId = order.Id,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Songs = db.OrderDetails
                .Where(od => od.OrderId == order.Id)
                .Join(db.Songs, od => od.SongId, s => s.SongId, (od, s) => new OrderDetailItem
                {
                    Title = s.Title,
                    Artist = s.Artist,
                    Price = od.UnitPrice,
                    Quantity = od.Quantity
                }).ToList()
        }).ToList();

        return View("OrderHistory", orderHistory);
    }


    // VIEW WISHLIST
    public ActionResult ViewWishlist()
    {
        if (Session["UserId"] == null)
            return RedirectToAction("Login", "Account");

        int userId = (int)Session["UserId"];
        var wishlist = db.WishlistItems.Where(w => w.UserId == userId).ToList();

        return View(wishlist);
    }


    // ADD TO WISHLIST
    [HttpPost]
    public ActionResult AddToWishlist(int songId)
    {
        if (Session["UserId"] == null)
        {
            return Json(new { success = false, message = "Please log in to add to wishlist" });
        }

        int userId = (int)Session["UserId"];

        // Get song details
        var song = db.Songs.Find(songId);
        if (song == null)
        {
            return Json(new { success = false, message = "Song not found" });
        }

        var existingItem = db.WishlistItems.FirstOrDefault(w => w.UserId == userId && w.SongId == songId);
        if (existingItem == null)
        {
            var wishlistItem = new WishlistItem
            {
                UserId = userId,
                SongId = songId,
                Title = song.Title,           // Copy song title
                Artist = song.Artist,         // Copy artist
                CoverImageUrl = song.CoverImageUrl,  // Copy cover image URL
                Price = song.Price
            };
            db.WishlistItems.Add(wishlistItem);
            db.SaveChanges();
        }

        return Json(new { success = true, message = "Added to wishlist successfully" });
    }

    // REMOVE FROM WISHLIST
    [HttpPost]
    public ActionResult RemoveFromWishlist(int id)
    {
        var itemToRemove = db.WishlistItems.Find(id);
        if (itemToRemove != null)
        {
            db.WishlistItems.Remove(itemToRemove);
            db.SaveChanges();
            return Json(new { success = true });
        }
        return Json(new { success = false });
    }


    [HttpGet]
    public ActionResult SubmitFeedback()
    {
        if (Session["UserId"] == null)
            return RedirectToAction("Login", "Account");

        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult SubmitFeedback(Feedback feedback)
    {
        if (Session["UserId"] == null)
            return RedirectToAction("Login", "Account");

        if (ModelState.IsValid)
        {
            feedback.UserId = (int)Session["UserId"];
            feedback.SubmittedAt = DateTime.Now;

            db.Feedbacks.Add(feedback);
            db.SaveChanges();

            ViewBag.Message = "Thank you for your feedback!";
            ModelState.Clear();
            return View();
        }

        return View(feedback);
    }

    public ActionResult ViewFeedback()
    {
        var userId = (int)Session["UserId"]; // Get the logged-in user's ID from session
        var feedbacks = db.Feedbacks.Where(f => f.UserId == userId).ToList();
        return View(feedbacks);
    }

}

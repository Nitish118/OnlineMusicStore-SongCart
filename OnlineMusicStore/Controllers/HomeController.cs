using System;
using System.Linq;
using System.Web.Mvc;
using OnlineMusicStore.Data;
using OnlineMusicStore.Models;

namespace OnlineMusicStore.Controllers
{
    public class HomeController : Controller
    {
        private MusicStoreContext db = new MusicStoreContext();

        public ActionResult Index()
        {
            var latestSongs = db.Songs
                .OrderByDescending(s => s.SongId) // Assuming newly added songs have higher IDs
                .Take(5)
                .ToList();

            var topSongs = db.Songs
                .OrderByDescending(s => s.SongId)
                .Take(5)
                .ToList();

            var trendingSongs = db.Songs
                .OrderByDescending(s => s.SongId)
                .Take(5)
                .ToList();

            var featuredArtists = db.Songs
                .GroupBy(s => s.Artist)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => g.Key)
                .ToList();

            ViewBag.LatestSongs = latestSongs;
            ViewBag.TopSongs = topSongs;
            ViewBag.TrendingSongs = trendingSongs;
            ViewBag.FeaturedArtists = featuredArtists;

            return View();
        }




        public ActionResult About()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Contact()
        {
            return View(new ContactMessage());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(ContactMessage contact)
        {
            if (ModelState.IsValid)
            {
                contact.SubmittedAt = DateTime.Now;
                db.ContactMessages.Add(contact);
                db.SaveChanges();

                ViewBag.Message = "Thank you! Your message has been received.";
                ModelState.Clear();
                return View(new ContactMessage());
            }

            return View(contact);
        }

        public ActionResult BrowseSongs(string search, string genre)
        {
            using (var db = new MusicStoreContext())
            {
                var songs = db.Songs.AsQueryable();

                // Populate genres list
                ViewBag.Genres = db.Songs.Select(s => s.Genre).Distinct().ToList();

                // Search filter
                if (!string.IsNullOrEmpty(search))
                {
                    songs = songs.Where(s =>
                        s.Title.Contains(search) ||
                        s.Artist.Contains(search) ||
                        s.Genre.Contains(search));
                }

                // Genre filter
                if (!string.IsNullOrEmpty(genre))
                {
                    songs = songs.Where(s => s.Genre == genre);
                }

                return View(songs.ToList());
            }
        }


    }
}
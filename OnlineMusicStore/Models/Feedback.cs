using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineMusicStore.Models
{
    public class Feedback
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        public string Message { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.Now;

        public virtual User User { get; set; }

        public string ReplyMessage { get; set; } // field for admin reply

    }
}
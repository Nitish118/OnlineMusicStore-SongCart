using System;
using System.Collections.Generic;

namespace OnlineMusicStore.Models
{
    public class OrderHistoryViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderDetailItem> Songs { get; set; }
    }

    public class OrderDetailItem
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace Cafe.Model
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TableNumber { get; set; }

        [Required]
        public int WaiterId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        public Waiter Waiter { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}

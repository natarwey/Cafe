using System.ComponentModel.DataAnnotations;

namespace Cafe.Model
{
    public class MenuItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}

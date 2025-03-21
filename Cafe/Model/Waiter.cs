using System.ComponentModel.DataAnnotations;

namespace Cafe.Model
{
    public class Waiter
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string ContactInfo { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}

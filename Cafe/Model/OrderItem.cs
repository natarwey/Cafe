using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Cafe.Model
{
    public class OrderItem
    {
        public int OrderId { get; set; }

        [JsonIgnore]
        public Order? Order { get; set; }  // Сделали nullable

        public int MenuItemId { get; set; }

        [JsonIgnore]
        public MenuItem? MenuItem { get; set; }  // Сделали nullable

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}

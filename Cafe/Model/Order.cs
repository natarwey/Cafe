using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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

        public DateTime OrderDate { get; set; } // Убрали [Required], будет заполняться автоматически

        public decimal TotalAmount { get; set; } // Убрали [Required], будет вычисляться

        // Сделали свойство необязательным для валидации
        [JsonIgnore]
        public Waiter? Waiter { get; set; }

        // Разрешили null и пустую коллекцию
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}

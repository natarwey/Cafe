using Cafe.DataBaseContext;
using Cafe.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Cafe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly CafeContext _context;

        public OrdersController(CafeContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.Include(o => o.Waiter).ToListAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders.Include(o => o.Waiter).FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            // Проверяем существование официанта
            if (!await _context.Waiters.AnyAsync(w => w.Id == order.WaiterId))
                return BadRequest("Waiter not found");

            // Устанавливаем дату и время
            order.OrderDate = DateTime.Now;

            // Инициализируем коллекцию, если её нет
            order.OrderItems ??= new List<OrderItem>();

            // Вычисляем общую сумму
            var menuItemIds = order.OrderItems.Select(oi => oi.MenuItemId).ToList();
            var menuItems = await _context.MenuItems
                .Where(mi => menuItemIds.Contains(mi.Id))
                .ToDictionaryAsync(mi => mi.Id);

            order.TotalAmount = order.OrderItems.Sum(oi =>
                menuItems.TryGetValue(oi.MenuItemId, out var mi) ? mi.Price * oi.Quantity : 0);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, OrderUpdateDto orderUpdate)
        {
            // Находим существующий заказ
            var existingOrder = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (existingOrder == null)
            {
                return NotFound();
            }

            // Обновляем основные поля
            existingOrder.TableNumber = orderUpdate.TableNumber;
            existingOrder.WaiterId = orderUpdate.WaiterId;

            // Обновляем позиции заказа
            if (orderUpdate.OrderItems != null && orderUpdate.OrderItems.Any())
            {
                // Удаляем старые позиции
                _context.OrderItems.RemoveRange(existingOrder.OrderItems);

                // Добавляем новые
                existingOrder.OrderItems = orderUpdate.OrderItems.Select(oi => new OrderItem
                {
                    MenuItemId = oi.MenuItemId,
                    Quantity = oi.Quantity
                }).ToList();
            }

            // Пересчитываем сумму
            await UpdateOrderTotalAmount(existingOrder);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await OrderExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // Вспомогательный метод для проверки существования заказа
        private async Task<bool> OrderExists(int id)
        {
            return await _context.Orders.AnyAsync(e => e.Id == id);
        }

        // Вспомогательный метод для обновления суммы заказа
        private async Task UpdateOrderTotalAmount(Order order)
        {
            var menuItemIds = order.OrderItems.Select(oi => oi.MenuItemId).ToList();
            var menuItems = await _context.MenuItems
                .Where(mi => menuItemIds.Contains(mi.Id))
                .ToDictionaryAsync(mi => mi.Id);

            order.TotalAmount = order.OrderItems
                .Sum(oi => menuItems.TryGetValue(oi.MenuItemId, out var mi) ? mi.Price * oi.Quantity : 0);
        }

        public class OrderUpdateDto
        {
            [Required]
            public int TableNumber { get; set; }

            [Required]
            public int WaiterId { get; set; }

            public List<OrderItemUpdateDto> OrderItems { get; set; }
        }

        public class OrderItemUpdateDto
        {
            [Required]
            public int MenuItemId { get; set; }

            [Required]
            [Range(1, int.MaxValue)]
            public int Quantity { get; set; }
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

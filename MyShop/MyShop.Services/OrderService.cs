using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Services
{
    public class OrderService : IOrderService
    {
        IRepository<Order> orderContext;

        public OrderService(IRepository<Order> OrderContext) {
            this.orderContext = OrderContext;
        }
        public void CreateOrder(Order baseOrder, List<BasketItemViewModel> basketItems)
        {
            foreach (var item in basketItems)
            {
                baseOrder.OrderItems.Add(new OrderItem() {
                    ProductId = item.Id,
                    ProductName = item.ProductName,
                    Image = item.Image,
                    Price = item.Price,
                    Quantity = item.Quantity
                    
                });
            }
            orderContext.Insert(baseOrder);
            orderContext.Commit();
        }

        public List<Order> GetOrders() {
            return orderContext.Collection().ToList();
        }

        public Order GetOrder(string id) {
            return orderContext.Find(id);

        }

        public void UpdateOrder(Order updatedOrder) {
            orderContext.Update(updatedOrder);
            orderContext.Commit();
        }
    }
}

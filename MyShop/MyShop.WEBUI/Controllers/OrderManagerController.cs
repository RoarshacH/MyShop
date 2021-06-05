﻿using MyShop.Core.Contracts;
using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WEBUI.Controllers
{
    public class OrderManagerController : Controller
    {
        IOrderService OrderService;

        public OrderManagerController(IOrderService orderService) {
            this.OrderService = orderService;
        }
        // GET: OrderManager
        public ActionResult Index()
        {
            List<Order> orders = OrderService.GetOrders();
            return View();
        }

        public ActionResult UpdateOrder(string Id) {

            Order order = OrderService.GetOrder(Id);
            
                return View(order);            
            
        }
        [HttpPost]
        public ActionResult UpdateOrder(Order updatedOrder, string Id)
        {

            Order order = OrderService.GetOrder(Id);
            order.OrderStatus = updatedOrder.OrderStatus;
            OrderService.UpdateOrder(order);

            return RedirectToAction("Index");            

        }
    }
}
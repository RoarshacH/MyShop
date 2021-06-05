using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Services;
using MyShop.WEBUI.Controllers;
using System;
using System.Linq;
using System.Security.Principal;

namespace MyShop.WEBUI.Tests.Controllers
{
    [TestClass]
    public class BasketControllerTests
    {
        [TestMethod]
        public void canbasketAddItem()
        {
            //Setup
            IRepository<Basket> BasketRepository = new Mocks.MockContext<Basket>();
            IRepository<Product> ProductRepository = new Mocks.MockContext<Product>();
            IRepository<Order> OrderRepository = new Mocks.MockContext<Order>();
            IRepository<Customer> CustomerRepository = new Mocks.MockContext<Customer>();

            var httpContext = new Mocks.MockHttpContext();  
            IBasketService basketService = new BasketService(ProductRepository, BasketRepository);
            IOrderService orderService = new OrderService(OrderRepository);

            var controller = new BasketController(basketService, orderService, CustomerRepository);

            controller.ControllerContext = new System.Web.Mvc.ControllerContext(httpContext, new System.Web.Routing.RouteData(), controller);
            
            
            //Act
            controller.AddToBasket("1");            
            //basketService.AddToBasket(httpContext, "1");
            Basket basket = BasketRepository.Collection().FirstOrDefault();

            
            //Assert
            Assert.IsNotNull(basket);

            Assert.AreEqual(1, basket.BasketItems.Count);

            Assert.AreEqual("1", basket.BasketItems.ToList().FirstOrDefault().ProductId);
        }

        [TestMethod]
        public void CanGetSummeryViewModel() {
            //Setup
            IRepository<Basket> BasketRepository = new Mocks.MockContext<Basket>();
            IRepository<Product> ProductRepository = new Mocks.MockContext<Product>();
            IRepository<Order> OrderRepository = new Mocks.MockContext<Order>();
            IRepository<Customer> CustomerRepository = new Mocks.MockContext<Customer>();

            ProductRepository.Insert(new Product()
            {
                Id = "1",
                Price = 400.00M,                
            });

            ProductRepository.Insert(new Product()
            {
                Id = "2",
                Price = 250.00M,
            });

            ProductRepository.Insert(new Product()
            {
                Id = "3",
                Price = 1000.00M,
            });         

            var httpContext = new Mocks.MockHttpContext();
            IBasketService basketService = new BasketService(ProductRepository, BasketRepository);
            IOrderService orderService = new OrderService(OrderRepository);

            var controller = new BasketController(basketService, orderService, CustomerRepository);
            
            controller.ControllerContext = new System.Web.Mvc.ControllerContext(httpContext, new System.Web.Routing.RouteData(), controller);

            //Act
            controller.AddToBasket("1");
            controller.AddToBasket("1");
            controller.AddToBasket("2");            
            controller.AddToBasket("3");


            //Assert
            Basket basket = BasketRepository.Collection().FirstOrDefault();
            var summery = basketService.GetBasketSummary(httpContext);

            Assert.IsNotNull(summery);
            Assert.AreEqual(4, summery.BasketCount);
            Assert.AreEqual(2050.00M, summery.BasketTotal);

        }

        [TestMethod]
        public void CanCheckout() {
            //Setup
            IRepository<Basket> BasketRepository = new Mocks.MockContext<Basket>();
            IRepository<Product> ProductRepository = new Mocks.MockContext<Product>();
            IRepository<Order> OrderRepository = new Mocks.MockContext<Order>();
            IRepository<Customer> CustomerRepository = new Mocks.MockContext<Customer>();

            ProductRepository.Insert(new Product()
            {
                Id = "1",
                Price = 400.00M,
            });

            ProductRepository.Insert(new Product()
            {
                Id = "2",
                Price = 250.00M,
            });

            ProductRepository.Insert(new Product()
            {
                Id = "3",
                Price = 1000.00M,
            });

            Basket basket = new Basket();
            basket.BasketItems.Add(new BasketItem() { BasketId = basket.Id, ProductId ="1", Quantity = 2 });
            basket.BasketItems.Add(new BasketItem() { BasketId = basket.Id, ProductId = "3", Quantity = 1 });

            BasketRepository.Insert(basket);
            IBasketService basketService = new BasketService(ProductRepository, BasketRepository);
            IOrderService orderService = new OrderService(OrderRepository);

            CustomerRepository.Insert(new Customer() { Id = "1", Email = "TestUserMain@test.com", ZipCode = "34000"});

            
            

            
            var controller = new BasketController(basketService, orderService, CustomerRepository);
            var httpContext = new Mocks.MockHttpContext();

            IPrincipal fakeUser = new GenericPrincipal(new GenericIdentity("TestUserMain@test.com", "Forms"), null);
            httpContext.User = fakeUser;

            httpContext.Request.Cookies.Add(new System.Web.HttpCookie("eCommerceBasket") { 
                Value = basket.Id
            });

            controller.ControllerContext = new System.Web.Mvc.ControllerContext(httpContext, new System.Web.Routing.RouteData(), controller);

            //ACT
            Order order = new Order();
            controller.Checkout(order);


            //ASSERT
            Assert.AreEqual(2, order.OrderItems.Count);
            Assert.AreEqual(0, basket.BasketItems.Count);


            Order inRep = OrderRepository.Find(order.Id);
            Assert.AreEqual(2, inRep.OrderItems.Count);

        }
    }
}

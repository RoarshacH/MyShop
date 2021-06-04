using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Services;
using MyShop.WEBUI.Controllers;
using System;
using System.Linq;

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

            var httpContext = new Mocks.MockHttpContext();  
            IBasketService basketService = new BasketService(ProductRepository, BasketRepository);

            var controller = new BasketController(basketService);
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

            var controller = new BasketController(basketService);
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
    }
}

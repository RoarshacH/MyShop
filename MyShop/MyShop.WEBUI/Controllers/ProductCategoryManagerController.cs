using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.DataAccess.InMemory;

namespace MyShop.WEBUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductCategoryManagerController : Controller
    {
        IRepository<ProductCategory> context;
        public ProductCategoryManagerController(IRepository<ProductCategory> productCategoryContext)
        {
            this.context = productCategoryContext;
        }

        // GET: ProductManager
        public ActionResult Index()
        {
            List<ProductCategory> productCat = context.Collection().ToList();
            return View(productCat);
        }

        public ActionResult Create()
        {
            ProductCategory productCat = new ProductCategory();
            return View(productCat);
        }

        [HttpPost]
        public ActionResult Create(ProductCategory productCat)
        {
            if (!ModelState.IsValid)
            {
                return View(productCat);
            }
            else
            {
                context.Insert(productCat);
                context.Commit();
                return RedirectToAction("Index");
            }
        }

        public ActionResult Edit(String Id)
        {
            ProductCategory productCat = context.Find(Id);

            if (productCat == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(productCat);
            }

        }

        [HttpPost]
        public ActionResult Edit(ProductCategory productCat, string Id)
        {
            if (productCat == null)
            {
                return HttpNotFound();
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    return View(productCat);
                }
                else
                {
                    ProductCategory productOld = context.Find(Id);
                    productOld.Category = productCat.Category;                    
                    context.Commit();
                    return RedirectToAction("Index");
                }
            }
        }

        public ActionResult Delete(string Id)
        {
            ProductCategory productCat = context.Find(Id);

            if (productCat == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(productCat);
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult ConfirmDelete(string Id)
        {
            ProductCategory productCat = context.Find(Id);

            if (productCat == null)
            {
                return HttpNotFound();
            }
            else
            {
                context.Delete(Id);
                context.Commit();
                return RedirectToAction("Index");
            }

        }
    }
}
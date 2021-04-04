using AppCore.Business.Bases;
using AppCore.DataAccess.Repositories;
using AppCore.DataAccess.Repositories.Bases;
using ETradeBusiness.Models;
using ETradeBusiness.Services;
using ETradeDataAccess.Contexts;
using ETradeEntities.Entities;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ETradeMvcWebUI.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ETradeContext db;
        private readonly RepositoryBase<Product> productRepository;
        private readonly RepositoryBase<Category> categoryRepository;
        private readonly IService<Product, ProductModel> productService;
        private readonly IService<Category, CategoryModel> categoryService;

        public ProductsController()
        {
            db = new ETradeContext();
            productRepository = new Repository<Product>(db);
            categoryRepository = new Repository<Category>(db);
            productService = new ProductService(productRepository);
            categoryService = new CategoryService(categoryRepository);
        }

        // GET: Products
        public ActionResult Index(string message = "")
        {
            //var productList = productService.GetQuery().ToList().Select(m => new ProductModel()
            //{
            //    Id = m.Id,
            //    Name = m.Name,
            //    UnitPrice = m.UnitPrice,
            //    StockAmount = m.StockAmount,
            //    CategoryId = m.CategoryId,
            //    CreateDate = m.CreateDate,
            //    UpdateDate = m.UpdateDate,
            //    IsDeleted = m.IsDeleted,
            //    CategoryName = m.CategoryName,
            //    CreateDateText = m.CreateDate.ToString(new CultureInfo("en")),
            //    UpdateDateText = m.UpdateDate.HasValue ? m.UpdateDate.Value.ToString(new CultureInfo("en")) : "",
            //    StockAmountText = m.StockAmountText
            //}).ToList();
            var productList = productService.GetQuery().ToList(); 
            
            // bu dönüştürme işleminin de serviste bir GetList() methodunda yapılması çok daha iyi
            foreach (var product in productList)
            {
                product.CreateDateText = product.CreateDate.ToString(new CultureInfo("en"));
                product.UpdateDateText = product.UpdateDate.HasValue
                    ? product.UpdateDate.Value.ToString(new CultureInfo("en"))
                    : "";
            }

            ViewBag.Message = message;
            return View(productList);
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductModel product = productService.GetById(id.Value);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            var categoryList = categoryService.GetQuery().ToList();
            ViewBag.Categories = new SelectList(categoryList, "Id", "Name");
            var model = new ProductModel();
            return View(model);
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductModel product)
        {
            if (ModelState.IsValid)
            {
                productService.Add(product);
                return RedirectToAction("Index", new { message = "Operation successful." });
            }

            var categoryList = categoryService.GetQuery().ToList();
            ViewBag.Categories = new SelectList(categoryList, "Id", "Name");
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ProductModel product = productService.GetById(id.Value);
            if (product == null)
            {
                return HttpNotFound();
            }

            List<CategoryModel> categoryList = categoryService.GetQuery().ToList();
            ViewBag.Categories = new SelectList(categoryList, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductModel product)
        {
            if (ModelState.IsValid)
            {
                productService.Update(product);
                return RedirectToAction("Index", new { message = "Operation successful." });
            }
            
            List<CategoryModel> categoryList = categoryService.GetQuery().ToList();
            ViewBag.Categories = new SelectList(categoryList, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ProductModel product = productService.GetById(id.Value);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            productService.Delete(id);
            return RedirectToAction("Index", new { message = "Operation successful." });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

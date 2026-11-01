using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ProductsWithRouting.Models;
using ProductsWithRouting.Services;

namespace ProductsWithRouting.Controllers
{
    // Both prefixes lead to this controller: /products/... and /items/...
    [Route("products")]
    [Route("items")]
    public class ProductsController : Controller
    {
        private readonly List<Product> myProducts;

        public ProductsController(Data data)
        {
            myProducts = data.Products;
        }

        // /products, /products/index, /items, /items/index
        // Filter via query string: ?id=1&name=prod&description=desc
        [HttpGet("")]
        [HttpGet("index")]
        public IActionResult Index(int? id, string name, string description)
        {
            IEnumerable<Product> result = myProducts;

            if (id.HasValue)
            {
                result = result.Where(p => p.Id == id.Value);
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                result = result.Where(p => p.Name != null &&
                    p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrWhiteSpace(description))
            {
                result = result.Where(p => p.Description != null &&
                    p.Description.Contains(description, StringComparison.OrdinalIgnoreCase));
            }

            ViewData["FilterId"] = id;
            ViewData["FilterName"] = name;
            ViewData["FilterDescription"] = description;

            return View("Index", result.ToList());
        }

        // /products/5
        [HttpGet("{id:int}")]
        public IActionResult View(int id)
        {
            var product = myProducts.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return RedirectToAction(nameof(ProductNotFound));
            }
            return View("View", product);
        }

        // /products/edit/5
        [HttpGet("edit/{id:int}")]
        public IActionResult Edit(int id)
        {
            var product = myProducts.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return RedirectToAction(nameof(ProductNotFound));
            }
            return View("Edit", product);
        }

        [HttpPost("edit/{id:int}")]
        public IActionResult Edit(int id, Product product)
        {
            var existing = myProducts.FirstOrDefault(p => p.Id == id);
            if (existing == null)
            {
                return RedirectToAction(nameof(ProductNotFound));
            }

            // Id comes from the route; the posted Id is ignored
            product.Id = id;

            if (!ModelState.IsValid)
            {
                return View("Edit", product);
            }

            existing.Name = product.Name;
            existing.Description = product.Description;
            return RedirectToAction(nameof(Index));
        }

        // /products/create and /products/new
        [HttpGet("create")]
        [HttpGet("new")]
        public IActionResult Create()
        {
            var product = new Product { Id = NextId() };
            return View("Create", product);
        }

        [HttpPost("create")]
        [HttpPost("new")]
        public IActionResult Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                product.Id = NextId();
                return View("Create", product);
            }

            product.Id = NextId();
            myProducts.Add(product);
            return RedirectToAction(nameof(Index));
        }

        // /products/delete/5
        [HttpGet("delete/{id:int}")]
        public IActionResult Delete(int id)
        {
            var product = myProducts.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return RedirectToAction(nameof(ProductNotFound));
            }

            myProducts.Remove(product);
            return RedirectToAction(nameof(Index));
        }

        // Custom 404 page: /products/notfound
        [HttpGet("notfound")]
        public IActionResult ProductNotFound()
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            return View("NotFound");
        }

        private int NextId()
        {
            return myProducts.Count == 0 ? 1 : myProducts.Max(p => p.Id) + 1;
        }
    }
}
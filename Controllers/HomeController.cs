using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FormsApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FormsApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index(string searchString,string category)
    {
        var products = Repository.Products;
        if (!string.IsNullOrEmpty(searchString))
        {
            ViewBag.SearchString = searchString; //for this and for value="@ViewBag.SearchString" we can provide filter word may still remain in the filter box

            products = products.Where(i => i.Name!.ToLower().Contains(searchString)).ToList(); //add extra escurity for null values ,or by alternative we might use this in Name
        }
        
        if(!String.IsNullOrEmpty(category) && category != "0")
        {
            products = products.Where(i => i.CategoryId == int.Parse(category)).ToList();
        }

        //ViewBag.Categories = new SelectList(Repository.Categories,"CategoryId","Name");

        var model = new ProductViewModel
        {
            Products = products,
            Categories = Repository.Categories,
            SelectedCategory = category
        };//filterelediğimiz category yi bu şekilde döndürdük ama burası dışında epey değişiklik yaptık
        return View(model);
    }

    public IActionResult Edit(int? id)
    {
        if(id == null) 
        {
            return NotFound();
        }
        var entity = Repository.Products.FirstOrDefault(p => p.ProductId == id);
        if(entity == null)
        {
            return NotFound();
        }
        ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
        return View(entity);
    }

     public async Task<IActionResult> Edit(int id, Product model, IFormFile? imageFile)
    {
        var extension = ""; 
        
        if(id != model.ProductId)
        {
            return NotFound();
        }

        if(ModelState.IsValid)
        {
            if(imageFile != null) 
            {
                var randomFileName = string.Format($"{Guid.NewGuid().ToString()}{extension}");
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", randomFileName);

                using(var stream = new FileStream(path, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                model.Image = randomFileName;
            }
            Repository.EditProduct(model);
            return RedirectToAction("Index");
        }
        ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
        return View(model);
    }

     public IActionResult Delete(int? id)
    {
        if(id == null)
        {
            return NotFound();        
        }

        var entity = Repository.Products.FirstOrDefault(p => p.ProductId == id);
        if(entity == null)
        {
            return NotFound();
        }

       // Repository.DeleteProduct(entity);
        return View("DeleteConfirm", entity);
    }

    [HttpPost]
    public IActionResult Delete(int id, int ProductId)
    {
        if(id != ProductId)
        {
            return NotFound();
        }

        var entity = Repository.Products.FirstOrDefault(p => p.ProductId == ProductId);
        if(entity == null)
        {
            return NotFound();
        }

        Repository.DeleteProduct(entity);
        return RedirectToAction("Index");
    }

    
    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Categories = new SelectList(Repository.Categories,"CategoryId","Name");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(int id,Product model,IFormFile imageFile)
    {
       if(id != model.ProductId)
        {
            return NotFound();
        }

        if(ModelState.IsValid)
        {
            if(imageFile != null) 
            {
                var extension = Path.GetExtension(imageFile.FileName); // abc.jpg
                var randomFileName = string.Format($"{Guid.NewGuid().ToString()}{extension}");
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", randomFileName);

                using(var stream = new FileStream(path, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                model.Image = randomFileName;
            }
            Repository.EditProduct(model);
            return RedirectToAction("Index");
        }

        ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
        return View(model);
    }

    [HttpPost]
    public IActionResult EditProducts(List<Product> Products)
    {
        foreach (var product in Products)
        {
            Repository.EditIsActive(product);
        }
        return RedirectToAction("Index");
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

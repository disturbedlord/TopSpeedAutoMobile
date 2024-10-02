using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TopSpeed.Web.Data;
using TopSpeed.Web.Models;

namespace TopSpeed.Web.Controllers
{
    public class BrandController : Controller
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public BrandController(ApplicationDBContext context, IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = context;
            _webHostEnvironment = webHostEnvironment;
        }


        [HttpGet]
        public IActionResult Index()
        {

            List<Brand> brands = _dbContext.Brand.ToList();

            return View(brands);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Create(Brand brand)
        {
            if (ModelState.IsValid)
            {

                string webRootPath = _webHostEnvironment.WebRootPath;
                var file = HttpContext.Request.Form.Files;

                if(file.Any())
                    brand.BrandLogo = SaveBrandLogo(webRootPath, file);

                _dbContext.Brand.Add(brand);
                _dbContext.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        [HttpGet]
        public IActionResult Details(Guid id)
        {
            Brand brand = _dbContext.Brand.FirstOrDefault(b => b.Id == id);
            
            return View(brand);
        }

        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            Brand brand = _dbContext.Brand.FirstOrDefault(b => b.Id == id);

            return View(brand);
        }

        [HttpPost]
        public IActionResult Edit(Brand brand)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = _webHostEnvironment.WebRootPath;
                var file = HttpContext.Request.Form.Files;
                if (file.Any()) {
                    var oldImageObj = _dbContext.Brand.AsNoTracking().FirstOrDefault(b => b.Id == brand.Id).BrandLogo;
                    oldImageObj = oldImageObj?.StartsWith('\\') == true ? oldImageObj?.Trim('\\') : oldImageObj;
                    var imagePath = Path.Combine(webRootPath, oldImageObj);
                    if (oldImageObj != null && System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(Path.Combine(webRootPath, oldImageObj));
                    }
                    brand.BrandLogo = SaveBrandLogo(webRootPath, file);
                }
                _dbContext.Brand.Update(brand);
                _dbContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpGet]
        public IActionResult Delete(Guid id)
        {
            Brand brand = _dbContext.Brand.FirstOrDefault(b => b.Id == id);

            return View(brand);
        }

        [HttpPost]
        public IActionResult Delete(Brand brand)
        {
            if (ModelState.IsValid)
            {
                string logoPath = brand.BrandLogo;
                _dbContext.Brand.Remove(brand);
                _dbContext.SaveChanges();

                if (logoPath != null && logoPath.Length > 0)
                {
                    string webRootPath = _webHostEnvironment.WebRootPath;
                    var oldImageObj = logoPath?.StartsWith('\\') == true ? logoPath?.Trim('\\') : logoPath;
                    var imagePath = Path.Combine(webRootPath, oldImageObj);
                    if (oldImageObj != null && System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View();
        }


        private static string SaveBrandLogo(string _webRootPath , IFormFileCollection _file)
        {
            string webRootPath = _webRootPath;
            var file = _file;
            string newFileName = Convert.ToString(Guid.NewGuid());
            var extension = Path.GetExtension(file[0].FileName);
            var uploadPath = Path.Combine(webRootPath, $@"images\brand\{newFileName}{extension}");
            using (var fStream = new FileStream(uploadPath, FileMode.Create))
            {
                file[0].CopyTo(fStream);
            }
            return $@"\images\brand\{newFileName}{extension}";
        }
    }
}

using Microsoft.AspNetCore.Mvc;
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

                if (file.Any())
                {
                    string newFileName = Convert.ToString(Guid.NewGuid());
                    var extension = Path.GetExtension(file[0].FileName);
                    var uploadPath = Path.Combine(webRootPath, $@"images\brand\{newFileName}{extension}");

                    using (var fStream = new FileStream(uploadPath, FileMode.Create))
                    {
                        file[0].CopyTo(fStream);
                    }

                    brand.BrandLogo = uploadPath;
                }

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
    }
}

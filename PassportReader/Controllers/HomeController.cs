using Microsoft.AspNetCore.Mvc;
using PassportReader.Services;
using System.Diagnostics;
using IronOcr;
namespace PassportReader.Controllers
{
    public class HomeController : Controller
    {
        private readonly MrzService _mrzService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public HomeController(MrzService mrzService, IWebHostEnvironment webHostEnvironment)
        {
            _mrzService = mrzService;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            try
            {
                string[] images = new string[]
                {
                    Path.Combine(_webHostEnvironment.WebRootPath, "img", "pasaport.jpg"),
                    Path.Combine(_webHostEnvironment.WebRootPath, "img", "pasaport2.jpg")
                };
                IList<MrzData> datas = new List<MrzData>();
                foreach (var imgPath in images)
                {
                    var mrzData = _mrzService.ParseMrz(imgPath);
                    datas.Add(mrzData);
                }
                
                return View(datas);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

    }
}

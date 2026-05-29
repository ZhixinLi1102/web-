using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ResumeGenerator.Models;
using ResumeGenerator.Services;
using QuestPDF.Fluent;
using Microsoft.AspNetCore.Http;


namespace ResumeGenerator.Controllers
{
    public class ResumeController : Controller
    {
        private readonly IPdfService _pdfService;

        public ResumeController(IPdfService pdfService)
        {
            _pdfService = pdfService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Create");

        }
        // GET: 输入页
        [HttpGet]
        public IActionResult Create()
        {
            var json = HttpContext.Session.GetString("ResumeData");
            if (!string.IsNullOrEmpty(json))
            {
                var model = JsonConvert.DeserializeObject<ResumeModel>(json);
                return View(model);
            }
            return View(new ResumeModel());
        }



        // POST: 提交表单
        [HttpPost]
        public async Task<IActionResult> Create(ResumeModel model, IFormFile photo)
        {
            //if (!ModelState.IsValid)
            //    return View(model);

            if (photo != null && photo.Length > 0)
            {
                using var ms = new MemoryStream();
                await photo.CopyToAsync(ms);
                model.PhotoBase64 =
                    $"data:{photo.ContentType};base64,{Convert.ToBase64String(ms.ToArray())}";
            }

            HttpContext.Session.SetString("ResumeData",
                JsonConvert.SerializeObject(model));


            return RedirectToAction("Preview");
        }

        // 预览页
        public IActionResult Preview()
        {
            var json = HttpContext.Session.GetString("ResumeData");
            if (string.IsNullOrEmpty(json))
                return RedirectToAction("Create");

            var model = JsonConvert.DeserializeObject<ResumeModel>(json);
            return View(model);
        }

        // 下载 PDF
        public IActionResult DownloadPdf()
        {
            var json = HttpContext.Session.GetString("ResumeData");
            if (string.IsNullOrEmpty(json))
                return RedirectToAction("Create");

            var model = JsonConvert.DeserializeObject<ResumeModel>(json);
            var pdf = _pdfService.GeneratePdf(model);

            return File(pdf, "application/pdf", $"{model.FullName}_简历.pdf");
        }
    }
}
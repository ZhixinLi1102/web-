using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ResumeGenerator.Models;
using ResumeGenerator.Services;
using QuestPDF.Fluent;


namespace ResumeGenerator.Controllers
{
    public class ResumeController : Controller
    {
        private readonly IPdfService _pdfService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ResumeController(IPdfService pdfService, IHttpContextAccessor httpContextAccessor)
        {
            _pdfService = pdfService;
            _httpContextAccessor = httpContextAccessor;
        }

        // 1. 首页 (对应 Views/Resume/Index.cshtml)
        public IActionResult Index(ResumeModel model, IFormFile Photo)
        {
            if (ModelState.IsValid)
            {
                // 1. 处理照片上传
                if (Photo != null && Photo.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        Photo.CopyTo(ms);
                        model.PhotoBase64 = Convert.ToBase64String(ms.ToArray());
                    }
                }

                // 2. 【关键步骤】将数据存入 TempData，以便跳转到预览页读取
                // 注意：TempData 默认只存活一次请求，非常适合这种跳转场景
                TempData["ResumeData"] = JsonConvert.SerializeObject(model);

                // 3. 跳转到预览页
                return RedirectToAction("Preview");
            }

            // 如果验证失败，返回原页面
            return View(model);
        }

        // 预览页的 Action
        public IActionResult Preview()
        {
            // 1. 从 TempData 中取出数据
            var json = TempData["ResumeData"] as string;

            if (string.IsNullOrEmpty(json))
            {
                // 如果没有数据，可能是直接访问的链接，重定向回首页
                return RedirectToAction("Index");
            }

            // 2. 反序列化回对象
            var model = JsonConvert.DeserializeObject<ResumeModel>(json);

            // 3. 传递给 View
            return View(model);
        }

        // 2. 输入页 (GET请求，对应 Views/Resume/Create.cshtml)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // 3. 预览页 (POST请求，接收表单数据)
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Preview(ResumeModel model)
        {
            // 处理照片上传
            if (model.Photo != null)
            {
                using var ms = new MemoryStream();
                await model.Photo.CopyToAsync(ms);
                var bytes = ms.ToArray();
                model.PhotoBase64 = $"data:{model.Photo.ContentType};base64," + Convert.ToBase64String(bytes);
            }

            // 存入 Session
            HttpContext.Session.SetString("ResumeData", JsonConvert.SerializeObject(model));

            return View(model);
        }

        // 4. 下载 PDF
        // 下载PDF的 Action
        public IActionResult DownloadPdf()
        {
            var json = TempData["ResumeData"] as string;
            if (string.IsNullOrEmpty(json)) return RedirectToAction("Index");

            var model = JsonConvert.DeserializeObject<ResumeModel>(json);

            // 这里调用你写好的 PdfService 生成 PDF
            var pdfBytes = _pdfService.GeneratePdf(model);

            return File(pdfBytes, "application/pdf", $"{model.FullName}_简历.pdf");
        }


    }
}
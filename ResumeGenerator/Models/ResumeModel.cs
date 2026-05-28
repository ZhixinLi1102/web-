using Microsoft.AspNetCore.Http;

namespace ResumeGenerator.Models
{
    public class ResumeModel
    {
        // 1. 个人信息
        public string FullName { get; set; }
        public string JobTitle { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Summary { get; set; }

        // 照片
        public IFormFile Photo { get; set; }
        public string PhotoBase64 { get; set; }

        // 2. 教育背景
        public string School { get; set; }
        public string Major { get; set; }
        public string EducationDuration { get; set; }

        // 3. 经历类 (改为大文本，用换行分隔不同条目)
        public string Internships { get; set; }
        public string Projects { get; set; }
        public string CampusExps { get; set; }

        // 4. 专业特长
        public string Skills { get; set; }

        public string JobIntention { get; set; }
    }
}
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ResumeGenerator.Models;
using ResumeGenerator.Services;

public class PdfService : IPdfService
{
    public byte[] GeneratePdf(ResumeModel model)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("SimSun"));

                page.Content().Column(col =>
                {
                    // 头部信息 (姓名、联系方式等)
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c => {
                            c.Item().Text(model.FullName).FontSize(24).Bold();
                            c.Item().Text(model.JobIntention).FontSize(16).SemiBold();
                            c.Item().Text($"📧 {model.Email} | 📞 {model.Phone}");
                        });
                        if (!string.IsNullOrEmpty(model.PhotoBase64))
                            row.ConstantItem(80).Height(100).Image(model.PhotoBase64);
                    });
                    col.Item().LineHorizontal(1);

                    // 教育背景
                    RenderSimpleSection(col, "教育背景", $"{model.School} - {model.Major} ({model.EducationDuration})");

                    // 经历类通用渲染 (实习、项目、校园)
                    RenderTextAreaSection(col, "实习经历", model.Internships);
                    RenderTextAreaSection(col, "项目经历", model.Projects);
                    RenderTextAreaSection(col, "校园经历", model.CampusExps);

                    // 专业特长
                    RenderSimpleSection(col, "专业特长", model.Skills);
                });
            });
        }).GeneratePdf();
    }

    // 辅助方法：渲染简单的单行或多行文本
    void RenderSimpleSection(ColumnDescriptor col, string title, string content)
    {
        if (string.IsNullOrWhiteSpace(content)) return;
        col.Item().Text(title).Bold().FontSize(14);
        col.Item().Text(content);
        col.Item().LineHorizontal(0.5f);
    }

    // 辅助方法：渲染复杂的“经历”文本（支持多段经历和分点）
    void RenderTextAreaSection(ColumnDescriptor col, string title, string content)
    {
        if (string.IsNullOrWhiteSpace(content)) return;

        col.Item().Text(title).Bold().FontSize(14);

        // 用两个换行符分割，代表不同的经历段
        var segments = content.Split(new[] { "\n\n", "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var seg in segments)
        {
            // 第一段作为标题/公司名，其余作为描述
            var lines = seg.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Select(l => l.Trim()).ToList();

            if (lines.Any())
            {
                // 第一行加粗作为经历标题
                col.Item().Text(lines[0]).SemiBold();

                // 剩下的行作为要点
                foreach (var line in lines.Skip(1))
                {
                    // 如果行首没有 • 或 -，自动加上
                    var text = line.StartsWith("-") || line.StartsWith("•") ? line : $"• {line}";
                    col.Item().Text(text).FontSize(10);
                }
                col.Item().PaddingBottom(5); // 段间距
            }
        }
        col.Item().LineHorizontal(0.5f);
    }
}
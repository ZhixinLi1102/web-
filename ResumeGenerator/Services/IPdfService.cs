using ResumeGenerator.Models;

namespace ResumeGenerator.Services
{
    public interface IPdfService
    {
        /// <summary>
        /// 生成简历的 PDF 字节数组
        /// </summary>
        /// <param name="model">简历数据模型</param>
        /// <returns>PDF 文件的字节数组</returns>
        byte[] GeneratePdf(ResumeModel model);
    }
}
namespace ECom.Application.Interfaces.Services.Admin
{
    public interface IReportService
    {
        // Excel Reports
        Task<byte[]> ExportSalesReportExcelAsync(DateTime from, DateTime to);
        Task<byte[]> ExportProductsReportExcelAsync();
        Task<byte[]> ExportUsersReportExcelAsync();

        // PDF Reports
        Task<byte[]> ExportSalesReportPdfAsync(DateTime from, DateTime to);
        Task<byte[]> ExportProductsReportPdfAsync();
        Task<byte[]> ExportUsersReportPdfAsync();
    }
}

using ECom.Application.Interfaces.Services.Admin;
using ECom.Application.Sharing;
using Microsoft.AspNetCore.Mvc;

namespace ECom.API.Controllers.Admin
{
    public class AdminReportController : AdminBaseController
    {
        private readonly IReportService _reportService;

        public AdminReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        // ✅ Excel Reports
        [HttpGet("sales-excel")]
        public async Task<IActionResult> ExportSalesExcel(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            try
            {
                var bytes = await _reportService.ExportSalesReportExcelAsync(from, to);
                return File(bytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"SalesReport_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx");
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        [HttpGet("products-excel")]
        public async Task<IActionResult> ExportProductsExcel()
        {
            try
            {
                var bytes = await _reportService.ExportProductsReportExcelAsync();
                return File(bytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"ProductsReport_{DateTime.UtcNow:yyyyMMdd}.xlsx");
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        [HttpGet("users-excel")]
        public async Task<IActionResult> ExportUsersExcel()
        {
            try
            {
                var bytes = await _reportService.ExportUsersReportExcelAsync();
                return File(bytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"UsersReport_{DateTime.UtcNow:yyyyMMdd}.xlsx");
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        // ✅ PDF Reports
        [HttpGet("sales-pdf")]
        public async Task<IActionResult> ExportSalesPdf(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            try
            {
                var bytes = await _reportService.ExportSalesReportPdfAsync(from, to);
                return File(bytes, "application/pdf",
                    $"SalesReport_{from:yyyyMMdd}_{to:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        [HttpGet("products-pdf")]
        public async Task<IActionResult> ExportProductsPdf()
        {
            try
            {
                var bytes = await _reportService.ExportProductsReportPdfAsync();
                return File(bytes, "application/pdf",
                    $"ProductsReport_{DateTime.UtcNow:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        [HttpGet("users-pdf")]
        public async Task<IActionResult> ExportUsersPdf()
        {
            try
            {
                var bytes = await _reportService.ExportUsersReportPdfAsync();
                return File(bytes, "application/pdf",
                    $"UsersReport_{DateTime.UtcNow:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

    }
}

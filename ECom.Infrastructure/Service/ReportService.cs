using ECom.Application.Interfaces.Services.Admin;
using ECom.Core.Entities;
using ECom.Core.Entities.Order;
using ECom.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace ECom.Infrastructure.Service
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ReportService(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            // ✅ Required for EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // ✅ Required for QuestPDF
            QuestPDF.Settings.License = LicenseType.Community;
        }

        // =====================================
        // ✅ EXCEL REPORTS
        // =====================================
        public async Task<byte[]> ExportSalesReportExcelAsync(DateTime from, DateTime to)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.DeliveryMethod)
                .Where(o => o.OrderDate >= from && o.OrderDate <= to)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Sales Report");

            // ✅ Title
            sheet.Cells["A1:G1"].Merge = true;
            sheet.Cells["A1"].Value = $"Sales Report ({from:dd/MM/yyyy} - {to:dd/MM/yyyy})";
            sheet.Cells["A1"].Style.Font.Bold = true;
            sheet.Cells["A1"].Style.Font.Size = 16;
            sheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // ✅ Summary row
            sheet.Cells["A2"].Value = "Total Orders:";
            sheet.Cells["B2"].Value = orders.Count;
            sheet.Cells["A3"].Value = "Total Revenue:";
            sheet.Cells["B3"].Value = orders
                .Where(o => o.Status == PaymentStatus.PaymentReceived)
                .Sum(o => o.SubTotal);
            sheet.Cells["B3"].Style.Numberformat.Format = "$#,##0.00";

            // ✅ Headers
            var headers = new[]
            {
                "Order ID", "Buyer Email", "Order Date",
                "Status", "Items", "SubTotal", "Total"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                sheet.Cells[5, i + 1].Value = headers[i];
                sheet.Cells[5, i + 1].Style.Font.Bold = true;
                sheet.Cells[5, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[5, i + 1].Style.Fill.BackgroundColor
                    .SetColor(System.Drawing.Color.FromArgb(70, 130, 180));
                sheet.Cells[5, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
            }

            // ✅ Data rows
            int row = 6;
            foreach (var order in orders)
            {
                sheet.Cells[row, 1].Value = order.Id;
                sheet.Cells[row, 2].Value = order.BuyerEmail;
                sheet.Cells[row, 3].Value = order.OrderDate.ToString("dd/MM/yyyy HH:mm");
                sheet.Cells[row, 4].Value = order.Status.ToString();
                sheet.Cells[row, 5].Value = order.OrderItems.Count;
                sheet.Cells[row, 6].Value = order.SubTotal;
                sheet.Cells[row, 6].Style.Numberformat.Format = "$#,##0.00";
                sheet.Cells[row, 7].Value = order.GetTotal();
                sheet.Cells[row, 7].Style.Numberformat.Format = "$#,##0.00";

                // Alternate row colors
                if (row % 2 == 0)
                {
                    sheet.Cells[row, 1, row, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    sheet.Cells[row, 1, row, 7].Style.Fill.BackgroundColor
                        .SetColor(System.Drawing.Color.FromArgb(240, 248, 255));
                }
                row++;
            }

            sheet.Cells.AutoFitColumns();
            return await package.GetAsByteArrayAsync();
        }

        public async Task<byte[]> ExportProductsReportExcelAsync()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToListAsync();

            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Products Report");

            // ✅ Title
            sheet.Cells["A1:H1"].Merge = true;
            sheet.Cells["A1"].Value = $"Products Report - {DateTime.UtcNow:dd/MM/yyyy}";
            sheet.Cells["A1"].Style.Font.Bold = true;
            sheet.Cells["A1"].Style.Font.Size = 16;
            sheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // ✅ Headers
            var headers = new[]
            {
                "ID", "Name", "Category", "Old Price",
                "New Price", "Stock", "Reviews", "Avg Rating"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                sheet.Cells[3, i + 1].Value = headers[i];
                sheet.Cells[3, i + 1].Style.Font.Bold = true;
                sheet.Cells[3, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[3, i + 1].Style.Fill.BackgroundColor
                    .SetColor(System.Drawing.Color.FromArgb(34, 139, 34));
                sheet.Cells[3, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
            }

            // ✅ Data rows
            int row = 4;
            foreach (var product in products)
            {
                sheet.Cells[row, 1].Value = product.Id;
                sheet.Cells[row, 2].Value = product.Name;
                sheet.Cells[row, 3].Value = product.Category?.Name ?? "N/A";
                sheet.Cells[row, 4].Value = product.OldPrice;
                sheet.Cells[row, 4].Style.Numberformat.Format = "$#,##0.00";
                sheet.Cells[row, 5].Value = product.NewPrice;
                sheet.Cells[row, 5].Style.Numberformat.Format = "$#,##0.00";
                sheet.Cells[row, 6].Value = product.StockQuantity;
                sheet.Cells[row, 7].Value = product.TotalReviews;
                sheet.Cells[row, 8].Value = product.AverageRating;

                // ✅ Highlight out of stock in red
                if (product.StockQuantity == 0)
                {
                    sheet.Cells[row, 6].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    sheet.Cells[row, 6].Style.Font.Bold = true;
                }

                row++;
            }

            sheet.Cells.AutoFitColumns();
            return await package.GetAsByteArrayAsync();
        }

        public async Task<byte[]> ExportUsersReportExcelAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Users Report");

            // ✅ Title
            sheet.Cells["A1:F1"].Merge = true;
            sheet.Cells["A1"].Value = $"Users Report - {DateTime.UtcNow:dd/MM/yyyy}";
            sheet.Cells["A1"].Style.Font.Bold = true;
            sheet.Cells["A1"].Style.Font.Size = 16;
            sheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // ✅ Summary
            sheet.Cells["A2"].Value = "Total Users:";
            sheet.Cells["B2"].Value = users.Count;
            sheet.Cells["A3"].Value = "Blocked Users:";
            sheet.Cells["B3"].Value = users.Count(u => u.IsBlocked);

            // ✅ Headers
            var headers = new[]
            {
                "ID", "Username", "Email",
                "Display Name", "Email Confirmed", "Status"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                sheet.Cells[5, i + 1].Value = headers[i];
                sheet.Cells[5, i + 1].Style.Font.Bold = true;
                sheet.Cells[5, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[5, i + 1].Style.Fill.BackgroundColor
                    .SetColor(System.Drawing.Color.FromArgb(128, 0, 128));
                sheet.Cells[5, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
            }

            // ✅ Data rows
            int row = 6;
            foreach (var user in users)
            {
                sheet.Cells[row, 1].Value = user.Id;
                sheet.Cells[row, 2].Value = user.UserName;
                sheet.Cells[row, 3].Value = user.Email;
                sheet.Cells[row, 4].Value = user.DisplayName;
                sheet.Cells[row, 5].Value = user.EmailConfirmed ? "✅" : "❌";
                sheet.Cells[row, 6].Value = user.IsBlocked ? "Blocked" : "Active";

                // ✅ Highlight blocked users
                if (user.IsBlocked)
                {
                    sheet.Cells[row, 6].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    sheet.Cells[row, 6].Style.Font.Bold = true;
                }

                row++;
            }

            sheet.Cells.AutoFitColumns();
            return await package.GetAsByteArrayAsync();
        }

        // =====================================
        // ✅ PDF REPORTS
        // =====================================

        public async Task<byte[]> ExportSalesReportPdfAsync(DateTime from, DateTime to)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.DeliveryMethod)
                .Where(o => o.OrderDate >= from && o.OrderDate <= to)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var totalRevenue = orders
                .Where(o => o.Status == PaymentStatus.PaymentReceived)
                .Sum(o => o.SubTotal);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);

                    page.Header().Element(header =>
                    {
                        header.Text($"Sales Report ({from:dd/MM/yyyy} - {to:dd/MM/yyyy})")
                            .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);
                    });

                    page.Content().Element(content =>
                    {
                        content.Column(col =>
                        {
                            // Summary
                            col.Item().Padding(10).Row(row =>
                            {
                                row.RelativeItem().Text($"Total Orders: {orders.Count}")
                                    .FontSize(12);
                                row.RelativeItem().Text($"Total Revenue: ${totalRevenue:F2}")
                                    .FontSize(12).SemiBold();
                            });

                            // Table
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(50);
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                });

                                // Headers
                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Blue.Medium)
                                        .Padding(5).Text("ID").FontColor(Colors.White).Bold();
                                    header.Cell().Background(Colors.Blue.Medium)
                                        .Padding(5).Text("Email").FontColor(Colors.White).Bold();
                                    header.Cell().Background(Colors.Blue.Medium)
                                        .Padding(5).Text("Date").FontColor(Colors.White).Bold();
                                    header.Cell().Background(Colors.Blue.Medium)
                                        .Padding(5).Text("Status").FontColor(Colors.White).Bold();
                                    header.Cell().Background(Colors.Blue.Medium)
                                        .Padding(5).Text("Total").FontColor(Colors.White).Bold();
                                });

                                // Data
                                foreach (var order in orders)
                                {
                                    var bgColor = orders.IndexOf(order) % 2 == 0
                                        ? Colors.White
                                        : Colors.Grey.Lighten3;

                                    table.Cell().Background(bgColor)
                                        .Padding(5).Text(order.Id.ToString());
                                    table.Cell().Background(bgColor)
                                        .Padding(5).Text(order.BuyerEmail);
                                    table.Cell().Background(bgColor)
                                        .Padding(5).Text(order.OrderDate.ToString("dd/MM/yyyy"));
                                    table.Cell().Background(bgColor)
                                        .Padding(5).Text(order.Status.ToString());
                                    table.Cell().Background(bgColor)
                                        .Padding(5).Text($"${order.GetTotal():F2}");
                                }
                            });
                        });
                    });

                    page.Footer().AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                            x.Span(" of ");
                            x.TotalPages();
                        });
                });
            });

            return document.GeneratePdf();
        }

        public async Task<byte[]> ExportProductsReportPdfAsync()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToListAsync();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(2, Unit.Centimetre);

                    page.Header()
                        .Text($"Products Report - {DateTime.UtcNow:dd/MM/yyyy}")
                        .SemiBold().FontSize(20).FontColor(Colors.Green.Medium);

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            foreach (var h in new[] { "ID", "Name", "Category", "Price", "Stock", "Rating" })
                            {
                                header.Cell().Background(Colors.Green.Medium)
                                    .Padding(5).Text(h).FontColor(Colors.White).Bold();
                            }
                        });

                        foreach (var product in products)
                        {
                            var bgColor = products.IndexOf(product) % 2 == 0
                                ? Colors.White : Colors.Grey.Lighten3;

                            table.Cell().Background(bgColor).Padding(5)
                                .Text(product.Id.ToString());
                            table.Cell().Background(bgColor).Padding(5)
                                .Text(product.Name);
                            table.Cell().Background(bgColor).Padding(5)
                                .Text(product.Category?.Name ?? "N/A");
                            table.Cell().Background(bgColor).Padding(5)
                                .Text($"${product.NewPrice:F2}");
                            table.Cell().Background(bgColor).Padding(5)
                                .Text(product.StockQuantity.ToString())
                                .FontColor(product.StockQuantity == 0
                                    ? Colors.Red.Medium : Colors.Black);
                            table.Cell().Background(bgColor).Padding(5)
                                .Text(product.AverageRating.ToString("F1"));
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" of ");
                        x.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }

        public async Task<byte[]> ExportUsersReportPdfAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);

                    page.Header()
                        .Text($"Users Report - {DateTime.UtcNow:dd/MM/yyyy}")
                        .SemiBold().FontSize(20).FontColor(Colors.Purple.Medium);

                    page.Content().Column(col =>
                    {
                        col.Item().Padding(10).Row(row =>
                        {
                            row.RelativeItem().Text($"Total Users: {users.Count}").FontSize(12);
                            row.RelativeItem()
                                .Text($"Blocked: {users.Count(u => u.IsBlocked)}")
                                .FontSize(12).FontColor(Colors.Red.Medium);
                        });

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                foreach (var h in new[] { "Username", "Email", "Display Name", "Status" })
                                {
                                    header.Cell().Background(Colors.Purple.Medium)
                                        .Padding(5).Text(h).FontColor(Colors.White).Bold();
                                }
                            });

                            foreach (var user in users)
                            {
                                var bgColor = users.IndexOf(user) % 2 == 0
                                    ? Colors.White : Colors.Grey.Lighten3;

                                table.Cell().Background(bgColor).Padding(5)
                                    .Text(user.UserName);
                                table.Cell().Background(bgColor).Padding(5)
                                    .Text(user.Email);
                                table.Cell().Background(bgColor).Padding(5)
                                    .Text(user.DisplayName ?? "N/A");
                                table.Cell().Background(bgColor).Padding(5)
                                    .Text(user.IsBlocked ? "Blocked" : "Active")
                                    .FontColor(user.IsBlocked
                                        ? Colors.Red.Medium : Colors.Green.Medium);
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" of ");
                        x.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}

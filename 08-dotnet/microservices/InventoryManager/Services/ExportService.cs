using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting;
using DevExpress.Export;

namespace InventoryManager.Services
{
    /// <summary>
    /// Service for exporting data to various formats
    /// </summary>
    public static class ExportService
    {
        /// <summary>
        /// Export grid to Excel file
        /// </summary>
        public static void ExportGridToExcel(GridView gridView, string defaultFileName)
        {
            using var dialog = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx|Excel 97-2003|*.xls",
                FileName = $"{defaultFileName}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var options = new XlsxExportOptionsEx
                    {
                        ExportType = ExportType.WYSIWYG,
                        SheetName = defaultFileName,
                        TextExportMode = TextExportMode.Value
                    };

                    gridView.ExportToXlsx(dialog.FileName, options);

                    if (XtraMessageBox.Show("Export completed! Open file?", "Success",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = dialog.FileName,
                            UseShellExecute = true
                        });
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"Export error: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Export grid to PDF file
        /// </summary>
        public static void ExportGridToPdf(GridView gridView, string defaultFileName)
        {
            using var dialog = new SaveFileDialog
            {
                Filter = "PDF Files|*.pdf",
                FileName = $"{defaultFileName}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var options = new PdfExportOptions
                    {
                        ShowPrintDialogOnOpen = false,
                        DocumentOptions =
                        {
                            Title = defaultFileName,
                            Subject = $"Generated on {DateTime.Now:yyyy-MM-dd HH:mm}",
                            Author = "Inventory Manager Pro"
                        }
                    };

                    gridView.ExportToPdf(dialog.FileName, options);

                    if (XtraMessageBox.Show("Export completed! Open file?", "Success",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = dialog.FileName,
                            UseShellExecute = true
                        });
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"Export error: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Export grid to HTML file
        /// </summary>
        public static void ExportGridToHtml(GridView gridView, string defaultFileName)
        {
            using var dialog = new SaveFileDialog
            {
                Filter = "HTML Files|*.html",
                FileName = $"{defaultFileName}_{DateTime.Now:yyyyMMdd_HHmmss}.html"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var options = new HtmlExportOptions
                    {
                        ExportMode = HtmlExportMode.SingleFile,
                        Title = defaultFileName
                    };

                    gridView.ExportToHtml(dialog.FileName, options);

                    XtraMessageBox.Show("Export completed!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"Export error: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Export grid to text file
        /// </summary>
        public static void ExportGridToText(GridView gridView, string defaultFileName)
        {
            using var dialog = new SaveFileDialog
            {
                Filter = "Text Files|*.txt|CSV Files|*.csv",
                FileName = $"{defaultFileName}_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (dialog.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                    {
                        gridView.ExportToCsv(dialog.FileName);
                    }
                    else
                    {
                        gridView.ExportToText(dialog.FileName);
                    }

                    XtraMessageBox.Show("Export completed!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"Export error: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Print grid with preview
        /// </summary>
        public static void PrintGrid(GridView gridView, string reportTitle)
        {
            try
            {
                var printingSystem = new PrintingSystem();
                var printableComponent = new PrintableComponentLink(printingSystem)
                {
                    Component = gridView.GridControl
                };

                // Configure header
                printableComponent.CreateReportHeaderArea += (s, e) =>
                {
                    e.Graph.StringFormat = new BrickStringFormat(StringAlignment.Center);
                    e.Graph.Font = new Font("Segoe UI", 18, FontStyle.Bold);
                    var rect = new RectangleF(0, 0, e.Graph.ClientPageSize.Width, 50);
                    e.Graph.DrawString(reportTitle, Color.Black, rect, BorderSide.None);
                    
                    e.Graph.Font = new Font("Segoe UI", 10);
                    var dateRect = new RectangleF(0, 30, e.Graph.ClientPageSize.Width, 20);
                    e.Graph.DrawString($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm}", Color.Gray, dateRect, BorderSide.None);
                };

                printableComponent.CreateDocument();

                // Show preview
                using var preview = new DevExpress.XtraPrinting.Preview.PrintPreviewFormEx
                {
                    PrintingSystem = printingSystem
                };
                preview.ShowDialog();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Print error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Export data to all formats at once
        /// </summary>
        public static void ExportAll(GridView gridView, string baseFileName, string folderPath)
        {
            try
            {
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var basePath = Path.Combine(folderPath, $"{baseFileName}_{timestamp}");

                gridView.ExportToXlsx($"{basePath}.xlsx");
                gridView.ExportToPdf($"{basePath}.pdf");
                gridView.ExportToHtml($"{basePath}.html");
                gridView.ExportToCsv($"{basePath}.csv");

                XtraMessageBox.Show($"Exported to:\n{folderPath}", "Export Complete",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Export error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

using Aspose.Pdf;
using Aspose.Pdf.Text;
using Core;
using Core.Helpers;
using OfficeOpenXml;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helper;

public static class ImportExportHelper<T> where T : class
{
    public static ExportStream? ExportFile(ExportFileInfoDTO exportFileInfo, IEnumerable<T> fileContent)
    {
        string exportType = exportFileInfo.Type;

        return String.Equals(exportType, AppDocument.EXCEL, StringComparison.OrdinalIgnoreCase)
            ? ExportExcel(exportFileInfo, fileContent)
            : String.Equals(exportType, AppDocument.PDF, StringComparison.OrdinalIgnoreCase)
                ? ExportPdf(exportFileInfo, fileContent)
                : null;
    }

    public static ExportStream ExportExcel(ExportFileInfoDTO exportFileInfo, IEnumerable<T> fileContent)
    {
        if (string.IsNullOrEmpty(exportFileInfo.FileName) || string.IsNullOrEmpty(exportFileInfo.SheetName)) return null;
        try
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                ExportDefault(package, exportFileInfo, fileContent);
            }
            stream.Position = 0;

            return new ExportStream()
            {
                FileName = $"{exportFileInfo.FileName}.xlsx",
                Stream = stream,
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static void ExportDefault(ExcelPackage package, ExportFileInfoDTO exportFileInfo, IEnumerable<T> fileContent)
    {
        var objectType = typeof(T);
        var properties = objectType.GetProperties();
        var headers = GetHeaders(objectType);
        var rows = properties.Select(p => p.Name).ToList();

        // name of the sheet
        var workSheet = package.Workbook.Worksheets.Add(exportFileInfo.SheetName);

        // 2.Title
        var titleCell = workSheet.Cells[1, 1];
        titleCell.Value = exportFileInfo.Title.ToUpper();
        titleCell.Style.Font.Bold = true;
        titleCell.Style.Font.Size = 16;

        // 3. Description
        var descCell = workSheet.Cells[2, 1];
        descCell.Value = exportFileInfo.Description;
        descCell.Style.Font.Size = 12;

        // Header of the Excel sheet
        for (int i = 0; i < headers.Count; i++)
        {
            workSheet.Cells[4, i + 1].Value = headers[i];
            workSheet.Cells[4, i + 1].Style.Font.Bold = true;
            workSheet.Cells[4, i + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        }

        // Inserting the data into excel
        // sheet by using the for each loop
        // As we have values to the first row
        // we will start with second row
        int recordIndex = 5;
        foreach (var item in fileContent)
        {
            for (int i = 0; i < rows.Count; i++)
            {
                workSheet.Cells[recordIndex, i + 1].Value = objectType.GetProperty(rows[i]).GetValue(item);
                workSheet.Cells[recordIndex, i + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                if (i == 0)
                {
                    workSheet.Cells[recordIndex, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }
                if (workSheet.Cells[recordIndex, i + 1].Value != null &&
                    workSheet.Cells[recordIndex, i + 1].Value.GetType() == typeof(DateTime))
                {
                    workSheet.Cells[recordIndex, i + 1].Style.Numberformat.Format = "dd-MM-yyyy HH:mm:ss";
                }
            }
            recordIndex++;
        }

        workSheet.Cells.AutoFitColumns();
        workSheet.Column(1).Width = 5;
        package.Save();
    }

    public static ExportStream ExportPdf(ExportFileInfoDTO exportFileInfo, IEnumerable<T> fileContent)
    {
        var objectType = typeof(T);
        var headers = GetHeaders(objectType);
        var rows = objectType.GetProperties().Select(p => p.Name).ToList();

        var document = new Aspose.Pdf.Document
        {
            PageInfo = new PageInfo
            {
                Margin = new MarginInfo(28, 28, 28, 40)
            }
        };
        Page page = document.Pages.Add();

        // Title
        var title = new TextFragment(exportFileInfo.Title.ToUpper());
        title.TextState.FontSize = 18;
        title.TextState.FontStyle = FontStyles.Bold;
        title.TextState.HorizontalAlignment = HorizontalAlignment.Center;
        title.TextState.ForegroundColor = Aspose.Pdf.Color.Black; ;

        // Description
        TextFragment description = new TextFragment(exportFileInfo.Description);
        description.TextState.FontSize = 14;
        description.TextState.HorizontalAlignment = HorizontalAlignment.Center;
        description.Margin.Bottom = 10;

        page.Paragraphs.Add(title);
        page.Paragraphs.Add(description);

        // data table
        Table table = new()
        {
            ColumnAdjustment = ColumnAdjustment.AutoFitToWindow,
            DefaultCellPadding = new MarginInfo(5, 5, 5, 5),
            Border = new BorderInfo(BorderSide.All, .5f, Aspose.Pdf.Color.Black),
            DefaultCellBorder = new BorderInfo(BorderSide.All, .2f, Aspose.Pdf.Color.Black),
        };

        var headerRow = table.Rows.Add();
        foreach (string header in headers)
        {
            Cell cell = headerRow.Cells.Add(header);
            cell.DefaultCellTextState.ForegroundColor = Aspose.Pdf.Color.Black;
            cell.DefaultCellTextState.FontStyle = FontStyles.Bold;
        }

        foreach (var item in fileContent)
        {
            var dataRow = table.Rows.Add();
            for (int i = 0; i < rows.Count; i++)
            {
                var rowData = Convert.ToString(objectType.GetProperty(rows[i]).GetValue(item));

                if (objectType.GetProperty(rows[i]).GetValue(item) != null &&
                    objectType.GetProperty(rows[i]).GetValue(item).GetType() == typeof(DateTime))//format datetime
                {
                    DateTime date = (DateTime)objectType.GetProperty(rows[i]).GetValue(item);
                    rowData = date.ToString("dd-MM-yyyy HH:mm:ss");
                }
                else if (objectType.GetProperty(rows[i]).GetValue(item) != null &&
                    objectType.GetProperty(rows[i]).GetValue(item).GetType() == typeof(decimal))// format number
                {
                    decimal number = (decimal)objectType.GetProperty(rows[i]).GetValue(item);
                    NumberFormatInfo numberFormatInfo =
                        (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
                    numberFormatInfo.NumberDecimalSeparator = ",";
                    numberFormatInfo.NumberGroupSeparator = ".";
                    rowData = string.Format(numberFormatInfo, "{0:n}", number);
                }
                dataRow.Cells.Add(rowData);
            }
        }

        page.Paragraphs.Add(table);
        document.PageInfo.IsLandscape = true;
        var stream = new MemoryStream();
        document.Save(stream);
        stream.Position = 0;

        return new ExportStream
        {
            FileName = $"{exportFileInfo.FileName}.pdf",
            Stream = stream,
            ContentType = "application/pdf"
        };
    }

    private static List<string> GetHeaders(Type type)
    {
        var properties = type.GetProperties();
        var headers = new List<string>();
        foreach (var property in properties)
        {
            var attributes = property.GetCustomAttributes(typeof(DataMemberAttribute), false);
            foreach (DataMemberAttribute dma in attributes.Cast<DataMemberAttribute>())
            {
                if (!string.IsNullOrEmpty(dma.Name))
                {
                    headers.Add(dma.Name);
                }
            }
        }
        return headers;
    }
}


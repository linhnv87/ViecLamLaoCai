using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helper;

public static class DateFormatHelper
{
    public static string ConvertDateRange(string fromDate, string toDate)
    {
        string fromDateFormatted = FormatDate(fromDate);
        string toDateFormatted = FormatDate(toDate);

        if (!string.IsNullOrEmpty(fromDateFormatted) && !string.IsNullOrEmpty(toDateFormatted))
            return $"Từ {fromDateFormatted} đến {toDateFormatted}";
        else if (!string.IsNullOrEmpty(fromDateFormatted))
            return $"Từ {fromDateFormatted}";
        else if (!string.IsNullOrEmpty(toDateFormatted))
            return $"Đến {toDateFormatted}";
        else
            return "Không có ngày cụ thể";
    }

    public static string FormatDate(string date)
    {
        if (string.IsNullOrEmpty(date)) return null;

        if (DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
        {
            return $"ngày {parsedDate.Day} tháng {parsedDate.Month} năm {parsedDate.Year}";
        }

        return null;
    }
}

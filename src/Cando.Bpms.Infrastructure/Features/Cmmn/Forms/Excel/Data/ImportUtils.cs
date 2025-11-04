namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Excel.Data;

class ImportUtils
{
    internal static string Normalize(string s)
    {
        return string.IsNullOrWhiteSpace(s) || s == "-" ? null : s.NormalizeFarsi().Trim();
    }
    internal static DateTime GetDate(string text)
    {
        DateTime tim = DateTime.MinValue;
        if (!string.IsNullOrWhiteSpace(text))
        {
            string[] parts = text.Split('-', '/');
            if (parts.Length >= 3)
            {
                if (int.TryParse(parts[0].Trim(), out int y) && int.TryParse(parts[1].Trim(), out int m) && int.TryParse(parts[2].Trim(), out int day))
                {
                    if (y < 1600)
                    {
                        PersianCalendar pc = new();
                        tim = pc.ToDateTime(y, m, day, 0, 0, 0, 0);
                    }
                    else
                        tim = new DateTime(y, m, day, 0, 0, 0, 0);
                }
            }
        }

        return tim;
    }

    internal static string GetTimeSpan(string text)
    {
        string GetResult(TimeSpan ts)
        {
            return $"{ts.Hours}:{ts.Minutes}:{ts.Seconds}";
        }
        if (string.IsNullOrWhiteSpace(text))
            return null;
        if (double.TryParse(text, out double d) && d <= 1)
        {
            return GetResult(TimeSpan.FromMilliseconds(d * 86400000L));
        }
        string[] parts = text.Split(':');
        if (parts.Length == 2)
        {
            if (int.TryParse(parts[0].Trim(), out int h) && int.TryParse(parts[1].Trim(), out int m) && h < 24 && m < 60)
            {
                return GetResult(new TimeSpan(h, m, 0));
            }
        }
        else if (parts.Length == 3)
        {
            string[] p0S = parts[0].Split(' ');
            string[] p2S = parts[2].Split(' ');
            bool pm = p2S.Length > 1 && (p2S[1] == "ب.ظ" || p2S[1] == "PM");
            if (int.TryParse(p0S[p0S.Length - 1], out int h) && int.TryParse(parts[1], out int m) && int.TryParse(p2S[0], out int s) && h < 24 && m < 60 && s < 60)
            {
                if (h == 12) h = 0; if (pm) h += 12;
                return GetResult(new TimeSpan(h, m, s));
            }
        }
        return GetResult(TimeSpan.MinValue);
    }
}

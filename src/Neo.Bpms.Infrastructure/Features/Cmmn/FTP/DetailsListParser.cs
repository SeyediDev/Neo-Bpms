using System.Text.RegularExpressions;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.FTP;

public class ListingDetailsParser
{
    public static string ExtractDate(string detailRecord)
    {
        #region UNIX LIKE LISTING

        // EXAMPLE OF POSSIBLE LISTINGS THAT CAN BE RESOLVED: 
        // -rw-r--r--    1 ftp      ftp        659450 Jun 15 05:07 TEST.TXT
        // drwxrwxr-x    2 ftp      ftp          4096 May 06 12:24 dropoff

        var match1 = Regex.Match(detailRecord, @"\w{3}\s{1,2}\d{1,2}\s{1,2}\d{1,2}:\d{1,2}");
        if (match1.Success) return match1.Value;

        // EXAMPLE OF POSSIBLE LISTINGS THAT CAN BE RESOLVED: 
        // dr-xr-xr-x   1 owner    group               0 Nov 25  2002 bussys
        // d--x--x--x    2 ftp      ftp          4096 Mar 07  2002 bin
        // -rw-r--r--    1 ftp      ftp      101786380 Sep 08  2008 TEST03-05.TXT

        var match2 = Regex.Match(detailRecord, @"\w{3}\s{1,2}\d{1,2}\s{1,2}\d{4}");
        if (match2.Success) return match2.Value;

        #endregion

        #region WINDOWS LIKE LISTING

        // EXAMPLE OF POSSIBLE LISTINGS THAT CAN BE RESOLVED: 
        // 02-03-04  07:46pM       <DIR>          Append
        // 08-10-11  12:02am       <DIR>          Version2
        // 06-25-09   02:41AM            144700153 image34.gif
        // 06-25-09     02:51PM            144700153 updates.txt
        // 11-04-10  02:45PM            144700214 digger.tif

        var match3 = Regex.Match(detailRecord, @"\d{1,2}-\d{1,2}-\d{1,2}\s+\d{1,2}:\d{1,2}(PM|pm|AM|am)");
        if (match3.Success) return match3.Value;

        #endregion

        throw new Exception("Could not extract date: unknown listing format.");
    }

    public static string ExtractObjectName(string detailRecord)
    {
        #region UNIX LIKE LISTING

        // EXAMPLE OF POSSIBLE LISTINGS THAT CAN BE RESOLVED: 
        // -rw-r--r--    1 ftp      ftp        659450 Jun 15 05:07 TEST.TXT
        // drwxrwxr-x    2 ftp      ftp          4096 May 06 12:24 dropoff

        var match1 = Regex.Match(detailRecord, @"\w{3}\s{1,2}\d{1,2}\s{1,2}\d{1,2}:\d{1,2}");
        if (match1.Success) return detailRecord[(match1.Index + match1.Length)..].Trim();

        // EXAMPLE OF POSSIBLE LISTINGS THAT CAN BE RESOLVED: 
        // dr-xr-xr-x   1 owner    group               0 Nov 25  2002 bussys
        // d--x--x--x    2 ftp      ftp          4096 Mar 07  2002 bin
        // -rw-r--r--    1 ftp      ftp      101786380 Sep 08  2008 TEST03-05.TXT

        var match2 = Regex.Match(detailRecord, @"\w{3}\s{1,2}\d{1,2}\s{1,2}\d{4}");
        if (match2.Success) return detailRecord[(match2.Index + match2.Length)..].Trim();

        #endregion

        #region WINDOWS LIKE LISTING

        // EXAMPLE OF POSSIBLE LISTINGS THAT CAN BE RESOLVED: 
        // 02-03-04  07:46pM       <DIR>          Append
        // 08-10-11  12:02am       <DIR>          Version2
        // 06-25-09  02:41AM            144700153 image34.gif
        // 06-25-09  02:51PM            144700153 updates.txt
        // 11-04-10  02:45PM            144700214 digger.tif

        var match3 = Regex.Match(detailRecord, @"\d{9}");
        if (match3.Success) return detailRecord[(match3.Index + match3.Length)..].Trim();

        #endregion

        throw new Exception("Could not extract directory or file name: unknown listing format.");
    }
}

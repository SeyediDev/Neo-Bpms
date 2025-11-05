#pragma warning disable SYSLIB0014 // WebRequest, HttpWebRequest, ServicePoint, and WebClient are obsolete. Use HttpClient instead.
using System.Net;

namespace Neo.Bpms.Infrastructure.Utility.FTP;

public class FtpFile : FtpFileSystemObject
{
    public string FilePath { get; private set; }
    //  public long Length { get; internal set; }
    public FtpDirectory ParentDirectory { get; private set; }

    public FtpFile(Uri requestUri, NetworkCredential credentials)
        : base(requestUri, credentials)
    {
        FileInfo fi = new(requestUri.AbsolutePath);
        if (fi.Name == string.Empty && fi.Extension == string.Empty)
            throw new Exception("Specified path is not a file");

        FilePath = requestUri.AbsoluteUri;

        var directoryName = Path.GetDirectoryName(CurrentDirectory.ToString());
        if (directoryName != null)
        {
            var parentDir = directoryName.Replace(@"\", @"/").Replace("ftp:/", "ftp://");
            ParentDirectory = new FtpDirectory(parentDir, credentials);
        }
    }

    public FtpFile(string requestUri, NetworkCredential credentials)
        : this(new Uri(requestUri), credentials) { }

    public override void Download(string destination)
    {
        string localPath = Path.Combine(destination, Name);
        WebClient.DownloadFile(FullPath, localPath);
    }

    public override void Delete()
    {
        Request = (FtpWebRequest)WebRequest.Create(FullPath);
        Request.Credentials = Credentials;
        Request.Method = WebRequestMethods.Ftp.DeleteFile;

        //FtpWebResponse response = (FtpWebResponse)
        Request.GetResponse();
    }
}
#pragma warning restore SYSLIB0014

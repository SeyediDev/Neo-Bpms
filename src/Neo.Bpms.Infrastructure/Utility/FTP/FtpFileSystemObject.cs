#pragma warning disable SYSLIB0014 // WebRequest, HttpWebRequest, ServicePoint, and WebClient are obsolete. Use HttpClient instead.
using System.Dynamic;
using System.Net;

namespace Neo.Bpms.Infrastructure.Utility.FTP;

public abstract class FtpFileSystemObject : DynamicObject
{
    public class PermissionAttributes
    {
        public bool IsDirectory { get; private set; } //this property may not seem strictly pertinent here..

        public class Rights
        {
            public bool Read { get; private set; }
            public bool Write { get; private set; }
            public bool Execute { get; private set; }

            public Rights(string rights)
            {
                if (rights.Length != 3)
                    throw new ArgumentException("Permission string must be 3 chars long.");

                if (rights.Any(@char => @char != 'r' && @char != 'w' && @char != 'x' && @char != '-'))
                    throw new ArgumentException("Permission string (except first char) must contain only 'r', 'w', 'x' or '-' chars.");

                Read = rights.Contains('r');
                Write = rights.Contains('w');
                Execute = rights.Contains('x');
            }
        }

        public Rights OwnerRights { get; private set; }
        public Rights GroupRights { get; private set; }
        public Rights OtherRights { get; private set; }

        /// <param name="rights">chmod-style rights (ex: drwxrwxrwx)</param>
        public PermissionAttributes(string rights)
        {
            if (rights.Length != 10)
                throw new ArgumentException("Permission string must be 10 chars long.");

            rights = rights.ToLower();
            if (rights.First() != '-' && rights.First() != 'd')
                throw new ArgumentException("Permission's first char must be '-' or 'd'.");

            if (rights.Skip(1).Any(@char => @char != 'r' && @char != 'w' && @char != 'x' && @char != '-'))
                throw new ArgumentException("Permission string (except first char) must contain only 'r', 'w', 'x' or '-' chars.");

            OwnerRights = new Rights(rights.Substring(1, 3));
            GroupRights = new Rights(rights.Substring(4, 3));
            OtherRights = new Rights(rights.Substring(7, 3));

            IsDirectory = string.Compare(rights[0].ToString(), "d", StringComparison.OrdinalIgnoreCase) == 0;
        }
    }

    protected FtpWebRequest Request { get; set; }
    protected WebClient WebClient { get; set; }

    public PermissionAttributes Permissions { get; internal set; }

    //To avoid conversion problems due to unpredictable data format I use a string to represent a Date
    public string LastModified { get; internal set; }

    public string Name { get; private set; }
    public string FullPath { get; private set; }

    public Uri CurrentDirectory { get; set; }
    public NetworkCredential Credentials { get; protected set; }

    public FtpFileSystemObject(Uri requestUri, NetworkCredential credentials)
    {
        Credentials = credentials;

        CurrentDirectory = requestUri;

        Request = (FtpWebRequest)WebRequest.Create(requestUri);
        Request.ConnectionGroupName = GetHashCode().ToString();
        Request.Credentials = credentials;
        Request.KeepAlive = true;

        WebClient = new WebClient();
        WebClient.Credentials = credentials;

        Name = GetDirectoryName(requestUri);
        FullPath = requestUri.AbsoluteUri;
    }

    private string GetDirectoryName(Uri requestUri)
    {
        if (requestUri.AbsolutePath == "/")
            return requestUri.ToString();

        FileInfo fi = new(requestUri.AbsolutePath);
        return fi.Directory != null ? fi.Directory.Name : null;
    }

    public FtpFileSystemObject(string requestUri, NetworkCredential credentials)
        : this(new Uri(requestUri), credentials) { }

    public void Rename(string newName)
    {
        var request = (FtpWebRequest)WebRequest.Create(FullPath);
        request.Method = WebRequestMethods.Ftp.Rename;
        request.RenameTo = newName;

        request.GetResponse();
    }

    protected IEnumerable<string> GetDirectoryListDetails()
    {
        Request = (FtpWebRequest)WebRequest.Create(CurrentDirectory);
        Request.ConnectionGroupName = GetHashCode().ToString();
        Request.Credentials = Credentials;
        Request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
        Request.KeepAlive = true;

        FtpWebResponse response = (FtpWebResponse)Request.GetResponse();
        using (Stream responseStream = response.GetResponseStream())
        {
            if (responseStream == null) return null;
            using (StreamReader reader = new(responseStream))
            {
                var fileSystemDetails = reader.ReadToEnd();
                return fileSystemDetails.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            }
        }
    }

    public override sealed int GetHashCode()
    {
        return Credentials.Password.GetHashCode() ^
               Credentials.UserName.GetHashCode() ^
               Credentials.Domain.GetHashCode() ^
               // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
               base.GetHashCode() ^
               101;
    }

    public abstract void Download(string destination);

    public abstract void Delete();
}
#pragma warning restore SYSLIB0014


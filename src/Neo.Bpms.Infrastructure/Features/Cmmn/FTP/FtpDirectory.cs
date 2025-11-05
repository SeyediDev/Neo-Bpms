#pragma warning disable SYSLIB0014 // WebRequest, HttpWebRequest, ServicePoint, and WebClient are obsolete. Use HttpClient instead.
using System.Dynamic;
using System.Net;
using System.Text.RegularExpressions;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.FTP;

public class FtpDirectory : FtpFileSystemObject
{
    public FtpDirectory(Uri initialUri, NetworkCredential credentials)
        : base(initialUri, credentials)
    {
        FileInfo fi = new(initialUri.AbsolutePath);
        if (fi.Name != string.Empty && fi.Extension != string.Empty)
            throw new Exception(string.Format("Specified path '{0}' is not a directory", initialUri));
    }

    public FtpDirectory(string initialUri, NetworkCredential credentials)
        : this(new Uri(initialUri), credentials) { }

    /// <summary>
    /// Reference to the upper level directory
    /// </summary>
    public FtpDirectory UpperLevel
    {
        get
        {
            if (CurrentDirectory.Segments.Length <= 1) return null;
            var tmp = CurrentDirectory.ToString().LastIndexOf(CurrentDirectory.Segments.Last(), StringComparison.Ordinal);
            var upperLevelUri = CurrentDirectory.ToString()[..tmp];

            return new FtpDirectory(upperLevelUri, Credentials);
        }
    }

    #region SubDirectories

    public IEnumerable<FtpDirectory> SubDirectories
    {
        get
        {
            foreach (var listingDetail in GetDirectoryListDetails())
            {
                var rights = listingDetail[..10];
                var perms = new PermissionAttributes(rights);

                if (perms.IsDirectory)
                {
                    string dirName = ListingDetailsParser.ExtractObjectName(listingDetail);

                    if (dirName != "." && dirName != "..")
                    {
                        Uri newUri = new(dirName);

                        yield return new FtpDirectory(newUri, Credentials)
                        {
                            Permissions = new PermissionAttributes(rights),
                            LastModified = ListingDetailsParser.ExtractDate(listingDetail)
                        };
                    }
                }
            }
        }
    }

    /// <summary>
    /// Get all subdirectories from all sublevels starting from current directory
    /// </summary>
    public IEnumerable<FtpDirectory> GetAllSubDirectories()
    {
        return GetAllSubDirectories(this);
    }

    private IEnumerable<FtpDirectory> GetAllSubDirectories(FtpDirectory dir)
    {
        foreach (var subDir in dir.SubDirectories)
        {
            yield return new FtpDirectory(subDir.FullPath, Credentials);

            foreach (var subSubDir in GetAllSubDirectories(subDir))
                yield return subSubDir;
        }
    }

    #endregion

    #region Files

    public IEnumerable<FtpFile> Files
    {
        get
        {
            foreach (var listingDetail in GetDirectoryListDetails())
            {
                var rights = listingDetail[..10];
                var perms = new PermissionAttributes(rights);

                if (!perms.IsDirectory)
                {
                    string fileName = ListingDetailsParser.ExtractObjectName(listingDetail);

                    Uri newUri = new(fileName);

                    yield return new FtpFile(newUri, Credentials)
                    {
                        Permissions = new PermissionAttributes(rights),
                        LastModified = ListingDetailsParser.ExtractDate(listingDetail)
                    };
                }
            }
        }
    }

    /// <summary>
    /// Get all files from all subdirectories starting from current directory
    /// </summary>
    public IEnumerable<FtpFile> GetAllFiles()
    {
        return GetAllFiles(this);
    }

    private IEnumerable<FtpFile> GetAllFiles(FtpDirectory ftp)
    {
        foreach (var subDir in ftp.SubDirectories)
        {
            foreach (var file in subDir.Files)
                yield return file;

            foreach (var file in GetAllFiles(subDir))
                yield return file;
        }
    }

    #endregion

    #region Download

    public override void Download(string destination)
    {
        Download(destination, ".*");
    }

    public void Download(string destination, string regexSearchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        if (regexSearchPattern == null) throw new ArgumentNullException("regexSearchPattern");
        Directory.CreateDirectory(destination);

        foreach (var file in Files)
        {
            if (Regex.IsMatch(file.Name, regexSearchPattern))
                file.Download(destination);
        }

        if (searchOption == SearchOption.AllDirectories)
        {
            foreach (var subdir in SubDirectories)
                subdir.Download(Path.Combine(destination, subdir.Name), regexSearchPattern, searchOption);
        }
    }

    public IEnumerable<FtpFile> DownloadAndGetFtpReference(string destination, string regexSearchPattern = ".*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        Directory.CreateDirectory(destination);

        foreach (var file in Files)
        {
            if (Regex.IsMatch(file.Name, regexSearchPattern))
            {
                file.Download(destination);
                yield return file;
            }
        }

        if (searchOption == SearchOption.AllDirectories)
        {
            foreach (var subdir in SubDirectories)
            {
                var files = subdir.DownloadAndGetFtpReference(Path.Combine(destination, subdir.Name), regexSearchPattern, searchOption);
                foreach (var file in files)
                    yield return file;
            }
        }
    }

    #endregion

    #region Upload

    public void UploadDirectory(string localDir)
    {
        var files = Directory.GetFiles(localDir, "*.*", SearchOption.TopDirectoryOnly);
        foreach (var file in files)
            UploadFile(file);

        var dirs = Directory.GetDirectories(localDir, "*.*", SearchOption.TopDirectoryOnly);
        foreach (var dir in dirs)
        {
            var dirInfo = new DirectoryInfo(dir);

            var newFtpDir = CreateSubDirectory(dirInfo.Name);
            newFtpDir.UploadDirectory(dir);
        }
    }

    public FtpFile UploadFile(string localFile)
    {
        string localFileName = Path.GetFileName(localFile);
        string remoteFile = Path.Combine(FullPath, localFileName).ToString();

        WebClient.UploadFile(remoteFile, localFile);

        return new FtpFile(remoteFile, Credentials);
    }

    #endregion

    #region Delete

    /// <summary>
    /// Removes all files and subdirectories from current directory
    /// </summary>
    public override void Delete()
    {
        foreach (var file in Files)
            file.Delete();

        foreach (var subdir in SubDirectories)
            subdir.Delete();

        Request = (FtpWebRequest)WebRequest.Create(CurrentDirectory);
        Request.Credentials = Credentials;
        Request.Method = WebRequestMethods.Ftp.RemoveDirectory;

        //FtpWebResponse response = (FtpWebResponse)
        Request.GetResponse();
    }

    #endregion

    /// <summary>
    /// Creates all directories in specified path as subdirectories.
    /// If directory already exists retuns a reference to that directory.
    /// </summary>
    /// <example> \sub1\sub2\sub3</example>
    /// <returns>Returns a reference to the new directory</returns>
    public FtpDirectory CreateSubDirectory(string path)
    {
        // 1. test if final path exists: if so return a reference to it
        string finalPath = Path.Combine(FullPath, path).Replace(@"\", @"/");

        if (GetAllSubDirectories().All(dir => dir.FullPath != finalPath))
        {
            // 2. if final path do not exists create each subdirectory
            var subDirs = path.Replace(@"\", @"/").Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            var newDir = FullPath;
            foreach (var subDir in subDirs)
            {
                newDir = Path.Combine(newDir, subDir).Replace(@"\", @"/");

                if (GetAllSubDirectories().All(dir => dir.FullPath != newDir))
                {
                    var request = (FtpWebRequest)WebRequest.Create(newDir);
                    request.Credentials = Credentials;
                    request.Method = WebRequestMethods.Ftp.MakeDirectory;
                    request.GetResponse();
                }
            }
        }

        return new FtpDirectory(finalPath, Credentials);
    }

    public FtpDirectory ChangeDirectory(string path)
    {
        string finalPath = Path.Combine(FullPath, path).Replace(@"\", @"/");

        if (GetAllSubDirectories().Any(dir => dir.FullPath == finalPath))
            return new FtpDirectory(finalPath, Credentials);

        throw new DirectoryNotFoundException(string.Format("Could not find '{0}'", finalPath));
    }

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        result = ChangeDirectory(binder.Name);
        return true;
    }
}
#pragma warning restore SYSLIB0014

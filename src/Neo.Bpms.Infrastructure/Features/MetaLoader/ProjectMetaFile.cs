using System.Xml;

namespace Neo.Bpms.Infrastructure.Features.MetaLoader;

public static class ProjectMetaFile
{
    public static string MetaPath
    {
        get
        {
            if (!string.IsNullOrEmpty(_metaPath))
                return _metaPath;
            var config = DependencyInjectionHolder.Instance.Configuration;
            _metaPath = config["MetaPath"];
            if (string.IsNullOrWhiteSpace(_metaPath))
                _metaPath = "C:\\Meta";
            return _metaPath;
        }
    }

    private static string _metaPath;

    internal static void SaveFile(string fileContent, string path, string fileName)
    {
        InitDirectory(path);
        fileName = fileName.Replace(":", "-");
        var fullFileName = $"{path}\\{fileName}";
        using (var bpmnFile = new StreamWriter(new FileStream(fullFileName, FileMode.Create)))
        {
            bpmnFile.Write(fileContent);
        }
    }

    internal static void SaveObjectToFile<T>(T o, string path, string fileName)
    {
        InitDirectory(path);
        var json = o.ToJson();
        var fullFileName = $"{path}\\{fileName}";
        using (var xmlFile = new StreamWriter(new FileStream(fullFileName, FileMode.Create)))
        {
            xmlFile.Write(json);
        }
    }

    //public static string InitMetaFolder()
    //{
    //	var directory = MetaPath;
    //	InitDirectory(directory);
    //	return directory;
    //}

    public static void InitDirectory(string directory)
    {
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
    }

    public static XmlReader ReadFile(string fileName)
    {
        using (var sr = new StreamReader(fileName))
        {
            var xmlText = sr.ReadToEnd();
            return XmlReader.Create(new StringReader(xmlText));
        }
    }

    public static string LastDirectoryName(string directory)
    {
        var splitFolder = directory.Split('\\');
        return splitFolder[splitFolder.Length - 1];
    }

    public static string FetchFileName(string fileName)
    {
        var splitFolder = fileName.Split('\\');
        var last = splitFolder[splitFolder.Length - 1];
        splitFolder = last.Split('.');
        return splitFolder[0];
    }


    public static bool DeletePhysicalFile(string path, string fileName)
    {
        if (string.IsNullOrEmpty(path))
            return false;
        if (string.IsNullOrEmpty(fileName))
            return false;
        path += '\\' + fileName;
        if (File.Exists(path))
        {
            try
            {
                File.Delete(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        return false;
    }

    public static bool DeletePhisycalDirectory(string path, string directoryName = null)
    {
        if (!string.IsNullOrEmpty(directoryName))
            path += '\\' + directoryName;
        if (Directory.Exists(path))
        {
            try
            {
                Directory.Delete(path, true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        return false;
    }
}

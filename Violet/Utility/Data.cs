using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Violet;

/// <summary>
/// Handles the loading of things in the data folder.
/// </summary>
public class DataHandler
{
    public static DataHandler instance;

    //Key: File Name
    //Value: File Path
    private Dictionary<string, string> fileNamesToPaths;


    public static void Initialize()
    {
        if (instance != null)
        {
            Debug.LogWarning($"Another instance of the Data class tried to be initialized!");
            return;
        }

        instance = new DataHandler();
    }

    public DataHandler()
    {
        fileNamesToPaths = new Dictionary<string, string>();
        ProcessDirectory("Data" + Path.DirectorySeparatorChar);
    }

    // stolen from
    // https://learn.microsoft.com/en-us/dotnet/api/system.io.directory.getfiles?view=net-7.0
    private void ProcessDirectory(string targetDirectory)
    {
        // Process the list of files found in the directory.
        string[] fileEntries = Directory.GetFiles(targetDirectory);
        foreach (string fileName in fileEntries)
        {
            ProcessFile(fileName);
        }

        // Recurse into subdirectories of this directory.
        string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
        foreach (string subdirectory in subdirectoryEntries)
        {
            ProcessDirectory(subdirectory);

        }
    }


    private void ProcessFile(string path)
    {
        string fileName = Path.GetFileName(path);
        if (fileName.Contains("lua"))
        {
            return;
        }

        if (fileNamesToPaths.ContainsKey(fileName))
        {
            fileNamesToPaths.TryGetValue(fileName, out string value);
            Debug.LogWarning($"Duplicate file detected! Original: {value} - Duplicate: {path} ");
            return;
        }
        fileNamesToPaths.Add(fileName, path);
        Debug.LogInfo($"Processed data file '{Path.GetFileName(path)}'");
    }

    public string Load(string file)
    {
       fileNamesToPaths.TryGetValue(file, out string val);
        return val;
    }
}

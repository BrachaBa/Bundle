using System;
using System.Collections.Generic;
using System.IO;
using System.CommandLine;
using System.CommandLine.Invocation;
//using EO.WebBrowser;

List<string> GetFiles(string path, string language)
{
    List<string> fileList = new List<string>();
    GetFilesRecursive(path, language, fileList);
    return fileList;
}



void GetFilesRecursive(string path, string language, List<string> fileList)
{
    fileList.AddRange(Directory.GetFiles(path, "*" + GetFileExtension(language)));

    foreach (var directory in Directory.GetDirectories(path))
    {
        GetFilesRecursive(directory, language, fileList);
    }
}

string GetFileExtension(string language)
{
    switch (language.ToLower())
    {
        case "c#":
            return ".cs";
        case "java":
            return ".java";
        case "python":
            return ".py";
        case "js":
            return ".js";
        case "html":
            return ".html";
        case "sql":
            return ".sql";
        case "react":
            return ".jsx";
        case "c++":
            return ".cpp";
        case "c":
            return ".c";
        case "css":
            return ".css";
        case "angular":
            return ".ts";
        default:
            {
                Console.WriteLine("This language is invalid");
                return null;
            }
    }
}
var rootCommand = new RootCommand("Root Command For File Bundle CLI");

var bundleCommand = new Command("bundle", "Bundle code files to a single file");


var bundleOutput = new Option<FileInfo>("--output", "File path and name");
bundleOutput.IsRequired = true;

var bundleLanguage = new Option<string>("--language", "Programming language to include in the bundle")
    .FromAmong("c#", "c", "c++", "java", "python", "angular", "js", "html", "css");
bundleLanguage.AddAlias("-l");
bundleLanguage.IsRequired = true;

var bundleNote = new Option<bool>("--note", "Include source code information as a comment in the bundle");
bundleNote.AddAlias("-n");

var bundleSort = new Option<bool>("--sort", "Sort order for copying code files (name or type)");
bundleSort.AddAlias("-s");

var bundleRemoveEmptyLines = new Option<bool>("--remove-empty-lines", "Remove empty lines from code files before copying");
bundleRemoveEmptyLines.AddAlias("-e");

var bundleAuthor = new Option<string>("--author", "Author name to be included in the bundle as a comment");
bundleAuthor.AddAlias("-a");

bundleCommand.AddOption(bundleOutput);
bundleCommand.AddOption(bundleLanguage);
bundleCommand.AddOption(bundleNote);
bundleCommand.AddOption(bundleSort);
bundleCommand.AddOption(bundleRemoveEmptyLines);
bundleCommand.AddOption(bundleAuthor);

bundleCommand.SetHandler<FileInfo, string, bool, bool, bool, string>((output, language, note, sort, removeEmptyLines, author) =>
{
    string path = output.Directory.FullName;
    List<string> fileList = GetFiles(path, language);
    File.Create(output.FullName).Dispose();

    //string language = bundleLanguage.ToString();
    //Console.WriteLine(path);

    // Optionally sort fileList
    if (sort)
    {
        fileList.Sort();
    }

    // Optionally remove empty lines from code files
    if (removeEmptyLines)
    {
        // Implement logic to remove empty lines
    }

    // Optionally include source code information as a comment
    // Optionally include source code information as a comment
    if (note)
    {
        if (!File.Exists(output.FullName))
        {
            File.Create(output.FullName).Dispose();
        }
        // הוסף הערה עם מידע על קובץ הפלט
        string comment = $"// Bundle file for {language} at {output.FullName}";
        File.AppendAllText(output.FullName, comment + Environment.NewLine);
        foreach (var filePath in fileList)
        {
            // הלוגיקה הקיימת שלך להוספת הערות וקוד
            string fileName = Path.GetFileName(filePath);
            // Implement logic to include source code information as a comment
            // Append the comment to the new file
            File.AppendAllText(output.FullName, comment + Environment.NewLine);
            // Now append the content of the file
            File.AppendAllText(output.FullName, File.ReadAllText(filePath) + Environment.NewLine);
        }
    }
    // Optionally include author name as a comment
    if (!string.IsNullOrEmpty(author))
    {
        // Implement logic to include author name as a comment
    }

    //File.WriteAllLines(output.FullName, fileList);
    using (StreamWriter writer = new StreamWriter(output.FullName, true))
    {
        foreach (var filePath in fileList)
        {
            string fileName = Path.GetFileName(filePath);

            // Implement logic to include source code information as a comment
            string comment = $"// Source code information for: {fileName}";

            // Append the comment to the new file
            writer.WriteLine(comment);

            // Now append the content of the file
            writer.WriteLine(File.ReadAllText(filePath));
        }
    }

    Console.WriteLine("File was created");

});


rootCommand.AddCommand(bundleCommand);

rootCommand.InvokeAsync(args).Wait();

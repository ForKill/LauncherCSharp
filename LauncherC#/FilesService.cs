using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LauncherC_
{
  internal class FilesService
  {
    private const string Path = "files_education/";

    private List<Files> files = new List<Files>();

    public async Task AddFile(string path)
    {
      FileInfo fileInfo = new FileInfo(path);
      Console.WriteLine(fileInfo.Length);
      files.Add(new Files(Utils.GetHash(fileInfo.LastWriteTime.ToString())));
    } 
  }
}

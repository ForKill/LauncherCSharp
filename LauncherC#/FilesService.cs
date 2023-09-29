using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LauncherC_
{
  internal class FilesService
  {
    private Config config = new Config();

    public async Task Add(ApiData apiData)
    {
      string path = config.FilesPath + "\\" + apiData.Name;
      FileInfo fileInfo = new FileInfo(path);

      string json;
      List<Files> files = new List<Files>();
      if (File.Exists(config.FilesSave))
      {
        json = File.ReadAllText(config.FilesSave);
        files = JsonSerializer.Deserialize<List<Files>>(json);
      }

      files.Add(new Files(Utils.GetHash(fileInfo.LastWriteTime.ToString()), apiData));
      json = JsonSerializer.Serialize(files);
      File.WriteAllText(config.FilesSave, json);
    }

    public async Task CheckFiles()
    {
      if (!File.Exists(config.FilesSave))
        return;

      List<Files> files = new List<Files>();
      string json = File.ReadAllText(config.FilesSave);
      files = JsonSerializer.Deserialize<List<Files>>(json);

      if (files.Count == 0)
        return;

      FileInfo fileInfo;
      Files file;
      await Task.Run(async () =>
      {
        foreach (var fullPath in Directory.EnumerateFiles(config.FilesPath, "*.*", SearchOption.AllDirectories))
        {
          file = files.FirstOrDefault(f => f.Path == fullPath);
          if (file == null)
            continue;
          
          fileInfo = new FileInfo(Path.GetFileName(fullPath));
          var hash = Utils.GetHash(fileInfo.LastWriteTime.ToString());
          if (hash != file.WriteTimeHash)
          {
            ApiData apiData = new ApiData(file.Name, file.Path, file.Hash, file.Size, file.Read);
            await Add(apiData);
          }
        }
      });
    }
  }
}

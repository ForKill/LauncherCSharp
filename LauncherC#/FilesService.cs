using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LauncherC_
{
  internal class FilesService
  {
    private Config config = new Config();

    public async Task Add(string fullPathName)
    {
      string path = config.FilesPath + "\\" + fullPathName;
      FileInfo fileInfo = new FileInfo(path);

      string json;
      List<Files> files = new List<Files>();
      if (File.Exists(config.FilesSave))
      {
        json = File.ReadAllText(config.FilesSave);
        files = JsonSerializer.Deserialize<List<Files>>(json);
      }
      files.Add(new Files(fullPathName, Utils.GetHash(fileInfo.LastWriteTime.ToString())));
      json = JsonSerializer.Serialize(files);
      File.WriteAllText(config.FilesSave, json);
    }

    public async Task Delete(Files file)
    {
      string json;
      List<Files> files = new List<Files>();
      if (File.Exists(config.FilesSave))
      {
        json = File.ReadAllText(config.FilesSave);
        files = JsonSerializer.Deserialize<List<Files>>(json);
      }
      
      if(!files.Remove(file))
      {
        Lines.ShowErrorInfo("Ошибка удаления.");
      }
      json = JsonSerializer.Serialize(files);
      File.WriteAllText(config.FilesSave, json);
    }

    public async Task<List<Files>> GetFiles()
    {
      string json;
      if (!File.Exists(config.FilesSave))
        return null;

      List<Files> files = new List<Files>();
      json = File.ReadAllText(config.FilesSave);
      files = JsonSerializer.Deserialize<List<Files>>(json);
      return files;
    }
  }
}

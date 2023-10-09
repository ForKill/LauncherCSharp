using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LauncherC_
{
  /// <summary>
  /// Сервис работы с files.json
  /// </summary>
  internal class FilesService
  {
    #region Поля и свойства

    /// <summary>
    /// Экземпляр конфигураций.
    /// </summary>
    private Config config = new Config();

    #endregion

    #region Методы

    /// <summary>
    /// Добавить данные в files.json
    /// </summary>
    /// <param name="path">Полный путь.</param>
    /// <param name="hash">Хэш файла.</param>
    public async Task Add(string path, string hash)
    {
      string fullPathName = config.FilesPath + "\\" + path;
      FileInfo fileInfo = new FileInfo(fullPathName);

      string json;
      List<Files> files = new List<Files>();
      if (File.Exists(config.FilesSave))
      {
        json = File.ReadAllText(config.FilesSave);
        files = JsonSerializer.Deserialize<List<Files>>(json);
      }
      files.Add(new Files(path, hash, Utils.GetStringHash(fileInfo.LastWriteTime.ToString())));
      json = JsonSerializer.Serialize(files);
      File.WriteAllText(config.FilesSave, json);
    }

    /// <summary>
    /// Удалить из files.json
    /// </summary>
    /// <param name="file">Сущность файла.</param>
    public async Task Delete(Files file)
    {
      if (!File.Exists(config.FilesSave))
        return;

      string json = File.ReadAllText(config.FilesSave);
      List<Files> files = JsonSerializer.Deserialize<List<Files>>(json);
      if (!files.Remove(file))
      {
        Lines.ShowErrorInfo("Ошибка удаления.");
      }
      json = JsonSerializer.Serialize(files);
      File.WriteAllText(config.FilesSave, json);
    }

    /// <summary>
    /// Удалить из files.json
    /// </summary>
    /// <param name="path">Путь файла/Ключ API</param>
    public async Task Delete(string path)
    {
      if (!File.Exists(config.FilesSave))
        return;

      string json = File.ReadAllText(config.FilesSave);
      List<Files> files = JsonSerializer.Deserialize<List<Files>>(json);

      Files file = files.FirstOrDefault(f => f.Path == path);

      if (file == null)
        return;

      files.Remove(file);
      json = JsonSerializer.Serialize(files);
      File.WriteAllText(config.FilesSave, json);
    }

    /// <summary>
    /// Список файлов в files.json
    /// </summary>
    /// <returns>Список файлов.</returns>
    public async Task<List<Files>> GetFiles()
    {
      if (!File.Exists(config.FilesSave))
        return null;

      string json = File.ReadAllText(config.FilesSave);
      List<Files> files = JsonSerializer.Deserialize<List<Files>>(json);
      return files;
    }

    /// <summary>
    /// Обновить данные файла в files.json
    /// </summary>
    /// <param name="path">Путь до файла.</param>
    /// <param name="hash">Хэш файла.</param>
    public async Task Update(string path, string hash)
    {
      if (!File.Exists(config.FilesSave))
      {
        await Add(path, hash);
        return;
      }
      string fullPathName = config.FilesPath + "\\" + path;
      string json = File.ReadAllText(config.FilesSave);
      List<Files> files = JsonSerializer.Deserialize<List<Files>>(json);

      Files file = files.FirstOrDefault(f => f.Path == path);

      if (file == null)
      {
        await Add(path, hash);
        return;
      }
      FileInfo fileInfo = new FileInfo(fullPathName);
      file.Hash = hash;
      file.WriteTimeHash = Utils.GetStringHash(fileInfo.LastWriteTime.ToString());

      json = JsonSerializer.Serialize(files);
      File.WriteAllText(config.FilesSave, json);
    }

    /// <summary>
    /// Очистить и удалить files.json
    /// </summary>
    public void Clear()
    {
      if (!File.Exists(config.FilesSave))
        return;
      
      File.Delete(config.FilesSave);
    }

    #endregion
  }
}

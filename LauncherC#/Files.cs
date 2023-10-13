using System;

namespace LauncherC_
{
  /// <summary>
  /// Обращение к данным файлов.
  /// </summary>
  public class Files
  {
    /// <summary>
    /// Время хеш последнего изменения.
    /// </summary>
    public string WriteTimeHash { get; set; }

    /// <summary>
    /// Путь до файла.
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// Хэш файла полученные из API.
    /// </summary>
    public string Hash { get; set; }

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="path">Путь до файла.</param>
    /// <param name="hash">Хэш файла полученные из API.</param>
    /// <param name="writetimehash">Время хеш последнего изменения.</param>
    public Files(string path, string hash, string writetimehash)
    {
      Path = path;
      Hash = hash;
      WriteTimeHash = writetimehash;
    }

    public override bool Equals(object obj)
    {
      if (obj is Files other)
        return this.Path == other.Path && this.WriteTimeHash == other.WriteTimeHash && this.Hash == other.Hash;
      return false;
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(Path, Hash, WriteTimeHash);
    }
  }
}

using System;
using System.Collections.Generic;

namespace LauncherC_
{
  /// <summary>
  /// Обращение к API данным.
  /// </summary>
  public class ApiData
  {
    /// <summary>
    /// Имя файла.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Путь до файла.
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// Хэш файла.
    /// </summary>
    public string Hash { get; set; }

    /// <summary>
    /// Размер файла.
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// Конструктор файлов.
    /// </summary>
    public ApiData() { }

    /// <summary>
    /// Конструктор файлов.
    /// </summary>
    /// <param name="name">Имя файла.</param>
    /// <param name="path">Путь до файла.</param>
    /// <param name="hash">Хэш файла.</param>
    /// <param name="size">Размер файла.</param>
    public ApiData(string name, string path, string hash, long size) 
    {
      Name = name;
      Path = path;
      Hash = hash;
      Size = size;
    }

    public override bool Equals(object obj)
    {
      if (obj is ApiData other)
        return this.Name == other.Name && this.Path == other.Path && this.Hash == other.Hash && this.Size == other.Size;
      return false;
    }

    public override int GetHashCode() => HashCode.Combine(Name, Path, Hash, Size);

  }

  /// <summary>
  /// Обращение к API данным с ключом.
  /// </summary>
  public class ApiDataRoot
  {
    public Dictionary<string, ApiData> apidata { get; set; }
  }

  /// <summary>
  /// Обращение к данным API версии сборки.
  /// </summary>
  public class ApiDataApp
  {
    /// <summary>
    /// Хэш сборки.
    /// </summary>
    public string Hash { get; set; }

    /// <summary>
    /// Версия сборки.
    /// </summary>
    public int Version { get; set; }

    public override bool Equals(object obj)
    {
      if (obj is ApiDataApp other)
        return this.Hash == other.Hash && this.Version == other.Version;
      return false;
    }

    public override int GetHashCode() => HashCode.Combine(Hash, Version);
  }
}

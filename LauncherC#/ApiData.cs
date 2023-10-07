using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml.Linq;

namespace LauncherC_
{
  public class ApiData
  {
    public string Name { get; set; }
    public string Path { get; set; }
    public string Hash { get; set; }
    public long Size { get; set; }

    public ApiData() { }

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

  public class ApiDataRoot
  {
    public Dictionary<string, ApiData> apidata { get; set; }
  }

  public class ApiDataApp
  {
    public string Hash { get; set; }
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

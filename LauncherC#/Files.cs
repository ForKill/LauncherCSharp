using System;
using System.Collections.Generic;

namespace LauncherC_
{
  public class Files
  {
    public string WriteTimeHash { get; set; }
    public string Path { get; set; }
    public string Hash { get; set; }

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

    public override int GetHashCode() =>
      HashCode.Combine(Path, Hash, WriteTimeHash);
    
  }
}

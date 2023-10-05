using System;
using System.Collections.Generic;

namespace LauncherC_
{
  public class Files
  {
    public string WriteTimeHash { get; set; }
    public string Path { get; set; }

    public Files(string path, string writetimehash)
    {
      Path = path;
      WriteTimeHash = writetimehash;
    }

    public override bool Equals(object obj)
    {
      if (obj is Files other)
        return this.Path == other.Path && this.WriteTimeHash == other.WriteTimeHash;
      return false;
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(Path, WriteTimeHash);
    }
  }
}

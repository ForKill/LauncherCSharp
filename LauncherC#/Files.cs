using System;

namespace LauncherC_
{
  public class Files : ApiData
  {
        public string WriteTimeHash { get; set; }

    public Files(string WriteTimeHash) => this.WriteTimeHash = WriteTimeHash;
    
  }
}

﻿using System.Collections.Generic;
namespace LauncherC_
{
  public class ApiData
  {
    public string Name { get; set; }
    public string Path { get; set; }
    public string Hash { get; set; }
    public long Size { get; set; }
    public bool Read { get; set; }
  }

  public class ApiDataRoot
  {
    public Dictionary<string, ApiData> apidata { get; set; }
  }

  public class ApiDataApp
  {
    public string Hash { get; set; }
    public int Version { get; set; }
  }
}

using System.Collections.Generic;
namespace LauncherC_
{
  public class ApiData
  {
    public string name { get; set; }
    public string path { get; set; }
    public string hash { get; set; }
    public long size { get; set; }
    public bool read { get; set; }
  }

  public class ApiDataRoot
  {
    public Dictionary<string, ApiData> apidata { get; set; }
  }

  public class ApiDataApp
  {
    public string hash { get; set; }
    public int version { get; set; }
  }
}

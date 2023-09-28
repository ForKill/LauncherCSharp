namespace LauncherC_
{
  public class Download
  {
    public string Url;
    public ApiData ApiData;

    public Download(string url, ApiData apiData)
    {
      Url = url;
      ApiData = apiData;
    }
  }
}

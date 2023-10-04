namespace LauncherC_
{
  public class Files : ApiData
  {
    public string WriteTimeHash { get; set; }

    public Files() { }

    public Files(string WriteTimeHash, ApiData apiData)
      : base(apiData.Name, apiData.Path, apiData.Hash, apiData.Size, apiData.Read)
    {
      this.WriteTimeHash = WriteTimeHash;
    }
  }
}

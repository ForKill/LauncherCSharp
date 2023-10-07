namespace LauncherC_
{
  /// <summary>
  /// Обращение к очереди файлов для скачивания.
  /// </summary>
  public class Download
  {
    /// <summary>
    /// Ссылка на файл.
    /// </summary>
    public string Url;

    /// <summary>
    /// Сущность API данных.
    /// </summary>
    public ApiData ApiData;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="url">Ссылка на файл.</param>
    /// <param name="apiData">Сущность API данных.</param>
    public Download(string url, ApiData apiData)
    {
      Url = url;
      ApiData = apiData;
    }
  }
}

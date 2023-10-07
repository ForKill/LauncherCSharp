using System;

namespace LauncherC_
{
  public class Config
  {
    /// <summary>
    /// Путь до страницы работы сервиса API и скачивания.
    /// </summary>
    public const string Url = "http://test.criminalrussia.org/launcher_c_sharp/";

    /// <summary>
    /// Папка откуда качать файлы.
    /// </summary>
    public const string UrlFiles = Url + "files/";

    /// <summary>
    /// URL Путь к API.
    /// </summary>
    public const string Api = Url + "api.php?token=gjjs8j340ssad&p=";

    /// <summary>
    /// URL API к данным файлов.
    /// </summary>
    public const string FilesHash = Api + "hash";
    
    /// <summary>
    /// URL путь к версии сборки.
    /// </summary>
    public const string AppUpdate = Api + "appupdate";

    /// <summary>
    /// Папка в которую будем скачивать сборку.
    /// </summary>
    public string FilesPath { get; } = AppDomain.CurrentDomain.BaseDirectory + "Education\\";

    /// <summary>
    /// Файл в котором будем хранить данные скаченных файлов.
    /// </summary>
    public string FilesSave { get; } = AppDomain.CurrentDomain.BaseDirectory + "files.json";

    /// <summary>
    /// Файл в котором храним последнюю версию сборки файлов.
    /// </summary>
    public string VersionSave { get; } = AppDomain.CurrentDomain.BaseDirectory + "version.json";

    /// <summary>
    /// Ширина консоли.
    /// </summary>
    public const int ConsoleWidth = 160;

    /// <summary>
    /// Высота консоли.
    /// </summary>
    public const int ConsoleHeight = 48;

    /// <summary>
    /// Стандартные цвет шрифта.
    /// </summary>
    public const ConsoleColor ColorFont = ConsoleColor.White;

    /// <summary>
    /// Стандартный цвет фона строки консоли.
    /// </summary>
    public const ConsoleColor ColorBG = ConsoleColor.Black;
  }
}

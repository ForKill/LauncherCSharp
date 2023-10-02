using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace LauncherC_
{
  public class Config
  {
    public const string Url = "http://test.criminalrussia.org/launcher_c_sharp/";
    public const string UrlFiles = Url + "files/";
    public const string Api = Url + "api.php?token=gjjs8j340ssad&p=";
    public const string FilesHash = Api + "hash";
    public const string AppUpdate = Api + "appupdate";
    public string FilesPath { get; } = AppDomain.CurrentDomain.BaseDirectory + "Education\\";
    public string FilesSave { get; } = AppDomain.CurrentDomain.BaseDirectory + "files.json";

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

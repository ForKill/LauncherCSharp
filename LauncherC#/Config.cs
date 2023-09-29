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
  }
}

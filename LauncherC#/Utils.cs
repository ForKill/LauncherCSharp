using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LauncherC_
{
  public static class Utils
  {
    public static string GetDownloadSpeed(long bytes, double totalseconds) =>
      (Convert.ToDouble(bytes) / 1024 / totalseconds).ToString("0,00") + " КБ/с";

    public static string GetHash(string input)
    {
      var md5 = MD5.Create();
      var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
      return Convert.ToBase64String(hash);
    }
  }
}

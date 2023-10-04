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
    public static string GetHash(string input)
    {
      var md5 = MD5.Create();
      var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
      return Convert.ToBase64String(hash);
    }

    public static string HumanizeByteSize(this long byteCount)
    {
      string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
      if (byteCount == 0)
        return "0" + suf[0];
      long bytes = Math.Abs(byteCount);
      int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
      double num = Math.Round(bytes / Math.Pow(1024, place), 1);
      return Math.Sign(byteCount) * num + suf[place];
    }

    public static string HumanizeByteSize(this double byteCount)
    {
      if (double.IsNaN(byteCount) || double.IsInfinity(byteCount) || byteCount == 0)
        return string.Empty;

      return HumanizeByteSize((long)byteCount);
    }
  }
}

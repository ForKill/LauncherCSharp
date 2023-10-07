using System;
using System.IO;
using System.Security.Cryptography;

namespace LauncherC_
{
  /// <summary>
  /// Полезные методы.
  /// </summary>
  public static class Utils
  {
    /// <summary>
    /// Хэш строки.
    /// </summary>
    /// <param name="input">Текст.</param>
    /// <returns>Хэш.</returns>
    public static string GetStringHash(string input)
    {
      using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
      {
        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        byte[] hashBytes = md5.ComputeHash(inputBytes);

        return Convert.ToHexString(hashBytes); // .NET 5 +
      }
    }

    /// <summary>
    /// Конвертация скорости скачивания.
    /// </summary>
    /// <param name="byteCount">Байты.</param>
    /// <returns>Формат скорости.</returns>
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

    /// <summary>
    /// Вычисление хэша файла.
    /// </summary>
    /// <param name="filename">Путь к файлу.</param>
    /// <returns></returns>
    public static string CalculateMD5(string filename)
    {
      try
      {
        using (var md5 = IncrementalHash.CreateHash(HashAlgorithmName.MD5))
        {
          using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
          {
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            do
            {
              bytesRead = stream.Read(buffer, 0, buffer.Length);
              md5.AppendData(buffer, 0, bytesRead);
            }
            while (bytesRead > 0);
          }
          string actualHash = BitConverter.ToString(md5.GetHashAndReset()).Replace("-", string.Empty).ToLowerInvariant();
          return actualHash;
        }
      }
      catch (Exception ex)
      {
        return filename;
      }
    }
  }
}

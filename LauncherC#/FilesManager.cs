﻿using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

namespace LauncherC_
{
  /// <summary>
  /// Работа с сервисами.
  /// </summary>
  internal class FilesManager
  {
    #region Поля и свойства

    /// <summary>
    /// Экземпляр конфигураций.
    /// </summary>
    Config config = new Config();

    /// <summary>
    /// Скачать ли принудительно все заново.
    /// </summary>
    private bool downloadAll = false;

    #endregion

    #region Методы

    /// <summary>
    /// Установка значения для скачивания без проверок.
    /// </summary>
    /// <param name="status">true/false</param>
    public void DownloadAll(bool status)
    {
      downloadAll = status;
    }

    /// <summary>
    /// Статус для скачивания без проверок.
    /// </summary>
    /// <returns>true/false</returns>
    public bool DownloadAll()
    {
      return downloadAll;
    }

    /// <summary>
    /// Полный процесс проверки файлов.
    /// </summary>
    /// <param name="apiDataService">Сущность данных API сервиса.</param>
    /// <param name="filesService">Сущность данных файлового сервиса.</param>
    /// <param name="downloadService">Сущ</param>
    /// <returns></returns>
    public async Task CheckingForDownload(ApiDataService apiDataService, FilesService filesService, DownloadService downloadService)
    {
      Lines.DeleteFromLast(Lines.InfoLineNumber + 1);
      Lines.ShowInfo($"Получаем API списка файлов.", ConsoleColor.DarkGray);

      Dictionary<string, ApiData> apiData;
      try
      {
        apiData = await apiDataService.GetData();

        if (apiData.Count == 0)
        {
          Lines.ShowErrorInfo("Данных API нет.");
          return;
        }
      }
      catch (Exception ex)
      {
        Lines.ShowErrorInfo(ex.Message);
        return;
      }

      var uFiles = await apiDataService.GetUnnecessaryFiles();
      foreach (var file in uFiles)
        File.Delete(file);

      await downloadService.Clear();
      if (downloadAll)
      {
        filesService.Clear();
        foreach (var apidata in apiData)
          await downloadService.AddDownloadQueue(apidata.Key, apidata.Value);
      }
      else
      {
        var jFiles = await filesService.GetFiles();

        if (jFiles == null)
        {
          foreach (var apidata in apiData)
          {
            string path = config.FilesPath + apidata.Key;
            if (File.Exists(path))
            {
              string hash = Utils.CalculateMD5(path);
              if (apidata.Value.Hash != hash)
              {
                File.Delete(path);
                await downloadService.AddDownloadQueue(apidata.Key, apidata.Value);
                continue;
              }
              await filesService.Add(apidata.Key, apidata.Value.Hash);
              continue;
            }
            await downloadService.AddDownloadQueue(apidata.Key, apidata.Value);
          }
        }
        else
        {
          foreach (var file in jFiles)
          {
            if (!apiData.Keys.Contains(file.Path) || !apiData.Values.Any(value => value.Hash.Equals(file.Hash)))
              await filesService.Delete(file);
          }

          jFiles = await filesService.GetFiles();
          foreach (var apidata in apiData)
          {
            string path = config.FilesPath + apidata.Key;
            if (File.Exists(path))
            {
              FileInfo fileInfo = new FileInfo(path);
              string lastWriteTime = Utils.GetStringHash(fileInfo.LastWriteTime.ToString());

              if (jFiles.Any(f => f.Path.Equals(apidata.Key) && f.WriteTimeHash.Equals(lastWriteTime)))
                continue;

              string hash = Utils.CalculateMD5(path);
              if (apidata.Value.Hash != hash)
              {
                File.Delete(path);
                await downloadService.AddDownloadQueue(apidata.Key, apidata.Value);
                continue;
              }
              await filesService.Update(apidata.Key, apidata.Value.Hash);
              continue;
            }

            await downloadService.AddDownloadQueue(apidata.Key, apidata.Value);
          }
        }
      }

      var filesQueue = await downloadService.GetDownloadQueue();
      if (filesQueue.Count > 0)
        Lines.ShowInfo($"Кол-во файлов требущих обновление: {filesQueue.Count}", ConsoleColor.DarkGray);
      else
        Lines.ShowInfo("Нет файлов требующих обновления.", ConsoleColor.DarkGray);
    }

    #endregion
  }
}

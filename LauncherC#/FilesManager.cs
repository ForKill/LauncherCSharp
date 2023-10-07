using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

namespace LauncherC_
{
  internal class FilesManager
  {
    Config config = new Config();

    private bool downloadAll = false;

    public void DownloadAll(bool status) => downloadAll = status;

    public bool DownloadAll() => downloadAll;

    public async Task CheckingForDownload(ApiDataService apiDataService, FilesService filesService, DownloadService downloadService)
    {
      Lines.DeleteFromLast(Lines.InfoLineNumber + 1);
      Lines.ShowInfo($"Получаем API списка файлов.", ConsoleColor.DarkGray);

      Dictionary<string, ApiData> apiData = await apiDataService.GetData();

      if (apiData.Count == 0)
      {
        Lines.ShowErrorInfo("Данных API нет.");
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
      if(filesQueue.Count > 0)
        Lines.ShowInfo($"Кол-во файлов требущих обновление: {filesQueue.Count}", ConsoleColor.DarkGray);
      else
        Lines.ShowInfo("Нет файлов требующих обновления.", ConsoleColor.DarkGray);
    }
  }
}

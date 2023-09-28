﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace LauncherC_
{
  public class Config
  {
    public const string Url = "http://endless-rp.com/launcher_c_sharp/api.php?token=gjjs8j340ssad&p=";
    public const string FilesHash = Url + "hash";
    public const string AppUpdate = Url + "appupdate";
    public string FilesPath { get; } = AppDomain.CurrentDomain.BaseDirectory + "Education\\";
  }
}
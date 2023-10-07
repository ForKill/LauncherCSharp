using System;
using System.Threading;

namespace LauncherC_
{
  /// <summary>
  /// Работа со строками консоли.
  /// </summary>
  public class Lines
  {
    #region Поля и свойства
    /// <summary>
    /// Сколько всего отрисованных строк в консоли нами используется.
    /// </summary>
    public static int LineNumber;

    /// <summary>
    /// Номер строки в которой отоброжаются ошибки.
    /// </summary>
    public static int ErrorInfoLineNumber;

    /// <summary>
    /// Номер строки для вывода прочей информации или уведомлений.
    /// </summary>
    public static int InfoLineNumber;

    /// <summary>
    /// Номер строки для вывода версии сборки.
    /// </summary>
    public static int VersionLineNumber = -1;

    /// <summary>
    /// Строка загрузки.
    /// </summary>
    public static int DownloadLineNumber;

    /// <summary>
    /// Текст ошибки
    /// </summary>
    private static string ErrorInfoText = string.Empty;

    /// <summary>
    /// Переменная таймера ошибки.
    /// </summary>
    private static Timer ErrorTimer = null;

    /// <summary>
    /// Отоброжается ли сейчас ошибка (Анимация моргания поля ошибки).
    /// </summary>
    public static bool ErrorTimerEnabled = false;

    /// <summary>
    /// Скорость моргания поля ошибки.
    /// </summary>
    private const int ErrorTimerMilliseconds = 250;

    /// <summary>
    /// Кол-во тиков анимации ошибки, оно же время отображения ошибки (ErrorTimerMilliseconds * ErrorTimerCounts).
    /// </summary>
    private const int ErrorTimerCounts = 6;

    #endregion

    #region Методы

    /// <summary>
    /// Отоброжение поля информации или уведомления.
    /// </summary>
    /// <param name="text">Текст информации.</param>
    /// <param name="color">Цвет заднего фона строки информации.</param>
    public static void ShowInfo(string text, ConsoleColor color)
    {
      Console.SetCursorPosition(0, InfoLineNumber);
      Console.BackgroundColor = color;
      Centered(text);
      Console.BackgroundColor = Config.ColorBG;
    }

    /// <summary>
    /// Отображение информации/уведомления ошибки.
    /// </summary>
    /// <param name="text">Текст ошибки.</param>
    public static void ShowErrorInfo(string text)
    {
      ErrorInfoText = text;
      if (ErrorTimerEnabled == false)
      {
        ErrorTimerEnabled = true;
        ErrorTimer = new Timer(TimerErrorCallback, ErrorTimerCounts, ErrorTimerMilliseconds, 1);

        Console.SetCursorPosition(0, ErrorInfoLineNumber);
        Console.BackgroundColor = ConsoleColor.Red;
        Centered(ErrorInfoText);
      }
    }

    /// <summary>
    /// Калбэк тика таймера ошибки.
    /// </summary>
    /// <param name="obj">Оставшееся кол-во срабатываний таймера.</param>
    private static void TimerErrorCallback(object obj)
    {
      ErrorTimer.Dispose();
      int count = (int)obj;
      if (count == 0)
      {
        Console.SetCursorPosition(0, ErrorInfoLineNumber);
        SetDefaultColor();
        Console.Write(new String(' ', Config.ConsoleWidth));
        ErrorTimerEnabled = false;
        Console.SetCursorPosition(0, Lines.LineNumber);
      }
      else
      {
        count--;
        Console.SetCursorPosition(0, ErrorInfoLineNumber);
        if (count % 2 == 0)
          Console.BackgroundColor = ConsoleColor.Red;
        else
          Console.BackgroundColor = ConsoleColor.Black;

        Centered(ErrorInfoText);
        ErrorTimer = new Timer(TimerErrorCallback, count, ErrorTimerMilliseconds, 1);
      }
    }
    /// <summary>
    /// Вывод текста в строку консоли с подсчетом строк.
    /// </summary>
    /// <param name="text">Текст.</param>
    /// <returns>Возвращает следующий актуальный номер строки.</returns>
    public static int WriteLine(string text)
    {
      int currentLine = LineNumber;
      Console.WriteLine(text);
      LineNumber++;
      return currentLine;
    }

    /// <summary>
    /// Вывод текста в строку консоли с подсчетом строк.
    /// </summary>
    /// <param name="line">Строка в которой вывести.</param>
    /// <param name="text">Текст.</param>
    /// <returns>Указанная строка</returns>
    public static int WriteLine(int line, string text)
    {
      Console.SetCursorPosition(0, line);
      Console.Write(text + new String(' ', Config.ConsoleWidth - text.Length));
      return line;
    }

    /// <summary>
    /// Центрирование текста в консоли.
    /// </summary>
    /// <param name="text">Текст который будет по центру консоли.</param>
    private static void Centered(string text)
    {
      int leftIndent = (Config.ConsoleWidth / 2) - (text.Length / 2);
      int rightIndent = Config.ConsoleWidth - leftIndent - text.Length;
      Console.Write($"{new String(' ', leftIndent)}{text}{new String(' ', rightIndent)}");
    }
    /// <summary>
    /// Вывод текста в строку консоли с подсчетом строк.
    /// </summary>
    /// <param name="text">Текст.</param>
    /// <returns>Возращает слудующий актуальный номер строки.</returns>
    public static int Write(string text)
    {
      Console.Write(text);
      return LineNumber;
    }

    /// <summary>
    /// Вывод строки информации с красным фоном.
    /// </summary>
    /// <param name="text">Текст информации.</param>
    /// <param name="FillLine">Для закрашиваемого заднего фона.</param>
    /// <returns>Возращает актуальный номер строки.</returns>
    public static int WriteLineInfo(string text, int FillLine = Config.ConsoleWidth)
    {
      int currentLine = LineNumber;
      Console.BackgroundColor = ConsoleColor.White;
      Console.ForegroundColor = ConsoleColor.Black;

      int size = text.Length;
      if (size > FillLine)
        Console.WriteLine(text);
      else
        Console.WriteLine(text + new String(' ', FillLine - size));

      SetDefaultColor();

      LineNumber++;
      return currentLine;
    }

    /// <summary>
    /// Вывод строки информации с красным фоном.
    /// </summary>
    /// <param name="line">Строка в которой вывести.</param>
    /// <param name="text">Текст.</param>
    /// <param name="FillLine">Для закрашиваемого заднего фона.</param>
    /// <returns>Указанная строка.</returns>
    public static int WriteLineInfo(int line, string text, int FillLine = Config.ConsoleWidth)
    {
      Console.BackgroundColor = ConsoleColor.White;
      Console.ForegroundColor = ConsoleColor.Black;

      int size = text.Length;
      Console.SetCursorPosition(0, line);
      if (size > FillLine)
        Console.Write(text);
      else
        Console.Write(text + new String(' ', FillLine - size));

      SetDefaultColor();
      Console.SetCursorPosition(0, LineNumber);
      return line;
    }

    /// <summary>
    /// Очистка строк консоли.
    /// </summary>
    /// <param name="toLine">После какой строки требуется очистика.</param>
    /// <returns>Возвращает кол-во очищенных строки.</returns>
    public static int DeleteFromLast(int toLine = 0)
    {
      Console.SetCursorPosition(0, toLine);
      int ret = LineNumber - toLine;
      for (int i = toLine; i < LineNumber; i++)
      {
        Console.WriteLine(new String(' ', Config.ConsoleWidth));
      }
      LineNumber = toLine;
      Console.SetCursorPosition(0, 0);
      Console.SetCursorPosition(0, toLine);
      return ret;
    }

    /// <summary>
    /// Установка цветового оформления по умолчанию.
    /// </summary>
    public static void SetDefaultColor()
    {
      Console.ForegroundColor = Config.ColorFont;
      Console.BackgroundColor = Config.ColorBG;
    }

    #endregion
  }
}
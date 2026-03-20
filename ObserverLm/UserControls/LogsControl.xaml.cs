using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Path = System.IO.Path;

namespace ObserverLm
{
    /// <summary>
    /// Логика взаимодействия для LogsControl.xaml
    /// </summary>
    public partial class LogsControl : UserControl,IDisposable
    {
        private readonly ObservableCollection<string> _logLines1 = new ObservableCollection<string>();
        private readonly ObservableCollection<string> _logLines2 = new ObservableCollection<string>();
        private CancellationTokenSource _cts1;
        private CancellationTokenSource _cts2;
        private readonly MySettings _settings;
        public LogsControl()
        {
            InitializeComponent();

            _settings = MySettings.GetSettings();
            LogMonitor1.ItemsSource = _logLines1;
            LogMonitor2.ItemsSource = _logLines2;
            // Запускаем мониторинг
            {
                string filePath = Path.Combine(_settings.FolderLog, "regime.log");
                if (File.Exists(filePath))
                    _ = WatchLogFile1(filePath,LogMonitor1,_logLines1,_cts1);
                else
                {
                    MessageBox.Show($"Директория логов не найдена.{Environment.NewLine}{_settings.FolderLog}");
                }
            }
            {
                string filePath = Path.Combine(_settings.FolderLog, "yenisei.log");
                if (File.Exists(filePath))
                    _ = WatchLogFile1(filePath, LogMonitor2,_logLines2,_cts2);
                else
                {
                    MessageBox.Show($"Директория логов не найдена.{Environment.NewLine}{_settings.FolderLog}");
                }
            }

        }

        private async Task WatchLogFile1(string path, ListBox logMonitor, ObservableCollection<string> logLines,
            CancellationTokenSource cts)
        {
            cts = new CancellationTokenSource();

            await Task.Run(() =>
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = new StreamReader(stream, Encoding.UTF8);
                // Сначала читаем последние 100 строк
                var lastLines = ReadLastLines(path, _settings.Tail);
                Dispatcher.Invoke(() => {
                    foreach (var line in lastLines)
                        logLines.Add(line);
                    logMonitor.ScrollIntoView(logLines.Last());
                });

                // Переходим в конец файла для отслеживания новых строк
                stream.Seek(0, SeekOrigin.End);

                while (!cts.Token.IsCancellationRequested)
                {
                    string line = reader.ReadLine();
                    if (line != null)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            logLines.Add(line);
                            // Держим в памяти только последние 100 строк
                            if (logLines.Count > 100) logLines.RemoveAt(0);

                            // Автопрокрутка вниз
                            logMonitor.ScrollIntoView(logLines.Last());
                        });
                    }
                    else
                    {
                        // Ждем появления новых данных
                        Thread.Sleep(250);
                    }
                }
            }, cts.Token);
        }

        

        private List<string> ReadLastLines(string path, int count)
        {
            var lines = new List<string>();
            // Открываем файл с разрешением на чтение и запись для других процессов
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sr = new StreamReader(fs, Encoding.UTF8);
            // Для больших файлов лучше читать с конца, но для хвоста в 100 строк 
            // самый надежный способ — вычитать всё через общий поток:
            while (sr.ReadLine() is { } line)
            {
                lines.Add(line);
                if (lines.Count > count) lines.RemoveAt(0);
            }

            return lines;
        }



        private void CopyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (LogMonitor1.SelectedItems.Count > 0)
            { 
                var lines = LogMonitor1.SelectedItems.Cast<object>().Select(item => item.ToString()); 
                string textToCopy = string.Join(Environment.NewLine, lines); 
                Clipboard.SetText(textToCopy);
            }
            if (LogMonitor2.SelectedItems.Count > 0)
            {
             
                var lines = LogMonitor2.SelectedItems.Cast<object>().Select(item => item.ToString());
                string textToCopy = string.Join(Environment.NewLine, lines);
                Clipboard.SetText(textToCopy);
             
            }

        }
        //invalid cis format

        public void Dispose()
        {
            _cts1?.Cancel();
            _cts2?.Cancel();
            _cts1?.Dispose();
            _cts2?.Dispose();

        }
    }
}

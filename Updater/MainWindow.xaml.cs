using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace Updater
{
    public partial class MainWindow
    {
        private readonly Stopwatch sw = new Stopwatch();
        private readonly Queue<string> fileNames = new Queue<string>(new []{ "HotelManager.exe", "Core.dll", "Handlers.dll", "Templates.dll" });
        private readonly string ftpAddress;
        private readonly string ftpUserName;
        private readonly string ftpPassword;
        
        public MainWindow(string webAddress, string ftpAddress, string ftpUserName, string ftpPassword)
        {
            InitializeComponent();
            WebAddress.Text = webAddress;
            this.ftpAddress = ftpAddress;
            this.ftpUserName = ftpUserName;
            this.ftpPassword = ftpPassword;
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string userPath = Directory.CreateDirectory(Path.Combine(appDataPath, "HotelManager")).FullName;
            TryUpdateFile(userPath, "Config.xml");
        }

        private void TryUpdateFile(string path, string fileName)
        {
            try
            {
                if (File.Exists(Path.Combine(path, fileName)) && IsLocalFileNewer(path, fileName))
                {
                    Downloading.Text = $"{fileName} is up to date.";
                }
                else DownloadFile(path, fileName);
            }
            catch
            {
                Downloading.Text = $"Failed to update {fileName}";
            }
        }

        private bool IsLocalFileNewer(string path, string fileName)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Path.Combine(ftpAddress, fileName));
                request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
                request.Method = WebRequestMethods.Ftp.GetDateTimestamp;
                using FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                return File.GetLastWriteTime(Path.Combine(path, fileName)) >= response.LastModified;
            }
            catch
            {
                Downloading.Text = $"Failed to check {fileName}";
                return false;
            }
        }

        public void DownloadFile(string path, string fileName)
        {
            using WebClient webClient = new WebClient();
            webClient.DownloadFileCompleted += Completed;
            webClient.DownloadProgressChanged += ProgressChanged;
            sw.Start();
            Downloading.Text = $"Downloading {fileName}";
            try
            {
                webClient.DownloadFileAsync(new Uri(Path.Combine(WebAddress.Text, fileName)), Path.Combine(path, fileName));
            }
            catch
            {
                Status.Text += $"Failed to download \"{fileName}\".\n";
                Downloading.Text = $"Failed to download \"{fileName}\".";
            }
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadSpeed.Text = $"{e.BytesReceived / 1024d / sw.Elapsed.TotalSeconds:N2} KB/s";
            ProgressBar.Value = e.ProgressPercentage;
            Percent.Text = $"{e.ProgressPercentage}%";
            Downloaded.Text = $"{e.BytesReceived / 1024d:N2} KB's / {e.TotalBytesToReceive / 1024d:N2} KB's";
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            Downloading.Text = "Download completed!";
        }

        private void Downloading_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (Downloading.Text == "Download completed!")
            {
                Status.Text += $"Successfully downloaded \"{fileNames.Dequeue()}\".\n";
            }
            else if (Downloading.Text.Contains("is up to date.") && !Downloading.Text.Contains("Config"))
            {
                Status.Text += $"{fileNames.Dequeue()} is up to date.\n";
            }

            if (fileNames.Any() && !Downloading.Text.Contains("Failed"))
            {
                string programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                string installPath = Directory.CreateDirectory(Path.Combine(programFilesPath, "HotelManager")).FullName;
                TryUpdateFile(installPath, fileNames.Peek());
            }
            else Close();
        }
    }
}
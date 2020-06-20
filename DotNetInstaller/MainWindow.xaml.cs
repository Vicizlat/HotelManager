using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;

namespace DotNetInstaller
{
    public partial class MainWindow
    {
        private WebClient webClient;
        private readonly Stopwatch sw = new Stopwatch();
        private readonly string dotNetFileName = "windowsdesktop-runtime-3.1.5-win-";

        public MainWindow()
        {
            InitializeComponent();
            string[] args = { "x86" };
            string dotNetDownload = "https://download.visualstudio.microsoft.com/download/pr/";
            if (args[0] == "x86") dotNetDownload += "df7b90d9-b93e-4974-85ef-c1de418bc186/e380e58bbd8505ebaee6c3abb23baade/";
            if (args[0] == "x64") dotNetDownload += "86835fe4-93b5-4f4e-a7ad-c0b0532e407b/f4f2b1239f1203a05b9952028d54fc13/";
            dotNetFileName += $"{args[0]}.exe";
            DownloadFile(Path.Combine(dotNetDownload, dotNetFileName), Path.Combine(Path.GetTempPath(), dotNetFileName));
        }

        public void DownloadFile(string urlAddress, string location)
        {
            using (webClient = new WebClient())
            {
                webClient.DownloadFileCompleted += Completed;
                webClient.DownloadProgressChanged += ProgressChanged;
                sw.Start();
                try
                {
                    webClient.DownloadFileAsync(new Uri(urlAddress), location);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadSpeed.Text = $"{e.BytesReceived / 1024d / sw.Elapsed.TotalSeconds:N2} KB/s";
            DownloadProgBar.Value = e.ProgressPercentage;
            Percent.Text = $"{e.ProgressPercentage}%";
            Downloaded.Text = $"{e.BytesReceived / 1024d:N2} KB's / {e.TotalBytesToReceive / 1024d:N2} KB's";
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            Downloading.Text = "Download completed!";
            Process.Start(Path.Combine(Path.GetTempPath(), dotNetFileName), "-q")?.WaitForExit();
            Close();
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            File.Delete(Path.Combine(Path.GetTempPath(), dotNetFileName));
        }
    }
}
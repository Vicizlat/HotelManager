﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace HotelManager
{
    public static class FileHandler
    {
        public static string LocalPath => Directory.CreateDirectory(Path.Combine(AppDataPath, "HotelManager")).FullName;
        private static readonly string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public static bool IsLocalFileNewer()
        {
            if (TryGetLocalFile(out FileInfo localFile))
            {
                try
                {
                    DateTime localFileLastWrite = localFile.LastWriteTime;
                    Logging.Instance.WriteLine($"Local file time: {localFileLastWrite:dd.MM.yyyy HH:mm:ss:ffff}");
                    FtpWebRequest request = FtpRequest("Reservations", WebRequestMethods.Ftp.GetDateTimestamp);
                    using FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    DateTime remoteFileLastWrite = response.LastModified;
                    Logging.Instance.WriteLine($"Remote file time: {remoteFileLastWrite:dd.MM.yyyy HH:mm:ss:ffff}");
                    bool localFileIsNewer = localFileLastWrite >= remoteFileLastWrite;
                    Logging.Instance.WriteLine(localFileIsNewer ? "Local file is newer." : "Remote file is newer. Downloading remote file.");
                    return localFileIsNewer;
                }
                catch
                {
                    Logging.Instance.WriteLine("Could not connect to server! Using local file only.");
                    return true;
                }
            }
            else return false;
        }

        private static bool TryGetLocalFile(out FileInfo localFile)
        {
            localFile = Directory.CreateDirectory(LocalPath).GetFiles().ToList().Find(f => f.Name == "Reservations");
            return localFile != null;
        }

        public static bool TryGetRemoteFile()
        {
            try
            {
                FtpWebRequest request = FtpRequest("Reservations", WebRequestMethods.Ftp.DownloadFile);
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    using StreamReader reader = new StreamReader(response.GetResponseStream());
                    File.WriteAllText(Path.Combine(LocalPath, "temp"), reader.ReadToEnd());
                }
                Reservations.Instance.LoadReservations("temp");
                Logging.Instance.WriteLine("Successfully downloaded 'Reservations'!");
                return true;
            }
            catch
            {
                Logging.Instance.WriteLine("Download failed - could not connect to server!");
                return false;
            }
        }

        public static bool TryGetConfigFile()
        {
            try
            {
                SelectWebsiteWindow dialog = new SelectWebsiteWindow();
                dialog.ShowDialog();
                WebRequest request = WebRequest.Create($"https://{dialog.Address.Text}/HotelManager/Config.xml");
                request.Method = WebRequestMethods.File.DownloadFile;
                using (WebResponse response = request.GetResponse())
                {
                    using StreamReader reader = new StreamReader(response.GetResponseStream());
                    File.WriteAllText(Path.Combine(LocalPath, "Config.xml"), reader.ReadToEnd());
                }
                Logging.Instance.WriteLine("Successfully downloaded 'Config.xml'!");
                return true;
            }
            catch
            {
                Logging.Instance.WriteLine("Download failed - could not connect to server!");
                return false;
            }
        }

        public static void TryUploadFile(string fileName, bool isLogWriterClosed = false)
        {
            try
            {
                FtpWebRequest request = FtpRequest(fileName, WebRequestMethods.Ftp.UploadFile);
                StreamReader reader = new StreamReader(Path.Combine(LocalPath, fileName));
                byte[] fileContents = Encoding.UTF8.GetBytes(reader.ReadToEnd());
                reader.Close();
                request.ContentLength = fileContents.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();
                if (!isLogWriterClosed) Logging.Instance.WriteLine("Upload completed successfully!");
            }
            catch
            {
                if (!isLogWriterClosed) Logging.Instance.WriteLine("Upload failed - could not connect to server!");
            }
        }

        private static FtpWebRequest FtpRequest(string fileName, string method)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Path.Combine(GetXmlData("RemotePath"), fileName));
            request.Credentials = new NetworkCredential(GetXmlData("FtpUserName"), GetXmlData("FtpPassword"));
            request.Method = method;
            return request;
        }

        private static string GetXmlData(string xmlData)
        {
            using (XmlReader reader = XmlReader.Create($"{LocalPath}\\Config.xml"))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement() && reader.Name == xmlData)
                    {
                        xmlData = reader.ReadString();
                        break;
                    }
                }
            }
            return xmlData;
        }

        //public static void WriteConfigFile()
        //{
        //    XmlWriter xml = XmlWriter.Create($"{LocalPath}\\Config.xml");
        //    xml.WriteStartDocument();
        //    xml.WriteStartElement("Configuration");
        //    xml.WriteElementString("RemotePath", FtpPath);
        //    xml.WriteElementString("FtpUserName", UserName);
        //    xml.WriteElementString("FtpPassword", Password);
        //    xml.WriteEndElement();
        //    xml.WriteEndDocument();
        //    xml.Close();
        //}
    }
}
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Reflection;
using System.Windows;
using NLog;

namespace Feedling
{
    /// <summary>
    /// Interaction logic for AutoUpdate.xaml
    /// </summary>
    public partial class AutoUpdate : Window
    {
        public AutoUpdate()
        {
            InitializeComponent();
        }
        private Logger Log = LogManager.GetCurrentClassLogger();
        private Uri applicationupdateuri = new Uri(Properties.Settings.Default.ApplicationUpdateUrl);
        private string msipath;
        internal void CheckForUpdates(bool silent = false)
        {
            if (!silent)
            {
                this.Visibility = Visibility.Visible;
            }
            try
            {
                Log.Info("Entering update block");

                WebRequest request = WebRequest.Create(applicationupdateuri);
                Log.Debug("Downloading version definition");
                WebResponse response = request.GetResponse();
                if (response.ContentLength > 0)
                {
                    StreamReader sr = new StreamReader(response.GetResponseStream());
                    string upgrademeta = sr.ReadToEnd();
                    if (upgrademeta.Contains("|"))
                    {
                        Log.Info("Received valid version definition");
                        string[] parts = upgrademeta.Split("|".ToCharArray(), 3);
                        msipath = Properties.Settings.Default.ApplicationUpdateUrl.Replace(applicationupdateuri.Segments[applicationupdateuri.Segments.Length - 1].ToString(), parts[1]).Trim();
                        Version availableversion = new Version(parts[0]);
                        if (Assembly.GetExecutingAssembly().GetName().Version.CompareTo(availableversion) < 0)
                        {
                            Log.Debug("New version available: {0}", availableversion);
                            this.Visibility = Visibility.Visible;
                            ApplyBtn.IsEnabled = true;
                            UpdateDescription.Text = string.Format(Properties.Resources.UpdatesAvailableText, availableversion, parts[2]);
                        }
                        else
                        {
                            UpdateDescription.Text = Properties.Resources.NoUpdatesAvailableText;
                        }
                    }
                }
                else
                {
                    Log.Error("There was an error fetching the update definition version. Content Length appeared to be 0");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                if (!silent)
                {
                    UpdateDescription.Text = string.Format(Properties.Resources.UpdatesErrorText, ex.Message);
                }
            }
            if (silent && this.Visibility == Visibility.Collapsed)
            {
                this.Close();
            }
        }

        private void ApplyBtn_Click(object sender, RoutedEventArgs e)
        {
            Log.Debug("User requests upgrade to new version");
            string savefile = string.Concat(Path.GetTempPath(), Path.DirectorySeparatorChar, "Feedling.msi");

            HttpRequestCachePolicy nocachepolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            WebRequest msirequest = WebRequest.Create(msipath);
            msirequest.CachePolicy = nocachepolicy;
            Log.Debug("Downloading new MSI: {0}", msipath);
            WebResponse msiresponse = msirequest.GetResponse();
            using (Stream responsestream = msiresponse.GetResponseStream())
            {
                Log.Info("Saving MSI in {0}", savefile);
                using (FileStream fs = new FileStream(savefile, FileMode.Create))
                {
                    using (BinaryReader br = new BinaryReader(responsestream))
                    {
                        byte[] buff = new byte[1024];
                        int bytesread = 0;
                        using (BinaryWriter bw = new BinaryWriter(fs))
                        {
                            do
                            {
                                bytesread = br.Read(buff, 0, buff.Length);
                                bw.Write(buff, 0, bytesread);
                            } while (bytesread > 0);
                        }
                    }
                }
            }
            Log.Debug("MSI saved.");
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.Arguments = "";
            psi.FileName = savefile;
            Log.Info("Starting installer");
            Process.Start(psi);
            Log.Info("Exiting.");
            Application.Current.Shutdown();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

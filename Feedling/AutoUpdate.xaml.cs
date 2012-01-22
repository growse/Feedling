/*
Copyright © 2008-2012, Andrew Rowson
All rights reserved.

See LICENSE file for license details.
*/
using System;
using System.Diagnostics;
using System.Globalization;
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
    public partial class AutoUpdate
    {
        public AutoUpdate()
        {
            InitializeComponent();
        }
        private readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly Uri applicationupdateuri = new Uri(Properties.Settings.Default.ApplicationUpdateUrl);
        private string remoteMsiPath;
        internal void CheckForUpdates(bool silent = false)
        {
            if (!silent)
            {
                Visibility = Visibility.Visible;
            }
            try
            {
                Log.Info("Entering update block");

                var request = (HttpWebRequest)WebRequest.Create(applicationupdateuri);
                request.UserAgent = string.Format("Feedling v{0}", Assembly.GetExecutingAssembly().GetName().Version.ToString(3));


                Log.Debug("Downloading version definition");
                var response = request.GetResponse();
                if (response.ContentLength > 0)
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream != null)
                        {
                            var sr = new StreamReader(stream);
                            var upgrademeta = sr.ReadToEnd();
                            if (upgrademeta.Contains("|"))
                            {
                                Log.Info("Received valid version definition");
                                var parts = upgrademeta.Split("|".ToCharArray(), 3);
                                remoteMsiPath =
                                    Properties.Settings.Default.ApplicationUpdateUrl.Replace(
                                        applicationupdateuri.Segments[applicationupdateuri.Segments.Length - 1].ToString(CultureInfo.InvariantCulture),
                                        parts[1]).Trim();
                                var availableversion = new Version(parts[0]);
                                if (Assembly.GetExecutingAssembly().GetName().Version.CompareTo(availableversion) < 0)
                                {
                                    Log.Debug("New version available: {0}", availableversion);
                                    Visibility = Visibility.Visible;
                                    ApplyBtn.IsEnabled = true;
                                    UpdateDescription.Text = string.Format(Properties.Resources.UpdatesAvailableText,
                                                                           availableversion, parts[2]);
                                }
                                else
                                {
                                    UpdateDescription.Text = Properties.Resources.NoUpdatesAvailableText;
                                }
                            }
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
                    UpdateDescription.Text = string.Format(Properties.Resources.UpdatesErrorCheckText, ex.Message);
                }
            }
            if (silent && Visibility == Visibility.Collapsed)
            {
                Close();
            }
        }
        private string localMsiFilePath;
        private void ApplyBtn_Click(object sender, RoutedEventArgs e)
        {
            CloseBtn.IsEnabled = false;
            ApplyBtn.IsEnabled = false;
            try
            {
                Log.Debug("User requests upgrade to new version");
                localMsiFilePath = string.Concat(Path.GetTempPath(), Path.DirectorySeparatorChar, "Feedling.msi");

                var nocachepolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);

                var wc = new WebClient { CachePolicy = nocachepolicy };

                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileCompleted += wc_DownloadFileCompleted;
                wc.DownloadFileAsync(new Uri(remoteMsiPath), localMsiFilePath);

            }
            catch (Exception ex)
            {
                Log.Error("Error Applying update", ex);
                UpdateDescription.Text = string.Format(Properties.Resources.UpdatesErrorApplyText, ex.Message);
                CloseBtn.IsEnabled = true;
            }
        }

        void wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Log.Debug("MSI saved.");
                var psi = new ProcessStartInfo { Arguments = "", FileName = localMsiFilePath };
                Log.Info("Starting installer");
                Process.Start(psi);
                Log.Info("Exiting.");
                Application.Current.Shutdown();
            }
            else
            {
                Log.Error("Error Applying update", e.Error);
                UpdateDescription.Text = string.Format(Properties.Resources.UpdatesErrorApplyText, e.Error.Message);
            }
            CloseBtn.IsEnabled = true;
        }

        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

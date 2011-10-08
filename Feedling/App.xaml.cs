/*
Copyright © 2008-2011, Andrew Rowson
All rights reserved.

See LICENSE file for license details.
*/
using System;
using System.Windows;
using NLog;

[assembly: CLSCompliant(false)]
namespace Feedling
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        static Logger Log;
        public App()
        {
            Log = LogManager.GetCurrentClassLogger();

            
            #if DEBUG
            LogManager.GlobalThreshold = LogLevel.Trace;
            #endif
            App.Current.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(Current_DispatcherUnhandledException);
        }

        void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Log = LogManager.GetCurrentClassLogger();
            Log.Error("Exception thrown by application", e.Exception);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args!=null && e.Args.Length>0 && e.Args[0] == "-D")
            {
                LogManager.GlobalThreshold = LogLevel.Trace;
            }
        }
    }
}

using System.Windows;
using System;

namespace Feedling
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            log4net.Config.XmlConfigurator.Configure();
            
            #if DEBUG
            //Configure the root logger.
            log4net.Repository.Hierarchy.Hierarchy h = (log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository();
            log4net.Repository.Hierarchy.Logger rootLogger = h.Root;
            rootLogger.Level = h.LevelMap["ALL"];
            #endif
            
            foreach (log4net.Appender.RollingFileAppender appender in log4net.LogManager.GetRepository().GetAppenders())
            {
                if (appender != null)
                {
                    appender.File = string.Format(@"{0}\Feedling\log-file.txt", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
                    appender.ActivateOptions();
                }
            }
            App.Current.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(Current_DispatcherUnhandledException);
        }

        void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            Log.Error("Exception thrown by application", e.Exception);
        }
    }
}

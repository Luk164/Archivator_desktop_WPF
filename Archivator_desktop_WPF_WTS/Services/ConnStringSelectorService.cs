
using Microsoft.Win32;

namespace Archivator_desktop_WPF_WTS.Services
{
    public class ConnStringSelectorService
    {
        public ConnStringSelectorService()
        {
            SystemEvents.UserPreferenceChanging += OnUserPreferenceChanging;
        }

        public bool SetConnString(string connString = null)
        {
            if (connString == null)
            {
                if (App.Current.Properties.Contains("ConnString"))
                {
                    // Saved theme
                    connString = App.Current.Properties["ConnString"].ToString();
                }
                else
                {
                    // Default theme
                    connString = @"Server=(localdb)\mssqllocaldb;Database=Archivator_autoMigTest;Trusted_Connection=True;";
                }
            }

            App.Current.Properties["ConnString"] = connString;
            return true;
        }

        public string GetConnString()
        {
            return App.Current.Properties["ConnString"]?.ToString();
        }

        private void OnUserPreferenceChanging(object sender, UserPreferenceChangingEventArgs e)
        {
            SetConnString();
        }
    }
}

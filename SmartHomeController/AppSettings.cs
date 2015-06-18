using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SmartHomeController
{

    public class AppSettings
    {
        private static AppSettings _appSettings;
        private ApplicationDataContainer roamingSettings = null;

        private static string SonosIPKey = "SONOS_IP";
        private static string SonosPortKey = "SONOS_PORT";

        // For location task
        private static string DefaultSonosPort = "1400";

        private AppSettings()
        {
            roamingSettings = ApplicationData.Current.LocalSettings;
            SonosIP = "192.168.178.3";
        }

        public static AppSettings Instance
        {
            get
            {
                if (_appSettings == null)
                    _appSettings = new AppSettings();
                return _appSettings;
            }
        }

        public string GetInstallationId()
        {
            if (!roamingSettings.Values.ContainsKey("InstallationId"))
            {
                roamingSettings.Values["InstallationId"] = Guid.NewGuid().ToString();
            }
            return (string)roamingSettings.Values["InstallationId"];
        }


        public string SonosIP
        {
            get
            {
                return roamingSettings.Values[SonosIPKey] as string;
            }
            set
            {
                roamingSettings.Values[SonosIPKey] = value;
            }
        }
        public int SonosPort
        {
            get
            {
                return Int32.Parse(roamingSettings.Values[SonosPortKey] as string);
            }
            set
            {
                roamingSettings.Values[SonosPortKey] = value;
            }
        }
    }
}

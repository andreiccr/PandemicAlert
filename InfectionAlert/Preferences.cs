using System.IO;

namespace PandemicAlert.Settings
{

    public class Preferences
    {

        private readonly string PATH_SETTINGS = Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, "settings.json");
        _Preferences preferences;

        public bool ShowInfectionTrend
        {
            get
            {
                return preferences.ShowInfectionTrend;
            }
            set
            {
                preferences.ShowInfectionTrend = value;
                SaveSettings();
            }
        }
        public bool ShowTodayDeaths
        {
            get
            {
                return preferences.ShowTodayDeaths;
            }
            set
            {
                preferences.ShowTodayDeaths = value;
                SaveSettings();
            }
        }
        public bool ShowHealInfectionTrend
        {
            get
            {
                return preferences.ShowHealInfectionTrend;
            }
            set
            {
                preferences.ShowHealInfectionTrend = value;
                SaveSettings();
            }
        }
        public bool ShowActiveCases
        {
            get
            {
                return preferences.ShowActiveCases;
            }
            set
            {
                preferences.ShowActiveCases = value;
                SaveSettings();
            }
        }
        public bool ShowDangerPercentage
        {
            get
            {
                return preferences.ShowDangerPercentage;
            }
            set
            {
                preferences.ShowDangerPercentage = value;
                SaveSettings();
            }
        }
        public bool ShowTodayInfected
        {
            get
            {
                return preferences.ShowTodayInfected;
            }
            set
            {
                preferences.ShowTodayInfected = value;
                SaveSettings();
            }
        }
        public bool ShowTodayHealed
        {
            get
            {
                return preferences.ShowTodayHealed;
            }
            set
            {
                preferences.ShowTodayHealed = value;
                SaveSettings();
            }
        }
        public bool ShowImmunizedPercentage
        {
            get
            {
                return preferences.ShowImmunizedPercentage;
            }
            set
            {
                preferences.ShowImmunizedPercentage = value;
                SaveSettings();
            }
        }
        public bool ShowDeathTrend
        {
            get
            {
                return preferences.ShowDeathTrend;
            }
            set
            {
                preferences.ShowDeathTrend = value;
                SaveSettings();
            }
        }
        public bool ShowIncidence
        {
            get
            {
                return preferences.ShowIncidence;
            }
            set
            {
                preferences.ShowIncidence = value;
                SaveSettings();
            }
        }
        public bool ShowVaccinationRate
        {
            get
            {
                return preferences.ShowVaccinationRate;
            }
            set
            {
                preferences.ShowVaccinationRate = value;
                SaveSettings();
            }
        }
        public bool ShowProjectionImmunization
        {
            get
            {
                return preferences.ShowProjectionImmunization;
            }
            set
            {
                preferences.ShowProjectionImmunization = value;
                SaveSettings();
            }
        }
        public bool ShowProjectionVaccination
        {
            get
            {
                return preferences.ShowProjectionVaccination;
            }
            set
            {
                preferences.ShowProjectionVaccination = value;
                SaveSettings();
            }
        }
        public bool ShowProjectionImmunizationHalf
        {
            get
            {
                return preferences.ShowProjectionImmunizationHalf;
            }
            set
            {
                preferences.ShowProjectionImmunizationHalf = value;
                SaveSettings();
            }
        }
        public bool ShowProjectionVaccinationHalf
        {
            get
            {
                return preferences.ShowProjectionVaccinationHalf;
            }
            set
            {
                preferences.ShowProjectionVaccinationHalf = value;
                SaveSettings();
            }
        }
        public bool ShowProjectionImmunizationQuarter
        {
            get
            {
                return preferences.ShowProjectionImmunizationQuarter;
            }
            set
            {
                preferences.ShowProjectionImmunizationQuarter = value;
                SaveSettings();
            }
        }
        public bool ShowProjectionVaccinationQuarter
        {
            get
            {
                return preferences.ShowProjectionVaccinationQuarter;
            }
            set
            {
                preferences.ShowProjectionVaccinationQuarter = value;
                SaveSettings();
            }
        }
        public bool EnableNotifTodayInfections
        {
            get
            {
                return preferences.EnableNotifTodayInfections;
            }
            set
            {
                preferences.EnableNotifTodayInfections = value;
                SaveSettings();
            }
        }
        public bool EnableNotifTodayHealed
        {
            get
            {
                return preferences.EnableNotifTodayHealed;
            }
            set
            {
                preferences.EnableNotifTodayHealed = value;
                SaveSettings();
            }
        }
        public bool EnableNotifIncidence
        {
            get
            {
                return preferences.EnableNotifIncidence;
            }
            set
            {
                preferences.EnableNotifIncidence = value;
                SaveSettings();
            }
        }

        public Preferences()
        {
            preferences = new _Preferences();

            LoadSettings();
        }

        void LoadSettings()
        {
            //Load all preferences from file
            using (var reader = new StreamReader(PATH_SETTINGS, true))
            {
                string json = reader.ReadToEnd();
                
                preferences = System.Text.Json.JsonSerializer.Deserialize<_Preferences>(json);
            }
        }

        async void SaveSettings()
        {
            string jsonStr = System.Text.Json.JsonSerializer.Serialize(preferences);

            using (var writer = File.CreateText(PATH_SETTINGS))
            {
                await writer.WriteAsync(jsonStr);
                writer.Close();
            }
        }

        private class _Preferences
        {
            public bool ShowInfectionTrend;
            public bool ShowHealInfectionTrend;
            public bool ShowActiveCases;
            public bool ShowDangerPercentage;
            public bool ShowTodayInfected;
            public bool ShowTodayHealed;
            public bool ShowImmunizedPercentage;
            public bool ShowTodayDeaths;
            public bool ShowDeathTrend;
            public bool ShowIncidence;
            public bool ShowVaccinationRate;
            public bool ShowProjectionImmunization;
            public bool ShowProjectionVaccination;
            public bool ShowProjectionImmunizationHalf;
            public bool ShowProjectionVaccinationHalf;
            public bool ShowProjectionImmunizationQuarter;
            public bool ShowProjectionVaccinationQuarter;

            public bool EnableNotifTodayInfections;
            public bool EnableNotifTodayHealed;
            public bool EnableNotifIncidence;

        }

    }
}
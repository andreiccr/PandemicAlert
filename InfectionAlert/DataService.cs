using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using InfectionAlert.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Json = System.Text.Json;

namespace PandemicAlert
{
    class DataService : IDataService
    {
        private readonly string CACHE_INFECTIONS = Path.Combine(Xamarin.Essentials.FileSystem.CacheDirectory, "infections.json");
        private readonly string CACHE_HEALED = Path.Combine(Xamarin.Essentials.FileSystem.CacheDirectory, "healed.json");
        private readonly string CACHE_DEATHS = Path.Combine(Xamarin.Essentials.FileSystem.CacheDirectory, "deaths.json");
        private readonly string CACHE_VAX = Path.Combine(Xamarin.Essentials.FileSystem.CacheDirectory, "vax.json");

        public Dictionary<DateTime, long> Infections { get; private set; }

        public Dictionary<DateTime, long> Healed { get; private set; }

        public Dictionary<DateTime, long> Deaths { get; private set; }

        public Dictionary<DateTime, long> Icu { get; private set; }

        public Dictionary<DateTime, Vaccines> Vaccines { get; private set; }

        CovidData covidData;

        public DataService()
        {
            Infections = new Dictionary<DateTime, long>();
            Healed = new Dictionary<DateTime, long>();
            Deaths = new Dictionary<DateTime, long>();
            Vaccines = new Dictionary<DateTime, Vaccines>();
        }

        public async Task ClearData()
        {
            //Remove cached data so values have to be parsed again from the online source
            File.Delete(CACHE_INFECTIONS);
            File.Delete(CACHE_HEALED);
            File.Delete(CACHE_DEATHS);
            File.Delete(CACHE_VAX);

            Infections = new Dictionary<DateTime, long>();
            Healed = new Dictionary<DateTime, long>();
            Deaths = new Dictionary<DateTime, long>();
            Vaccines = new Dictionary<DateTime, Vaccines>();
        }

        /// <summary>
        /// Returns incidence in Bucharest. Will be replaced in the near future.
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public double GetIncidenceInB()
        {
            GetDataOnline();
            if (covidData == null || covidData.Json.Count == 0)
            {
                //Error
                throw new Exception("CovidData: No data from remote source exists");
            }

            return double.Parse(covidData.Json["currentDayStats"]["incidence"]["B"].ToString());
        }

        public async Task LoadData()
        {
            //If there is no data cached on the device or some data is missing, parse it from the online source
            if ((CACHE_INFECTIONS == null || !File.Exists(CACHE_INFECTIONS)) ||
                (CACHE_HEALED == null || !File.Exists(CACHE_HEALED)) ||
                (CACHE_DEATHS == null || !File.Exists(CACHE_DEATHS)) ||
                (CACHE_DEATHS == null || !File.Exists(CACHE_VAX)))
            {

                GetDataOnline();
                if (covidData == null || covidData.Json.Count == 0)
                {
                    //Error
                    throw new Exception("CovidData: No data from remote source exists");
                }

                //Get current day data
                DateTime currentDay = DateTime.Parse(covidData.Json["currentDayStats"]["parsedOnString"].ToString());

                Infections.Add(currentDay.Date, long.Parse(covidData.Json["currentDayStats"]["numberInfected"].ToString()));
                Healed.Add(currentDay.Date, long.Parse(covidData.Json["currentDayStats"]["numberCured"].ToString()));
                Deaths.Add(currentDay.Date, long.Parse(covidData.Json["currentDayStats"]["numberDeceased"].ToString()));

                Vaccines vac = new Vaccines();
                vac.All = long.Parse(covidData.Json["currentDayStats"]["numberTotalDosesAdministered"].ToString());
                vac.Pfizer = long.Parse(covidData.Json["currentDayStats"]["vaccines"]["pfizer"]["total_administered"].ToString());
                vac.ImmunizedPfizer = long.Parse(covidData.Json["currentDayStats"]["vaccines"]["pfizer"]["immunized"].ToString());
                vac.Moderna = long.Parse(covidData.Json["currentDayStats"]["vaccines"]["moderna"]["total_administered"].ToString());
                vac.ImmunizedModerna = long.Parse(covidData.Json["currentDayStats"]["vaccines"]["moderna"]["immunized"].ToString());
                vac.Astra = long.Parse(covidData.Json["currentDayStats"]["vaccines"]["astra_zeneca"]["total_administered"].ToString());
                vac.ImmunizedAstra = long.Parse(covidData.Json["currentDayStats"]["vaccines"]["astra_zeneca"]["immunized"].ToString());
                vac.ImmunizedAll = vac.ImmunizedPfizer + vac.ImmunizedModerna + vac.ImmunizedAstra;

                Vaccines.Add(currentDay.Date, vac);

                //Get historical data (past 3 months)
                DateTime targetDay = currentDay.AddDays(-90).Date;
                while(currentDay>=targetDay)
                {
                    currentDay = currentDay.AddDays(-1);
                    Infections.Add(currentDay.Date, long.Parse(covidData.Json["historicalData"][currentDay.Date.ToString("yyyy-MM-dd")]["numberInfected"].ToString()));
                    Healed.Add(currentDay.Date, long.Parse(covidData.Json["historicalData"][currentDay.Date.ToString("yyyy-MM-dd")]["numberCured"].ToString()));
                    Deaths.Add(currentDay.Date, long.Parse(covidData.Json["historicalData"][currentDay.Date.ToString("yyyy-MM-dd")]["numberDeceased"].ToString()));

                    vac = new Vaccines();
                    vac.All = long.Parse(covidData.Json["historicalData"][currentDay.Date.ToString("yyyy-MM-dd")]["numberTotalDosesAdministered"].ToString());
                    vac.Pfizer = long.Parse(covidData.Json["historicalData"][currentDay.Date.ToString("yyyy-MM-dd")]["vaccines"]["pfizer"]["total_administered"].ToString());
                    vac.ImmunizedPfizer = long.Parse(covidData.Json["historicalData"][currentDay.Date.ToString("yyyy-MM-dd")]["vaccines"]["pfizer"]["immunized"].ToString());
                    vac.Moderna = long.Parse(covidData.Json["historicalData"][currentDay.Date.ToString("yyyy-MM-dd")]["vaccines"]["moderna"]["total_administered"].ToString());
                    vac.ImmunizedModerna = long.Parse(covidData.Json["historicalData"][currentDay.Date.ToString("yyyy-MM-dd")]["vaccines"]["moderna"]["immunized"].ToString());
                    vac.Astra = long.Parse(covidData.Json["historicalData"][currentDay.Date.ToString("yyyy-MM-dd")]["vaccines"]["astra_zeneca"]["total_administered"].ToString());
                    vac.ImmunizedAstra = long.Parse(covidData.Json["historicalData"][currentDay.Date.ToString("yyyy-MM-dd")]["vaccines"]["astra_zeneca"]["immunized"].ToString());
                    vac.ImmunizedAll = vac.ImmunizedPfizer + vac.ImmunizedModerna + vac.ImmunizedAstra;

                    Vaccines.Add(currentDay.Date, vac);
                }

                await SaveData();

            }
            else
            {
                using (var reader = new StreamReader(CACHE_INFECTIONS, true))
                {
                    string json = reader.ReadToEnd();
                    Infections = Json.JsonSerializer.Deserialize<Dictionary<DateTime, long>>(json);
                }

                using (var reader = new StreamReader(CACHE_HEALED, true))
                {
                    string json = reader.ReadToEnd();
                    Healed = Json.JsonSerializer.Deserialize<Dictionary<DateTime, long>>(json);
                }

                using (var reader = new StreamReader(CACHE_DEATHS, true))
                {
                    string json = reader.ReadToEnd();
                    Deaths = Json.JsonSerializer.Deserialize<Dictionary<DateTime, long>>(json);
                }

                using (var reader = new StreamReader(CACHE_VAX, true))
                {
                    string json = reader.ReadToEnd();
                    Vaccines = Json.JsonSerializer.Deserialize<Dictionary<DateTime, Vaccines>>(json);
                }
            }
        }

        public async Task UpdateData()
        {
           
            DateTime day = DateTime.Now.Date;
            List<DateTime> toUpdate = new List<DateTime>();

            for(int i=0;i<90;i++)
            {
                
                if(Infections.ContainsKey(day) == false || Healed.ContainsKey(day) == false || Deaths.ContainsKey(day) == false || Vaccines.ContainsKey(day) == false)
                {
                    toUpdate.Add(day);
                }
                
                day = day.AddDays(-1);
            }

            if (toUpdate.Count == 0)
                return;

            GetDataOnline();

            Vaccines vac;

            foreach(var d in toUpdate)
            {
                if(d.ToString("yyyy-MM-dd") == covidData.Json["currentDayStats"]["parsedOnString"].ToString())
                {
                    //Data for this day is found in currendDayStats
                    Infections.Add(d.Date, long.Parse(covidData.Json["currentDayStats"]["numberInfected"].ToString()));
                    Healed.Add(d.Date, long.Parse(covidData.Json["currentDayStats"]["numberCured"].ToString()));
                    Deaths.Add(d.Date, long.Parse(covidData.Json["currentDayStats"]["numberDeceased"].ToString()));

                    vac = new Vaccines();

                    vac.All = long.Parse(covidData.Json["currentDayStats"]["numberTotalDosesAdministered"].ToString());
                    vac.Pfizer = long.Parse(covidData.Json["currentDayStats"]["vaccines"]["pfizer"]["total_administered"].ToString());
                    vac.ImmunizedPfizer = long.Parse(covidData.Json["currentDayStats"]["vaccines"]["pfizer"]["immunized"].ToString());
                    vac.Moderna = long.Parse(covidData.Json["currentDayStats"]["vaccines"]["moderna"]["total_administered"].ToString());
                    vac.ImmunizedModerna = long.Parse(covidData.Json["currentDayStats"]["vaccines"]["moderna"]["immunized"].ToString());
                    vac.Astra = long.Parse(covidData.Json["currentDayStats"]["vaccines"]["astra_zeneca"]["total_administered"].ToString());
                    vac.ImmunizedAstra = long.Parse(covidData.Json["currentDayStats"]["vaccines"]["astra_zeneca"]["immunized"].ToString());
                    vac.ImmunizedAll = vac.ImmunizedPfizer + vac.ImmunizedModerna + vac.ImmunizedAstra;

                    Vaccines.Add(d.Date, vac);
                }
                else if(d.Date < DateTime.Parse(covidData.Json["currentDayStats"]["parsedOnString"].ToString()).Date)
                {
                    //Data for this day is found in historicalData
                    
                    Infections.Add(d.Date, long.Parse(covidData.Json["historicalData"][d.Date.ToString("yyyy-MM-dd")]["numberInfected"].ToString()));
                    Healed.Add(d.Date, long.Parse(covidData.Json["historicalData"][d.Date.ToString("yyyy-MM-dd")]["numberCured"].ToString()));
                    Deaths.Add(d.Date, long.Parse(covidData.Json["historicalData"][d.Date.ToString("yyyy-MM-dd")]["numberDeceased"].ToString()));

                    vac = new Vaccines();

                    vac.All = long.Parse(covidData.Json["historicalData"][d.Date.ToString("yyyy-MM-dd")]["numberTotalDosesAdministered"].ToString());
                    vac.Pfizer = long.Parse(covidData.Json["historicalData"][d.Date.ToString("yyyy-MM-dd")]["vaccines"]["pfizer"]["total_administered"].ToString());
                    vac.ImmunizedPfizer = long.Parse(covidData.Json["historicalData"][d.Date.ToString("yyyy-MM-dd")]["vaccines"]["pfizer"]["immunized"].ToString());
                    vac.Moderna = long.Parse(covidData.Json["historicalData"][d.Date.ToString("yyyy-MM-dd")]["vaccines"]["moderna"]["total_administered"].ToString());
                    vac.ImmunizedModerna = long.Parse(covidData.Json["historicalData"][d.Date.ToString("yyyy-MM-dd")]["vaccines"]["moderna"]["immunized"].ToString());
                    vac.Astra = long.Parse(covidData.Json["historicalData"][d.Date.ToString("yyyy-MM-dd")]["vaccines"]["astra_zeneca"]["total_administered"].ToString());
                    vac.ImmunizedAstra = long.Parse(covidData.Json["historicalData"][d.Date.ToString("yyyy-MM-dd")]["vaccines"]["astra_zeneca"]["immunized"].ToString());
                    vac.ImmunizedAll = vac.ImmunizedPfizer + vac.ImmunizedModerna + vac.ImmunizedAstra;

                    Vaccines.Add(d.Date, vac);

                }
                else
                {
                    //Data for this day doesn't exist
                    return;
                }
            }

            await SaveData();
        }

        async Task SaveData()
        {

            //Format infections, healed and deaths dictionary arrays into json
            string jsonInfections = Json.JsonSerializer.Serialize(Infections);
            string jsonHealed = Json.JsonSerializer.Serialize(Healed);
            string jsonDeaths = Json.JsonSerializer.Serialize(Deaths);
            string jsonVax = Json.JsonSerializer.Serialize(Vaccines);

            //Write to file
            using (var writer = File.CreateText(CACHE_INFECTIONS))
            {
                await writer.WriteAsync(jsonInfections);
                writer.Close();
            }

            using (var writer = File.CreateText(CACHE_HEALED))
            {
                await writer.WriteAsync(jsonHealed);
                writer.Close();
            }

            using (var writer = File.CreateText(CACHE_DEATHS))
            {
                await writer.WriteAsync(jsonDeaths);
                writer.Close();
            }

            using (var writer = File.CreateText(CACHE_VAX))
            {
                await writer.WriteAsync(jsonVax);
                writer.Close();
            }
        }

        void GetDataOnline()
        {
            string url = Uri.EscapeUriString(@"https://d35p9e4fm9h3wo.cloudfront.net/latestData.json");

            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                string json;
                json = client.DownloadString(url);
                covidData = JsonConvert.DeserializeObject<CovidData>(json);
            }

        }
    }
}
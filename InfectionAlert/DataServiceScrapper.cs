using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Android.Util;
using InfectionAlert.Models;

namespace PandemicAlert
{

    /**
     ******************* DEPRECATED IMPLEMENTATION
     */
    class DataServiceScrapper : IDataService
    {
        private readonly string CACHE_INFECTIONS = Path.Combine(Xamarin.Essentials.FileSystem.CacheDirectory, "infections.json");
        private readonly string CACHE_HEALED = Path.Combine(Xamarin.Essentials.FileSystem.CacheDirectory, "healed.json");
        private readonly string CACHE_DEATHS = Path.Combine(Xamarin.Essentials.FileSystem.CacheDirectory, "deaths.json");

        public Dictionary<DateTime, long> Infections { get; private set; }
        public Dictionary<DateTime, long> Healed { get; private set; }
        public Dictionary<DateTime, long> Deaths { get; private set; }

        public Dictionary<DateTime, long> Icu => throw new NotImplementedException();

        public Dictionary<DateTime, Vaccines> Vaccines => throw new NotImplementedException();

        public DataServiceScrapper()
        {
            Infections = new Dictionary<DateTime, long>();
            Healed = new Dictionary<DateTime, long>();
            Deaths = new Dictionary<DateTime, long>();
        }

        public async Task ClearData()
        {
            //Remove cached data so values have to be parsed again from the online source
            File.Delete(CACHE_INFECTIONS);
            File.Delete(CACHE_HEALED);
            File.Delete(CACHE_DEATHS);

            Infections = new Dictionary<DateTime, long>();
            Healed = new Dictionary<DateTime, long>();
            Deaths = new Dictionary<DateTime, long>();

        }

        public async Task LoadData()
        {
            //Load values from all days into memory
            //Load from Cache or parse the values if Cache doesn't exist

            Log.Debug("TAGTAG", "Entered LoadData()");

            //If there is no data cached on the device or some data is missing, parse it from the online source
            if ((CACHE_INFECTIONS == null || !File.Exists(CACHE_INFECTIONS)) ||
                (CACHE_HEALED == null || !File.Exists(CACHE_HEALED)) ||
                (CACHE_DEATHS == null || !File.Exists(CACHE_DEATHS)) )
            {

                Log.Debug("TAGTAG", "LoadData(): Cached data not found");

                DateTime day = DateTime.Now.AddDays(-10).Date;
                DateTime today = DateTime.Now.Date;

                while(day <= today)
                {
                    Log.Debug("TAGTAG", "LoadData(): Calling ParseData()");
                    ParseData(day);
                    day = day.AddDays(1);
                }

                //Cache the data on the device
                Log.Debug("TAGTAG", "LoadData(): Calling SaveData()");
                await SaveData();
            }
            else
            {
                Log.Debug("TAGTAG", "LoadData(): Cached data found. Loading in memory...");
                using (var reader = new StreamReader(CACHE_INFECTIONS, true))
                {
                    string json = reader.ReadToEnd();
                    Infections = JsonSerializer.Deserialize<Dictionary<DateTime, long>>(json);
                }

                using (var reader = new StreamReader(CACHE_HEALED, true))
                {
                    string json = reader.ReadToEnd();
                    Healed = JsonSerializer.Deserialize<Dictionary<DateTime, long>>(json);
                }

                using (var reader = new StreamReader(CACHE_DEATHS, true))
                {
                    string json = reader.ReadToEnd();
                    Deaths = JsonSerializer.Deserialize<Dictionary<DateTime, long>>(json);
                }

            }

        }

        //Save data to cache file
        async Task SaveData()
        {

            //Format infections, healed and deaths dictionary arrays into json
            string jsonInfections = JsonSerializer.Serialize(Infections);
            string jsonHealed = JsonSerializer.Serialize(Healed);
            string jsonDeaths = JsonSerializer.Serialize(Deaths);

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

        }


        //Parse values for a day from HTML
        void ParseData(DateTime day)
        {
            Log.Debug("TAGTAG", "Entered ParseData()");

            string date = FormatDateUrl(day);

            string urlAddress = @"https://stirioficiale.ro/informatii/buletin-de-presa-"+ date +"-ora-13-00";


            //Parse HTML to get infections, healed and deaths
            Log.Debug("TAGTAG", "ParseData(): Preparing HtmlWeb object");
            var htmlWeb = new HtmlWeb();
            var doc = htmlWeb.Load(urlAddress);

            if(doc == null)
            {
                Log.Debug("TAGTAG", "ParseData(): HtmlDocument is null. Exiting...");
                return;
            }

            Log.Debug("TAGTAG", "ParseData(): Parsing from " + urlAddress + "...");

            string paragraph = doc.DocumentNode
                .SelectNodes("//div[contains(@class,'break-words')]")
                .First().InnerText;

            //Log.Debug("TAGTAG", "ParseData(): Data from website: " + paragraph);

            Regex rg = new Regex(@"\d*\.{0,1}\d+"); //Match numbers
            var matches = rg.Matches(paragraph);

            long totalInfected = Int64.Parse(matches[1].Value, NumberStyles.AllowThousands, new CultureInfo("ro-RO"));
            long totalHealed = Int64.Parse(matches[3].Value, NumberStyles.AllowThousands, new CultureInfo("ro-RO"));

            Infections.Add(day, totalInfected);
            Healed.Add(day, totalHealed);

            /*TODO: Parse deaths */

            Deaths.Add(day, 0);

        }


        // Get any data missing since the last recorded day 
        public async Task UpdateData()
        {
            
            if(Infections.Count==0 || Healed.Count==0 || Deaths.Count==0)
            {
                return;
            }

            var dayList = Infections.Keys.ToList();
            dayList.Sort();
            DateTime lastDay = dayList.Last();
            dayList = Healed.Keys.ToList();
            dayList.Sort();
            lastDay = (dayList.Last()<lastDay)?dayList.Last():lastDay;
            dayList = Deaths.Keys.ToList();
            dayList.Sort();
            lastDay = (dayList.Last() < lastDay) ? dayList.Last() : lastDay;
            lastDay = lastDay.AddDays(1);

            if (lastDay <= DateTime.Now.Date)
            {
                while(lastDay<=DateTime.Now.Date)
                {
                    ParseData(lastDay);
                    lastDay = lastDay.AddDays(1);
                }

                await SaveData();
            }

        }

        string FormatDateUrl(DateTime date)
        {
            string month = date.Month switch
            {
                1 => "ianuarie",
                2 => "februarie",
                3 => "martie",
                4 => "aprilie",
                5 => "mai",
                6 => "iunie",
                7 => "iulie",
                8 => "august",
                9 => "septembrie",
                10 => "octombrie",
                11 => "noiembrie",
                12 => "decembrie",
                _ => throw new Exception("Month is out of valid range"),
            };

            return date.Day.ToString() + "-" + month + "-" + date.Year.ToString();
        }


    }
}
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfectionAlert.Models
{
    class CovidData
    {

        [JsonExtensionData]
        public IDictionary<string, JToken> Json { get; private set; }

    }
}
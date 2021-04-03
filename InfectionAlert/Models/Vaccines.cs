using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfectionAlert.Models
{
    public class Vaccines
    {
        public long All { get; set; }
        public long Pfizer { get; set; }
        public long Moderna { get; set; }
        public long Astra { get; set; }

        public long ImmunizedAll { get; set; }
        public long ImmunizedPfizer { get; set; }
        public long ImmunizedModerna { get; set; }
        public long ImmunizedAstra { get; set; }
    }
}
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using InfectionAlert.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PandemicAlert
{
    public interface IDataService
    {

        Dictionary<DateTime, long> Infections { get; }
        Dictionary<DateTime, long> Healed { get; }
        Dictionary<DateTime, long> Deaths { get; }
        Dictionary<DateTime, long> Icu { get; }
        Dictionary<DateTime, Vaccines> Vaccines { get; }

        Task ClearData();
        Task LoadData();
        Task UpdateData();
    }
}
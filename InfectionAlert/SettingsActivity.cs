using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Text.Style;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Android.Content.PM;
using System.Threading.Tasks;

namespace PandemicAlert
{
    [Activity(Label = "@string/settings_activity_title", Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class SettingsActivity : AppCompatActivity
    {

        Switch switch_showInfectionTrend, switch_showHealingInfectionTrend, switch_showActiveCases, switch_deathPercentage;
        Switch switch_showTodayInfections, switch_showTodayHealed, switch_showTodayDeaths;
        Switch switch_showDeathTrend;
        Switch switch_showIncidence;
        Switch switch_showVaccinationRate, switch_showImmunizedPercentage;
        Switch switch_showProjectedImmunization, switch_showProjectedVaccination, switch_showProjectedImmunizationHalf, switch_showProjectedVaccinationHalf, switch_showProjectedImmunizationQuarter, switch_showProjectedVaccinationQuarter;
        Switch switch_showVaccinatedPercentage;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_settings);

            switch_showInfectionTrend = FindViewById<Switch>(Resource.Id.switch_show_infection_trend);
            switch_showHealingInfectionTrend = FindViewById<Switch>(Resource.Id.switch_show_heal_infection_trend);
            switch_showActiveCases = FindViewById<Switch>(Resource.Id.switch_show_active_cases);
            switch_deathPercentage = FindViewById<Switch>(Resource.Id.switch_show_death_percentage);
            switch_showTodayInfections = FindViewById<Switch>(Resource.Id.switch_show_today_infections);
            switch_showTodayHealed = FindViewById<Switch>(Resource.Id.switch_show_today_healed);
            switch_showTodayDeaths = FindViewById<Switch>(Resource.Id.switch_show_today_deaths);
            switch_showDeathTrend = FindViewById<Switch>(Resource.Id.switch_show_death_trend);
            switch_showIncidence = FindViewById<Switch>(Resource.Id.switch_show_incidence);
            switch_showVaccinationRate = FindViewById<Switch>(Resource.Id.switch_show_vaccination_rate);
            switch_showImmunizedPercentage = FindViewById<Switch>(Resource.Id.switch_show_immunized_percentage);
            switch_showProjectedImmunization = FindViewById<Switch>(Resource.Id.switch_show_proj_immunization);
            switch_showProjectedVaccination = FindViewById<Switch>(Resource.Id.switch_show_proj_vaccination);
            switch_showVaccinatedPercentage = FindViewById<Switch>(Resource.Id.switch_show_vaccinated_percentage);

            switch_showProjectedImmunizationHalf = FindViewById<Switch>(Resource.Id.switch_show_proj_immunization_half);
            switch_showProjectedVaccinationHalf = FindViewById<Switch>(Resource.Id.switch_show_proj_vaccination_half);
            switch_showProjectedImmunizationQuarter = FindViewById<Switch>(Resource.Id.switch_show_proj_immunization_quarter);
            switch_showProjectedVaccinationQuarter = FindViewById<Switch>(Resource.Id.switch_show_proj_vaccination_quarter);


        }
    }
}
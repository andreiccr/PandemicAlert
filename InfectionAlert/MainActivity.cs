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
using Google.Android.Material.BottomNavigation;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Android.Content.PM;
using System.Threading.Tasks;
using OxyPlot.Xamarin.Android;
using OxyPlot.Axes;
using OxyPlot;
using OxyPlot.Series;
using PandemicAlert.Alert;
using Firebase.Messaging;

namespace PandemicAlert
{

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {

        IDataService dataService;
        DataCompute compute;
        ViewUpdater viewUpdater;
        AlertBuilder alert;

        // Status panel views
        TextView msg_infection_trend, msg_heal_infection_trend, msg_active_cases, msg_danger_percentage;
        ImageView image_infection_trend, image_heal_infection_trend;
        ScrollView subcontainer_status;

        // Data panel views
        ScrollView subcontainer_data;
        TextView msg_last_update;
        TextView msg_infections_today, msg_healed_today;
        TextView msg_immune_percentage;
        TextView msg_projected_immunity, msg_projected_vaccination;
        TextView msg_projected_immunity_half, msg_projected_vaccination_half;
        TextView msg_projected_immunity_quarter, msg_projected_vaccination_quarter;
        TextView msg_death_trend, msg_deaths_today, msg_incidence, msg_vaccination_rate;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            dataService = new DataService();
            viewUpdater = new ViewUpdater((Context)this);
            compute = new DataCompute();
            alert = new AlertBuilder();
            
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            alert.CreateChannel((NotificationManager)GetSystemService(Context.NotificationService));
            FirebaseMessaging.Instance.SubscribeToTopic("general");

            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);

            var toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            msg_infection_trend = FindViewById<TextView>(Resource.Id.msg_infection_trend);
            msg_heal_infection_trend = FindViewById<TextView>(Resource.Id.msg_heal_infection_trend);
            msg_active_cases = FindViewById<TextView>(Resource.Id.msg_active_cases);
            msg_danger_percentage = FindViewById<TextView>(Resource.Id.msg_danger_percentage);
            image_infection_trend = FindViewById<ImageView>(Resource.Id.image_infection_trend);
            image_heal_infection_trend = FindViewById<ImageView>(Resource.Id.image_heal_infection_trend);
            subcontainer_status = FindViewById<ScrollView>(Resource.Id.subcontainer_status);
            subcontainer_data = FindViewById<ScrollView>(Resource.Id.subcontainer_data);

            msg_last_update = FindViewById<TextView>(Resource.Id.msg_last_updated);
            msg_infections_today = FindViewById<TextView>(Resource.Id.msg_today_infections);
            msg_healed_today = FindViewById<TextView>(Resource.Id.msg_today_healed);
            msg_immune_percentage = FindViewById<TextView>(Resource.Id.msg_immune_percentage);
            msg_projected_immunity = FindViewById<TextView>(Resource.Id.msg_projected_immunity);
            msg_projected_vaccination = FindViewById<TextView>(Resource.Id.msg_projected_vaccination);
            msg_projected_immunity_half = FindViewById<TextView>(Resource.Id.msg_projected_immunity_half);
            msg_projected_vaccination_half = FindViewById<TextView>(Resource.Id.msg_projected_vaccination_half);
            msg_projected_immunity_quarter = FindViewById<TextView>(Resource.Id.msg_projected_immunity_quarter);
            msg_projected_vaccination_quarter = FindViewById<TextView>(Resource.Id.msg_projected_vaccination_quarter);
            msg_death_trend = FindViewById<TextView>(Resource.Id.msg_death_trend);
            msg_deaths_today = FindViewById<TextView>(Resource.Id.msg_today_deaths);
            msg_incidence = FindViewById<TextView>(Resource.Id.msg_incidence);
            msg_vaccination_rate = FindViewById<TextView>(Resource.Id.msg_vaccination_rate);

            //Initialize ViewUpdater views
            viewUpdater.InfectionTrend(msg_infection_trend, image_infection_trend);
            viewUpdater.HealInfectionTrend(msg_heal_infection_trend, image_heal_infection_trend);
            viewUpdater.ActiveInfections(msg_active_cases);
            viewUpdater.DangerPercentage(msg_danger_percentage);
            viewUpdater.LastUpdated(msg_last_update);
            viewUpdater.InfectedToday(msg_infections_today);
            viewUpdater.HealedToday(msg_healed_today);
            viewUpdater.ImmunizedPercentage(msg_immune_percentage);
            viewUpdater.ProjectedDateImmunized(msg_projected_immunity);
            viewUpdater.ProjectedDateVaccinated(msg_projected_vaccination);
            viewUpdater.ProjectedDateImmunizedHalf(msg_projected_immunity_half);
            viewUpdater.ProjectedDateVaccinatedHalf(msg_projected_vaccination_half);
            viewUpdater.ProjectedDateImmunizedQuarter(msg_projected_immunity_quarter);
            viewUpdater.ProjectedDateVaccinatedQuarter(msg_projected_vaccination_quarter);
            viewUpdater.DeathTrend(msg_death_trend);
            viewUpdater.DeathsToday(msg_deaths_today);
            viewUpdater.Incidence(msg_incidence);
            viewUpdater.VaccinationRate(msg_vaccination_rate);

            Log.Debug("TAGTAG", "This is a log");

            try
            {
                await dataService.LoadData();
                UpdateUI();
            }
            catch(Exception)
            {
                Toast.MakeText(this, "Eroare: Nu s-au putut încărca datele", ToastLength.Short).Show();
                msg_last_update.Text = "Eroare: Nu s-au putut încărca datele";
            }

        }

        protected async override void OnResume()
        {
            base.OnResume();
            Log.Debug("TAGTAG", "OnResume()");
            try
            {
                await dataService.UpdateData();
                UpdateUI();
            }
            catch (Exception)
            {
                Log.Debug("TAGTAG", "OnResume(): Updating data failed");
            }
            
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_status:
                    subcontainer_data.Visibility = ViewStates.Gone;
                    subcontainer_status.Visibility = ViewStates.Visible;

                    return true;
                case Resource.Id.navigation_data:
                    subcontainer_status.Visibility = ViewStates.Gone;
                    subcontainer_data.Visibility = ViewStates.Visible;

                    return true;
            }
            return false;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_buttons, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        private async Task OnOptionsItemSelectedAsync(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_sync:
                    Toast.MakeText(this, "Se aduc ultimele date", ToastLength.Short).Show();
                    try
                    {
                        await dataService.UpdateData();
                    } 
                    catch (Exception)
                    {
                        msg_last_update.Text = "Eroare: Nu s-au putut sincroniza datele";
                        return;
                    }
                    
                    break;

                case Resource.Id.menu_remove:
                    try
                    {
                        await dataService.ClearData();
                    }
                    catch (Exception)
                    {
                        return;
                    }

                    break;

                case Resource.Id.menu_reload:
                    Toast.MakeText(this, "Se reîncarcă toate datele", ToastLength.Short).Show();
                    try
                    {
                        await dataService.ClearData();
                        await dataService.LoadData();
                    }
                    catch(Exception)
                    {
                        msg_last_update.Text = "Eroare: Nu s-au putut sincroniza datele";
                        return;
                    }

                    break;

                case Resource.Id.menu_settings:
                    Toast.MakeText(this, "Debug: Meniu setari", ToastLength.Long).Show();
                    break;

            }

            UpdateUI();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            _ = OnOptionsItemSelectedAsync(item);

            return true;
        }

        void UpdateUI()
        {
            if (dataService.Infections == null || dataService.Infections.Count == 0)
                return;

            Log.Debug("TAGTAG", "UpdateUI(): Loading data to compute...");
            compute.Load(dataService.Infections, dataService.Healed, dataService.Deaths, dataService.Vaccines);

            Log.Debug("TAGTAG", "UpdateUI(): Computing...");
            compute.ComputeAll();

            Log.Debug("TAGTAG", "UpdateUI(): Updating views...");
            viewUpdater.UpdateInfectionTrend(compute.InfectionTrend);
            viewUpdater.UpdateHealInfectionTrend(compute.HealingTrend);
            viewUpdater.UpdateActiveInfections(compute.ActiveInfections);
            viewUpdater.UpdateDangerPercentage(19.0, Math.Round(compute.DeathPercentage*100,2));

            viewUpdater.UpdateLastUpdated(dataService.Infections.Keys.Max());
            viewUpdater.UpdateInfectedToday(compute.GetInfectedDay(dataService.Infections.Keys.Max()));
            viewUpdater.UpdateHealedToday(compute.GetHealedDay(dataService.Healed.Keys.Max()));

            viewUpdater.UpdateImmunizedPercentage(Math.Round(compute.GetImmunized(true, true), 2));
            viewUpdater.UpdateProjectedDateImmunized(compute.ProjectImmunizationDate(0.7));
            viewUpdater.UpdateProjectedDateVaccinated(compute.ProjectVaccinationDate(0.7));

            viewUpdater.UpdateProjectedDateImmunizedHalf(compute.ProjectImmunizationDate(0.5));
            viewUpdater.UpdateProjectedDateVaccinatedHalf(compute.ProjectVaccinationDate(0.5));

            viewUpdater.UpdateProjectedDateImmunizedQuarter(compute.ProjectImmunizationDate(0.25));
            viewUpdater.UpdateProjectedDateVaccinatedQuarter(compute.ProjectVaccinationDate(0.25));

            viewUpdater.UpdateVaccinationRate(Math.Round(compute.GetImmunizationRate(false, true)));
            viewUpdater.UpdateDeathsToday(compute.GetDeathsOnDay(dataService.Deaths.Keys.Max()));
            viewUpdater.UpdateIncidence((new DataService()).GetIncidenceInB());
            viewUpdater.UpdateDeathTrend(compute.GetDeathTrend());
        }

        

    }
}


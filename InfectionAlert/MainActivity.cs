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

namespace PandemicAlert
{

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {

        IDataService dataService;
        DataCompute compute;
        StatusContainer statusContainer;

        /* Status panel views */
        TextView msg_infections, msg_healratio, msg_activeinfections, msg_dangerpercentage;
        ImageView image_infections, image_healratio, image_activeinfections, image_dangerpercentage;
        GridLayout subcontainer_status;

        /* Data panel views */
        GridLayout subcontainer_data;
        TextView msg_last_update;
        TextView msg_infections_today, msg_healed_today;
        TextView msg_immune_percentage;
        TextView msg_projected_immunity, msg_projected_vaccination;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            dataService = new DataService();
            statusContainer = new StatusContainer(this);
            compute = new DataCompute();

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);

            var toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            msg_infections = FindViewById<TextView>(Resource.Id.message_infections);
            msg_healratio = FindViewById<TextView>(Resource.Id.message_healratio);
            msg_activeinfections = FindViewById<TextView>(Resource.Id.message_activecases);
            msg_dangerpercentage = FindViewById<TextView>(Resource.Id.message_dangerpercentage);
            image_infections = FindViewById<ImageView>(Resource.Id.image_infections);
            image_healratio = FindViewById<ImageView>(Resource.Id.image_healratio);
            image_activeinfections = FindViewById<ImageView>(Resource.Id.image_activecases);
            image_dangerpercentage = FindViewById<ImageView>(Resource.Id.image_dangerpercentage);
            subcontainer_status = FindViewById<GridLayout>(Resource.Id.subcontainer_status);
            subcontainer_data = FindViewById<GridLayout>(Resource.Id.subcontainer_data);

            msg_last_update = FindViewById<TextView>(Resource.Id.msg_last_updated);
            msg_infections_today = FindViewById<TextView>(Resource.Id.msg_today_infections);
            msg_healed_today = FindViewById<TextView>(Resource.Id.msg_today_healed);
            msg_immune_percentage = FindViewById<TextView>(Resource.Id.msg_immune_percentage);
            msg_projected_immunity = FindViewById<TextView>(Resource.Id.msg_projected_immunity);
            msg_projected_vaccination = FindViewById<TextView>(Resource.Id.msg_projected_vaccination);


            //Initialize StatusContainer views
            statusContainer.SetInfectionStatusView(msg_infections, image_infections);
            statusContainer.UpdateInfectionStatusView(InfectionStatus.NoData);
            statusContainer.SetHealedStatusView(msg_healratio, image_healratio);
            statusContainer.UpdateHealingStatusView(HealingStatus.NoData);
            statusContainer.SetActiveInfectionsView(msg_activeinfections);
            statusContainer.UpdateActiveInfectionsView(-1);
            statusContainer.SetDangerPercentageView(msg_dangerpercentage);
            statusContainer.UpdateDangerPercentageView(-1, -1);
            statusContainer.SetLastUpdatedView(msg_last_update);
            statusContainer.UpdateLastUpdatedView(null);

            statusContainer.SetInfectedTodayView(msg_infections_today);
            statusContainer.UpdateInfectedTodayView(-1);
            statusContainer.SetHealedTodayView(msg_healed_today);
            statusContainer.UpdateHealedTodayView(-1);

            statusContainer.SetImmunizedPercentageView(msg_immune_percentage);
            statusContainer.UpdateImmunizedPercentageView(-1);

            statusContainer.SetProjectedDateImmunizedView(msg_projected_immunity);
            statusContainer.UpdateProjectedDateImmunizedView(null);
            statusContainer.SetProjectedDateVaccinatedView(msg_projected_vaccination);
            statusContainer.UpdateProjectedDateVaccinatedView(null);

            Log.Debug("TAGTAG", "This is a log");

            await dataService.ClearData();

            try
            {
                await dataService.LoadData();
            }
            catch(Exception)
            {
                Toast.MakeText(this, "Eroare: Nu s-au putut încărca datele", ToastLength.Short).Show();
            }

            UpdateUI();

            foreach(DateTime day in dataService.Infections.Keys.ToList())
            {
                Log.Debug("TAGTAG", "List key: " + day);
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
                case Resource.Id.navigation_home:
                    subcontainer_data.Visibility = ViewStates.Gone;
                    subcontainer_status.Visibility = ViewStates.Visible;

                    return true;
                case Resource.Id.navigation_dashboard:
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
                        Toast.MakeText(this, "Eroare: Datele nu au putut fi șterse", ToastLength.Short).Show();
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

            foreach (DateTime day in dataService.Infections.Keys.ToList())
            {
                Log.Debug("TAGTAG", "List key: " + day);
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
            Log.Debug("TAGTAG", "UpdateUI(): Loading data to compute...");
            compute.Load(dataService.Infections, dataService.Healed, dataService.Deaths, dataService.Vaccines);

            Log.Debug("TAGTAG", "UpdateUI(): Computing...");
            compute.ComputeAll();

            Log.Debug("TAGTAG", "UpdateUI(): Updating views...");
            statusContainer.UpdateInfectionStatusView(compute.InfectionTrend);
            statusContainer.UpdateHealingStatusView(compute.HealingTrend);
            statusContainer.UpdateActiveInfectionsView(compute.ActiveInfections);
            statusContainer.UpdateDangerPercentageView(20, (int)(compute.DeathPercentage*100));

            statusContainer.UpdateLastUpdatedView(dataService.Infections.Keys.Max());
            statusContainer.UpdateInfectedTodayView(compute.GetInfectedDay(dataService.Infections.Keys.Max()));
            statusContainer.UpdateHealedTodayView(compute.GetHealedDay(dataService.Healed.Keys.Max()));

            statusContainer.UpdateImmunizedPercentageView(compute.GetImmunized());
            statusContainer.UpdateProjectedDateImmunizedView(compute.ProjectImmunizationDate(0.7));
            statusContainer.UpdateProjectedDateVaccinatedView(compute.ProjectVaccinationDate(0.7));
        }

        

    }
}


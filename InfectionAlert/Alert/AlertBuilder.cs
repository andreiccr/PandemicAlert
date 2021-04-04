using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PandemicAlert.Alert
{
    public class AlertBuilder
    {

        private const string CHANNEL = "General";
        private Context context;
        private Notification notification;
        public string LastError { get; private set; }


        /// <summary>
        /// WARNING: THROWS AN EXCEPTION
        /// Check if Google Play Services are installed on the device. They are required for Firebase Messaging to work. The LastError property is set if this function returns false.
        /// </summary>
        /// <returns>True if Google Play Services are installed on the device, false otherwise</returns>
        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(context);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                    LastError = GoogleApiAvailability.Instance.GetErrorString(resultCode);
                else
                {
                    //Device not supported
                    LastError = "Device not supported";
                }

                return false;
            }
            else
            {
                LastError = "";
                return true;
            }
        }

        /// <summary>
        /// Creates a notification channel. This is required on API levels >= 26.
        /// </summary>
        /// <param name="manager">The notification manager object from the current context</param>
        public void CreateChannel(NotificationManager manager)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            var channelName = CHANNEL;
            var channelDescription = "Notificari legate de evolutia pandemiei de Sars-CoV-2";
            var channel = new NotificationChannel(CHANNEL, channelName, NotificationImportance.Default)
            {
                Description = channelDescription
            };

            manager.CreateNotificationChannel(channel);
        }

        public AlertBuilder()
        {

        }

        public AlertBuilder(Context context, string message, string title)
        {
            this.context = context;

            Make(message, title);
        }

        public void Make(string message, string title)
        {
            NotificationCompat.Builder builder = new NotificationCompat.Builder(this.context, CHANNEL)
                .SetContentTitle(title)
                .SetContentText(message)
                .SetSmallIcon(Android.Resource.Drawable.ArrowUpFloat);

            this.notification = builder.Build();
        }

        public void Show()
        {
            NotificationManager notificationManager =
                this.context.GetSystemService(Context.NotificationService) as NotificationManager;

            notificationManager.Notify(0, this.notification);
        }


    }
}
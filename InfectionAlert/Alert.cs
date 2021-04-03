using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PandemicAlert
{
    class Alert
    {

        private const string CHANNEL = "General Channel";
        private Context Context;
        private Notification Notification;

        public static void CreateChannel(NotificationManager manager)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            var channelName = CHANNEL;
            var channelDescription = CHANNEL;
            var channel = new NotificationChannel(CHANNEL, channelName, NotificationImportance.Default)
            {
                Description = channelDescription
            };

            manager.CreateNotificationChannel(channel);
        }

        public Alert(Context context, string message, string title)
        {
            this.Context = context;

            Make(message, title);
        }

        public void Make(string message, string title)
        {
            NotificationCompat.Builder builder = new NotificationCompat.Builder(this.Context, CHANNEL)
                .SetContentTitle(title)
                .SetContentText(message)
                .SetSmallIcon(Android.Resource.Drawable.ArrowUpFloat);

            this.Notification = builder.Build();
        }

        public void Show()
        {
            NotificationManager notificationManager =
                this.Context.GetSystemService(Context.NotificationService) as NotificationManager;

            notificationManager.Notify(0, this.Notification);
        }


    }
}
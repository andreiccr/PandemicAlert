using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PandemicAlert
{
    class StatusContainer
    {
        TextView msgInfectionStatus;
        TextView msgHealedStatus;
        TextView msgActiveInfections;
        TextView msgDangerPercentage;

        ImageView imageInfectionStatus;
        ImageView imageHealedStatus;

        TextView msgLastUpdate;
        TextView msgInfectedToday, msgHealedToday;
        TextView msgImmunizedPercentage;
        TextView msgProjectedDateImmunized, msgProjectedDateVaccinated;

        //Context Context;

        public StatusContainer(Context context)
        {
            //this.Context = context;
        }

        public void SetInfectionStatusView(TextView textView, ImageView imageView)
        {
            this.msgInfectionStatus = textView;
            this.imageInfectionStatus = imageView;
        }

        public void SetHealedStatusView(TextView textView, ImageView imageView)
        {
            this.msgHealedStatus = textView;
            this.imageHealedStatus = imageView;
        }

        public void SetActiveInfectionsView(TextView textView)
        {
            this.msgActiveInfections = textView;
        }

        public void SetDangerPercentageView(TextView textView)
        {
            this.msgDangerPercentage = textView;
        }

        public void SetLastUpdatedView(TextView textView)
        {
            this.msgLastUpdate = textView;
        }

        public void SetInfectedTodayView(TextView textView)
        {
            this.msgInfectedToday = textView;
        }

        public void SetHealedTodayView(TextView textView)
        {
            this.msgHealedToday = textView;
        }

        public void SetImmunizedPercentageView(TextView textView)
        {
            this.msgImmunizedPercentage = textView;
        }

        public void SetProjectedDateImmunizedView(TextView textView)
        {
            this.msgProjectedDateImmunized = textView;
        }

        public void SetProjectedDateVaccinatedView(TextView textView)
        {
            this.msgProjectedDateVaccinated = textView;
        }

        public void UpdateProjectedDateImmunizedView(DateTime? date)
        {
            if (date == null)
            {
                //this.msgProjectedDateImmunized.Text = this.Context.GetString(Resource.String.no_data);
                this.msgProjectedDateImmunized.Text = "Nu sunt date";
                return;
            }
            
            string msg = "Vor fi imunizați 70% din populație pe:\n" + ((DateTime)date).Date.ToString("dd-MMMM-yyyy");
            SpannableString spannable = new SpannableString(msg);

            spannable.SetSpan(new ForegroundColorSpan(Color.BlueViolet), msg.IndexOf(":") + 1, msg.Length, SpanTypes.ExclusiveExclusive);
            spannable.SetSpan(new ForegroundColorSpan(Color.Black), msg.IndexOf("imunizați"), msg.IndexOf("imunizați") + "imunizați".Length, SpanTypes.ExclusiveExclusive);

            this.msgProjectedDateImmunized.TextFormatted = spannable;

        }

        public void UpdateProjectedDateVaccinatedView(DateTime? date)
        {
            if (date == null)
            {
                //this.msgProjectedDateVaccinated.Text = this.Context.GetString(Resource.String.no_data);
                this.msgProjectedDateVaccinated.Text = "Nu sunt date";
                return;
            }

            string msg = "Vor fi vaccinați 70% din populație pe:\n" + ((DateTime)date).Date.ToString("dd-MMMM-yyyy");
            SpannableString spannable = new SpannableString(msg);

            spannable.SetSpan(new ForegroundColorSpan(Color.BlueViolet), msg.IndexOf(":") + 1, msg.Length, SpanTypes.ExclusiveExclusive);
            spannable.SetSpan(new ForegroundColorSpan(Color.Black), msg.IndexOf("vaccinați"), msg.IndexOf("vaccinați") + "vaccinați".Length, SpanTypes.ExclusiveExclusive);
            this.msgProjectedDateVaccinated.TextFormatted = spannable;

        }

        public void UpdateImmunizedPercentageView(double percentage)
        {
            if (percentage == -1)
            {
                //this.msgImmunizedPercentage.Text = this.Context.GetString(Resource.String.no_data);
                this.msgImmunizedPercentage.Text = "Nu sunt date";
                return;
            }

            string msg = "Imunizați: " + Math.Round(percentage*100).ToString() +  "%  Ținta: 50%-70%" ;
            SpannableString spannable = new SpannableString(msg);

            spannable.SetSpan(new ForegroundColorSpan(Color.Blue), msg.IndexOf(":") + 1, msg.IndexOf("Ținta")-1, SpanTypes.ExclusiveExclusive);
            spannable.SetSpan(new ForegroundColorSpan(Color.BlueViolet), msg.IndexOf("Ținta") + 6, msg.Length, SpanTypes.ExclusiveExclusive);

            this.msgImmunizedPercentage.TextFormatted = spannable;

        }

        public void UpdateInfectedTodayView(long infectionNumber)
        {
            if(infectionNumber == -1)
            {
                //this.msgInfectedToday.Text = this.Context.GetString(Resource.String.no_data);
                this.msgInfectedToday.Text = "Nu sunt date";
                return;
            }

            string msg = "Infectări astăzi: " + infectionNumber.ToString();
            SpannableString spannable = new SpannableString(msg);

            spannable.SetSpan(new ForegroundColorSpan(Color.Red), msg.IndexOf(":") + 1, msg.Length, SpanTypes.ExclusiveExclusive);

            this.msgInfectedToday.TextFormatted = spannable;

        }

        public void UpdateHealedTodayView(long healNumber)
        {
            if (healNumber == -1)
            {
                //this.msgHealedToday.Text = this.Context.GetString(Resource.String.no_data);
                this.msgHealedToday.Text = "Nu sunt date";
                return;
            }

            string msg = "Vindecări astăzi: " + healNumber.ToString();
            SpannableString spannable = new SpannableString(msg);

            spannable.SetSpan(new ForegroundColorSpan(Color.Blue), msg.IndexOf(":") + 1, msg.Length, SpanTypes.ExclusiveExclusive);

            this.msgHealedToday.TextFormatted = spannable;

        }

        public void UpdateLastUpdatedView(DateTime? dateTime)
        {
            if (dateTime == null)
            {
                this.msgLastUpdate.Text = "";
                return;
            }
                

            DateTime dateTime1 = (DateTime)dateTime;
            this.msgLastUpdate.Text = "Ultima actualizare: " + dateTime1.ToString("dd-MMMM-yyyy");
        }



        public void UpdateInfectionStatusView(InfectionStatus status)
        {
            string msg;
            Color color;

            switch (status)
            {
                case InfectionStatus.Increasing:
                    msg = "Numărul de infectări este în CREȘTERE";
                    color = Color.Red;

                    this.imageInfectionStatus.SetImageResource(Resource.Drawable.arrow_up3);
                    break;

                case InfectionStatus.Decreasing:
                    msg = "Numărul de infectări este în SCĂDERE";
                    color = Color.Blue;

                    this.imageInfectionStatus.SetImageResource(Resource.Drawable.arrow_down3);
                    break;

                case InfectionStatus.Constant:
                    this.msgInfectionStatus.Text = "Numărul de infectări a rămas la fel";
                    this.imageInfectionStatus.SetImageResource(Resource.Drawable.unchanged);
                    return;

                default:
                    //this.msgInfectionStatus.Text = this.Context.GetString(Resource.String.no_data);
                    this.msgInfectionStatus.Text = "Nu sunt date";
                    this.imageInfectionStatus.SetImageResource(Resource.Drawable.unchanged);
                    return;
            }

            string SeparatorStr = "este în";
            SpannableString spannable = new SpannableString(msg);

            spannable.SetSpan(new ForegroundColorSpan(color), msg.IndexOf(SeparatorStr) + SeparatorStr.Length, msg.Length, SpanTypes.ExclusiveExclusive);

            this.msgInfectionStatus.TextFormatted = spannable;
        }


        public void UpdateHealingStatusView(HealingStatus status)
        {
            string msg;

            switch (status)
            {
                case HealingStatus.InfectionFavor:
                    msg = "Se INFECTEAZĂ mai mulți decât se vindecă";
                    break;

                case HealingStatus.HealingFavor:
                    msg = "Se VINDECĂ mai mulți decât se infectează";
                    break;

                case HealingStatus.Constant:
                    this.msgHealedStatus.Text = "Se infectează și se vindecă în aceeași masură";
                    return;

                default:
                    //this.msgHealedStatus.Text = this.Context.GetString(Resource.String.no_data);
                    this.msgHealedStatus.Text = "Nu sunt date";
                    return;
            }

            string RedStr = "infectează";
            string BlueStr = "vindecă";

            SpannableString spannable = new SpannableString(msg);
            spannable.SetSpan(new ForegroundColorSpan(Color.Red), msg.ToLower().IndexOf(RedStr), msg.ToLower().IndexOf(RedStr) + RedStr.Length, SpanTypes.ExclusiveExclusive);
            spannable.SetSpan(new ForegroundColorSpan(Color.Blue), msg.ToLower().IndexOf(BlueStr), msg.ToLower().IndexOf(BlueStr) + BlueStr.Length, SpanTypes.ExclusiveExclusive);

            this.msgHealedStatus.TextFormatted = spannable;
        }

        public void UpdateActiveInfectionsView(long number)
        {
            if(number < 0)
            {
                //this.msgActiveInfections.Text = this.Context.GetString(Resource.String.no_data);
                this.msgActiveInfections.Text = "Nu sunt date";
                return;
            }

            string msg = "În acest moment sunt " + number + " persoane infectate";

            SpannableString spannable = new SpannableString(msg);
            spannable.SetSpan(new ForegroundColorSpan(Color.Red), msg.IndexOf("sunt") + 4, msg.IndexOf("persoane"), SpanTypes.ExclusiveExclusive);

            this.msgActiveInfections.TextFormatted = spannable;
        }

        public void UpdateDangerPercentageView(int severePercent, int deathPercent)
        {
            if (severePercent < 0 || deathPercent < 0)
            {
                //this.msgDangerPercentage.Text = this.Context.GetString(Resource.String.no_data);
                this.msgDangerPercentage.Text = "Nu sunt date";
                return;
            }

            string msg = "Ai " + severePercent + "% șanse să faci o formă gravă\nAi " + deathPercent + "% șanse să fi ucis de virus";

            int indexOf1 = msg.IndexOf("%", 0), indexOf2 = msg.IndexOf("%", indexOf1 + 1);

            SpannableString spannable = new SpannableString(msg);
            spannable.SetSpan(new ForegroundColorSpan(Color.Red), 3, indexOf1 + 1, SpanTypes.ExclusiveExclusive);
            spannable.SetSpan(new ForegroundColorSpan(Color.Red), msg.IndexOf("Ai", indexOf1) + 2, indexOf2 + 1, SpanTypes.ExclusiveExclusive);

            this.msgDangerPercentage.TextFormatted = spannable;

        }

    }
}
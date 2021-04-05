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
    class ViewUpdater
    {
        TextView msgInfectionTrend, msgHealInfectionTrend, msgActiveInfections, msgDangerPercentage;
        ImageView imageInfectionTrend, imageHealInfectionTrend;
        TextView msgLastUpdate, msgInfectedToday, msgHealedToday, msgImmunizedPercentage;
        TextView msgProjectedDateImmunized, msgProjectedDateVaccinated;

        Context context;

        public ViewUpdater(Context context)
        {
            this.context = context;
        }


        public void InfectionTrend(TextView textView, ImageView imageView, InfectionStatus trend=InfectionStatus.NoData)
        {
            this.msgInfectionTrend = textView;
            this.imageInfectionTrend = imageView;

            UpdateInfectionTrend(trend);
        }

        public void HealInfectionTrend(TextView textView, ImageView imageView, HealingStatus trend=HealingStatus.NoData)
        {
            this.msgHealInfectionTrend = textView;
            this.imageHealInfectionTrend = imageView;

            UpdateHealInfectionTrend(trend);
        }

        public void ActiveInfections(TextView textView, long value=-1)
        {
            this.msgActiveInfections = textView;

            UpdateActiveInfections(value);
        }

        public void DangerPercentage(TextView textView, double valueSeverity = -1, double valueDeath = -1)
        {
            this.msgDangerPercentage = textView;

            UpdateDangerPercentage(valueSeverity, valueDeath);
        }

        public void LastUpdated(TextView textView, DateTime? dateTime = null)
        {
            this.msgLastUpdate = textView;

            UpdateLastUpdated(dateTime);
        }

        public void InfectedToday(TextView textView, long value = -1)
        {
            this.msgInfectedToday = textView;

            UpdateInfectedToday(value);
        }

        public void HealedToday(TextView textView, long value = -1)
        {
            this.msgHealedToday = textView;

            UpdateHealedToday(value);
        }

        public void ImmunizedPercentage(TextView textView, double value = -1)
        {
            this.msgImmunizedPercentage = textView;

            UpdateImmunizedPercentage(value);
        }

        public void ProjectedDateImmunized(TextView textView, DateTime? dateTime = null)
        {
            this.msgProjectedDateImmunized = textView;

            UpdateProjectedDateImmunized(dateTime);
        }

        public void ProjectedDateVaccinated(TextView textView, DateTime? dateTime = null)
        {
            this.msgProjectedDateVaccinated = textView;

            UpdateProjectedDateVaccinated(dateTime);
        }

        public void UpdateProjectedDateImmunized(DateTime? date)
        {
            if (date == null)
            {
                this.msgProjectedDateImmunized.Text = this.context.GetString(Resource.String.no_data);
                return;
            }
            
            string msg = "Vor fi imunizați 70% din populație pe:\n" + date?.Date.ToString("dd-MMMM-yyyy");
            SpannableString spannable = new SpannableString(msg);

            spannable.SetSpan(new ForegroundColorSpan(Color.BlueViolet), msg.IndexOf(":") + 1, msg.Length, SpanTypes.ExclusiveExclusive);
            spannable.SetSpan(new ForegroundColorSpan(Color.Black), msg.IndexOf("imunizați"), msg.IndexOf("imunizați") + "imunizați".Length, SpanTypes.ExclusiveExclusive);

            this.msgProjectedDateImmunized.TextFormatted = spannable;

        }

        public void UpdateProjectedDateVaccinated(DateTime? date)
        {
            if (date == null)
            {
                this.msgProjectedDateVaccinated.Text = this.context.GetString(Resource.String.no_data);
                return;
            }

            string msg = "Vor fi vaccinați 70% din populație pe:\n" + date?.Date.ToString("dd-MMMM-yyyy");
            SpannableString spannable = new SpannableString(msg);

            spannable.SetSpan(new ForegroundColorSpan(Color.BlueViolet), msg.IndexOf(":") + 1, msg.Length, SpanTypes.ExclusiveExclusive);
            spannable.SetSpan(new ForegroundColorSpan(Color.Black), msg.IndexOf("vaccinați"), msg.IndexOf("vaccinați") + "vaccinați".Length, SpanTypes.ExclusiveExclusive);
            this.msgProjectedDateVaccinated.TextFormatted = spannable;

        }

        public void UpdateImmunizedPercentage(double percentage)
        {
            if (percentage == -1)
            {
                this.msgImmunizedPercentage.Text = this.context.GetString(Resource.String.no_data);
                return;
            }

            string msg = "Imunizați: " + Math.Round(percentage*100).ToString() +  "%  Ținta: 70%" ;
            SpannableString spannable = new SpannableString(msg);

            spannable.SetSpan(new ForegroundColorSpan(Color.Blue), msg.IndexOf(":") + 1, msg.IndexOf("Ținta")-1, SpanTypes.ExclusiveExclusive);
            spannable.SetSpan(new ForegroundColorSpan(Color.BlueViolet), msg.IndexOf("Ținta") + 6, msg.Length, SpanTypes.ExclusiveExclusive);

            this.msgImmunizedPercentage.TextFormatted = spannable;

        }

        public void UpdateInfectedToday(long infectionNumber)
        {
            if(infectionNumber == -1)
            {
                this.msgInfectedToday.Text = this.context.GetString(Resource.String.no_data);
                return;
            }

            string msg = "Infectări astăzi: " + infectionNumber.ToString();
            SpannableString spannable = new SpannableString(msg);

            spannable.SetSpan(new ForegroundColorSpan(Color.Red), msg.IndexOf(":") + 1, msg.Length, SpanTypes.ExclusiveExclusive);

            this.msgInfectedToday.TextFormatted = spannable;

        }

        public void UpdateHealedToday(long healNumber)
        {
            if (healNumber == -1)
            {
                this.msgHealedToday.Text = this.context.GetString(Resource.String.no_data);
                return;
            }

            string msg = "Vindecări astăzi: " + healNumber.ToString();
            SpannableString spannable = new SpannableString(msg);

            spannable.SetSpan(new ForegroundColorSpan(Color.Blue), msg.IndexOf(":") + 1, msg.Length, SpanTypes.ExclusiveExclusive);

            this.msgHealedToday.TextFormatted = spannable;

        }

        public void UpdateLastUpdated(DateTime? dateTime)
        {
            if (dateTime == null)
            {
                this.msgLastUpdate.Text = "";
                return;
            }
                
            this.msgLastUpdate.Text = "Ultima actualizare: " + dateTime?.ToString("dd-MMMM-yyyy");
        }



        public void UpdateInfectionTrend(InfectionStatus status)
        {
            string msg;
            Color color;

            switch (status)
            {
                case InfectionStatus.Increasing:
                    msg = "Numărul de infectări este în CREȘTERE";
                    color = Color.Red;

                    this.imageInfectionTrend.SetImageResource(Resource.Drawable.arrow_up3);
                    break;

                case InfectionStatus.Decreasing:
                    msg = "Numărul de infectări este în SCĂDERE";
                    color = Color.Blue;

                    this.imageInfectionTrend.SetImageResource(Resource.Drawable.arrow_down3);
                    break;

                case InfectionStatus.Constant:
                    this.msgInfectionTrend.Text = "Numărul de infectări a rămas la fel";
                    this.imageInfectionTrend.SetImageResource(Resource.Drawable.unchanged);
                    return;

                default:
                    this.msgInfectionTrend.Text = this.context.GetString(Resource.String.no_data);
                    this.imageInfectionTrend.SetImageResource(Resource.Drawable.unchanged);
                    return;
            }

            string SeparatorStr = "este în";
            SpannableString spannable = new SpannableString(msg);

            spannable.SetSpan(new ForegroundColorSpan(color), msg.IndexOf(SeparatorStr) + SeparatorStr.Length, msg.Length, SpanTypes.ExclusiveExclusive);

            this.msgInfectionTrend.TextFormatted = spannable;
        }


        public void UpdateHealInfectionTrend(HealingStatus status)
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
                    this.msgHealInfectionTrend.Text = "Se infectează și se vindecă în aceeași masură";
                    return;

                default:
                    this.msgHealInfectionTrend.Text = this.context.GetString(Resource.String.no_data);
                    return;
            }

            string RedStr = "infectează";
            string BlueStr = "vindecă";

            SpannableString spannable = new SpannableString(msg);
            spannable.SetSpan(new ForegroundColorSpan(Color.Red), msg.ToLower().IndexOf(RedStr), msg.ToLower().IndexOf(RedStr) + RedStr.Length, SpanTypes.ExclusiveExclusive);
            spannable.SetSpan(new ForegroundColorSpan(Color.Blue), msg.ToLower().IndexOf(BlueStr), msg.ToLower().IndexOf(BlueStr) + BlueStr.Length, SpanTypes.ExclusiveExclusive);

            this.msgHealInfectionTrend.TextFormatted = spannable;
        }

        public void UpdateActiveInfections(long number)
        {
            if(number < 0)
            {
                this.msgActiveInfections.Text = this.context.GetString(Resource.String.no_data);
                return;
            }

            string msg = "În acest moment sunt " + number + " persoane infectate";

            SpannableString spannable = new SpannableString(msg);
            spannable.SetSpan(new ForegroundColorSpan(Color.Red), msg.IndexOf("sunt") + 4, msg.IndexOf("persoane"), SpanTypes.ExclusiveExclusive);

            this.msgActiveInfections.TextFormatted = spannable;
        }

        public void UpdateDangerPercentage(double severePercent, double deathPercent)
        {
            if (severePercent < 0 || deathPercent < 0)
            {
                this.msgDangerPercentage.Text = this.context.GetString(Resource.String.no_data);
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
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using InfectionAlert.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PandemicAlert
{
    class DataCompute
    {
        Dictionary<DateTime, long> infections;
        Dictionary<DateTime, long> healed;
        Dictionary<DateTime, long> deaths;
        Dictionary<DateTime, Vaccines> vaccines;

        //Population size in 2020 according to UN Data
        const int POPULATION_SIZE = 19237691;

        const int SAMPLE_SIZE = 7;

        public InfectionStatus InfectionTrend { get; private set; }
        public HealingStatus HealingTrend { get; private set; }
        public long ActiveInfections { get; private set; }
        public double DeathPercentage { get; private set; }
        
        public DataCompute()
        {

        }

        public DataCompute(Dictionary<DateTime, long> infections, Dictionary<DateTime, long> healed, Dictionary<DateTime, long> deaths)
        {
            Load(infections, healed, deaths);
        }

        public DataCompute(Dictionary<DateTime, long> infections, Dictionary<DateTime, long> healed, Dictionary<DateTime, long> deaths, Dictionary<DateTime, Vaccines> vaccines)
        {
            Load(infections, healed, deaths, vaccines);
        }

        public void Load(Dictionary<DateTime, long> infections, Dictionary<DateTime, long> healed, Dictionary<DateTime, long> deaths)
        {

            if(infections.Count != healed.Count || healed.Count != deaths.Count || infections.Count != deaths.Count)
            {
                throw new Exception("Data sets must not differ in size");
            }

            this.infections = infections;
            this.healed = healed;
            this.deaths = deaths;
        }

        public void Load(Dictionary<DateTime, long> infections, Dictionary<DateTime, long> healed, Dictionary<DateTime, long> deaths, Dictionary<DateTime, Vaccines> vaccines)
        {

            if (infections.Count != healed.Count || healed.Count != deaths.Count || infections.Count != deaths.Count || infections.Count != vaccines.Count)
            {
                throw new Exception("Data sets must not differ in size");
            }

            this.infections = infections;
            this.healed = healed;
            this.deaths = deaths;
            this.vaccines = vaccines;
        }


        //Get number of new infections for a particular day
        public long GetInfectedDay(DateTime day)
        {
            if (infections.ContainsKey(day) == false)
                return 0;
            
            if (infections.ContainsKey(day.AddDays(-1)) == false)
                return infections[day] - infections.Values.Min();

            return infections[day] - infections[day.AddDays(-1)];
        }


        //Get number of recovered patients for a particular day
        public long GetHealedDay(DateTime day)
        {
            if (healed.ContainsKey(day) == false)
                return 0;

            if (healed.ContainsKey(day.AddDays(-1)) == false)
                return healed[day] - healed.Values.Min();


            return healed[day] - healed[day.AddDays(-1)];
        }


        //Returns the slope value of the regression line. Positive value indicate increasing line
        double ComputeLinearRegression(List<long> xIndependent, List<long> yDependent)
        {
            Log.Debug("TAGTAG", "Computing linear regression");

            long sumX = 0, sumY = 0, sumCoef = 0, sumXSqr = 0;
            long n = xIndependent.LongCount();

            sumX = xIndependent.Sum();
            sumY = yDependent.Sum();

            //Sum of x^2
            foreach(long x in xIndependent)
            {
                sumXSqr += x * x;
            }

            //Sum of x*y
            for(int i=0;i<xIndependent.Count;i++)
            {
                sumCoef += xIndependent[i] * yDependent[i];
            }

            return (double)(sumY * sumXSqr - sumX * sumCoef) / (n * sumXSqr - sumX * sumX);
        }

        double SevenDayAverageInfections(DateTime day)
        {
            if(infections.ContainsKey(day.AddDays(-7)) == false)
            {
                return -1;
            }

            var dayList = infections.Keys.ToList();
            dayList.Sort();
            dayList.Reverse();

            long sum = 0;
            int i;

            for(i=0; i<7; i++)
            {
                day = day.AddDays(-i);
                if (infections.ContainsKey(day) == false)
                    break;
                sum += GetInfectedDay(day);
            }

            return (double)sum / (i+1);
        }

        double SevenDayAverageHealed(DateTime day)
        {
            if (healed.ContainsKey(day.AddDays(-7)) == false)
            {
                return -1;
            }

            var dayList = healed.Keys.ToList();
            dayList.Sort();
            dayList.Reverse();

            long sum = 0;
            int i;

            for (i = 0; i < 7; i++)
            {
                day = day.AddDays(-i);
                if (healed.ContainsKey(day) == false)
                    break;
                sum += GetHealedDay(day);
            }

            return (double)sum / (i + 1);
        }


        public long GetActiveCases()
        {
            if(infections == null || infections.Count == 0)
            {
                ActiveInfections = -1;
                return ActiveInfections;
            }

            var dayList = infections.Keys.ToList();
            dayList.Sort();

            ActiveInfections = infections[dayList.Last()] - ( healed[dayList.Last()] + deaths[dayList.Last()] );
            return ActiveInfections;
        }


        public InfectionStatus GetInfectionTrend()
        {
            if(infections == null || infections.Count == 0)
            {
                InfectionTrend = InfectionStatus.NoData;
                return InfectionTrend;
            }

            //List of dictionary keys in decreasing order
            var dayList = infections.Keys.ToList();
            dayList.Sort();
            dayList.Reverse();

            //Create sample data
            /*List<long> xValues = new List<long>();
            List<long> yValues = new List<long>();
            int n = Math.Min(7, infections.Count); //Sample size
            for (int i=0; i<n; i++) {
                xValues.Add(i);
                yValues.Add(GetInfectedDay(dayList[i]));
            }
            yValues.Reverse();

            double trend = ComputeLinearRegression(xValues, yValues);
            */

            double averageA, averageB;

            averageB = SevenDayAverageInfections(dayList[0]);
            averageA = SevenDayAverageInfections(dayList[7]);

            int trend = 0;
            if (averageA > averageB) trend = -1;
            else if (averageA < averageB) trend = 1; //Increasing

            Log.Debug("TAGTAG", "I Trend: " + averageA + " " + averageB);

            if (trend > 0) InfectionTrend = InfectionStatus.Increasing;
            else if (trend < 0 ) InfectionTrend = InfectionStatus.Decreasing;
            else InfectionTrend = InfectionStatus.Constant;
            
            return InfectionTrend;
        }

        public HealingStatus GetHealingTrend()
        {
            if (healed == null || healed.Count == 0)
            {
                HealingTrend = HealingStatus.NoData;
                return HealingTrend;
            }

            //List of dictionary keys in decreasing order
            var dayList = healed.Keys.ToList();
            dayList.Sort();
            dayList.Reverse();

            double averageHeal, averageInfection;

            averageHeal = SevenDayAverageHealed(dayList[0]);
            averageInfection = SevenDayAverageInfections(dayList[0]);

            if (averageHeal > averageInfection) HealingTrend = HealingStatus.HealingFavor;
            else if (averageHeal < averageInfection) HealingTrend = HealingStatus.InfectionFavor;
            else HealingTrend = HealingStatus.Constant;

            Log.Debug("TAGTAG", "HI Trend: " + averageHeal + " " + averageInfection);

            return HealingTrend;
        }

        public double GetDeathPercentage()
        {
            if (infections == null || infections.Count == 0)
            {
                DeathPercentage = -1;
                return DeathPercentage;
            }

            var dayList = infections.Keys.ToList();
            dayList.Sort();
            dayList.Reverse();
            

            DeathPercentage = (double)deaths[dayList[0]] / (double)infections[dayList[0]];
            return DeathPercentage;
        }

        public long GetVaccineCummulative(VaccineType vaccineType, bool immunizedOnly = false, DateTime? day = null)
        {
            if(this.vaccines == null)
            {
                throw new Exception("Vaccine data is not set");
            }

            DateTime d;
            if (day == null) d = DateTime.Today.Date;
            else d = (DateTime)day;

            if (this.vaccines.ContainsKey(d.Date) == false)
            {
                throw new Exception("No vaccine data for this day");
            }

            switch (vaccineType)
            {
                case VaccineType.All:
                    if (immunizedOnly)
                        return this.vaccines[d].ImmunizedAll;
                    return this.vaccines[d].All;

                case VaccineType.Pfizer:
                    {
                        long sum = 0;
                        DateTime dateTime = this.vaccines.Keys.Min();

                        while (dateTime.Date <= d)
                        {
                            if (immunizedOnly)
                                sum += vaccines[dateTime].ImmunizedPfizer;
                            else
                                sum += vaccines[dateTime].Pfizer;

                            d = d.AddDays(1);
                        }

                        return sum;
                    }
                case VaccineType.Moderna:
                    {
                        long sum = 0;
                        DateTime dateTime = this.vaccines.Keys.Min();

                        while (dateTime.Date <= d)
                        {
                            if (immunizedOnly)
                                sum += vaccines[dateTime].ImmunizedModerna;
                            else
                                sum += vaccines[dateTime].Moderna;

                            d = d.AddDays(1);
                        }

                        return sum;
                    }

                case VaccineType.AstraZeneca:
                    {
                        long sum = 0;
                        DateTime dateTime = this.vaccines.Keys.Min();

                        while (dateTime.Date <= d)
                        {
                            if (immunizedOnly)
                                sum += vaccines[dateTime].ImmunizedAstra;
                            else
                                sum += vaccines[dateTime].Astra;

                            d = d.AddDays(1);
                        }

                        return sum;
                    }

                default:
                    throw new Exception("Invalid vaccine option");
            }
            
            
        }

        public long GetVaccineOnDay(VaccineType vaccineType, bool immunizedOnly = false, DateTime? day = null)
        {
            if (this.vaccines == null)
            {
                throw new Exception("Vaccine data is not set");
            }

            DateTime d;
            if (day == null) d = DateTime.Today.Date;
            else d = ((DateTime)day).Date;

            if(this.vaccines.ContainsKey(d.Date) == false)
            {
                throw new Exception("No vaccine data for this day");
            }

            switch (vaccineType)
            {
                case VaccineType.All:

                    if(this.vaccines.ContainsKey(d.AddDays(-1).Date) == false)
                    {
                        if (immunizedOnly)
                            return this.vaccines[d].ImmunizedAll;
                        else
                            return this.vaccines[d].All;
                    }

                    long number;

                    if (immunizedOnly)
                        number = this.vaccines[d].ImmunizedAll - this.vaccines[d.AddDays(-1)].ImmunizedAll;
                    else 
                        number = this.vaccines[d].All - this.vaccines[d.AddDays(-1)].All;

                    return number;


                case VaccineType.Pfizer:
                    if (immunizedOnly)
                        return this.vaccines[d].ImmunizedPfizer;
                    return this.vaccines[d].Pfizer;

                case VaccineType.Moderna:
                    if (immunizedOnly)
                        return this.vaccines[d].ImmunizedModerna;
                    return this.vaccines[d].Moderna;

                case VaccineType.AstraZeneca:
                    if (immunizedOnly)
                        return this.vaccines[d].ImmunizedAstra;
                    return this.vaccines[d].Astra;

                default:
                    throw new Exception("Invalid vaccine option");
            }

        }

        public double GetImmunized(bool bothDosesOnly = false, bool excludeHealed = false)
        {
            long healed = (excludeHealed) ? 0 : this.healed[this.healed.Keys.Max()];
            long vaccinated = (bothDosesOnly) ? this.vaccines[this.vaccines.Keys.Max()].ImmunizedAll : this.vaccines[this.vaccines.Keys.Max()].All;

            return (double)(healed + vaccinated) / POPULATION_SIZE;
        }

        public double GetImmunizationRate(bool bothDosesOnly = false, bool excludeHealed = false)
        {

            DateTime day = this.vaccines.Keys.Max();
            DateTime target = day.AddDays(-14);

            long sum = 0;

            while(day > target)
            {
                sum += GetVaccineOnDay(VaccineType.All, bothDosesOnly, day);
                if(excludeHealed == false)
                {
                   sum += GetHealedDay(day);
                }

                day = day.AddDays(-1);
            }

            return (double)sum / 14;
        }

        public DateTime ProjectImmunizationDate(double targetPercentage)
        {
            double immunizationRate = GetImmunizationRate();

            double targetPopulation = POPULATION_SIZE * targetPercentage;

            DateTime day = DateTime.Now.AddDays(Math.Round(targetPopulation / immunizationRate)).Date;

            return day;
        }

        public DateTime ProjectVaccinationDate(double targetPercentage)
        {
            double immunizationRate = GetImmunizationRate(false, true);

            double targetPopulation = POPULATION_SIZE * targetPercentage;

            DateTime day = DateTime.Now.AddDays(Math.Round(targetPopulation / immunizationRate)).Date;

            return day;
        }

        public void ComputeAll()
        {
            GetInfectionTrend();
            GetHealingTrend();
            GetActiveCases();
            GetDeathPercentage();
        }


    }

    enum InfectionStatus
    {
        NoData = -1,
        Constant = 0,
        Increasing = 1,
        Decreasing = 2
    };

    enum HealingStatus
    {
        NoData = -1,
        Constant = 0,
        InfectionFavor = 1,
        HealingFavor = 2
    }

    enum VaccineType
    {
        All = 0,
        Pfizer = 1,
        Moderna = 2,
        AstraZeneca, Vaxzevria = 3
    }
}
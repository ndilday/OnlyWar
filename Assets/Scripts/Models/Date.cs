using System;
using UnityEngine;
namespace OnlyWar.Scripts.Models
{
    [Serializable]
    public class Date : IComparable
    {
        public int Millenium;
        public int Year;
        public int Week;
        public Date(int millenium, int year, int week)
        {
            Millenium = millenium;
            Year = year;
            Week = week;
        }

        public void IncrementWeek()
        {
            if(Week == 52)
            {
                Week = 1;
                if (Year == 999)
                {
                    Year = 0;
                    Millenium++;
                }
                else
                {
                    Year++;
                }
            }
            else
            {
                Week++;
            }
        }
        public override string ToString()
        {
            return Week.ToString() + "." + Year.ToString() + ".M" + Millenium.ToString();
        }

        public bool IsBetweenInclusive(Date earlierDate, Date laterDate)
        {
            return IsAfterOrEqual(earlierDate) && IsBeforeOrEqual(laterDate);
        }

        public bool IsBeforeOrEqual(Date otherDate)
        {
            if(Millenium > otherDate.Millenium
                || (Millenium == otherDate.Millenium && Year > otherDate.Year)
                || (Millenium == otherDate.Millenium && Year == otherDate.Year && Week > otherDate.Week))
            {
                return false;
            }
            return true;
        }

        public bool IsAfterOrEqual(Date otherDate)
        {
            if (Millenium < otherDate.Millenium
                || (Millenium == otherDate.Millenium && Year < otherDate.Year)
                || (Millenium == otherDate.Millenium && Year == otherDate.Year && Week < otherDate.Week))
            {
                return false;
            }
            return true;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            if (!(obj is Date otherDate))
            {
                throw new ArgumentException("Object is not a Date");
            }
            if (this == otherDate) return 0;
            if (this.IsBeforeOrEqual(otherDate)) return -1;
            return 1;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is Date otherDate))
            {
                return false;
            }
            return Millenium == otherDate.Millenium
                && Year == otherDate.Year
                && Week == otherDate.Week;
        }

        public int GetWeeksDifference(Date otherDate)
        {
            return ((Millenium - otherDate.Millenium) * 52000)
                + ((Year - otherDate.Year) * 52)
                + (Week - otherDate.Week);
        }
    }
}

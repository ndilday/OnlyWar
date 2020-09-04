using System;
using UnityEngine;
namespace Iam.Scripts.Models
{
    [Serializable]
    public class Date
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
    }
}

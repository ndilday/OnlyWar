namespace Iam.Scripts.Models
{
    public class Date
    {
        private string _string;
        public int Millenium { get; private set; }
        public int Year { get; private set; }
        public int Week { get; private set; }
        public Date(int millenium, int year, int week)
        {
            Millenium = millenium;
            Year = year;
            Week = week;
            _string = Week.ToString() + "." + Year.ToString() + ".M" + Millenium.ToString();
        }
        public override string ToString()
        {
            return _string;
        }
    }
}

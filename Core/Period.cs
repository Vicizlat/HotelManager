using System;

namespace Core
{
    public class Period
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Nights => (EndDate - StartDate).Days;

        public Period(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        public bool ContainsDate(DateTime date)
        {
            return StartDate <= date && date < EndDate;
        }

        public override string ToString()
        {
            return $"{StartDate:dd.MM.yyyy}|{EndDate:dd.MM.yyyy}";
        }
    }
}
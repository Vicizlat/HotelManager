namespace Core
{
    public class Sums
    {
        public decimal Total { get; set; }
        public decimal Paid { get; set; }
        public decimal Remaining => Total - Paid;

        public Sums(decimal total, decimal paid)
        {
            Total = total;
            Paid = paid;
        }

        public override string ToString()
        {
            return $"{Total}|{Paid}";
        }
    }
}
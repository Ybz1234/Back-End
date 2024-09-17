namespace Back_End.Models
{
    public class Airport
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public City City { get; set; }
        public Country Country { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}\nName: {Name}\nCity: {City?.ToString()}\nCountry: {Country?.ToString()}";
        }
    }
}

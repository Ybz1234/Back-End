namespace Back_End.Models
{
    public class Hotel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public City City { get; set; }
        public Country Country { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}\nName: {Name}\nAddress: {Address}\nCity: {City?.ToString()}\nCountry: {Country?.ToString()}";
        }
    }
}

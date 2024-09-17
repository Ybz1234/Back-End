namespace Back_End.Models
{
    public class TripTicket
    {
        public Queue<City> Cities { get; set; }
        public Queue<Hotel> Hotels { get; set; }
        public Queue<FlightTicket> FlightTickets { get; set; }
    }
}

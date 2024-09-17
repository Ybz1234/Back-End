namespace Back_End.Models
{
    public class FlightTicket
    {
        public int Id { get; set; }
        public User User { get; set; }
        public Flight Flight { get; set; }
        public override string ToString()
        {
            return $"Flight ticket id:{Id}, user id: {User.Id}, flight id: {Flight.Id}";
        }
    }
}

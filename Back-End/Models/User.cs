namespace Back_End.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public override string ToString()
        {
            return $"Id: {Id}\nName: {FirstName} {LastName}\nEmail: {Email}";
        }
    }
}

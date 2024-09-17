namespace Back_End.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}\nName: {Name}";
        }
    }
}

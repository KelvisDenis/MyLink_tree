namespace MySite.Models
{
    public class Links
    {
        public int Id { get; set; }
        public User? User { get; set; }
        public string? Url { get; set; }
        public Links() { }

        public Links(int id, string? url, User user)
        {
            Id = id;
            Url = url;
            User = user;
        }
    }
}

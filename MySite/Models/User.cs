using System.Collections;

namespace MySite.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? UserName { get; set; }
        public string[]? Urls { get; set; }

        public ICollection<Links>? Links { get; set; } = new List<Links>();

        public User()
        {
        }

        public User(int id, string email, string password, string userName)
        {
            Id = id;
            Email = email;
            Password = password;
            UserName = userName;
        }
        public void Add(string link) {
         Links links = new Links { Url = link};
         Links.Add(links);
        }
        public void Remove(Links links) {
        Links.Remove(links);
        }
        public bool checkpassword (string email, string senha)
        {
            if (email == Email && senha == Password) { 
                return true;
            }
            return false;
        }

    }
}

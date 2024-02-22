using Microsoft.EntityFrameworkCore;
using MySite.Data;
using MySite.Models;
using MySite.Models.OutherModels;

namespace MySite.Services
{
    public class UserServices
    {
        private readonly MySiteContext _context;

        public UserServices(MySiteContext context)
        {
            _context = context;
        }
        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

        }
        public async Task<User> GetUserAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(o => o.Id == id);
            return user;

        }
        public async Task<ICollection<Links>> GetLinksAsync(User user)
        {
            var getLinks = await _context.Users.FindAsync(user.Links);
            return getLinks.Links;

        }



    }
}

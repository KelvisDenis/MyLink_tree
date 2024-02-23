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
        public async Task<User> GetUserAsync(string log)
        {
            var user = await _context.Users.FirstOrDefaultAsync(o => o.Email == log);
            return user;

        }
        public async Task<ICollection<Links>> GetLinksAsync(User user)
        {
            var getLinks = await _context.Users.FindAsync(user.Links);
            return getLinks.Links;
        }
        public async Task AddLinksAsync(User user)
        {
            await _context.Links.AddRangeAsync(user.Links);
            await _context.SaveChangesAsync();

        }



    }
}

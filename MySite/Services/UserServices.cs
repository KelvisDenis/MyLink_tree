using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySite.Data;
using MySite.Models;
using MySite.Models.OutherModels;
using System.Reflection.Metadata.Ecma335;

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

        public async Task<User> GetUserAsync(User log)
        {
            var user = await _context.Users.FirstOrDefaultAsync(o => o.Email == log.Email);
            return  user;

        }
        public async Task<User> GetLinksAsync(User user)
        {
            var getLinks = await _context.Users.FindAsync(user.Email);
            return getLinks;
        }
        public async Task AddLinksAsync(Links links)
        {
            await _context.Links.AddAsync(links);
            await _context.SaveChangesAsync();

        }
        public async Task<User> GetUseridsAsync(int? id)
        {
            return await _context.Users.FirstOrDefaultAsync(o => o.Id == id);
        }
        public async Task Update(User user)
        {
            var teste = await _context.Users.AnyAsync(o => o.Email == user.Email);
            if (teste)
            {
                _context.Update(user);
                _context.SaveChanges();
            }
           
        }
      


    }
}

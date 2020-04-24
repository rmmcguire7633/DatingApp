using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.DataAccess
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContex _contex;

        public DatingRepository(DataContex contex)
        {
            _contex = contex;
        }

        public void add<T>(T entity) where T : class
        {
            _contex.Add(entity);
        }

        public void delete<T>(T entity) where T : class
        {
            _contex.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            var user =  await _contex.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);

            return user;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await _contex.Users.Include(p => p.Photos).ToListAsync();

            return users;
        }

        public async Task<bool> SaveAll()
        {
            return await _contex.SaveChangesAsync() > 0;
        }
    }
}

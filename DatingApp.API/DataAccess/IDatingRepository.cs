using DatingApp.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.DataAccess
{
    public interface IDatingRepository
    {
        void add<T>(T entity) where T : class;

        void delete<T>(T entity) where T : class;
        
        // when we save changesd there will be 0 or more than 0 changes, this will help keep track of if there is information to save
        Task<bool> SaveAll();

        Task<IEnumerable<User>> GetUsers();

        Task<User> GetUser(int id);
    }
}

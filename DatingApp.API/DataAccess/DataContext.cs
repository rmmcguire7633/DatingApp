using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.DataAccess
{
    public class DataContex : DbContext
    {
        public DataContex(DbContextOptions<DataContex> options) : base(options) {}

        public DbSet<Value>  Values { get; set; }
    }
}

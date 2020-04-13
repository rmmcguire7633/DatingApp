﻿using DatingApp.API.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.DataAccess
{
    public class Seed
    {
        public static void SeedUsers(DataContex context) 
        {
            // if there are no users we will create some
            if (!context.Users.Any())
            {
                var userData = System.IO.File.ReadAllText("DataAccess/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);

                // add the users in the seed json to the db
                foreach (var user in users)
                {
                    byte[] passwordhash, passwordSalt;
                    CreatePasswordHash("password", out passwordhash, out passwordSalt);

                    user.PasswordHash = passwordhash;
                    user.PasswordSalt = passwordSalt;
                    user.Username = user.Username.ToLower();

                    context.Users.Add(user);
                }

                // saves the users into the db
                context.SaveChanges();
            }
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

        }
    }
}

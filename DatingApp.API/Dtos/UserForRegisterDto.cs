using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Dtos
{
    /**
     * This is used to trasfor data from the post message of a http post request
     * Validation is built in where the username is required and the password is required and needs to be between 4 - 8 charcters
     * **/
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "Password must be between 4 and 8 charcters")]
        public string Password { get; set; }
    }
}

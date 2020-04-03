﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace JWTAuthentication.Model
{
    public class UserModelLogin
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }

        public string Password { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maonot_Net.Controllers
{
    public class functions
    {
        //check if the password that the user entered is like in the DB
        public bool CheckPassword(string submittedPassword, string hashedPassword)
        {

            bool validPassword = BCrypt.Net.BCrypt.Verify(submittedPassword, hashedPassword);
            return validPassword;
        }
        // check if the date is over
        public Boolean Comper(DateTime EndDate)
        {
            //DateTime EndDate = new DateTime(2019, 7, 30);
            DateTime Today = DateTime.Now;
            int result = DateTime.Compare(EndDate, Today);

            if (result < 0)
                return false;
            else
                return true;
        }
    }
}

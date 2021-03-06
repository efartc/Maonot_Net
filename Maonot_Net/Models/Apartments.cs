﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maonot_Net.Models
{
    public class Apartments
    {
        public int ID { get; set; }
        public int ApartmentNum { get; set; }

        public String Type { get; set; }

        public Choose? LivingWithReligious { get; set; }
   
        public Choose? LivingWithSmoker { get; set; }

        public Religious? ReligiousType { get; set; }

        public Gender? Gender { get; set; }

        public int? capacity { get; set; }

        public Assigning assigning { get; set; }



    }
}

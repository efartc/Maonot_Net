﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace Maonot_Net.Models
{
    public enum RoomNum
    {
        [Description("1")]
        OneA = 1,
        [Description("2")]
        TwoA = 2,
        [Description("3")]
        ThreeA = 3,
        [Description("4")]
        FourA = 4,
    };
    public class VisitorsLog
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "תאריך ושעת כניסה")]
        [DisplayFormat(ApplyFormatInEditMode = false)]
        [DataType(DataType.Date)]
        public DateTime EnteryDate { get; set; }

        [Required]
        [Display(Name = "שם אורח")]
        public string VistorName { get; set; }
        [Required]
        [Display(Name = "ת.ז. אורח")]
        public int VisitorID { get; set; }
        [Required]
        [Display(Name = "שם מלא של הדייר")]
        public string StudentFullName { get; set; }

        public int? StudentId { get; set; }

        [Display(Name = "תאריך ושעת יציאה")]
        [DisplayFormat(ApplyFormatInEditMode = false)]
        [DataType(DataType.Date)]
       
       
        public DateTime? ExitDate { get; set; }


        [Required]
        [Display(Name = "מספר דירה")]
        public int ApartmentNum { get; set; }
        [Required]
        [Display(Name = "מספר חדר")]
        public RoomNum? Room { get; set; }
        [Display(Name = "חתימה")]
        public Boolean Signature { get; set; }



        public ICollection<ApprovalKit> App { get; set; }

    }






}


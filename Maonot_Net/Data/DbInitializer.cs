﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Maonot_Net.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Maonot_Net.Data
{
    public class DbInitializer
    {
        public static void Initialize(MaonotNetContext context)
        {
            context.Database.EnsureCreated();
            if (context.Users.Any())
            {
                return;
            }
            var users = new User[]
            {
                new User{StundetId= 302875125, FirstName= "Miki", LastName= "Rotenstain", Password=BCrypt.Net.BCrypt.HashPassword("ABCabc123!"), Email = "mikir2127@gmail.com",Authorization=2},
                new User{StundetId= 308242122, FirstName= "Efrat", LastName= "Cohen", Password= BCrypt.Net.BCrypt.HashPassword("ABCabc123!"), Email= "efratc66@gmail.com",Authorization=7},
                new User{StundetId= 123456789, FirstName= "מנהל", LastName= "מערכת", Password= BCrypt.Net.BCrypt.HashPassword("ABCabc123!"), Email= "efratc66@gmail.com",Authorization=1}

            };
            foreach (User u in users)
            {
                context.Users.Add(u);
            }
            context.SaveChanges();

            var reg = new Registration[]
            {
                new Registration{StundetId= users.Single(u=> u.StundetId == 302875125).StundetId, FirstName= users.Single(u=> u.StundetId == 302875125).FirstName , LastName= users.Single(u=> u.StundetId == 302875125).LastName,gender=Gender.זכר, Bday= DateTime.Parse("1989-11-04"), City= "נשר", Adress= "עמוס 18",PhoneNumber= "050-2480441",
                    FieldOfStudy=FieldStudy.מערכות_מידע,SteadyYear=Year.ג, TypeOfService=Service.צבאי, HealthCondition=HealthCondition.ללא_מגבלה, Seniority=Year.ג, ApertmantType= ApertmantType.יחיד},
               // new Registration{StundetId= users.Single(u=> u.StundetId == 308242122).StundetId, FirstName= users.Single(u=> u.StundetId == 308242122).FirstName , LastName= users.Single(u=> u.StundetId == 308242122).LastName,gender=Gender.נקבה, Bday= DateTime.Parse("1992-02-14"), City= "להבים", Adress= "גומא 2",PhoneNumber= "054-6814427",
                   // FieldOfStudy=FieldStudy.מערכות_מידע,SteadyYear=Year.ג, TypeOfService=Service.צבאי, HealthCondition=HealthCondition.ללא_מגבלה, Seniority=Year.ג, ApertmantType= ApertmantType.יחיד},
            };
            foreach (Registration u in reg)
            {
                context.Registrations.Add(u);
            }
            context.SaveChanges();

            var App = new ApprovalKit[]
            {
                new ApprovalKit{StundetId= users.Single(u=> u.StundetId == 302875125).StundetId, FirstName= users.Single(u=> u.StundetId == 302875125).FirstName , LastName= users.Single(u=> u.StundetId == 302875125).LastName, RoomType=RoomType.חדר_ליחיד,
                    LivingWithSmoker = Choose.מעוניין,LivingWithReligious=Choose.לא_מעוניין, ReligiousType=Religious.דרוזי, HealthCondition=HealthCondition.ללא_מגבלה,Gender= reg.Single(r=>r.StundetId==302875125).gender}
               // new ApprovalKit{StundetId= users.Single(u=> u.StundetId == 308242122).StundetId, FirstName= users.Single(u=> u.StundetId == 308242122).FirstName , LastName= users.Single(u=> u.StundetId == 308242122).LastName, RoomType=RoomType.חדר_ליחיד,
                  //  LivingWithSmoker = Choose.לא_מעוניין,LivingWithReligious=Choose.מעוניין, ReligiousType=Religious.יהודי, HealthCondition=HealthCondition.ללא_מגבלה}
            };
            foreach (ApprovalKit u in App)
            {
                context.ApprovalKits.Add(u);
            }
            context.SaveChanges();

            var Aut = new Authorization[]
{
                new Authorization{ AutName = "מנהל מערכת"},//1
                new Authorization{ AutName = "מנהל"},//2
                new Authorization{ AutName = "אבות בית"},//3
                new Authorization{ AutName = "ועדת משמעת"},//4
                new Authorization{ AutName = "ועדת תרבות"},//5
                new Authorization{ AutName = "עובד אבטחה"},//6
                new Authorization{ AutName = "אורח"},//7
                new Authorization{ AutName = "מועמד"},//8
                new Authorization{ AutName = "דייר"}//9
};
            foreach (Authorization u in Aut)
            {
                context.Authorizations.Add(u);
            }
            context.SaveChanges();

            var Fault = new FaultForm[]
            {
                new FaultForm{StundetId=308242122, Apartment= 927, RoomNum= RoomNum.OneA, FullName= users.Single(u=> u.StundetId==308242122).FirstName+""+users.Single(u=> u.StundetId==308242122).LastName, PhoneNumber="050-2480441",Description="הבוילר לא עובד לי אשמח לעזרתכם" }
            };
            foreach (FaultForm u in Fault)
            {
                context.FaultForms.Add(u);
            }
            context.SaveChanges();

            var Messg = new Message[]
            {
                new Message{ From="ענת לוין",MsgTime=DateTime.Now, Addressee="אפרת כהן", Subject="משעמם לי בדירה", Content="לה לה הלה לה" }
            };
            foreach (Message u in Messg)
            {
                context.Messages.Add(u);
            }
            context.SaveChanges();

            var Vlog = new VisitorsLog[]
            {
                new VisitorsLog{VistorName= "דני שובבני", StudentFullName= users.Single(u=> u.StundetId== 302875125).FullName,
                     ApartmentNum=927,Room=RoomNum.OneA, EnteryDate= DateTime.Parse("2018-06-22"),VisitorID= 123456789,StudentId=302875125
                     ,ExitDate= DateTime.Parse("2018-06-24")
                }
            };
            foreach (VisitorsLog u in Vlog)
            {
                context.VisitorsLogs.Add(u);
            }
            context.SaveChanges();

            var War = new Warning[]
            {
               // new Warning{WarningNumber=WarningNumber.ראשונה, StudentId=users.Single(u=> u.StundetId==302875125).StundetId, Date=DateTime.Parse("2018-05-20")}
            };
            foreach (Warning u in War)
            {
                context.Warnings.Add(u);
            }
            context.SaveChanges();

        }

    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Maonot_Net.Data;
using Maonot_Net.Models;
using Microsoft.AspNetCore.Http;

namespace Maonot_Net.Controllers
{
    public class ApartmentsController : Controller
    {
        private readonly MaonotNetContext _context;

        public ApartmentsController(MaonotNetContext context)
        {
            _context = context;
        }

        //capacity כמה מקום פנוי יש
        //The function goes through all the Approval kits and assigning the students to the apartments
        public async Task<IActionResult> Assigning()
        {
            //Couples ApprovalKit

            List<ApprovalKit> Couples = _context.ApprovalKits.Where(
                a =>
                a.RoomType == RoomType.דירה_זוגית).ToList();

            //Single ApprovalKit
            List<ApprovalKit> Single = _context.ApprovalKits.Where(
                r =>
                r.RoomType == RoomType.חדר_ליחיד &&
                r.HealthCondition == HealthCondition.ללא_מגבלה 
                ).ToList();


            //Accessible ApprovalKit
            List<ApprovalKit> Accessible = _context.ApprovalKits.Where(
                    r =>
                    r.RoomType == RoomType.חדר_ליחיד && (
                    r.HealthCondition == HealthCondition.מגבלה_פיזית_אחרת ||
                    r.HealthCondition == HealthCondition.נכה_צהל ||
                    r.HealthCondition == HealthCondition.נכות)
                    ).ToList();


            //Couples

            foreach (ApprovalKit a in Couples)
            {
                var asaing = await _context.Assigning.SingleOrDefaultAsync(u => u.StundetId.Value == a.StundetId.Value);
                if (asaing == null)
                {
                    //a main student || c main student parnter
                    if (a.PartnerId1 != null)
                    {
                        var c = await _context.ApprovalKits.SingleOrDefaultAsync(m => m.StundetId.Value == a.PartnerId1.Value); //find the partner of the main student
                        if (c != null)
                        {
                            if (c.PartnerId1.Value == a.StundetId.Value)
                            {
                                //get empty apartment
                                var apartment = await _context.Apartments.FirstOrDefaultAsync(m => m.Type.Equals("Couples") && m.capacity == 2);
                                if (apartment != null)
                                {
                                    apartment.capacity = 0;
                                    _context.Update(apartment);
                                    // the user obj of a 
                                    var u1 = await _context.Users.SingleOrDefaultAsync(m => m.StundetId == a.StundetId);
                                    //the user obj of c
                                    var u2 = await _context.Users.SingleOrDefaultAsync(m => m.StundetId == c.StundetId);

                                    Assigning p1 = new Assigning
                                    {
                                        StundetId = a.StundetId.Value,
                                        ApartmentNum = apartment.ApartmentNum,
                                        Room = 1

                                    };
                                    u1.ApartmentNum = apartment.ApartmentNum;
                                    u1.Room = RoomNum.OneA;

                                    _context.Update(u1);
                                    Assigning p2 = new Assigning
                                    {
                                        StundetId = c.StundetId.Value,
                                        ApartmentNum = apartment.ApartmentNum,
                                        Room = 2

                                    };
                                    u2.ApartmentNum = apartment.ApartmentNum;
                                    u2.Room = RoomNum.TwoA;
                                    _context.Update(u2);

                                    _context.Add(p1);
                                    _context.Add(p2);
                                    await _context.SaveChangesAsync();
                                }
                            };
                        };
                        //if there is no partner


                    }
                };
            };
            //Accessible
            foreach (ApprovalKit a in Accessible)
            {
                var asaing = await _context.Assigning.SingleOrDefaultAsync(u => u.StundetId.Value == a.StundetId.Value);
                if (asaing == null)
                {
                    ApprovalKit[] roomies = new ApprovalKit[3];
                    roomies[0] = a;
                    int size = 1;
                    if (a.PartnerId1 != null)
                    {
                        var c = await _context.ApprovalKits.SingleOrDefaultAsync(m => m.StundetId.Value == a.PartnerId1.Value && m.Gender == a.Gender);
                        if (c != null)
                        {
                            if (c.PartnerId1.Value == a.StundetId.Value || c.PartnerId2.Value == a.StundetId.Value)
                            {
                                roomies[1] = c;
                                size++;
                            };
                        };

                    };
                    if (a.PartnerId2 != null)
                    {
                        var c = await _context.ApprovalKits.SingleOrDefaultAsync(m => m.StundetId == a.PartnerId2.Value && m.Gender == a.Gender);
                        if (c != null)
                        {
                            if (c.PartnerId1.Value == a.StundetId.Value || c.PartnerId2.Value == a.StundetId.Value)
                            {
                                roomies[2] = c;
                                size++;
                            };
                        };
                    };
                    //change proprties of apartment
                    var apartment = await _context.Apartments.FirstOrDefaultAsync(m => m.Type.Equals("Accessible") && m.capacity == 3);
                    if (apartment != null)
                    {
                        int c = apartment.capacity.Value;
                        apartment.LivingWithReligious = a.LivingWithReligious;
                        apartment.LivingWithSmoker = a.LivingWithSmoker;
                        apartment.Gender = a.Gender;
                        apartment.capacity = apartment.capacity - size;
                        _context.Update(apartment);
                        await _context.SaveChangesAsync();
                        
                       
                        // save proprties of roomeis
                        foreach (ApprovalKit u in roomies)
                        {
                            if (u != null)
                            {
                                var user = await _context.Users.SingleOrDefaultAsync(m => m.StundetId == u.StundetId.Value);
                                Assigning r = new Assigning
                                {
                                    StundetId = u.StundetId.Value,
                                    ApartmentNum = apartment.ApartmentNum,
                                    Room = c,
                                    
                                };
                                user.ApartmentNum = apartment.ApartmentNum;
                                if (r.Room == 1)
                                {
                                    user.Room = RoomNum.OneA;
                                }
                                else if (r.Room == 2)
                                {
                                    user.Room = RoomNum.TwoA;
                                }
                                else if (r.Room == 3)
                                {
                                    user.Room = RoomNum.ThreeA;
                                }
                                else
                                {
                                    user.Room = RoomNum.FourA;
                                }
                                _context.Update(user);

                                _context.Add(r);
                                await _context.SaveChangesAsync();
                                c--;
                            };
                        };

                    }
                };


            };
            //Single
            foreach (ApprovalKit a in Single)
            {
                var asaing = await _context.Assigning.FirstOrDefaultAsync(x => x.StundetId.Value == a.StundetId.Value);
                if (asaing == null)
                {
                    ApprovalKit[] roomies = new ApprovalKit[4];
                    roomies[0] = a;
                    int size = 1;
                    if (a.PartnerId1 != null)
                    {
                        var c = await _context.ApprovalKits.SingleOrDefaultAsync(m => m.StundetId.Value == a.PartnerId1.Value && m.Gender == a.Gender);
                        if (c != null)
                        {
                            if (c.PartnerId1.Value == a.StundetId.Value || c.PartnerId2.Value == a.StundetId.Value || c.PartnerId3.Value == a.StundetId.Value)
                            {
                                roomies[1] = c;
                                size++;
                            };
                        };

                    };
                    if (a.PartnerId2 != null)
                    {
                        var c = await _context.ApprovalKits.SingleOrDefaultAsync(m => m.StundetId.Value == a.PartnerId2.Value && m.Gender == a.Gender);
                        if (c != null)
                        {
                            if (c.PartnerId1.Value == a.StundetId.Value || c.PartnerId2.Value == a.StundetId.Value || c.PartnerId3.Value == a.StundetId.Value)
                            {
                                roomies[2] = c;
                                size++;
                            }
                        };
                    };
                    if (a.PartnerId3 != null)
                    {
                        var c = await _context.ApprovalKits.SingleOrDefaultAsync(m => m.StundetId == a.PartnerId3.Value && m.Gender == a.Gender);
                        if (c != null)
                        {
                            if (c.PartnerId1.Value == a.StundetId.Value || c.PartnerId2.Value == a.StundetId.Value || c.PartnerId3.Value == a.StundetId.Value)
                            {
                                roomies[3] = c;
                                size++;
                            }
                        }

                    };

                    Apartments apartment = await _context.Apartments.FirstOrDefaultAsync(m => m.capacity >= size &&
                    m.Type.Equals("Single") &&
                         m.LivingWithReligious == a.LivingWithReligious &&
                         m.LivingWithSmoker == a.LivingWithSmoker &&
                         m.ReligiousType == a.ReligiousType &&
                         m.Gender == a.Gender);
                    if (apartment == null)
                    {
                        apartment = _context.Apartments.FirstOrDefault(m=> m.capacity>=size && m.Type.Equals("Single"));
                    };

                    if (apartment != null)
                    {
                        int c = apartment.capacity.Value;
                        apartment.LivingWithReligious = a.LivingWithReligious;
                        apartment.LivingWithSmoker = a.LivingWithSmoker;
                        apartment.Gender = a.Gender;
                        apartment.capacity = apartment.capacity - size;
                        apartment.ReligiousType = a.ReligiousType;
                        _context.Update(apartment);
                        await _context.SaveChangesAsync();
                        foreach (ApprovalKit u in roomies)
                        {
                            
                            if (u != null)
                            {
                                var user = await _context.Users.SingleOrDefaultAsync(m => m.StundetId == u.StundetId);
                                Assigning r = new Assigning
                                {
                                    StundetId = u.StundetId.Value,
                                    ApartmentNum = apartment.ApartmentNum,
                                    Room = c,

                                };
                                user.ApartmentNum = apartment.ApartmentNum;
                                if (r.Room == 1)
                                {
                                    user.Room = RoomNum.OneA;
                                }
                                else if (r.Room == 2)
                                {
                                    user.Room = RoomNum.TwoA;
                                }
                                else if (r.Room == 3)
                                {
                                    user.Room = RoomNum.ThreeA;
                                }
                                else
                                {
                                    user.Room = RoomNum.FourA;
                                }
                                _context.Update(user);
                                _context.Add(r);
                                
                                await _context.SaveChangesAsync();
                                c--;
                            };//end if u!=null
                        };//end foreach roomies

                    }// apartment!=null

                }; //assing==null

            };//foreach approval kit

            //await _context.SaveChangesAsync();
            //ViewBag.NotAssigning = Globals.NotAssigning;

            return RedirectToAction("NotAssigning", "Apartments");
        }

        // return a list of un assining student
        public IActionResult NotAssigning()
        {
            string Aut = HttpContext.Session.GetString("Aut");

            if (!Aut.Equals("2"))
            {
                return RedirectToAction("NotAut", "Home");
            }
            List<ApprovalKit> NotAssigning = new List<ApprovalKit> { };
             List<User> users = _context.Users.Where(r => r.ApartmentNum == null && r.Authorization == 9).ToList();
            foreach (User u in users)
            {
                var item = _context.ApprovalKits.SingleOrDefault(a => a.StundetId.Value == u.StundetId);
                    NotAssigning.Add(item);

            }

            ViewBag.NotAssigning = NotAssigning;
            return View();
        }


    }// close controller
}// close name space


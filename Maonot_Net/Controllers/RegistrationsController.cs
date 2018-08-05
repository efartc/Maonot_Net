﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Maonot_Net.Data;
using Maonot_Net.Models;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Globalization;

namespace Maonot_Net.Controllers
{
    public class RegistrationsController : Controller
    {
        private readonly MaonotNetContext _context;

        public RegistrationsController(MaonotNetContext context)
        {

            _context = context;
        }

        // GET: Registrations
        public IActionResult Index()
        {
            string Aut = HttpContext.Session.GetString("Aut");
            if (Aut.Equals("2"))
            {
                ViewBag.cF = queryFemale().Count();

                ViewBag.cM = queryMale().Count();

                ViewBag.cC = queryCouples().Count();

                return View();
            }
            else
            {
                return RedirectToAction("NotAut", "Home");
            }
        }

        public async Task<IActionResult> Index_Couples(
    string currentFilter,
    string searchString,
    int? page
    )
        {
            string Aut = HttpContext.Session.GetString("Aut");
            if (Aut.Equals("2"))
            {
                ViewBag.type = "Index_Couples";
                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewData["CurrentFilter"] = searchString;

                var reg = queryCouples();
                ViewBag.c = reg.Count();

                if (!String.IsNullOrEmpty(searchString))
                {
                    reg = reg.Where(s => s.LastName.Contains(searchString)
                                           || s.FirstName.Contains(searchString));
                }

                int pageSize = 3;
                return View("viewReg", await PaginatedList<Registration>.CreateAsync(reg.AsNoTracking(), page ?? 1, pageSize));
            }
            else
            {
                return RedirectToAction("NotAut", "Home");
            }
        }

        public async Task<IActionResult> Index_Single_Female(

            string currentFilter,
            string searchString,
            int? page
            )
        {
            string Aut = HttpContext.Session.GetString("Aut");
            if (Aut.Equals("2"))
            {
                ViewBag.type = "Index_Single_Female";
                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewData["CurrentFilter"] = searchString;

                var reg = queryFemale();
                ViewBag.c = reg.Count();

                if (!String.IsNullOrEmpty(searchString))
                {
                    reg = reg.Where(s => s.LastName.Contains(searchString)
                                           || s.FirstName.Contains(searchString));
                }

                int pageSize = 3;
                return View("viewReg", await PaginatedList<Registration>.CreateAsync(reg.AsNoTracking(), page ?? 1, pageSize));
            }
            else
            {
                return RedirectToAction("NotAut", "Home");
            }

        }

        public async Task<IActionResult> Index_Single_Male(
            string currentFilter,
            string searchString,
            int? page
            )

        {
            string Aut = HttpContext.Session.GetString("Aut");
            if (Aut.Equals("2"))
            {
                ViewBag.type = "Index_Single_Male";
                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewData["CurrentFilter"] = searchString;

                var reg = queryMale();
                ViewBag.c = reg.Count();

                if (!String.IsNullOrEmpty(searchString))
                {
                    reg = reg.Where(s => s.LastName.Contains(searchString)
                                           || s.FirstName.Contains(searchString));
                }

                int pageSize = 3;
                return View("viewReg", await PaginatedList<Registration>.CreateAsync(reg.AsNoTracking(), page ?? 1, pageSize));
            }
            else
            {
                return RedirectToAction("NotAut", "Home");
            }

        }

        // GET: Registrations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            string Aut = HttpContext.Session.GetString("Aut");
            string Id = HttpContext.Session.GetString("User");
            var registration = await _context.Registrations.SingleOrDefaultAsync(m => m.ID == id);

            if (Aut.Equals("2") || id.Equals(registration.StundetId.ToString()))
            {
                if (id == null)
                {
                    return NotFound();
                }



                if (registration == null)
                {
                    return NotFound();
                }

                return View(registration);
            }
            return RedirectToAction("NotAut", "Home");



        }

        // GET: Registrations/Create
        public async Task<IActionResult> Create()
        {
            string Id = HttpContext.Session.GetString("User");
            string Aut = HttpContext.Session.GetString("Aut");
            if (Aut == null)
            {
                Aut = "0";
            }

            if (!Aut.Equals("7"))
            {
                return RedirectToAction("NotAut", "Home");
            }
            var u = await _context.Registrations.SingleOrDefaultAsync(m => m.StundetId.ToString().Equals(Id));
            if (u != null)
            {
                return RedirectToAction("ExistsForm", "Home");
            }
            ViewBag.Aut = Aut;

            return View();

        } 

        // POST: Registrations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Bday,gender,City,Adress,PostalCode,PhoneNumber,FieldOfStudy,SteadyYear,TypeOfService,HealthCondition,Seniority,ApertmantType,ParentID1,ParentfullName1,ParentAge1,ParentID2,ParentfullName2,ParentAge2," +
            "Familym1_name,Familym1_Age,Familym2_name,Familym2_Age,Familym3_name,Familym3_Age,Familym4_name,Familym4_Age,Familym5_name,Familym5_Age,Familym6_name,Familym6_Age,Familym7_name,Familym7_Age,Familym8_name,Familym8_Age" +
            "Total,Approved")] Registration registration)
        {
            string Aut = HttpContext.Session.GetString("Aut");
            string Id = HttpContext.Session.GetString("User");
            var u = await _context.Users.SingleOrDefaultAsync(m => m.StundetId.ToString().Equals(Id));

            registration.StundetId = u.StundetId;
            registration.LastName = u.LastName;
            registration.FirstName = u.FirstName;
            
            try
            {
                if (ModelState.IsValid)
                {
                    u.Authorization = 8;

                    _context.Add(registration);
                    _context.Update(u);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Wellcome", "Home");
                }

            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "לא היה ניתן לשמור את השינויים, נא נסה שנית במועד מאוחר יותר");

            }
            return View();
        }

       

        // GET: Registrations/Edit/5
        public async Task<IActionResult> Edit(int? id)

        {
            var functions = new functions();
            if (functions.Comper(new DateTime(2019, 7, 30)))
            {
               
                string Aut = HttpContext.Session.GetString("Aut");
                string Id = HttpContext.Session.GetString("User");
                if (id == null)
                {
                    return NotFound();
                }
                var registration = await _context.Registrations.SingleOrDefaultAsync(m => m.ID == id);

                if (Aut.Equals("2") || Id.Equals(registration.StundetId.ToString()))
                {
                   // var registration = await _context.Registrations.SingleOrDefaultAsync(m => m.ID == id);
                    if (registration == null)
                    {
                        return NotFound();
                    }
                    return View(registration);
                }
                else
                {
                    return RedirectToAction("NotAut", "Home");
                }
            }
            return RedirectToAction("NoMore", "Home");

        }

        // POST: Registrations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,StundetId,LastName,FirstName,Bday,gender,City,Adress,PostalCode,PhoneNumber,FieldOfStudy,SteadyYear,TypeOfService,HealthCondition,Seniority,ApertmantType,ParentID1,ParentfullName1,ParentAge1,ParentID2,ParentfullName2,ParentAge2," +
            "Familym1_name,Familym1_Age,Familym2_name,Familym2_Age,Familym3_name,Familym3_Age,Familym4_name,Familym4_Age,Familym5_name,Familym5_Age,Familym6_name,Familym6_Age,Familym7_name,Familym7_Age,Familym8_name,Familym8_Age" +
            "Total,Approved")] Registration registration)
        {
            
            if (id != registration.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(registration);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegistrationExists(registration.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
                return RedirectToAction(nameof(Index));
            }
            return View(registration);
        }

        // GET: Registrations/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            string Aut = HttpContext.Session.GetString("Aut");
            if (Aut.Equals("2")){

                if (id == null)
                {
                    return NotFound();
                }

                var registration = await _context.Registrations.AsNoTracking()
                    .SingleOrDefaultAsync(m => m.ID == id);
                if (registration == null)
                {
                    return NotFound();
                }
                if (saveChangesError.GetValueOrDefault())
                {
                    ViewData["EErrorMessage"] = "המחיקה נכשלה, נא נסה שנית במועד מאוחד יותר";
                }

                return View(registration);
            }

            return RedirectToAction("NotAut", "Home");

        }

        // POST: Registrations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var registration = await _context.Registrations.AsNoTracking().SingleOrDefaultAsync(m => m.ID == id);
            if (registration == null)
            {
                return RedirectToAction(nameof(Index));
            }
            try
            {
                _context.Registrations.Remove(registration);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Index), new { id = id, saveCahngeError = true });
            }

        }

        public async Task<IActionResult> App(int? id)

        {
            string Aut = HttpContext.Session.GetString("Aut");
            if (Aut.Equals("2"))
            {
                if (id == null)
                {
                    return NotFound();
                }

                var reg = await _context.Registrations.SingleOrDefaultAsync(m => m.ID == id);
                if (reg == null)
                {
                    return NotFound();
                }
                return View(reg);
            }
            return RedirectToAction("NotAut", "Home");

        }
        
        public async Task<ActionResult> Yes(int id)
        {

            var reg = await _context.Registrations.SingleOrDefaultAsync(m => m.ID == id);
            var Sid = reg.StundetId;
            var user = await _context.Users.SingleOrDefaultAsync(m => m.StundetId == Sid);

            if (reg != null)
            {
                reg.Approved = true;
              //  user.Authorization = 9;
                _context.Update(user);
                _context.Update(reg);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Registrations");



            }
            return RedirectToAction("Index", "Home");



        }
        private bool RegistrationExists(int id)
        {
            return _context.Registrations.Any(e => e.ID == id);
        }

        public System.Linq.IQueryable<Maonot_Net.Models.Registration> queryFemale()
        {
            var reg = from s in _context.Registrations
                      orderby s.Total descending
                      where s.gender.Equals(Gender.נקבה) && s.ApertmantType.Equals(ApertmantType.יחיד)
                      select s;
            return reg;
        }
        public System.Linq.IQueryable<Maonot_Net.Models.Registration> queryMale()
        {
            var reg = from s in _context.Registrations
                      orderby s.Total descending
                      where s.gender.Equals(Gender.זכר) && s.ApertmantType.Equals(ApertmantType.יחיד)
                      select s;
            return reg;
        }
        public System.Linq.IQueryable<Maonot_Net.Models.Registration> queryCouples()
        {
            var reg = from s in _context.Registrations
                      orderby s.Total descending
                      where s.ApertmantType.Equals(ApertmantType.זוגי)
                      select s;
            return reg;
        }


    }




}

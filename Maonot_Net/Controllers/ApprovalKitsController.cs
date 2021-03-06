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
    public class ApprovalKitsController : Controller
    {
        private readonly MaonotNetContext _context;

        public ApprovalKitsController(MaonotNetContext context)
        {
            _context = context;
        }
        // GET: ApprovalKits
        //return a list of Approval kits
        
        public async Task<IActionResult> Index(string sortOrder,
            string currentFilter,
            string searchString,
            int? page)
        {
            string Aut = HttpContext.Session.GetString("Aut");
            ViewBag.Aut = Aut;
            if (!Aut.Equals("2"))
            {
                return RedirectToAction("NotAut", "Home");

            }
            else
            {
                ViewData["CurrentSort"] = sortOrder;
                ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewData["CurrentFilter"] = searchString;

                var app = from s in _context.ApprovalKits
                          select s;
                if (!String.IsNullOrEmpty(searchString))
                {
                    app = app.Where(s => s.LastName.Contains(searchString)
                                           || s.FirstName.Contains(searchString));
                };
                switch (sortOrder)
                {
                    case "name_desc":
                        app = app.OrderByDescending(s => s.LastName);
                        break;
                    case "first_name_desc":
                        app = app.OrderByDescending(s => s.FirstName);
                        break;
                    case "first_name":
                        app = app.OrderBy(s => s.FirstName);
                        break;
                    case "room_type_desc":
                        app = app.OrderByDescending(s => s.RoomType);
                        break;
                    case "room_type":
                        app = app.OrderBy(s => s.RoomType);
                        break;

                    default:
                        app = app.OrderBy(s => s.LastName);
                        break;
                };

                int pageSize = 10;
                return View(await PaginatedList<ApprovalKit>.CreateAsync(app.AsNoTracking(), page ?? 1, pageSize));
            }

        }



        // GET: ApprovalKits/Details/5
        //get id of approval kit and return his details
        public async Task<IActionResult> Details(int? id)
        {
            string Aut = HttpContext.Session.GetString("Aut");
            ViewBag.Aut = Aut;
            string Id = HttpContext.Session.GetString("User");
            var approvalKit = await _context.ApprovalKits.SingleOrDefaultAsync(m => m.ID == id);

            if (Aut.Equals("2") || id.Equals(approvalKit.StundetId.ToString()))
            {
                if (id == null)
                {
                    return NotFound();
                }
                if (approvalKit == null)
                {
                    return NotFound();
                }

                return View(approvalKit);
            }
            return RedirectToAction("NotAut", "Home");
        }
        
        // GET: ApprovalKits/Create
        //uplode the view of the form approval kit
        public async Task<IActionResult> Create()
        {
            string Id = HttpContext.Session.GetString("User");
            string Aut = HttpContext.Session.GetString("Aut");
            ViewBag.Aut = Aut;
            if (Aut == null)
            {
                Aut = "0";
            }

            if (!Aut.Equals("9"))
            {
                return RedirectToAction("NotAut", "Home");
            }
            var u = await _context.ApprovalKits.SingleOrDefaultAsync(m => m.StundetId.ToString().Equals(Id));
            if (u != null)
            {
                return RedirectToAction("ExistsForm", "Home");
            }
            ViewBag.Aut = Aut;

            return View();

        }

        // POST: ApprovalKits/Create
        //validate the fields from the approval kit. 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StundetId,LastName,FirstName,RoomType,LivingWithReligious,LivingWithSmoker,ReligiousType,HealthCondition,PartnerId1,PartnerId2,PartnerId3,PartnerId4")] ApprovalKit approvalKit)
        {
            
            
            var functions = new functions();

            if (functions.Comper(new DateTime(2019, 9, 5)))
            {
                string Aut = HttpContext.Session.GetString("Aut");
                ViewBag.Aut = Aut;
                string Id = HttpContext.Session.GetString("User");
                var u = await _context.Registrations.SingleOrDefaultAsync(m => m.StundetId.ToString().Equals(Id));

                approvalKit.FirstName = u.FirstName;
                approvalKit.LastName = u.LastName;
                approvalKit.StundetId = u.StundetId;
                approvalKit.Gender = u.gender;
                try
                {
                    if (ModelState.IsValid)
                    {
                        //u.Authorization = 9;
                        _context.Add(approvalKit);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Wellcome", "Home");
                    }
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "לא היה ניתן לשמור את השינויים, נא נסה שנית במועד מאוחר יותר");

                }
                return View(approvalKit);
            }
            return RedirectToAction("NoMore", "Home");
        }

        // GET: ApprovalKits/Edit/5
        //get id of approval kit and reurnt is details to edit form
        public async Task<IActionResult> Edit(int? id)
        {
            var functions = new functions();
            if (functions.Comper(new DateTime(2019, 9, 5)))
            {
                string Aut = HttpContext.Session.GetString("Aut");
                ViewBag.Aut = Aut;
                string Id = HttpContext.Session.GetString("User");
                if (id == null)
                {
                    return NotFound();
                }
                
                var approvalKit = await _context.ApprovalKits.SingleOrDefaultAsync(m => m.ID == id);

                if (Aut.Equals("2") || Id.Equals(approvalKit.StundetId.ToString()))
                {
                    //var approvalKit = await _context.ApprovalKits.SingleOrDefaultAsync(m => m.ID == id);
                    if (approvalKit == null)
                    {
                        return NotFound();
                    }
                    return View(approvalKit);
                }
                return RedirectToAction("NotAut", "Home");

            }
            return RedirectToAction("NoMore", "Home");
        }  

        // POST: ApprovalKits/Edit/5
        //save the cahnges in the approval kit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,StundetId,LastName,FirstName,RoomType,LivingWithReligious,LivingWithSmoker,ReligiousType,HealthCondition,PartnerId1,PartnerId2,PartnerId3,PartnerId4")] ApprovalKit approvalKit)
        {
            string Aut = HttpContext.Session.GetString("Aut");
            ViewBag.Aut = Aut;
            if (id != approvalKit.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(approvalKit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApprovalKitExists(approvalKit.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (Aut.Equals("2"))
                {
                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction("Wellcome", "Home");
            }
            return View(approvalKit);
        }

        // GET: ApprovalKits/Delete/5
        // get the details of the approval kit and ask the user if he is sure
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            string Aut = HttpContext.Session.GetString("Aut");
            ViewBag.Aut = Aut;
            if (Aut.Equals("2"))
            {
                if (id == null)
                {
                    return NotFound();
                }

                var approvalKit = await _context.ApprovalKits.AsNoTracking()
                    .SingleOrDefaultAsync(m => m.ID == id);
                if (approvalKit == null)
                {
                    return NotFound();
                }
                if (saveChangesError.GetValueOrDefault())
                {
                    ViewData["EErrorMessage"] = "המחיקה נכשלה, נא נסה שנית במועד מאוחד יותר";
                }

                return View(approvalKit);
            }
            return RedirectToAction("NotAut", "Home");

        }

        // POST: ApprovalKits/Delete/5
        // if the user confirem the delete this function start and delete the record from the DB
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string Aut = HttpContext.Session.GetString("Aut");
            ViewBag.Aut = Aut;
            var approvalKit = await _context.ApprovalKits.AsNoTracking().SingleOrDefaultAsync(m => m.ID == id);
            if (approvalKit == null)
            {
                return RedirectToAction(nameof(Index));
            }
            try
            {
                _context.ApprovalKits.Remove(approvalKit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Index), new { id = id, saveCahngeError = true });
            }

        }

        private bool ApprovalKitExists(int id)
        {
            return _context.ApprovalKits.Any(e => e.ID == id);
        }
    }
}

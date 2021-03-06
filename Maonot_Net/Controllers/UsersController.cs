﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Maonot_Net.Data;
using Maonot_Net.Models;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using BCrypt.Net;
using Microsoft.AspNetCore.Http;

namespace Maonot_Net.Controllers
{

    public class UsersController : Controller
    {
        private readonly MaonotNetContext _context;

        public UsersController(MaonotNetContext context)
        {
            _context = context;
        }

        //return a list of users
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? page)
        {
            string Aut = HttpContext.Session.GetString("Aut");
          
            ViewBag.bbb = Aut;
          
            if (Aut.Equals("1"))
            {
                ViewData["CurrentSort"] = sortOrder;
                ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewBag.session = HttpContext.Session.GetString("User");

                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }
                ViewData["CurrentFilter"] = searchString;

                var users = from s in _context.Users
                            select s;
                if (!String.IsNullOrEmpty(searchString))
                {
                    users = users.Where(s => s.LastName.Contains(searchString)
                                           || s.FirstName.Contains(searchString));
                }
                switch (sortOrder)
                {
                    case "name_desc":
                        users = users.OrderByDescending(u => u.LastName);
                        break;
                    case "fname_desc":
                        users = users.OrderByDescending(u => u.FirstName);
                        break;
                    case "fname":
                        users = users.OrderBy(u => u.FirstName);
                        break;
                    default:
                        users = users.OrderBy(U => U.LastName);
                        break;
                }
                int pageSize = 10;

                return View(await PaginatedList<User>.CreateAsync(users.AsNoTracking(), page ?? 1, pageSize));
            }
            else
            {
               
                return RedirectToAction("NotAut", "Home");
            }

        }
        //return the details of a record by id

        public async Task<IActionResult> Details(int? id)
        {
            string s = HttpContext.Session.GetString("User");
            string Aut = HttpContext.Session.GetString("Aut");
           

            if (s != null)
            {
                var _user = await _context.Users.SingleOrDefaultAsync(m => m.StundetId.ToString().Equals(s));
                
                ViewBag.Aut = Aut;

                if (Aut.Equals("1") || _user.ID == id)
                {
               
                    if (id == null)
                    {
                        return NotFound();
                    }

                    var user = await _context.Users
                        .SingleOrDefaultAsync(m => m.ID == id);
                    if (user == null)
                    {
                        return NotFound();
                    }

                    return View(user);
                }
                else if (Aut.Equals("0"))
                {
                    return RedirectToAction("NotAut", "Home");
                }
                else
                {
                   
                    return RedirectToAction("NotAut", "Home");
                }
            }
            else
            {
                return RedirectToAction("NotAut", "Home");
            }

        }
        //return the User form
        public IActionResult Create()
        {
            ViewData["Authorization"] = new SelectList(_context.Authorizations, "Id", "AutName");
            string Aut = HttpContext.Session.GetString("Aut");
            if (Aut == null)
            {
                ViewBag.Aut = "0";
            }
            else
            {
                ViewBag.Aut = Aut;
                
            }
            return View();


        }

        // POST: Users/Create
        //validate the fields from the user and  open session
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StundetId,FirstName,LastName,Password,Email,ApartmentNum,Room, Authorization")] User user)
        {
            ViewData["Authorization"] = new SelectList(_context.Authorizations, "Id", "AutName");
            string Aut = HttpContext.Session.GetString("Aut");
            if (Aut == null)
            {
                Aut = "0";
            }

            try
            {
                if (user.Authorization == 0)
                { user.Authorization = 7; }
                
                if (ModelState.IsValid)
                {
                    Boolean u = _context.Users.Any(e => e.StundetId == user.StundetId);
                    if (u==false) { 
                    user.Password =
                         BCrypt.Net.BCrypt.HashPassword(user.Password);
                    
                        _context.Add(user);
                        await _context.SaveChangesAsync();
                        

                        if (Aut.Equals("1"))
                        {
                            ViewBag.Aut = Aut;
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            string s = user.StundetId.ToString();
                            HttpContext.Session.SetString("User", s);
                            string a = user.Authorization.ToString();
                            HttpContext.Session.SetString("Aut", a);
                            ViewBag.Aut = Aut;
                            return RedirectToAction("Wellcome", "Home");
                        }
                    }
                    else
                    {
                        TempData["msg"] = "<script>alert('משתמש קיים במערכת');</script>";
                        return View(user);
                    }
                }
            }
            catch(DbUpdateException)
            {
                ModelState.AddModelError("", "לא היה ניתן לשמור את השינויים, נא נסה שנית במועד מאוחר יותר");

            }
            return View(user);
        }

        // GET: Users/Edit/5
        //return the details of a record by id to a edit form
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["Authorization"] = new SelectList(_context.Authorizations, "Id", "AutName");
            string ID = HttpContext.Session.GetString("User");
            string Aut = HttpContext.Session.GetString("Aut");
            ViewBag.Aut = Aut;
            ViewBag.Aut = Aut;
            if (!Aut.Equals("0"))
            {
                if (id == null)
                {
                    return NotFound();
                }

                var user = await _context.Users.SingleOrDefaultAsync(m => m.ID == id);
                if (user == null)
                {
                    return NotFound();
                }
                if (ID.Equals(user.StundetId.ToString())|| Aut.Equals("1"))
                {
                    return View(user);
                }
                return RedirectToAction("NotAut", "Home");
            }

            else
            {
                return RedirectToAction("NotAut", "Home");
            }
        }

        // POST: Users/Edit/5
        //save the cahnges of the record
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,StundetId,FirstName,LastName,Password,Email,ApartmentNum,Room,Authorization")] User user)
        {
            string Aut = HttpContext.Session.GetString("Aut");
            ViewBag.Aut = Aut;
            if (id != user.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                   
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (Aut.Equals("1"))
                {
                    return RedirectToAction(nameof(Index));
                }
                return RedirectToAction("Wellcome", "Home");

            }
            return View(user);
        }

        // GET: Users/Delete/5
        // get the details of the recoerd and ask the user if he is sure
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            string Aut = HttpContext.Session.GetString("Aut");
            string ID = HttpContext.Session.GetString("User");

            
            if (Aut.Equals("1"))
            {
                if (id == null)
                {
                    return NotFound();
                }

                var user = await _context.Users.AsNoTracking()
                    .SingleOrDefaultAsync(m => m.ID == id);
                if (user == null)
                {
                    return NotFound();
                }
                if (saveChangesError.GetValueOrDefault())
                {
                    ViewData["EErrorMessage"] = "המחיקה נכשלה, נא נסה שנית במועד מאוחד יותר";
                }

                return View(user);
            }
            return RedirectToAction("NotAut", "Home");
        }

        // POST: Users/Delete/5
        // if the user confirem the delete this function start and delete the recore from the DB
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.AsNoTracking().SingleOrDefaultAsync(m => m.ID == id);
            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }
            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Index), new { id = id, saveCahngeError = true });
            }

        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.ID == id);
        }

    //return the LogIn view
        public IActionResult LogIn()
        {

            return View();
        }

        // //validate the log in form and open session
        [HttpPost, ActionName("LogIn")]
        public IActionResult LogIn(User _user)
        {
            var functions = new functions();
            var user = _context.Users.SingleOrDefault(u => u.StundetId == _user.StundetId);
            if (user != null)
            {
                if (functions.CheckPassword(_user.Password, user.Password))
                {
                    string s = _user.StundetId.ToString();
                    string a = user.Authorization.ToString();
                    HttpContext.Session.SetString("Aut", a);
                    HttpContext.Session.SetString("User", s);
                    
                    return RedirectToAction("Wellcome", "Home");
                }
                else
                {
                  ViewBag.Message = "Thank you!";
                  TempData["msg2"] = "<script>alert('סיסמה שגויה');</script>";
                    
                   
                }
            }
            else
            {
                TempData["msg1"] = "<script>alert('ת.ז לא נמצאה במערכת');</script>";
               
            }
            return View();

           
          
        }
// close session and return the main page 
        public ActionResult LogOut()
        {
            HttpContext.Session.Remove("User");
            return RedirectToAction("Index", "Home");

        }



    }




    }


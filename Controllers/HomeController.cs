using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;
        public HomeController(MyContext context)
        {
            _context = context;
        }
        
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        
        [HttpPost]
        public IActionResult Register(User newUser)
        {
            if (ModelState.IsValid){
                if (_context.Users.Any(user => user.Email == newUser.Email))
                { 
                    ModelState.AddModelError("Email", "Email already in use!");

                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    newUser.Password = Hasher.HashPassword(newUser, newUser.Password);

                    _context.Users.Add(newUser);
                    _context.SaveChanges();

            }
            return View("Index");
        }

        [HttpPost("checklogin")]
        public IActionResult CheckLogin(LoginUser login)
        {
            if(ModelState.IsValid){
                User userInDb = _context.Users.FirstOrDefault(user => user.Email == login.LoginEmail);
                
                if(userInDb == null){
                    ModelState.AddModelError("LoginEmail", 
                        "Invalid Login!  Please check your login credentials and retry.");
                    
                    return View("Index");
                }

                PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();

                var result = hasher.VerifyHashedPassword(login, userInDb.Password, login.LoginPassword);

                if (result == 0)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid Login!");

                    return View("Index");
                }

                HttpContext.Session.SetInt32("userId", userInDb.UserId);

                return RedirectToAction("Success");
            }

            return View("Index");
        }

        [HttpGet("success")]
        public IActionResult Success()
        {
            int? userID = HttpContext.Session.GetInt32("userId");
            if (!(userID > 0)){
                return View("Index");
            }

            return RedirectToAction("Dashboard");
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            int? userID = HttpContext.Session.GetInt32("userId");
            if (!(userID > 0)){
                return View("Index");
            }

            ViewBag.myCount = _context.Weddings.Count();

            if (ViewBag.myCount > 0)
            {
                ViewBag.User = _context.Users
                    .Where(u => u.UserId == (int)userID)
                    .FirstOrDefault();

                ViewBag.loggedUserId = (int)userID;

                ViewBag.Weddings = _context.Weddings
                    .Where(wedp => wedp.WedNParticipants.Any(wp => wp.UserId == (int) userID
                    && wp.UserTypeId == 3))
                    .ToList();

                ViewBag.Weddings = _context.Weddings
                    .Include(wed => wed.WedNParticipants)
                    .ThenInclude(wed => wed.User)
                    .ToList();
            }

            return View();
        }

        [HttpGet("display/{weddingId}")]
        public IActionResult Display(int weddingId)
        {
            int? userID = HttpContext.Session.GetInt32("userId");
            if (!(userID > 0)){
                return View("Index");
            }

            ViewBag.Weddings = _context.Weddings
                    .Include(wnp => wnp.WedNParticipants)
                    .Where(wed => wed.WeddingDate > DateTime.Now)
                    .OrderBy(orderPlanner => orderPlanner.WeddingDate)
                    .First(wed => wed.WeddingId == weddingId);

            ViewBag.Guests = _context.Users
                .Where(wedp => wedp.WedNParticipants.Any(wp => wp.WeddingId == weddingId))
                .ToList();

            return View();
        }


        [HttpGet("planwedding")]
        public IActionResult PlanWedding()
        {
            int? userID = HttpContext.Session.GetInt32("userId");
            if (!(userID > 0)){
                return View("Index");
            }

            return View();
        }

        [HttpPost("createwedding")]
        public IActionResult CreateWedding(Wedding newWedding)
        {
            int? userID = HttpContext.Session.GetInt32("userId");
            if (!(userID > 0)){
                return View("Index");
            }

            if (ModelState.IsValid)
            {
                newWedding.UserId = (int)userID;
                _context.Add(newWedding);
                _context.SaveChanges();

                WeddingParticipant wd = new WeddingParticipant();
                wd.WeddingId = newWedding.WeddingId;
                wd.UserId = (int) userID;
                wd.UserTypeId = 1;
                _context.Add(wd);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }

            return View("PlanWedding");
        }

        [HttpGet("setrsvp/{userTypeId}/{weddingId}")]
        public IActionResult SetRSVP(int userTypeId, int weddingId)
        {
            int? userID = HttpContext.Session.GetInt32("userId");
            if (!(userID > 0)){
                return View("Index");
            }

            if (userTypeId == 2)
            {
                WeddingParticipant wp = new WeddingParticipant();
                wp.UserId = (int)userID;
                wp.UserTypeId = userTypeId;
                wp.WeddingId = weddingId;
                _context.Add(wp);
                _context.SaveChanges();
                
            }
            else
            {
                var delobj = _context.WeddingParticipants
                    .Where(p => p.UserId == (int) userID
                    && p.WeddingId == weddingId)
                    .SingleOrDefault();

                if (delobj != null)
                {
                    _context.Remove(delobj);
                    _context.SaveChanges();
                }
            }
            
            

            return RedirectToAction("Dashboard");
        }

        [HttpGet("deletewedding/{weddingId}")]
        public IActionResult DeleteWedding(int weddingId)
        {
            int? userID = HttpContext.Session.GetInt32("userId");
            if (!(userID > 0)){                                                                                                                                                                                                                                
                return View("Index");
            }

            Wedding DeleteThisWedding = _context.Weddings
                .FirstOrDefault(wed => wed.WeddingId == weddingId);

            _context.Remove(DeleteThisWedding);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

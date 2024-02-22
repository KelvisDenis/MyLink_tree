using Microsoft.AspNetCore.Mvc;
using MySite.Models;
using MySite.Models.OutherModels;
using MySite.Services;

namespace MySite.Controllers
{
    public class UserController : Controller
    {
        private readonly UserServices _userServices;

        public UserController(UserServices userServices)
        {
            _userServices = userServices;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(int id, User user)
        {
            var again = new FormLogin { User = user };
            if (user == null) { 
                return View();
            }
            var create = _userServices.GetUserAsync(user.Email);
            if (create.Result == null) {
                return View(again);
            }
                return View("Index");
        }
             
        
        [HttpPost]
        public IActionResult Create(User user)
        {

            if (user == null)
            {
                return View("Login");
            }
            if (ModelState.IsValid)
            {

                var create = _userServices.AddAsync(user);
                return View("Index");
            }
            var again = new FormLogin { User = user };
            return View("Login", again);
        }
        public IActionResult Create()
        {
            return View();
        }
    }
}

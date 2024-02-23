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
        [ValidateAntiForgeryToken]

        public IActionResult Login(User user)
        {
            var again = new FormLogin { User = user };
            if (user.Email == null || user.Password == null) { 
                return View();
            }
            var create = _userServices.GetUserAsync(user.Email);
            if (create.Result == null) {
                return View(again);
            }
            var teste = create.Result.checkpassword(user.Email, user.Password);
            if (teste)
            {
                return View("Links");
            }
            return View(again);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Create(User user)
        {

            if (user.Email == null || user.Password == null || user.UserName == null)
            {
                return View();
            }
            if (ModelState.IsValid)
            {

                var create = _userServices.AddAsync(user);
                return View("Links");
            }
            var again = new FormLogin { User = user };
            return View("Login", again);
        }
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Links()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Links(User user)
        {
            if (user.Links == null) return View();

            var links = _userServices.AddLinksAsync(user);
            return View("Index");
        }
    }
}

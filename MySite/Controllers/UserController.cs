using Microsoft.AspNetCore.Mvc;
using MySite.Models;
using MySite.Models.OutherModels;
using MySite.Services;
using System.ComponentModel;

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
                ViewData["Email"] = create.Result.Email;

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
            List<Links> list = new List<Links>();
            return View(list);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Links(FormLogin form)
        {
            List<Links> list = new List<Links>();
            if (ModelState == null) return View();
            string[] x = ModelState["Link.Url"].AttemptedValue.Split(",");
            foreach (var item in x )
            {
                form.Link.Url = item;
                 _userServices.AddLinksAsync(form.Link);
            }
            /*foreach (var item in list)
            {
                form.User.Add(item);
            }
            var links = _userServices.AddLinksAsync(form.Link);
            */
            return View("Index", form.User);
        }
    }
}

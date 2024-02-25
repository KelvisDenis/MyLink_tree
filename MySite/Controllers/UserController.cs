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

        [TempData]
        public string Email { get; set; }

        [TempData]
        public string UserName { get; set; }

        [TempData]
        public string[] URL { get; set; }

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
            // Recriar o formulario
            var again = new FormLogin { User = user };
            // Se o form for nulo
            if (user.Email == null || user.Password == null) { 
                return View();
            }
            // busca no banco se o usuario existe
            var create = _userServices.GetUserAsync(user.Email);
            // se o retorno for nulo 
            if (create.Result == null) {
                return View(again);
            }
            // testa as senhas
            var teste = create.Result.checkpassword(user.Email, user.Password);
            // se o for verdadeiro retorna a pagina de criar links
            if (teste)
            {
                Email = $"{create.Result.Email}";

                return View("Links");
            }
            // se não retorna para o formulario
            return View(again);

        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Create(User user)
        {
            // testa se o model for nulo return para o form
            if (user.Email == null || user.Password == null || user.UserName == null)
            {
                return View();
            }
            // se o modelo for valido ele criar o user
            if (ModelState.IsValid)
            {

                var create = _userServices.AddAsync(user);
                //Email = $"{user.Email}";
                ViewBag.User = user.Email + user.UserName;
                UserName = $"{ user.UserName}";
                return View("Links");
            }
            var again = new FormLogin { User = user };
            return View("Login", again);
        }
     

        public IActionResult Links()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Links(FormLogin form)
        {
            List<Links> list = new List<Links>();
            if (ModelState == null) return View();
            string[] x = ModelState["Link.Url"].AttemptedValue.Split(",");
            URL = x;
            ViewBag.Url = x;
            foreach (var item in x )
            {
                form.Link.Url = item;
                 _userServices.AddLinksAsync(form.Link);
            }
         
            return View("Index", form.User);
        }
    }
}

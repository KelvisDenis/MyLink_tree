using Microsoft.AspNetCore.Mvc;
using MySite.Models;
using MySite.Models.OutherModels;
using MySite.Services;
using System.Collections.Generic;
using System.ComponentModel;

namespace MySite.Controllers
{
    public class UserController : Controller
    {
        private readonly UserServices _userServices;
        public const string SessionKeyName = "_Name";
        private readonly ILogger<UserController> _logger;

   

        public UserController(UserServices userServices, ILogger<UserController> logger)
        {
            _userServices = userServices;
            _logger = logger;
        }

        public IActionResult Index(User user)
        {
            var obj = _userServices.GetUserAsync(user);
            return View(obj);
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
            var create = _userServices.GetUserAsync(user);
           
            // testa as senhas
            var teste = create.Result.checkpassword(user.Email, user.Password);

            // se o for verdadeiro retorna a pagina de criar links
            if (teste)
            {
               
                    HttpContext.Session.SetString("Email", create.Result.Email);
                    HttpContext.Session.SetString("UserName", create.Result.UserName);

                    var Email = HttpContext.Session.GetString("Email");
                    var User = HttpContext.Session.GetString("UserName");
                    _logger.LogInformation("Session email: ", Email);
                    _logger.LogInformation("Session email: ", User);
                
            
                return View("Index", create.Result);
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

                //var create = _userServices.AddAsync(user);
                if (string.IsNullOrEmpty(HttpContext.Session.GetString(SessionKeyName)))
                {
                    HttpContext.Session.SetString("Email", user.Email);
                    HttpContext.Session.SetString("Pass", user.Password);
                    HttpContext.Session.SetString("UserName", user.UserName);
                    // cria uma sessão
                    var Email = HttpContext.Session.GetString("Email");
                    var User = HttpContext.Session.GetString("UserName");
                    var Senha = HttpContext.Session.GetString("Pass");

                    _logger.LogInformation("Session email: ", Email);
                    _logger.LogInformation("Session usernme: ", User);
                    _logger.LogInformation("Session senha: ", Senha);

                }
                // Cria um obl FormLogin pra mandar pra view links
                FormLogin formLogin = new FormLogin();
                formLogin.User = user;
                return View("Links", formLogin);
            }
            // se modelo enviado for invalid reenvia pro mesmo form
            var again = new FormLogin { User = user };
            return View("Create", again);
        }
     

        public async Task<IActionResult> Links()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Links(FormLogin form)
        {
            // Testa se modelo é valido
            if (ModelState == null) return View();
            // se sim cria uma lista de urls com os dados mandados 
            string[] listLinks = ModelState["Link.Url"].AttemptedValue.Split(",");

            // cria um obj user e adiciona sessão a ele
            form.User = new User ();
            form.User.Email = HttpContext.Session.GetString("Email");
            form.User.UserName = HttpContext.Session.GetString("UserName");
            form.User.Password = HttpContext.Session.GetString("Pass");
            form.User.Urls = listLinks;

            // cria links no bd
            foreach (var item in listLinks )
            {
                form.Link.Url = item;
                _userServices.AddLinksAsync(form.Link);
            }
            // criar de fato o user e manda pra pagina index
             _userServices.AddAsync(form.User);

            return View("Index", form.User);
        }
        
    }
}

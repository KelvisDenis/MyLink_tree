using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using MySite.Models;
using MySite.Models.OutherModels;
using MySite.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Serialization;

namespace MySite.Controllers
{
    public class UserController : Controller
    {
        private readonly UserServices _userServices;
        private readonly ILogger<UserController> _logger;

   

        public UserController(UserServices userServices, ILogger<UserController> logger)
        {
            _userServices = userServices;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var serch = HttpContext.Session.GetString("Sessao");
            var session = JsonConvert.DeserializeObject<User>(serch);
            var user1 = _userServices.GetUserAsync(session);
            return View("ListagemLinks",user1.Result);
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
           
            // testa se o usuario existe
            if (create.Result == null) return View(again);

            // testa as senhas
            var teste = create.Result.checkpassword(user.Email, user.Password);

            // se o for verdadeiro retorna a pagina de criar links
            if (teste)
            {
                // cria session do usuario 
                var sessao = JsonConvert.SerializeObject(user);
                HttpContext.Session.SetString("Sessao", sessao);
             

                   
                    _logger.LogInformation("Session email: ", sessao);
                
                // retorna view index passando um model
                return View("ListagemLinks", create.Result);
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
                    var sessao = JsonConvert.SerializeObject(user);
                    HttpContext.Session.SetString("Sessao", sessao);
                   
                    _logger.LogInformation("Session email: ", sessao);
                   
                
                // Cria um obj FormLogin pra mandar pra view links
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
            var serch = HttpContext.Session.GetString("Sessao");
            var obj = JsonConvert.DeserializeObject<User>(serch);
            form.User = obj;
            form.User.Urls = listLinks;
            var verif = _userServices.GetUserAsync(form.User);
            if (verif.Result == null) {
                _userServices.AddAsync(form.User);
                // cria links no bd
                foreach (var item in listLinks)
                {
                    form.Link.Url = item;
                    _userServices.AddLinksAsync(form.Link);
                }
                // criar de fato o user e manda pra pagina index
           
                return View("Index", form.User);
            }
            _userServices.Update(form.User);
            var sessao = JsonConvert.SerializeObject(form.User);
            HttpContext.Session.SetString("Sessao", sessao);

            _logger.LogInformation("Session email: ", sessao);
            return View("Index", form.User);
        }
        public async Task<IActionResult> ListagemLinks()
        {
            // create session user 
            User obj = new User();
            var serch = HttpContext.Session.GetString("Sessao");
            obj = JsonConvert.DeserializeObject<User>(serch);
            var user = _userServices.GetUserAsync(obj);
            return View(user.Result);
        }
        public async Task<IActionResult> Skip()
        {
            // create session user 
            User obj = new User();
            var serch = HttpContext.Session.GetString("Sessao");
            obj = JsonConvert.DeserializeObject<User>(serch);
            var user = _userServices.GetUserAsync(obj);
            if(user.Result == null) _userServices.AddAsync(obj);
            return View("ListagemLinks",obj);
        }


        // Editar os links 
        public async Task<IActionResult> Edit(int? id, string? list)
        {
            var user = _userServices.GetUseridsAsync(id);
            ViewBag.LinkEdit = list;
    
            return View("Edit",user.Result);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, User user, string? Url)
        {
            string x = ViewBag.Url;
        if (user == null || user.Id != id)  return View("Edit",user);
        if (ModelState.IsValid)
            {
                var serch = HttpContext.Session.GetString("Sessao");
                var session = JsonConvert.DeserializeObject<User>(serch);
                var obj = _userServices.GetUserAsync(session);
               
                for (var i = 0; i < obj.Result.Urls.Length; i++)
                {
                    if (obj.Result.Urls[i] == Url) obj.Result.Urls[i] = user.Urls[0];
                }
                
                await _userServices.Update(obj.Result);
                return View("Index", obj.Result);

            }
            return View();

        }
        // Delete links 
        public IActionResult Delete(int? id, string? list )
        {
           
            if (list == null) return View("ListagemLinks");
            var serch = HttpContext.Session.GetString("Sessao");
            var session = JsonConvert.DeserializeObject<User>(serch);
            var obj = _userServices.GetUserAsync(session);
            ViewBag.LinkExclud = list;
            return View("Delete", obj.Result);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id, User user)
        {
            if (user == null || user.Id != id) return View("Edit", user);
            if (ModelState.IsValid)
            {
                var obj = _userServices.GetUseridsAsync(id);
                for (int c=0; c<obj.Result.Urls.Length; c++)
                {
                    if (obj.Result.Urls[c] == user.Urls[0]) obj.Result.Urls[c] = null;
                }
                await _userServices.Update(obj.Result);
                return RedirectToAction("ListagemLinks", obj.Result);
            }
            return View();
        }
        // criar links do user
        public IActionResult Update(int? id)
        {
            var user = _userServices.GetUseridsAsync(id);
            return View("Update", user.Result);
        }
        // criar links do user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, string Urls)
        {
            var obj = _userServices.GetUseridsAsync(id);
            List<string> list = new List<string>() ;
            var teste = (obj.Result.Urls != null);
            if (teste )
            {
                if (obj.Result.Urls.Length <= 4)
                {
                    for (int i = 0; i < obj.Result.Urls.Length; i++)
                    {
                        if (obj.Result.Urls[i] == null) list.Add(Urls);
                        else
                        {
                            list.Add(obj.Result.Urls[i]);
                        }
                    }
                    if (!list.Contains(Urls)) list.Add(Urls);
                    string[] strings = new string[list.Count];
                    for (int i = 0; i < list.Count; i++)
                    {
                        strings[i] = list[i];
                    }
                    obj.Result.Urls = strings;
                    await _userServices.Update(obj.Result);
                    return View("ListagemLinks", obj.Result);
                }
               
            }
            else if(obj.Result.Urls == null)
            {
                string[] strings = new string[] { Urls};
                obj.Result.Urls = strings ;
                await _userServices.Update(obj.Result);
                return View("ListagemLinks", obj.Result);
            }
            return View("Update", obj.Result);
        }
       
    }
}

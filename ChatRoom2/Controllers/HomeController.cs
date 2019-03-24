using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace ChatRoom2.Controllers
{
    public class HomeController : Controller
    {
        private Database.UserDatabaseController UserDBController = DependencyResolver.Current.GetService<Database.UserDatabaseController>();

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "ChatRoom");
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View("Register");
        }

        [HttpPost]
        [AllowAnonymous]
        public string Login()
        {
            string username = Request.Headers["username"];
            string password = HashPass(Request.Headers["password"]);

            bool ValidUser = UserDBController.ValidateUser(new Models.UserModel
            {
                UserName = username,
                PasswordHash = password
            });

            if (ValidUser)
            {
                FormsAuthentication.SetAuthCookie(username, true);
                return this.Url.Action("Index", "ChatRoom");
            }
            return this.Url.Action("Index", "Home");
        }

        [AllowAnonymous]
        public bool UserExists(string username)
        {
            return UserDBController.GetAllUsers().Any(e => e.UserName.ToLower() == username.ToLower());
        }

        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        public string RegisterUser(string avatar)
        {
            string username = Request.Headers["username"];
            string password = HashPass(Request.Headers["password"]);
            //check if username already exists
            var user = UserDBController.GetUser(username);
            if (user == null)
            {
                bool success = UserDBController.RegisterUser(new Models.UserModel
                {
                    UserId = Guid.NewGuid(),
                    UserName = username,
                    PasswordHash = password,
                    AvatarUrl = avatar
                });

                         if (success)
                {
                    FormsAuthentication.SetAuthCookie(username, true);
                    return Url.Action("Index", "ChatRoom");
                }
            }
            return Url.Action("Register", "Home");
        }

        public JsonResult UserAutoComplete(String term)
        {
            List<Models.UserModel> userList = UserDBController.GetAllUsers().Where(e => e.UserName.ToLower() != User.Identity.Name.ToLower()).ToList();
            return new JsonResult { Data = userList.Where(e => e.UserName.ToLower().Contains(term.ToLower())).ToList(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        
        public string HashPass(string pass)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                UTF8Encoding utf8 = new UTF8Encoding();
                byte[] data = md5.ComputeHash(utf8.GetBytes(pass));
                return Convert.ToBase64String(data);
            }
        }
    }
}

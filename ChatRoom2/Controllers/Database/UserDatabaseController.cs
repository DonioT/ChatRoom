using ChatRoom2.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChatRoom2.Controllers.Database
{
    public class UserDatabaseController : Controller
    {
        // GET: UserDatabase
        public Models.UserModel GetUser(string userName)
        {
            Models.UserModel model = null;
            using (var db = new ChatDBContext())
            {
                try
                {
                    model = db.Users.Where(e => e.UserName.ToLower() == userName.ToLower()).FirstOrDefault();
                }
                catch (Exception e)
                {

                }
            }
            return model;
        }

        public List<Models.UserModel> GetAllUsers()
        {
            using (var db = new ChatDBContext())
            {
                try
                {
                    return db.Users.ToList();
                }catch(Exception e)
                {
                    return null;
                }
            }
        }

        public Models.UserModel GetUser(Guid userId)
        {
            using (var db = new ChatDBContext())
            {
                try
                {
                    return db.Users.FirstOrDefault(e => e.UserId == userId);
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public bool ValidateUser(Models.UserModel model)
        {
            bool success = false;

            using (var db = new ChatDBContext())
            {
                try
                {
                    success = db.Users.Any(x => x.UserName == model.UserName && x.PasswordHash == model.PasswordHash);

                }
                catch (Exception e)
                {
                    success = false;
                }
            }
            return success;
        }

        public bool UpdateUser(Models.UserModel model)
        {
            using (var db = new ChatDBContext())
            {
                try
                {
                    Models.UserModel dbModel = db.Users.FirstOrDefault(x => x.UserId == model.UserId);
                    dbModel.AvatarUrl = model.AvatarUrl;
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }

        public bool RegisterUser(Models.UserModel model)
        {
            bool success = false;
            using (var db = new ChatDBContext())
            {
                try
                {
                    model.CreatedOn = DateTime.Now;
                    db.Users.Add(model);
                    db.SaveChanges();
                    success = true;
                }
                catch (Exception e)
                {
                    success = false;
                }
            }
            return success;
        }
    }
}
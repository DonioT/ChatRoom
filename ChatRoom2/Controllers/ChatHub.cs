using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ChatRoom2.Controllers
{
    public class ChatHub : Hub
    {
        private Database.UserDatabaseController UserDBController = DependencyResolver.Current.GetService<Database.UserDatabaseController>();
        private Database.ChatDatabaseController ChatDBController = DependencyResolver.Current.GetService<Database.ChatDatabaseController>();
        private static readonly ConcurrentDictionary<string, Models.ChatUserModel> Users = new ConcurrentDictionary<string, Models.ChatUserModel>(StringComparer.OrdinalIgnoreCase);

       public void Send(string message)
         {
             // Call the addNewMessageToPage method to update clients.
             Clients.All.addNewMessageToPage(Context.User.Identity.Name, message, DateTime.Now.ToShortTimeString());
         }
   

        public ConcurrentDictionary<string, Models.ChatUserModel> GetAllActiveConnections()
        {
            return Users;
        }

        private Models.ChatUserModel GetUser(string username)
        {

            Models.ChatUserModel user;
            Users.TryGetValue(username, out user);

            return user;
        }


        public void Send(string message, Guid groupId)
        {
            if (groupId != null)
            {
                Models.ChatUserModel sender = GetUser(Context.User.Identity.Name);
                var groupMembers = ChatDBController.GetGroupMembers(groupId).ToList();
                foreach (var member in groupMembers)
                {
                    Models.ChatUserModel receiver;
                    if (Users.TryGetValue(member.User.UserName, out receiver))
                    {
                        foreach(var cid in receiver.ConnectionIds)
                        {
                            Clients.Client(cid).ReceivePrivateMessage(new
                            {
                                sender = sender.User.UserName,
                                message = message,
                                time = DateTime.Now.ToShortTimeString(),
                                isPrivate = true,
                                groupId = groupId
                            });
                        }
                       
                        /*
                        IEnumerable<string> allReceivers;
                        lock (receiver.ConnectionIds)
                        {
                            lock (sender.ConnectionIds)
                            {

                                allReceivers = receiver.ConnectionIds.Concat(
                                    sender.ConnectionIds);
                            }
                        }
                        allReceivers = allReceivers.Distinct();

                        foreach (var cid in allReceivers)
                        {

                            Clients.Client(cid).ReceivePrivateMessage(new
                            {
                                sender = sender.User.UserName,
                                message = message,
                                time = DateTime.Now.ToShortTimeString(),
                                isPrivate = true
                            });
                        }
                        */
                    }
                }
            }
        }


        public void UpdateUserConnection(string userName)
        {
            var user = UserDBController.GetUser(userName);
            Users.FirstOrDefault(x => x.Key == userName).Value.User.AvatarUrl = user.AvatarUrl;
        }

        public override Task OnConnected()
        {
            var userModel = UserDBController.GetUser(Context.User.Identity.Name);
            string connectionId = Context.ConnectionId;

            var user = Users.GetOrAdd(userModel.UserName, _ => new Models.ChatUserModel
            {
                User = userModel,
                ConnectionIds = new HashSet<string>()
            });

            lock (user.ConnectionIds)
            {

                user.ConnectionIds.Add(connectionId);


                if (user.ConnectionIds.Count == 1)
                {
                    Clients.Others.userConnected(userModel.UserName, DateTime.Now.ToShortTimeString());
                }
            }

            return base.OnConnected();
        }



        public override Task OnDisconnected(bool stopCalled)
        {
            var userModel = UserDBController.GetUser(Context.User.Identity.Name);
            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;

            Models.ChatUserModel chatUser;
            Users.TryGetValue(userName, out chatUser);

            if (chatUser != null)
            {
                lock (chatUser.ConnectionIds)
                {
                    chatUser.ConnectionIds.RemoveWhere(cid => cid.Equals(connectionId));

                    if (!chatUser.ConnectionIds.Any())
                    {
                        Models.ChatUserModel removedUser;
                        Users.TryRemove(userName, out removedUser);


                        // Clients.All.RemoveUserNameFromList(userName);
                        Clients.Others.userDisconnected(userName, DateTime.Now.ToShortTimeString());
                    }
                }
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}










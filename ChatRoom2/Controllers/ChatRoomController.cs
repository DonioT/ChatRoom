using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChatRoom2.Controllers
{
    public class ChatRoomController : Controller
    {
        // GET: ChatRoom
        private Controllers.ChatHub chatHubController = DependencyResolver.Current.GetService<Controllers.ChatHub>();
        private Controllers.Database.ChatDatabaseController chatDatabaseController = DependencyResolver.Current.GetService<Controllers.Database.ChatDatabaseController>();
        private Controllers.Database.UserDatabaseController userDatabaseController = DependencyResolver.Current.GetService<Controllers.Database.UserDatabaseController>();
        [Authorize]
        public ActionResult Index()
        {
            Models.UserModel user = userDatabaseController.GetUser(User.Identity.Name);
            ViewBag.NotificationCount = chatDatabaseController.GetUserMessageFlags(user.UserId).Count(e => e.MessageSeen == false);
            return View("ChatRoom", user);
        }

        [Authorize]
        public bool SaveChatMessage(string message = "")
        {
            var userModel = userDatabaseController.GetUser(User.Identity.Name);
            bool success = chatDatabaseController.SaveMessage(new Models.ChatMessageModel
            {
                ChatMessage = message,
                ChatMessageId = Guid.NewGuid(),
                DateSent = DateTime.Now,
                UserId = userModel.UserId,
                Private = false
            });

            return success;

        }

        [Authorize]
        public PartialViewResult ChatHistory()
        {
            var chatHistory = chatDatabaseController.GetChatMessages(5, false);
            ViewBag.NotificationCount = chatDatabaseController.GetUserMessageFlags(userDatabaseController.GetUser(User.Identity.Name).UserId).Count(e => e.MessageSeen == false);
            return PartialView("_chatMessages", chatHistory);
        }

        [Authorize]
        public bool UpdateUser(Models.UserModel model)
        {
            if (userDatabaseController.UpdateUser(model))
            {
                chatHubController.UpdateUserConnection(model.UserName);
                return true;
            }
            return false;
        }
        [Authorize]
        public ActionResult GetUserList()
        {
            var userList = chatHubController.GetAllActiveConnections();
            List<Models.UserModel> users = new List<Models.UserModel>();
            foreach (var model in userList)
            {
                Models.ChatUserModel user;
                userList.TryGetValue(model.Key, out user);
                users.Add(user.User);
            }
            ViewBag.NotificationCount = chatDatabaseController.GetUserMessageFlags(userDatabaseController.GetUser(User.Identity.Name).UserId).Count(e => e.MessageSeen == false);

            return PartialView("_chatUserList", users);
        }
        [Authorize]
        public ActionResult GetPrivateMessages(Guid userId)
        {
            var groupChats = chatDatabaseController.GetPrivateChats(userId);
            ViewBag.NotificationCount = chatDatabaseController.GetUserMessageFlags(userDatabaseController.GetUser(User.Identity.Name).UserId).Count(e => e.MessageSeen == false);
            return PartialView("_PrivateMessageList", groupChats);
        }

        [Authorize]
        public bool DeleteGroupChat(Guid groupId)
        {
            return chatDatabaseController.DeleteGroupChat(groupId);
        }
        [Authorize]
        public ActionResult GetRecentGroupChat(Guid userId)
        {
            var groups = chatDatabaseController.GetUserGroupChats(userId);
            Models.GroupChatModel model = new Models.GroupChatModel();

            if (groups != null && groups.Count > 0)
            {
                var groupChat = chatDatabaseController.GetGroupChat(groups.OrderByDescending(e => e.LastModified).FirstOrDefault().GroupId);
                if (groupChat.GroupMessages.Count > 0)
                {
                    model = groupChat;
                    chatDatabaseController.UpdateMessageFlags(groupChat.GroupId, userDatabaseController.GetUser(User.Identity.Name).UserId);
                }
            }

            return PartialView("_PrivateChatMessages", model);


        }
        [Authorize]
        public ActionResult GroupChat(List<Guid> chatMemberIds)
        {
            //check if all members being sent belong to a group already. fetch that group or create new one.
            var groupChat = chatDatabaseController.GetPrivateChat(chatMemberIds);

            if (groupChat == null)
            {
                groupChat = new Models.GroupChatModel();

                groupChat.GroupId = Guid.NewGuid();
                groupChat.GroupMembers = (from userId in chatMemberIds
                                          select new Models.GroupMemberModel
                                          {
                                              GroupId = groupChat.GroupId,
                                              GroupMemberId = Guid.NewGuid(),
                                              UserId = userId
                                          }).ToList();
                groupChat.GroupMessages = new List<Models.GroupMessageModel>();

                var success = chatDatabaseController.CreatePrivateChat(groupChat);
            }
            else
            {
                chatDatabaseController.UpdateMessageFlags(groupChat.GroupId, userDatabaseController.GetUser(User.Identity.Name).UserId);
            }
            return PartialView("_PrivateChatMessages", groupChat);
        }

        public ActionResult GetCreatePrivateChatPartial()
        {
            return PartialView("_CreatePrivateChat");
        }

        [Authorize]
        public ActionResult GetPrivateChat(Guid groupId)
        {
            var groupChat = chatDatabaseController.GetGroupChat(groupId);
            chatDatabaseController.UpdateMessageFlags(groupId, userDatabaseController.GetUser(User.Identity.Name).UserId);
            return PartialView("_PrivateChatMessages", groupChat);
        }

        public Guid CreatePrivateChat(List<Guid> chatUserIds, string message)
        {
            bool success = false;
            var user = userDatabaseController.GetUser(User.Identity.Name);
            var groupChat = chatDatabaseController.GetPrivateChat(chatUserIds);

            Models.ChatMessageModel chatMessageModel = new Models.ChatMessageModel
            {
                ChatMessage = message,
                ChatMessageId = Guid.NewGuid(),
                DateSent = DateTime.Now,
                UserId = user.UserId,
                Private = true
            };


            success = chatDatabaseController.SaveMessage(chatMessageModel);

            if (groupChat == null)
            {
                groupChat = new Models.GroupChatModel();

                groupChat.GroupId = Guid.NewGuid();
                groupChat.GroupMembers = (from userId in chatUserIds
                                          select new Models.GroupMemberModel
                                          {
                                              GroupId = groupChat.GroupId,
                                              GroupMemberId = Guid.NewGuid(),
                                              UserId = userId
                                          }).ToList();
                groupChat.GroupMessages = new List<Models.GroupMessageModel>();
                success = chatDatabaseController.CreatePrivateChat(groupChat);
            }

            success = chatDatabaseController.CreateGroupMessage(new Models.GroupMessageModel
            {
                ChatMessageId = chatMessageModel.ChatMessageId,
                CreatedOn = DateTime.Now,
                GroupId = groupChat.GroupId,
                GroupMessageId = Guid.NewGuid(),

            });

            List<Models.DatabaseModels.MessageFlagModel> messageFlags = new List<Models.DatabaseModels.MessageFlagModel>();

            for (int i = 0; i < groupChat.GroupMembers.Count; i++)
            {
                messageFlags.Add(new Models.DatabaseModels.MessageFlagModel
                {
                    MessageSeen = (user.UserId == groupChat.GroupMembers[i].UserId) ? true : false,
                    ChatMessageId = chatMessageModel.ChatMessageId,
                    UserId = groupChat.GroupMembers[i].UserId,
                    MessageFlagId = Guid.NewGuid()
                });
            }
            success = chatDatabaseController.CreateMessageFlags(messageFlags);

            return groupChat.GroupId;
        }
        [Authorize]
        public bool SendPrivateMessage(Guid groupId, string message)
        {
            var success = false;
            var userModel = userDatabaseController.GetUser(User.Identity.Name);
            var groupMembers = chatDatabaseController.GetGroupMembers(groupId);
            //create chat message
            Models.ChatMessageModel chatMessage = new Models.ChatMessageModel
            {
                ChatMessage = message,
                ChatMessageId = Guid.NewGuid(),
                DateSent = DateTime.Now,
                UserId = userModel.UserId,
                Private = true
            };

            success = chatDatabaseController.SaveMessage(chatMessage);

            var group = chatDatabaseController.GetGroup(groupId);
            success = chatDatabaseController.CreateGroupMessage(new Models.GroupMessageModel
            {
                CreatedOn = DateTime.Now,
                ChatMessageId = chatMessage.ChatMessageId,
                GroupMessageId = Guid.NewGuid(),
                GroupId = groupId
            });
            if (success)
            {
                List<Models.DatabaseModels.MessageFlagModel> messageFlags = new List<Models.DatabaseModels.MessageFlagModel>();

                for (int i = 0; i < groupMembers.Count; i++)
                {
                    messageFlags.Add(new Models.DatabaseModels.MessageFlagModel
                    {
                        MessageSeen = (User.Identity.Name.ToLower() == groupMembers[i].User.UserName.ToLower()) ? true : false,
                        ChatMessageId = chatMessage.ChatMessageId,
                        UserId = groupMembers[i].UserId,
                        MessageFlagId = Guid.NewGuid()
                    });
                }
                success = chatDatabaseController.CreateMessageFlags(messageFlags);
            }
            return success;
        }
    }
}
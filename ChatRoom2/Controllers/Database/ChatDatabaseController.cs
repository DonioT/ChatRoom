using ChatRoom2.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChatRoom2.Controllers.Database
{
    public class ChatDatabaseController : Controller
    {
        public List<Models.ChatMessageModel> GetChatMessages(int numOfMessages, bool privacy)
        {
            List<Models.ChatMessageModel> listOfMessages = new List<Models.ChatMessageModel>();
            using (var db = new ChatDBContext())
            {
                try
                {
                    listOfMessages = (from dbModel in db.ChatMessages
                                      join a in db.Users on dbModel.UserId equals a.UserId
                                      where dbModel.Private == privacy
                                      select new
                                      {
                                          User = new
                                          {
                                              UserId = dbModel.UserId,
                                              UserName = a.UserName
                                          },
                                          UserId = dbModel.UserId,
                                          DateSent = dbModel.DateSent,
                                          ChatMessage = dbModel.ChatMessage,
                                          ChatMessageId = dbModel.ChatMessageId
                                      }).OrderByDescending(x => x.DateSent).Take(numOfMessages).ToList()
                                                          .Select(p => new Models.ChatMessageModel
                                                          {
                                                              User = new Models.UserModel
                                                              {
                                                                  UserId = p.User.UserId,
                                                                  UserName = p.User.UserName
                                                              },
                                                              UserId = p.UserId,
                                                              DateSent = p.DateSent,
                                                              ChatMessage = p.ChatMessage,
                                                              ChatMessageId = p.ChatMessageId
                                                          }).ToList();

                }
                catch (Exception ex)
                {
                }
            }
            return listOfMessages;
        }

        public bool CreateGroup(Models.GroupModel model)
        {
            using (var db = new ChatDBContext())
            {
                try
                {
                    db.Groups.Add(model);
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }


        public Models.GroupModel GetGroup(Guid id)
        {
            using (var db = new ChatDBContext())
            {
                try
                {
                    return db.Groups.FirstOrDefault(e => e.GroupId == id);
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public bool CreateGroupMember(Models.GroupMemberModel model)
        {
            using (var db = new ChatDBContext())
            {
                try
                {
                    db.GroupMembers.Add(model);
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }
        public bool CreateGroupMessage(Models.GroupMessageModel model)
        {
            using (var db = new ChatDBContext())
            {
                try
                {
                    db.GroupMessages.Add(model);
                    db.Groups.FirstOrDefault(e => e.GroupId == model.GroupId).LastModified = DateTime.Now;
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }

        public bool CreateMessageFlags(List<Models.DatabaseModels.MessageFlagModel> flags)
        {
            using (var db = new ChatDBContext())
            {
                try
                {
                    db.MessageFlags.AddRange(flags);
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }



        public bool SaveMessage(Models.ChatMessageModel model)
        {
            using (var db = new ChatDBContext())
            {
                try
                {
                    db.ChatMessages.Add(model);
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }

        public List<Models.GroupMemberModel> GetGroupMembers(Guid groupId)
        {
            using (var db = new ChatDBContext())
            {
                try
                {
                    return (from groupMember in db.GroupMembers
                            join user in db.Users on groupMember.UserId equals user.UserId
                            where groupMember.GroupId == groupId
                            select new
                            {
                                CreatedOn = groupMember.CreatedOn,
                                GroupId = groupMember.GroupId,
                                GroupMemberId = groupMember.GroupMemberId,
                                UserId = groupMember.UserId,
                                User = new
                                {
                                    UserName = user.UserName,
                                    UserId = user.UserId,
                                    AvatarUrl = user.AvatarUrl,
                                    CreatedOn = user.CreatedOn,
                                }
                            }).ToList().Select(e => new Models.GroupMemberModel
                            {
                                GroupId = e.GroupId,
                                GroupMemberId = e.GroupMemberId,
                                User = new Models.UserModel
                                {
                                    UserId = e.User.UserId,
                                    UserName = e.User.UserName,
                                    AvatarUrl = e.User.AvatarUrl,
                                    CreatedOn = e.CreatedOn
                                },
                                UserId = e.UserId,
                                CreatedOn = e.CreatedOn
                            }).ToList();
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public List<Models.DatabaseModels.MessageFlagModel> GetUserMessageFlags(Guid userId)
        {
            using (var db = new ChatDBContext())
            {
                try
                {
                    return db.MessageFlags.Where(e => e.UserId == userId).ToList();
                }catch(Exception e)
                {
                    return null;
                }
            }

        }
        public bool UpdateMessageFlags(Guid groupId, Guid userId)
        {
            using (var db = new ChatDBContext())
            {
                try
                {
                    var groupMessages = db.GroupMessages.Where(x => x.GroupId == groupId);
                    var userFlags = db.MessageFlags.Where(e => e.UserId == userId);

                    return userFlags.Where(e => groupMessages.Any(x => x.ChatMessageId == e.ChatMessageId)).ToList().All(p => { p.MessageSeen = true; db.SaveChanges(); return true; });

                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }

        public List<Models.GroupModel> GetUserGroupChats(Guid userId)
        {
            using (var db = new ChatDBContext())
            {
                try
                {
                    return (from groupModel in db.Groups
                            join groupMember in db.GroupMembers on groupModel.GroupId equals groupMember.GroupId
                            where groupMember.UserId == userId
                            select new
                            {
                                GroupId = groupModel.GroupId,
                                CreatedOn = groupModel.CreatedOn,
                                LastModified = groupModel.LastModified
                            }).ToList()
                                 .Select(e => new Models.GroupModel
                                 {
                                     GroupId = e.GroupId,
                                     CreatedOn = e.CreatedOn,
                                     LastModified = e.LastModified
                                 }).ToList();
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public Models.GroupChatModel GetGroupChat(Guid groupId)
        {
            using (var db = new ChatDBContext())
            {
                try
                {
                    var query = (from groupModel in db.Groups
                                 join memberModel in db.GroupMembers on groupModel.GroupId equals memberModel.GroupId into members
                                 join groupMessage in db.GroupMessages on groupModel.GroupId equals groupMessage.GroupId into groupMessages
                                 where groupModel.GroupId == groupId
                                 select new
                                 {
                                     groupId = groupModel.GroupId,
                                     GroupMembers = (from memberModel in members
                                                     select new
                                                     {
                                                         groupId = memberModel.GroupId,
                                                         memberId = memberModel.GroupMemberId,
                                                         userId = memberModel.UserId,
                                                     }),
                                     GroupMessages = (from messageModel in groupMessages
                                                      join chatMessage in db.ChatMessages on messageModel.ChatMessageId equals chatMessage.ChatMessageId
                                                      join user in db.Users on chatMessage.UserId equals user.UserId
                                                      select new
                                                      {
                                                          groupMessageId = messageModel.GroupMessageId,
                                                          groupId = messageModel.GroupId,
                                                          chatMessageId = messageModel.ChatMessageId,
                                                          createdOn = messageModel.CreatedOn,
                                                          chatMessage = new
                                                          {
                                                              ChatMessageId = chatMessage.ChatMessageId,
                                                              UserId = chatMessage.UserId,
                                                              ChatMessage = chatMessage.ChatMessage,
                                                              DateSent = chatMessage.DateSent,
                                                              Private = chatMessage.Private
                                                          },
                                                          user = new
                                                          {
                                                              userId = user.UserId,
                                                              userName = user.UserName,
                                                              AvatarUrl = user.AvatarUrl
                                                          }
                                                      }),
                                 }).FirstOrDefault();

                    return new Models.GroupChatModel
                    {
                        GroupId = query.groupId,
                        GroupMembers = query.GroupMembers.Select(z =>
                                        new Models.GroupMemberModel
                                        {
                                            GroupId = z.groupId,
                                            GroupMemberId = z.memberId,
                                            UserId = z.userId,
                                        }).ToList(),
                        GroupMessages = query.GroupMessages.Select(z => new Models.GroupMessageModel
                        {
                            ChatMessage = new Models.ChatMessageModel
                            {
                                ChatMessage = z.chatMessage.ChatMessage,
                                ChatMessageId = z.chatMessage.ChatMessageId,
                                Private = z.chatMessage.Private,
                                DateSent = z.chatMessage.DateSent,
                                User = new Models.UserModel
                                {
                                    UserId = z.user.userId,
                                    UserName = z.user.userName,
                                    AvatarUrl = z.user.AvatarUrl
                                }
                            }
                        }).ToList()
                    };
                }

                catch (Exception e)
                {
                    return null;
                }
            }
        }


        public Models.GroupChatModel GetPrivateChat(List<Guid> userIds)
        {
            using (var db = new ChatDBContext())
            {
                try
                {
                    var queryToList = (from groupModel in db.Groups
                                       join memberModel in db.GroupMembers on groupModel.GroupId equals memberModel.GroupId into members
                                       join groupMessage in db.GroupMessages on groupModel.GroupId equals groupMessage.GroupId into groupMessages
                                       select new
                                       {
                                           groupId = groupModel.GroupId,
                                           GroupMembers = (from memberModel in members
                                                           select new
                                                           {
                                                               groupId = memberModel.GroupId,
                                                               memberId = memberModel.GroupMemberId,
                                                               userId = memberModel.UserId,
                                                           }),
                                           GroupMessages = (from messageModel in groupMessages
                                                            join chatMessage in db.ChatMessages on messageModel.ChatMessageId equals chatMessage.ChatMessageId
                                                            join user in db.Users on chatMessage.UserId equals user.UserId
                                                            select new
                                                            {
                                                                groupMessageId = messageModel.GroupMessageId,
                                                                groupId = messageModel.GroupId,
                                                                chatMessageId = messageModel.ChatMessageId,
                                                                createdOn = messageModel.CreatedOn,
                                                                chatMessage = new
                                                                {
                                                                    ChatMessageId = chatMessage.ChatMessageId,
                                                                    UserId = chatMessage.UserId,
                                                                    ChatMessage = chatMessage.ChatMessage,
                                                                    DateSent = chatMessage.DateSent,
                                                                    Private = chatMessage.Private
                                                                },
                                                                user = new
                                                                {
                                                                    userId = user.UserId,
                                                                    userName = user.UserName,
                                                                    AvatarUrl = user.AvatarUrl
                                                                }
                                                            }),
                                       }).ToList();

                    return queryToList.Select(e => new Models.GroupChatModel
                    {
                        GroupId = e.groupId,
                        GroupMembers = e.GroupMembers.Select(z =>
                                        new Models.GroupMemberModel
                                        {
                                            GroupId = z.groupId,
                                            GroupMemberId = z.memberId,
                                            UserId = z.userId,
                                        }).ToList(),
                        GroupMessages = e.GroupMessages.Select(z => new Models.GroupMessageModel
                        {
                            ChatMessage = new Models.ChatMessageModel
                            {
                                ChatMessage = z.chatMessage.ChatMessage,
                                ChatMessageId = z.chatMessage.ChatMessageId,
                                Private = z.chatMessage.Private,
                                DateSent = z.chatMessage.DateSent,
                                User = new Models.UserModel
                                {
                                    UserId = z.user.userId,
                                    UserName = z.user.userName,
                                    AvatarUrl = z.user.AvatarUrl
                                }
                            }
                        }).ToList()
                    }).FirstOrDefault((p => p.GroupMembers.Select(o => o.UserId).All(userIds.Contains) && p.GroupMembers.Count == userIds.Count));

                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public List<Models.GroupChatModel> GetPrivateChats(Guid userId)
        {
            using (var db = new ChatDBContext())
            {
                try
                {
                    var userGroups = db.GroupMembers.Where(e => e.UserId == userId).Select(e => e.GroupId).ToList();
                    var userMessageFlags = db.MessageFlags.Where(e => e.UserId == userId && e.MessageSeen == false).ToList();
                    return (from groupId in userGroups
                            join memberModel in db.GroupMembers on groupId equals memberModel.GroupId into members
                            join groupMessage in db.GroupMessages on groupId equals groupMessage.GroupId into groupMessages

                            select new
                            {
                                groupId = groupId,
                                GroupMembers = from memberModel in members
                                               select new
                                               {
                                                   groupId = memberModel.GroupId,
                                                   memberId = memberModel.GroupMemberId,
                                                   userId = memberModel.UserId,
                                               },
                                GroupMessages = from messageModel in groupMessages
                                                join chatMessage in db.ChatMessages on messageModel.ChatMessageId equals chatMessage.ChatMessageId
                                                select new
                                                {
                                                    groupMessageId = messageModel.GroupMessageId,
                                                    groupId = messageModel.GroupId,
                                                    chatMessageId = messageModel.ChatMessageId,
                                                    createdOn = messageModel.CreatedOn,
                                                    chatMessage = new
                                                    {
                                                        ChatMessageId = chatMessage.ChatMessageId,
                                                        UserId = chatMessage.UserId,
                                                        ChatMessage = chatMessage.ChatMessage,
                                                        DateSent = chatMessage.DateSent,
                                                        Private = chatMessage.Private
                                                    }
                                                },
                                UserUnreadCount = userMessageFlags.Count(e => groupMessages.Any(z => z.ChatMessageId == e.ChatMessageId))

                            }).Select(e => new Models.GroupChatModel
                            {
                                GroupId = e.groupId,
                                GroupMembers = e.GroupMembers.Select(x => new Models.GroupMemberModel
                                {
                                    GroupId = x.groupId,
                                    GroupMemberId = x.memberId,
                                    UserId = x.userId,
                                    User = db.Users.FirstOrDefault(o => o.UserId == x.userId)
                                }).ToList(),
                                GroupMessages = e.GroupMessages.Select(x => new Models.GroupMessageModel
                                {
                                    GroupId = x.groupId,
                                    ChatMessage = new Models.ChatMessageModel
                                    {
                                        ChatMessage = x.chatMessage.ChatMessage,
                                        ChatMessageId = x.chatMessage.ChatMessageId,
                                        Private = x.chatMessage.Private,
                                        DateSent = x.chatMessage.DateSent,
                                    },
                                    GroupMessageId = x.groupMessageId,
                                    CreatedOn = x.createdOn,
                                    ChatMessageId = x.chatMessageId
                                }).ToList(),
                                UnreadCount = e.UserUnreadCount
                            }).ToList();

                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }
        public bool CreatePrivateChat(Models.GroupChatModel model)
        {
            using (var db = new ChatDBContext())
            {
                try
                {
                    db.Groups.Add(new Models.GroupModel
                    {
                        GroupId = model.GroupId,
                        CreatedOn = DateTime.Now,
                        LastModified = DateTime.Now
                    });
                    db.GroupMembers.AddRange((from member in model.GroupMembers
                                              select new Models.GroupMemberModel
                                              {
                                                  GroupId = member.GroupId,
                                                  CreatedOn = DateTime.Now,
                                                  GroupMemberId = member.GroupMemberId,
                                                  UserId = member.UserId
                                              }).ToList());
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }

        public bool DeleteGroupChat(Guid groupId)
        {
            using (var db = new ChatDBContext())
            {

                try
                {
                    var model = db.Groups.FirstOrDefault(e => e.GroupId == groupId);
                    var chatMessages = db.GroupMessages.Where(e => e.GroupId == model.GroupId).Select(x => x.ChatMessageId);
                    var modelsToRemove = db.MessageFlags.Where(e => chatMessages.Select(x => x == e.ChatMessageId).FirstOrDefault()).ToList();
                    db.MessageFlags.RemoveRange(modelsToRemove);
                    db.Groups.Remove(model);
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }

            }
        }
    }
}
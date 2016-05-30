using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class ForumManager
    {
        public static User GetUserById(string id)
        {
            using (ForumEntities db = new ForumEntities())
            {
                return db.Users.Where(x => x.Id == id).FirstOrDefault();
            }
        }

        public static User CreateUser(string id, string name)
        {
            using (ForumEntities db = new ForumEntities())
            {
                User user = new User();
                user.Id = id;
                user.Name = name;

                db.Users.Add(user);
                db.SaveChanges();

                return user;
            }
        }

        public static void SavePost(User user, string postId, DateTime dt, string content)
        {

            using (ForumEntities db = new ForumEntities())
            {
                Post post = db.Posts.Where(x => x.Id == postId).FirstOrDefault();
                if (post == null)
                {
                    post = new Post();
                    post.Id = postId;
                    db.Posts.Add(post);
                }
                post.UserId = user.Id;
                post.Created = dt;
                post.Content = content;
                
                db.SaveChanges();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ForumGrabber.Models
{
    public class CriteriaModel
    {
        [DisplayName("Url страницы форума")]
        public string Url { get; set; }
        [DisplayName("Id пользователя")]
        public string UserId { get; set; }
        [DisplayName("Имя пользователя")]
        public string Name { get; set; }
        [DisplayName("Количество страниц форума")]
        public int PagesCount { get; set; }
         [DisplayName("Искать по Id")]
        public bool SearchById { get; set; }
    }
}
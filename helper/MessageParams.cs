using System;
namespace DatingApp.helper
{
    public class MessageParams
    {
        private const int maxPageSize = 50;
        private int pageSize = 10;


        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > maxPageSize) ? maxPageSize : value; }
        }
        public int UserId { get; set; }
        public string MessageContainer { get; set; } = "unread";
    }
}

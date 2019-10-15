using System;
namespace DatingApp.helper
{
    public class PagationHeader
    {
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public int Totalpages { get; set; }

        public PagationHeader(int currentpage, int itemsperpage, int totalitems, int totalpage)
        {
            CurrentPage = currentpage;
            ItemsPerPage = itemsperpage;
            TotalItems = totalitems;
            Totalpages = totalpage;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.helper
{
    public class PagedList<T> : List<T>
    {
        public int CurrPage { get; set; }
        public int totalPage { get; set; }
        public int PageSizes { get; set; }
        public int TotalCount { get; set; }


        public PagedList(List<T> items, int count, int pageNum, int pageSize)
        {
            TotalCount = count;
            PageSizes = pageSize;
            CurrPage = pageNum;
            totalPage = (int)(Math.Ceiling(count / (double)pageSize));
            this.AddRange(items);

        }
        public static async Task<PagedList<T>> creatAsync(IQueryable<T> source, int pageNum, int pageSize)
        {
            var count = await source.CountAsync();
            // ignore the first x records base on the current page
            var items = await source.Skip((pageNum - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, count, pageNum, pageSize);
        }
    }
}

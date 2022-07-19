using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Taye.Models;
using Taye.Pages.Components;
using Taye.Repositories;

namespace Taye.Pages.Images
{
    public partial class Image
    {
        [Inject]
        TayeContext Context { get; set; }

        public PageParameters PageParam { get; set; }

        public List<Archive> Archives { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Archives = new List<Archive>();
            PageParam = new PageParameters
            {
                PageIndex = 1,
            };

            //默认12个一页
            await QueryWithPagination(new PaginationEventArgs(1, 12));
        }

        public Task QueryWithPagination(PaginationEventArgs pgArgs)
        {
            PageParam.PageIndex = pgArgs.Page;
            PageParam.PageSize = pgArgs.PageSize;
            PageParam.Total = Context.Archives.Count();

            Archives = Context.Archives
                .Include(a => a.MediaFile)
                .Where(p => p.MediaFile.FileType == Enums.FileType.Image)
                .OrderByDescending(p => p.PubDate)
                .Skip(PageParam.PageSize * (PageParam.PageIndex - 1))
                .Take(PageParam.PageSize)
                .ToList();

            return Task.FromResult(Archives);
        }
    }
}

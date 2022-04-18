using AntDesign;
using AntDesign.TableModels;
using Microsoft.AspNetCore.Components;
using System.ComponentModel;
using System.Text.Json;
using Taye.Models;
using Taye.Pages.Components;
using Taye.Repositories;

namespace Taye.Pages.Admin.Archives
{
    public partial class ArchiveList : BaseRazor
    {
        public PageParameters PageParam { get; set; }

        List<Archive> Archives;

        [Inject]
        TayeContext Context { get; set; }


        IEnumerable<Archive> selectedRows;
        ITable table;

        protected override async Task<List<Archive>> OnInitializedAsync()
        {
            PageParam = new PageParameters
            {
                PageIndex = 1,
                PageSize = 10,
                Total = 0
            };

            Query();

            return await Task.FromResult(Archives);
        }

        async Task Query()
        {
            Archives = Context.Archives
                .Skip(PageParam.PageSize * (PageParam.PageIndex - 1))
                .Take(PageParam.PageSize)
                .ToList();
        }

        public void RemoveSelection(int id)
        {
            var selected = selectedRows.Where(x => x.Id != id);
            selectedRows = selected;
        }

        private async void Delete(Archive archive)
        {
            //用同步处理删除动作及后续的查询
            Context.Archives.Remove(archive);
            Context.SaveChanges();
            RefreshTable(archive);
            await ShowSuccess();
        }

        async void RefreshTable(Archive archive)
        {
            Archives = Archives.Where(p => p.Id != archive.Id).ToList();
        }

        void Cancel()
        {

        }

        void Detail(int id)
        {
            NavManager.NavigateTo($"/admin/archive/{id}");
        }
    }
}

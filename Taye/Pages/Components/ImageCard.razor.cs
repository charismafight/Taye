using Microsoft.AspNetCore.Components;
using Taye.Models;

namespace Taye.Pages.Components
{
    public partial class ImageCard
    {
        [Parameter]
        public Archive Archive { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string Description { get; set; }

        //[Parameter]
        //public RenderFragment Action { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        //[Parameter]
        //public RenderFragment Footer { get; set; }

        //[Parameter]
        //public bool Liked { get; set; }
    }
}

using AntDesign;
using Microsoft.AspNetCore.Components;
using Taye.Repositories;

namespace Taye.Pages
{
    public class BaseRazor : ComponentBase
    {
        [Inject]
        protected TayeContext Context { get; set; }

        [Inject]
        protected NavigationManager NavManager { get; set; }

        [Inject]
        protected NotificationService Notice { get; set; }

        [Inject]
        protected MessageService Message { get; set; }

        protected async Task<NotificationRef> ShowSuccess(string msg = "")
        {
            if (string.IsNullOrWhiteSpace(msg))
            {
                msg = "操作成功";
            }

            return await Notice.Open(new NotificationConfig()
            {
                Message = msg,
                NotificationType = NotificationType.Success
            });
        }

        protected async Task<NotificationRef> ShowFail(string msg = "")
        {
            if (string.IsNullOrWhiteSpace(msg))
            {
                msg = "操作失败";
            }

            return await Notice.Open(new NotificationConfig()
            {
                Message = msg,
                NotificationType = NotificationType.Error
            });
        }
    }
}

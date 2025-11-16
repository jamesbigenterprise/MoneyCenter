using CommunityToolkit.Maui.Alerts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCenter.Services
{
    public class ToastService : IToastService
    {
        public void Show(string message)
        {
            Show(message, ToastDuration.Short);
        }

        public void Show(string message, ToastDuration duration)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                var toast = Toast.Make(message,
                    duration == ToastDuration.Long ?
                        CommunityToolkit.Maui.Core.ToastDuration.Long :
                        CommunityToolkit.Maui.Core.ToastDuration.Short);

                await toast.Show();
            });
        }
    }

}

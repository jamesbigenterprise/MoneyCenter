using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCenter.Services
{
    public interface IToastService
    {
        void Show(string message);
        void Show(string message, ToastDuration duration);
    }

    public enum ToastDuration
    {
        Short,
        Long
    }
}

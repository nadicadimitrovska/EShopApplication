using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Service.Interface
{
    public interface IBackgroundEmailSender
    {
        Task DoWork();
    }
}

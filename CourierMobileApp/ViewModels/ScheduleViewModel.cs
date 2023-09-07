using CourierMobileApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourierMobileApp.ViewModels;

public class ScheduleViewModel : BaseViewModel
{
    ConnectionService connectionService;

    

    // lista przesyłek

    public ScheduleViewModel(ConnectionService connectionService)
    {
        this.connectionService = connectionService;
    }
}

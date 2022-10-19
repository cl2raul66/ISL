using CommunityToolkit.Mvvm.ComponentModel;
using ISL.Modelos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISL.VistaModelos
{
    public partial class MensjesVistaModelo : ObservableRecipient
    {
        [ObservableProperty]
        private ObservableCollection<MensajeItem> notificaciones = new();
    }
}

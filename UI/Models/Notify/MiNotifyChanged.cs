using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Models.Notify
{
    /// <summary>
    /// CLASE PARA TENER LAS FUNCIONES DE CAMBIO DE PROPIEDAD
    /// </summary>
    public class MiNotifyChanged: INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}

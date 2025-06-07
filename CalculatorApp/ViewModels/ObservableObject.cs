using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CalculatorApp.ViewModels
{
    /// <summary>
    /// A base class for objects that implements <see cref="INotifyPropertyChanged"/>.
    /// This simplifies the process of creating ViewModel classes by providing the
    /// boilerplate implementation for property change notifications, which is essential for WPF data binding.
    /// </summary>
    public class ObservableObject : INotifyPropertyChanged
    {
        
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        
        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property that changed. This is automatically determined
        /// by the compiler using the <see cref="CallerMemberNameAttribute"/>.
        /// </param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

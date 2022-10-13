using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Fuzzy.Models
{
    class Alpha:INotifyPropertyChanged
    {
        private double slice;
        private double less;
        private double greater;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public Alpha(double slice, double less, double greater)
        {
            Slice = slice;
            Less = less;
            Greater = greater;
        }

        public Alpha()
        {
        }

        public double Slice {
            get =>slice;
            set {
                if (value >= 0 && value <= 1) slice = value;
                OnPropertyChanged(nameof(Slice));   
            }
        }

        public double Less { get=>less; set {
                less = value;
                OnPropertyChanged(nameof(Less));    
            } }

        public double Greater { get=>greater; set {
                greater = value;
                OnPropertyChanged(nameof(Greater));
            } }
    }
}

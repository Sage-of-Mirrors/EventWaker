using GameFormatReader.Common;
using System.Collections.Generic;
using System.ComponentModel;

namespace EventWaker
{
    class Property : INotifyPropertyChanged
    {
        public enum PropertyType : int
        {
            Single,
            Vector3,
            Integer,
            String
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private string mName;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

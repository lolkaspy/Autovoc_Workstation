using System;
using System.ComponentModel;
namespace AutoVauxLauncher.HelpClasses
{
    public class Custom:INotifyPropertyChanged
    {
        public Custom() { }
        private string _route;
        private DateTime _ar_time;
        private DateTime _dp_time;
        private int _bus_id;
        private int _temp_id;
        public DateTime ARRIVAL_TIME {
            get { return _ar_time; }
            set { if (_ar_time != value) {
                    _ar_time = value;
                    OnPropertyChanged("ARRIVAL_TIME");
                } } }
        public DateTime DEPARTURE_TIME {
            get { return _dp_time; }
            set
            {
                if (_dp_time != value)
                {
                    _dp_time = value;
                    OnPropertyChanged("DEPARTURE_TIME");
                }
            }
        }
        public int BUS_ID{
            get { return _bus_id; }
            set
            {
                if (_bus_id != value)
                {
                    _bus_id = value;
                    OnPropertyChanged("BUS_ID");
                }
            }
        }
        public int TEMP_ID {
            get { return _temp_id; }
            set
            {
                if (_temp_id != value)
                {
                    _temp_id = value;
                    OnPropertyChanged("TEMP_ID");
                }
            }
        }
        public string ROUTE {
            get { return _route; }
            set
            {
                if (_route != value)
                {
                    _route = value;
                    OnPropertyChanged("ROUTE");
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

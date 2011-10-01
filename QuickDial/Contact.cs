using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;

namespace QuickDial
{
    public class Contact : ObservableObject
    {
        // Fields...
        private bool _ShowOnQuickLaunch;
        private string _PhoneNumber;
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name == value)
                    return;
                _Name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        public string PhoneNumber
        {
            get { return _PhoneNumber; }
            set
            {
                if (_PhoneNumber == value)
                    return;
                _PhoneNumber = value;
                RaisePropertyChanged(() => PhoneNumber);
            }
        }

        public bool ShowOnQuickLaunch
        {
            get { return _ShowOnQuickLaunch; }
            set
            {
                if (_ShowOnQuickLaunch == value)
                    return;
                _ShowOnQuickLaunch = value;
                RaisePropertyChanged(() => ShowOnQuickLaunch);
            }
        }
    }
}
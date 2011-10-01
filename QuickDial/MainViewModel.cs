using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Extensibility;

namespace QuickDial
{
    internal class MainViewModel : ViewModelBase
    {
        private const string STORAGE_FILE = "CONTACTS.DAT";

        private Contact _selectedItem = null;
        private ObservableCollection<Contact> _contacts = new ObservableCollection<Contact>();
        private ObservableCollection<MenuItem> _menuItems = new ObservableCollection<MenuItem>();

        public RelayCommand AddCommand { get; set; }

        public RelayCommand UpdateCommand { get; set; }

        public RelayCommand DeleteCommand { get; set; }

        public Contact SelectedItem
        {
            get
            {
                return _selectedItem;
            }

            set
            {
                if (_selectedItem == value)
                {
                    return;
                }
                _selectedItem = value;
                RaisePropertyChanged(() => SelectedItem);
            }
        }

        public ObservableCollection<Contact> Contacts
        {
            get { return _contacts; }

            set
            {
                if (_contacts == value)
                {
                    return;
                }
                _contacts = value;
                RaisePropertyChanged(() => Contacts);
                MessengerInstance.Send<ResetMenuItemsMessage>(new ResetMenuItemsMessage());
            }
        }

        public void LoadContacts()
        {
            if (!File.Exists(STORAGE_FILE))
            {
                return;
            }
            using (var filestream = File.OpenRead(STORAGE_FILE))
            {
                XmlSerializer ser = new XmlSerializer(_contacts.GetType());
                var temp = ser.Deserialize(filestream) as ObservableCollection<Contact>;
                if (temp == null)
                {
                    throw new Exception("Unable to deserialize stored data!");
                }
                else
                {
                    Contacts = temp;
                }
                filestream.Close();
            }
        }

        public void SaveContacts()
        {
            if (File.Exists(STORAGE_FILE))
            {
                File.Delete(STORAGE_FILE);
            }
            XmlSerializer ser = new XmlSerializer(_contacts.GetType());
            using (var writer = XmlWriter.Create(STORAGE_FILE))
            {
                ser.Serialize(writer, _contacts);
            }
        }

        public MainViewModel()
        {
            AddCommand = new RelayCommand(() => AddItem(), () => true);
            UpdateCommand = new RelayCommand(() => UpdateItem(), () => true);
            DeleteCommand = new RelayCommand(() => DeleteItem(), () => true);
            LoadContacts();
        }

        private void DeleteItem()
        {
            if (SelectedItem != null)
            {
                Contacts.Remove(SelectedItem);
                MessengerInstance.Send<ResetMenuItemsMessage>(new ResetMenuItemsMessage());
            }
        }

        private void UpdateItem()
        {
            SaveContacts();
            MessengerInstance.Send<ResetMenuItemsMessage>(new ResetMenuItemsMessage());
        }

        private void AddItem()
        {
            Contacts.Add(new Contact());
        }
    }
}
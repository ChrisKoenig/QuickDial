using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Extensibility;

namespace QuickDial
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private WindowState lastWindowState;
        private bool shouldClose;
        private readonly AutomationModalities conversationModes = AutomationModalities.Audio;
        private readonly Dictionary<AutomationModalitySettings, object> conversationSettings = new Dictionary<AutomationModalitySettings, object>();

        private readonly MenuItem _openMenuItem;
        private readonly MenuItem _exitMenuItem;
        private readonly MenuItem _spacer1MenuItem;
        private readonly MenuItem _spacer2MenuItem;
        private readonly MenuItem _spacer3MenuItem;
        private readonly MenuItem _dialMenuItem;

        private MainViewModel vm;

        public Window1()
        {
            InitializeComponent();

            _dialMenuItem = new MenuItem("Manual Dial", OnDialMenuClick);
            _openMenuItem = new MenuItem("Open", OnMenuItemOpenClick);
            _exitMenuItem = new MenuItem("Exit", OnMenuItemExitClick);
            _spacer1MenuItem = new MenuItem("-");
            _spacer2MenuItem = new MenuItem("-");
            _spacer3MenuItem = new MenuItem("-");

            Loaded += (s, e) =>
            {
                vm = new MainViewModel();
                DataContext = vm;
                Messenger.Default.Register<ResetMenuItemsMessage>(this, (m) => ResetMenuItems());
                Messenger.Default.Register<OpenManualDialerMessage>(this, (m) => ShowDialDialog());
                ResetMenuItems();
            };
        }

        protected override void OnStateChanged(EventArgs e)
        {
            lastWindowState = WindowState;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!shouldClose)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void OnNotificationAreaIconDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Open();
            }
        }

        private void OnDialMenuClick(object sender, EventArgs e)
        {
            ShowDialDialog();
        }

        private void ShowDialDialog()
        {
            var dialer = new DialDialog();
            dialer.ShowDialog();
        }

        private void OnMenuItemOpenClick(object sender, EventArgs e)
        {
            Open();
        }

        private void Open()
        {
            Show();
            WindowState = lastWindowState;
        }

        private void OnMenuItemExitClick(object sender, EventArgs e)
        {
            shouldClose = true;
            Close();
        }

        public void ResetMenuItems()
        {
            SuperMenu.MenuItems.Clear();
            SuperMenu.MenuItems.Add(_openMenuItem);
            SuperMenu.MenuItems.Add(_spacer1MenuItem);
            SuperMenu.MenuItems.Add(_dialMenuItem);
            SuperMenu.MenuItems.Add(_spacer2MenuItem);
            var temp = from c in vm.Contacts
                       where c.ShowOnQuickLaunch == true
                       select new MenuItem()
                       {
                           Text = c.Name,
                           Tag = c.PhoneNumber,
                       };

            foreach (var item in temp)
            {
                item.Click += CallContact;
                SuperMenu.MenuItems.Add(item);
            }
            SuperMenu.MenuItems.Add(_spacer3MenuItem);
            SuperMenu.MenuItems.Add(_exitMenuItem);
            SuperMenu.ResetMenuItems();
        }

        private void CallContact(object sender, EventArgs e)
        {
            var number = ((MenuItem)sender).Tag.ToString();
            var o = LyncClient.GetAutomation();
            o.BeginStartConversation(conversationModes, new string[] { number }, conversationSettings, (cb) => { }, null);
        }
    }
}
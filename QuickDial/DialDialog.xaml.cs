using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Extensibility;

namespace QuickDial
{
    /// <summary>
    /// Interaction logic for DialDialog.xaml
    /// </summary>
    public partial class DialDialog : Window
    {
        private readonly AutomationModalities conversationModes = AutomationModalities.Audio;
        private readonly Dictionary<AutomationModalitySettings, object> conversationSettings = new Dictionary<AutomationModalitySettings, object>();

        public DialDialog()
        {
            InitializeComponent();
            Loaded += (s, e) => PhoneNumberTextBlock.SelectAll();
        }

        private void DialNumberButton_Click(object sender, RoutedEventArgs e)
        {
            DialNumber();
        }

        private void PhoneNumberTextBlock_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                DialNumber();
        }

        private void DialNumber()
        {
            var number = PhoneNumberTextBlock.Text;
            var o = LyncClient.GetAutomation();
            o.BeginStartConversation(conversationModes, new string[] { number }, conversationSettings, (cb) => { }, null);
        }

        private void PhoneNumberTextBlock_GotFocus(object sender, RoutedEventArgs e)
        {
            PhoneNumberTextBlock.SelectAll();
        }
    }
}
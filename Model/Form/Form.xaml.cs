using Autodesk.Revit.DB;
using SingleData;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Model.Form
{
    public partial class Form : Window
    {
        private FormData FormData => FormData.Instance;
        public Form()
        {
            InitializeComponent();
            Closing += Window_Closing;
        }
    
        private void Button_OkClick(object sender, RoutedEventArgs e)
        {
            FormData.HandleOkClick();
        }

        private void Button_CancelClick(object sender, RoutedEventArgs e)
        {
            FormData.HandleCancleClick();
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            FormData.Instance = null;
            Data.Instance = null;
        }
    }
}

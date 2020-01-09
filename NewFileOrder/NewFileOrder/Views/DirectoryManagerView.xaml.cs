using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using NewFileOrder.Models.DbModels;
using NewFileOrder.ViewModels;
using System;

namespace NewFileOrder.Views
{
    public class DirectoryManagerView : UserControl
    {
        public DirectoryManagerView()
        {
            this.InitializeComponent();
            var aaa = this.FindControl<DataGrid>("d");
            aaa.SelectionChanged += delegate { (DataContext as DirectoryManagerViewModel).ActiveDirectory = (DirectoryModel)aaa.SelectedItem;  };
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}

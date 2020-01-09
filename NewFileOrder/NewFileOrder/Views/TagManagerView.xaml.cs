using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using NewFileOrder.Models.DbModels;
using NewFileOrder.ViewModels;

namespace NewFileOrder.Views
{
    public class TagManagerView : UserControl
    {
        public TagManagerView()
        {
            this.InitializeComponent();
            var t = this.FindControl<DataGrid>("t");
            t.SelectionChanged += delegate { (DataContext as TagManagerViewModel).ActiveTagFilePair = (TagFilePair)t.SelectedItem; };
            var f = this.FindControl<DataGrid>("f");
            f.SelectionChanged += delegate { (DataContext as TagManagerViewModel).ActiveFile = (FileModel)f.SelectedItem; };
            var save = this.FindControl<Button>("save");
            save.Click += delegate { (DataContext as TagManagerViewModel).Save(); (this.VisualRoot as Window).Close(); };
            var cancel = this.FindControl<Button>("cancel");
            cancel.Click += delegate { (this.VisualRoot as Window).Close(); };
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}

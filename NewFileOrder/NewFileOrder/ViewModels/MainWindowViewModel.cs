using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewFileOrder.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        ViewModelBase content;

        public ViewModelBase Content
        {
            get => content;
            private set => this.RaiseAndSetIfChanged(ref content, value);
        }

        public MainWindowViewModel()
        {
            content = new SidePanelViewModel();
        }

    }
}

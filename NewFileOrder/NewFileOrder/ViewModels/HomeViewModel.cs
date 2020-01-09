using NewFileOrder.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;

namespace NewFileOrder.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        List<TagModel> tags;

        public List<TagModel> Tags
        {
            get => tags;
            private set => this.RaiseAndSetIfChanged(ref tags, value);
        }

        string version;

        public string Version
        {
            get => version;
            private set => this.RaiseAndSetIfChanged(ref version, value);
        }

        public HomeViewModel(List<TagModel> t)
        {
            Tags = t;
            version = "Verze " + MainWindowViewModel.VERSION;
        }
    }
}

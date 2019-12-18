using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using NewFileOrder.Models.DbModels;

namespace NewFileOrder.ViewModels
{
    class AddFilesViewModel : ViewModelBase
    {
        List<FileModel> files;
        public List<FileModel> Files
        {
            get => files;
            set => this.RaiseAndSetIfChanged(ref files, value);
        }

        public AddFilesViewModel(List<FileModel> files)
        {
            Files = files;
        }
    }
}

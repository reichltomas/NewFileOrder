using NewFileOrder.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;

namespace NewFileOrder.ViewModels
{
    class DirectoryManagerDialogViewModel : ViewModelBase
    {
        List<DirectoryModel> directories;

        public List<DirectoryModel> Directories
        {
            get => directories;
            private set => this.RaiseAndSetIfChanged(ref directories, value);
        }

        DirectoryModel activeDirectory;

        public DirectoryModel ActiveDirectory
        {
            get => activeDirectory;
            private set => this.RaiseAndSetIfChanged(ref activeDirectory, value);
        }

        public void Cancel()
        {

        }

        public void Ok()
        {

        }
    }
}

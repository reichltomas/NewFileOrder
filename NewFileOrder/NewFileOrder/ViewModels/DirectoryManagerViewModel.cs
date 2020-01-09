using NewFileOrder.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using NewFileOrder.Models.Managers;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using NewFileOrder.Views;
using System.Threading.Tasks;
using System.Reactive;
using System.Linq;
using Avalonia.Native;
using Avalonia.Media;

namespace NewFileOrder.ViewModels
{
    class DirectoryManagerViewModel : ViewModelBase
    {
        private FileManager _fm;

        private Window _window;

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
            set => this.RaiseAndSetIfChanged(ref activeDirectory, value);
        }

        int i;
        public int I { get => i; set => this.RaiseAndSetIfChanged(ref i, value); }

        public DirectoryManagerViewModel(FileManager fm, Window w)
        {
            _window = w;
            _fm = fm;
            Directories = _fm.GetAllDirectories();
            ActiveDirectory = Directories[0];

            Add = ReactiveCommand.CreateFromTask(AddAsync);
        }

        public ReactiveCommand<Unit,Unit> Add { get; }

        public async Task AddAsync()
        {
            var dialog = new OpenFolderDialog() { Title = "Vyberte adresář k přidání" };
            var path = await dialog.ShowAsync(_window);

            _fm.AddRootIfNotInDb(path);
            Directories = _fm.GetAllDirectories();
        }

        public void Remove() { 
            var activeDirIndex = Directories.IndexOf(ActiveDirectory);
            _fm.RemoveDirectory(ActiveDirectory);
            Directories = _fm.GetAllDirectories();
            if (activeDirIndex >= Directories.Count)
                activeDirIndex--;
            ActiveDirectory = Directories[activeDirIndex];
        }
    }
}

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Avalonia.Threading;
using Microsoft.EntityFrameworkCore;
using NewFileOrder.Models;
using NewFileOrder.Models.DbModels;
using NewFileOrder.Models.Managers;
using NewFileOrder.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;

namespace NewFileOrder.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private MyDbContext _db;
        private bool shouldLook = false;
        private List<FileModel> newFiles = new List<FileModel>();

        private FileManager _fileManager;
        private TagManager _tagManager;
        private IManagedNotificationManager _notificator;
        ViewModelBase content;

        public MainWindow MainWindow { get; set; }

        public ReactiveCommand<Unit, Unit> ShowCustomManagedNotificationCommand { get; }
        public ViewModelBase Content
        {
            get => content;
            private set => this.RaiseAndSetIfChanged(ref content, value);
        }

        string searchPhrase = "";

        public string SearchPhrase
        {
            get => searchPhrase;
            private set => this.RaiseAndSetIfChanged(ref searchPhrase, value);
        }

        public MainWindowViewModel(MyDbContext db, IManagedNotificationManager mnm)
        {
            this._db = db;
            _notificator = mnm;
            Content = new HomeViewModel();

            _fileManager = new FileManager(_db);
            //it crashes less this way
            Thread.Sleep(1000);
            //_fileManager.AddRootIfNotInDb("C:/Test");
            _fileManager.InitialDirectory();
            _fileManager.NewFilesEvent += Fm_NewFilesAsync;
            _tagManager = new TagManager(_db);
            ShowCustomManagedNotificationCommand = ReactiveCommand.Create( () =>
            {
               NotificationManager.Show(new NotificationViewModel(NotificationManager) { Title = "Tag hele!", Message = "Chceš otagovat nové soubory?\nSpusť z menu Správce tagů: File > Manage tags" });
            });
        }

        private void Fm_NewFilesAsync(object sender, NewFileEventArgs e)
        {
            newFiles.AddRange(e.Files);
            // we have to use UI thread or it will crash
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                ShowCustomManagedNotificationCommand.Execute().Subscribe();
            });
            // todo do something with new files

        }

        public void Search()
        {
           
            string search = SearchPhrase.Trim().ToLower();

            ViewModelBase vm;
            try
            {
                if (search.Length == 0)
                    //show all (except missing)
                    vm = SearchAndSubscribeToCommands(_fileManager.GetAllFiles());
                else
                    vm = SearchAndSubscribeToCommands(_fileManager.GetFilesWithTags(_tagManager.GetTagsByName(search.Split(' '), true)));
            }
            catch (Exception e)
            {
                vm = new ErrorViewModel(e.Message);
            }
            Content = vm;

        }

        private ViewModelBase SearchAndSubscribeToCommands(List<FileModel> files)
        {
            SearchResultsViewModel vm = new SearchResultsViewModel(files);
            vm.OpenFile.Subscribe((file) => OpenFile(file));

            return vm;
        }

        public void OpenFile(FileModel file)
        {
            var fileWithFileTags = _db.Files.Include(f => f.FileTags).Single(f => f.Name == file.Name);
            foreach(var x in fileWithFileTags.FileTags)
            {
               var t = _db.Tags.Find(x.TagId);
               x.Tag = t;
            }
            FileViewModel vm = new FileViewModel(fileWithFileTags);
            // here subscribe to commands (if any)
            Content = vm;
        }
        public IManagedNotificationManager NotificationManager
        {
            get { return _notificator; }
            set { this.RaiseAndSetIfChanged(ref _notificator, value); }
        }

        public void OpenDirectoryManager()
        {
            var w = new ManagerDialogWindow(MainWindow).AsDirectoryManagerDialog(_fileManager);
            w.ShowDialog(MainWindow);
        }

        public void OpenTagManager()
        {
            var w = new ManagerDialogWindow(MainWindow).AsTagManagerDialog(_fileManager, _tagManager);
            w.ShowDialog(MainWindow);
        }
    }
}

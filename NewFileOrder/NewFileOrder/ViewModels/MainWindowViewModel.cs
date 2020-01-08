﻿using Avalonia.Controls.Notifications;
using Avalonia.Threading;
using Microsoft.EntityFrameworkCore;
using NewFileOrder.Models;
using NewFileOrder.Models.DbModels;
using NewFileOrder.Models.Managers;
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
        private List<FileModel> newFiles = new List<FileModel>();

        private FileManager _fileManager;
        private TagManager _tagManager;
        private IManagedNotificationManager _notificator;
        ViewModelBase content;

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
            Thread.Sleep(1000);
            _fileManager.AddRootIfNotInDb("C:/Test");
            _fileManager.NewFilesEvent += fm_NewFilesAsync;
            _tagManager = new TagManager(_db);
            ShowCustomManagedNotificationCommand = ReactiveCommand.Create( () =>
            {
               NotificationManager.Show(new NotificationViewModel(NotificationManager) { Title = "Tag hele!", Message = "Chceš otagovat nové soubory?" });
            });
        }

        private void fm_NewFilesAsync(object sender, NewFileEventArgs e)
        {
            newFiles.AddRange(e.Files);
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                ShowCustomManagedNotificationCommand.Execute().Subscribe();
            });

        }

        public void Search()
        {
           
            string search = SearchPhrase.Trim().ToLower();

            ViewModelBase vm;

            if (search.Length == 0)
            {
                //show all, not missing!
                try
                {
                    vm = SearchAndSubscribeToCommands(_db.Files.Where(a => a.IsMissing == false).ToList());
                }
                catch (Exception e) {
                    vm = new ErrorViewModel(e.Message);
                }
            }
            else
            {
                try
                {
                    vm = SearchAndSubscribeToCommands(_fileManager.GetFilesWithTags(_tagManager.GetTagsByName(search.Split(' '), true)));
                }
                catch (Exception e)
                {
                    vm = new ErrorViewModel(e.Message);
                }
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
    }
}

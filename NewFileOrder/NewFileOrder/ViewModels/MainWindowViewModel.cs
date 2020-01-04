using NewFileOrder.Models;
using NewFileOrder.Models.DbModels;
using NewFileOrder.Models.Managers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading;

namespace NewFileOrder.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private MyDbContext _db;

        private FileManager _fileManager;
        private TagManager _tagManager;

        ViewModelBase content;

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

        public MainWindowViewModel(MyDbContext db)
        {
            this._db = db;
            Content = new HomeViewModel();

            _fileManager = new FileManager(_db);
            Thread.Sleep(5500);
            //_fileManager.AddRoot("C:/Users/janpr/Desktop/testdir");
            _tagManager = new TagManager(_db);
        }
        
        public void Search()
        {
            
            string search = SearchPhrase.Trim().ToLower();

            ViewModelBase vm;

            if (search.Length == 0)
            {
                //show all
                vm = SearchAndSubscribeToCommands(_db.Files.ToList());
            }
            else
            {
                try
                {
                    vm = SearchAndSubscribeToCommands(_fileManager.GetFilesWithTags(_tagManager.GetTagsByName(search.Split(' '))));
                } catch (Exception e)
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
            FileViewModel vm = new FileViewModel(file);
            // here subscribe to commands (if any)
            content = vm;
        }
    }
}

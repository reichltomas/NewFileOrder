using NewFileOrder.Models.DbModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
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

        string searchPhrase;

        public string SearchPhrase
        {
            get => searchPhrase;
            private set => this.RaiseAndSetIfChanged(ref searchPhrase, value);
        }

        public ReactiveCommand<Unit, string[]> Seefwrwerarch;

        public MainWindowViewModel()
        {
            Content = new HomeViewModel();
        }
        
        public void Search()
        {
            string[] tags = SearchPhrase.Trim().ToLower().Split(' ');

            if (tags.Length == 0)
            {
                //show all
            }
            else
            {
                //check if all tags exist. if yes, show matching files. If not, show error
            }
            //test code
            // vidím VELK7 3PATN7
            List<FileModel> x = new List<FileModel>();
            foreach (string s in tags)
                x.Add(new FileModel { Name = s });
            Content = new SearchResultsViewModel(x);

        }
    }
}

using NewFileOrder.Models.DbModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewFileOrder.ViewModels
{
    class SearchResultsViewModel : ViewModelBase
    {
        List<FileModel> results;

        public List<FileModel> Results
        {
            get => results;
            private set => this.RaiseAndSetIfChanged(ref results, value);
        }

        public ReactiveCommand<FileModel, FileModel> OpenFile { get; }

        public SearchResultsViewModel(List<FileModel> results)
        {
            Results = results;
            OpenFile = ReactiveCommand.Create<FileModel, FileModel>((file) => file);
        }
    }
}

using NewFileOrder.Models.DbModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NewFileOrder.Models.Managers;
using NewFileOrder.Views;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace NewFileOrder.ViewModels
{
    class TagManagerViewModel : ViewModelBase
    {
        private FileManager _fm;
        private TagManager _tm;

        List<FileModel> files;

        public List<FileModel> Files
        {
            get => files;
            private set => this.RaiseAndSetIfChanged(ref files, value);
        }

        FileModel activeFile;

        public FileModel ActiveFile
        {
            get => activeFile;
            set => SelectAnotherFile(value);
        }

        List<TagModel> tags;

        public List<TagModel> Tags
        {
            get => tags;
            private set => this.RaiseAndSetIfChanged(ref tags, value);
        }

        List<TagFilePair> tagFilePairs;

        public List<TagFilePair> TagFilePairs
        {
            get => tagFilePairs;
            private set => this.RaiseAndSetIfChanged(ref tagFilePairs, value);
        }

        TagFilePair activeTagFilePair;

        public TagFilePair ActiveTagFilePair
        {
            get => activeTagFilePair;
            set => this.RaiseAndSetIfChanged(ref activeTagFilePair, value);
        }

        public TagManagerViewModel(FileManager fm, TagManager tm)
        {
            _fm = fm;
            _tm = tm;

            Tags = _tm.GetAllTags();
            Files = _fm.GetAllFiles(false, true);
            ActiveFile = Files[0];
        }

        private void SelectAnotherFile(FileModel f)
        {
            if (TagFilePairs == null)
            {
                TagFilePairs = new List<TagFilePair>();
            }
            else
            {
                TagFilePairs.Clear();
            }
            foreach(var t in Tags)
                TagFilePairs.Add(new TagFilePair(t, f));
            ActiveTagFilePair = TagFilePairs[0];
            this.RaiseAndSetIfChanged(ref activeFile, f);
        }

        public void Add()
        {
            var newtag = new TagModel { Name = "" };
            Tags.Add(newtag);
            var newTFP = new TagFilePair(newtag, ActiveFile);
            TagFilePairs.Add(newTFP);
            ActiveTagFilePair = newTFP;
        }

        public void Remove()
        {
            Tags.Remove(ActiveTagFilePair.Tag);
            _tm.RemoveTag(ActiveTagFilePair.Tag);
            TagFilePairs.Remove(ActiveTagFilePair);
            ActiveTagFilePair = TagFilePairs.First();
        }

        public async void Save()
        {
            _tm.UpdateTagsInDb(Tags);
            await _fm.UpdateFilesInDB(Files);
        }
    }

    public class TagFilePair
    {
        public TagModel Tag { get; }
        public FileModel File { get; }
        public bool IsAssigned
        {
            get => File.FileTags.Any(ft => ft.Tag == Tag);
            set
            {
                if (value && !IsAssigned)
                    File.FileTags.Add(new FileTag { File = File, Tag = Tag });
                else if (!value && IsAssigned)
                    File.FileTags.Remove(File.FileTags.Single(ft => ft.Tag == Tag));
            }
        }

        public TagFilePair(TagModel t, FileModel f)
        {
            Tag = t; File = f;
        }
    }
}

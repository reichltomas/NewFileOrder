using NewFileOrder.Models.DbModels;
using ReactiveUI;
using System.Diagnostics;
using System.Drawing;

namespace NewFileOrder.ViewModels
{
    class FileViewModel : ViewModelBase
    {
        FileModel file;
        string pth;
        public string Pth { get => pth;  }

        public FileModel File
        {
            get => file;
            private set => this.RaiseAndSetIfChanged(ref file, value);
        }

        public FileViewModel (FileModel file)
        {
            File = file;
            pth = file.Path + file.Name;
           //var x = Icon.ExtractAssociatedIcon(file.FullPath).ToBitmap();
        }

        public void Open()
        {
            try
            {
                new Process
                {
                    StartInfo = new ProcessStartInfo($"{file.Path}/{file.Name}")
                    {
                        UseShellExecute = true
                    }
                }.Start();
            }
            catch
            {
                //error in file system idk what to do about that
            }
        }
    }
}

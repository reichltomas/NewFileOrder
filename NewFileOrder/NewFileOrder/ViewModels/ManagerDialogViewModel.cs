using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using NewFileOrder.Models.Managers;
using NewFileOrder.Views;
using ReactiveUI;

namespace NewFileOrder.ViewModels
{
    class ManagerDialogViewModel : ViewModelBase
    {
        private ManagerDialogWindow _dialogWindow;
        private MainWindow _mainWindow;

        ViewModelBase content;

        public ViewModelBase Content
        {
            get => content;
            private set => this.RaiseAndSetIfChanged(ref content, value);
        }

        public ManagerDialogViewModel(ManagerDialogWindow mdw, MainWindow mw) 
        {
            _dialogWindow = mdw;
            _mainWindow = mw;
        }

        public void CreateTagManagerDialog(FileManager fm, TagManager tm) 
        {
            try
            {
                if (tm.GetAllTags().Count == 0)
                    tm.AddTag("novy_tag");

                if (fm.GetAllFiles().Count == 0)
                {
                    _dialogWindow.Width = 160;
                    _dialogWindow.Height = 120;
                    _dialogWindow.Title = "Chyba!";
                    Content = new ErrorViewModel("V databázi chybí soubory. Přidejte prosím soubory do některého z indexovaných adresářů, přidejte nový adresář nebo (jsou-li v indexovaných adresářích soubory) počkejte, než je aplikace na pozadí zaindexuje (to může chvíli trvat).");
                }
                else
                {
                    _dialogWindow.Width = 960;
                    _dialogWindow.Height = 640;
                    _dialogWindow.Title = "Správce tagů";
                    Content = new TagManagerViewModel(fm, tm);
                }
            }
            catch(Exception e)
            {
                Content = new ErrorViewModel(e.Message);
            }
        }

        public void CreateDirectoryManagerDialog(FileManager fm)
        {
            try
            { 
                if (fm.GetAllDirectories().Count == 0)
                {
                    _dialogWindow.Width = 160;
                    _dialogWindow.Height = 120;
                    _dialogWindow.Title = "Chyba!";
                    Content = new ErrorViewModel("Došlo k chybě v databázi. Zkuste prosím restartovat aplikaci.");
                    // todo figure out dialogs
                    /*
                    var dialog = new OpenFolderDialog() { Title = "Vyberte adresář k přidání do databáze..." };
                    var path = dialog.ShowAsync(_mainWindow).GetAwaiter().GetResult();
                    fm.AddRoot(path);*/
                }
                else
                {
                    _dialogWindow.Width = 420;
                    _dialogWindow.Height = 320;
                    _dialogWindow.Title = "Správce adresářů"; // TODO change to binding from view = do it the right way
                    Content = new DirectoryManagerViewModel(fm, _dialogWindow);
                }
            }
            catch (Exception e)
            {
                Content = new ErrorViewModel(e.Message);
            }
        }
    }
}

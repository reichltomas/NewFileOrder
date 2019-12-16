﻿using NewFileOrder.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;

namespace NewFileOrder.ViewModels
{
    class FileViewModel : ViewModelBase
    {
        FileModel file;
        public FileModel File
        {
            get => file;
            private set => this.RaiseAndSetIfChanged(ref file, value);
        }

        public FileViewModel (FileModel file)
        {
            File = file;
        }
    }
}
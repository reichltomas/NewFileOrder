using NewFileOrder.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewFileOrder.Models.Managers
{
    class NewFileEventArgs : EventArgs
    {
        public List<FileModel> Files{get;set ;}
    }
}

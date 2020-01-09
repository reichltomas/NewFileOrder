using NewFileOrder.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewFileOrder.Models.Managers
{
    public class NewFileEventArgs : EventArgs
    {
        public List<FileModel> Files{get;set ;}
    }
}

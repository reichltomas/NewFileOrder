using System;
using System.Collections.Generic;
using System.Text;

namespace NewFileOrder.Models.DbModels
{
    interface IFileSystemEntry
    {
        string Name { get; set; }
        string Path { get; set; }
    }
}

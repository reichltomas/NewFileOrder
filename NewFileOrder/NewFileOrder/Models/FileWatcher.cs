using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NewFileOrder.Models
{
    class FileWatcher
    {
        public event FileSystemEventHandler OnChanged;
        public event FileSystemEventHandler OnCreated;
        public event FileSystemEventHandler OnDeleted;
        public event RenamedEventHandler OnRenamed;
        private FileSystemWatcher _filesWatcher;
    }
}

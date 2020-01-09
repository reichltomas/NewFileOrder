using System;
using System.Collections.Generic;
using System.Text;

namespace NewFileOrder.Models.DbModels
{
    public class FileTag
    {
        public int FileId { get; set; }
        public FileModel File { get; set; }

        public int TagId { get; set; }
        public TagModel Tag { get; set; }
    }
}

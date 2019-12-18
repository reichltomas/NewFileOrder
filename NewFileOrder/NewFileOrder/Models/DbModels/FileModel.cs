﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NewFileOrder.Models.DbModels
{
    public class FileModel
    {
        [Key]
        public int FileId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Path { get; set; }
        [Required]
        public string Hash { get; set; }
        [Required]
        public DateTime LastChecked { get; set; }
        public string Metadata { get; set; }
        public IList<FileTag> FileTags { get; set; } 
        public bool IsMissing { get; set; } = false;

    }
}

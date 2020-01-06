using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NewFileOrder.Models.DbModels
{
    public class FileModel:IFileSystemEntry
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
        [Required]
        public DateTime Created { get; set; }
        public string Metadata { get; set; }
        public IList<FileTag> FileTags { get; set; } 
        public bool IsMissing { get; set; } = false;
        [NotMapped]
        public string FullPath
        {
            get => Path + "/" + Hash;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NewFileOrder.Models.DbModels
{
   public class DirectoryModel : IFileSystemEntry 
    {
        [Key]
        public int DirectoryId { get; set; }
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Path { get; set; }
        public virtual ICollection<FileModel> Files { get; set; }
        //public List<TagModel> Tags { get; set; } = new List<TagModel>();
        public List<DirectoryModel> Directories { get; set; } = new List<DirectoryModel>();
        [Required]
        public string Hash { get; set; }
        public bool IsRoot { get; set; } = false;
        [NotMapped]
        public string FullPath
        {
            get => Path + "/" + Hash;
        }
    }
}

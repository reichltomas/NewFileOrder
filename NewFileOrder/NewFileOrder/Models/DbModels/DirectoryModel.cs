using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NewFileOrder.Models.DbModels
{
    class DirectoryModel 
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
    }
}

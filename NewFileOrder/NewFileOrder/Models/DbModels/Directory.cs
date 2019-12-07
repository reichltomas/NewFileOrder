using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NewFileOrder.Models.DbModels
{
    class Directory
    {
        [Key]
        public int DirectoryId { get; set; }
        [Required]
        public string Path { get; set; }
        public List<File> Files { get; set; } = new List<File>();
        public List<Tag> Tags{ get; set; } = new List<Tag>();
        public List<Directory> Directories { get; set; } = new List<Directory>();
        //only content
        [Required]
        public string DirHash1 { get; set; }
        // Name and content
        [Required]
        public string DirHash2 { get; set; }
    }
}

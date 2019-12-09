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
        public string Path { get; set; }
        public List<FileModel> Files { get; set; } = new List<FileModel>();
        public List<TagModel> Tags{ get; set; } = new List<TagModel>();
        public List<DirectoryModel> Directories { get; set; } = new List<DirectoryModel>();
        //only content
        [Required]
        public string DirHash1 { get; set; }
        // Name and content
        [Required]
        public string DirHash2 { get; set; }
    }
}

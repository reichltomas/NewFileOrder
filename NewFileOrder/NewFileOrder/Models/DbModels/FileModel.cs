using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NewFileOrder.Models.DbModels
{
    class FileModel
    {
        [Key]
        public int FileId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Path { get; set; }
        [Required]
        public string Hash { get; set; }
        [Required]
        public DateTime LastChecked { get; set; }
        public string Metadata { get; set; }
        public List<TagModel> Tags { get; set; } = new List<TagModel>();
        public bool IsMissing { get; set; } = false;

    }
}

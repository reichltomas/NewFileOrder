using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NewFileOrder.Models.DbModels
{
    class File
    {
        [Key]
        public int FileId { get; set; }
        [Required]
        public string Path { get; set; }
        [Required]
        public string Hash { get; set; }
        [Required]
        public DateTime LastChecked { get; set; }
        public string Metadata { get; set; }
        public List<Tag> Tags { get; set; } = new List<Tag>();
        //public bool Missing { get; set; } = false;

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewFileOrder.Models.DbModels
{
    class TagModel
    {
        [Key]
        public int TagId { get; set; }
        [Required]
        public string Name { get; set; }
        public virtual ICollection<FileModel> Files { get; set; }
    }
}

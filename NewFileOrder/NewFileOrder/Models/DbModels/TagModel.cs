using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewFileOrder.Models.DbModels
{
    public class TagModel
    {
        [Key]
        public int TagId { get; set; }
        [Required]
        public string Name { get; set; }
        public IList<FileTag> FileTags { get; set; }
    }
}

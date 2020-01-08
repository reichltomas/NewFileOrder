using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace NewFileOrder.Models.DbModels
{
    public class TagModel
    {
        [Key]
        public int TagId { get; set; }
        [Required]
        public string Name { get; set; }
        public IList<FileTag> FileTags { get; set; }

        public bool IsAssignedToFile(FileModel file)
        {
            return FileTags.Where(ft => ft.FileId == file.FileId).Count() == 1;
        }
    }
}

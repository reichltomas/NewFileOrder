﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NewFileOrder.Models.DbModels
{
    class TagModel
    {
        [Key]
        public int TagId { get; set; }
        [Required]
        public string Name { get; set; }

    }
}
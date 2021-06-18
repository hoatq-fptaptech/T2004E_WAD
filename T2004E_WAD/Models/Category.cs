﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace T2004E_WAD.Models
{
    public class Category
    {
        [Required(ErrorMessage ="Vui lòng nhập tên danh mục")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập ảnh danh mục")]
        public string Image { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mô tả danh mục")]
        public string Description { get; set; }
    }
}
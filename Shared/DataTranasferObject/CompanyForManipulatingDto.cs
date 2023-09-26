﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTranasferObject
{
    public abstract record  CompanyForManipulatingDto
    {
        [Required(ErrorMessage = "Company name is a required field.")]
        [MaxLength(30, ErrorMessage = "Maximum length for the Name is 30 characters.")]
        public string? Name { get; init; }
        
        [Required(ErrorMessage = "Company address is a required field.")]
        [MaxLength(30, ErrorMessage = "Maximum length for the Company address is 30 characters.")]
        public string? Address { get; init; }

        [Required(ErrorMessage = "Country is a required field.")]
        [MaxLength(30, ErrorMessage = "Maximum length for the Country is 30 characters.")]
        public string? Country { get; init; }
 
    }
}

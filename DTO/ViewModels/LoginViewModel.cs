﻿using System.ComponentModel.DataAnnotations;
using BankMvc.DTO.ViewModels;

namespace BankMvc.DTO.ViewModels
{

    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

    }
}


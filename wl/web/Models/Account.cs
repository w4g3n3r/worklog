using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace web.Models
{
    public class Account
    {
        private string _redirectUrl;

        public Account()
        {
        }

        [Required]
        [Display(Name = "Username", Order = 1)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password", Order = 2)]
        public string Password { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string RedirectUrl {
            get
            {
                return _redirectUrl ?? "~/";
            }
            set
            {
                _redirectUrl = value;
            }
        }
    }
}
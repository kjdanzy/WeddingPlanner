using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingPlanner.Models
{
    public class LoginUser
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string LoginEmail { get; set; }
        [Required]
        [DataType(DataType.Password)]        
        public string LoginPassword { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace WeddingPlanner.Models
{
    public class Wedding
    {
        [Key]
        public int WeddingId { get; set; }
        [Required]
        public string WedderOne { get; set; }
        [Required]
        public string WedderTwo { get; set; }
        [Required]
        [MustBeFutureDate(2)]
        public DateTime WeddingDate { get; set; }
        [Required]
        public string WeddingAddress { get; set; }
        public int UserId { get; set; }
        public User Creator { get; set; }
        public List<WeddingParticipant> WedNParticipants { get; set; }
    }

    public class MustBeFutureDate : ValidationAttribute
    {
        private int _monthsToAdd { get; set; }

        public MustBeFutureDate (int monthsToAdd){
            _monthsToAdd = monthsToAdd;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            if (value is DateTime)
            {
                DateTime checkMe;
                checkMe = (DateTime)value;

                if (!(checkMe > DateTime.Now.AddMonths(_monthsToAdd)))
                {
                    return new ValidationResult(string.Format("Your date must be at least {0} month(s) from now. Please check your selected 'Wedding Date'.", _monthsToAdd));
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
            else
            {
                return new ValidationResult("not a 'DateTime'");
            }
        }
    }
}
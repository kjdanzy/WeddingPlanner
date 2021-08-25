using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace WeddingPlanner.Models
{
    public class WeddingParticipant
    {
        [Key]
        public int WeddingParticipantId { get; set; }
        public int WeddingId { get; set; }
        public Wedding Wedding { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int UserTypeId { get; set; }
        //public WeddingParticipant WedNParticipant { get; set; }


    }
}
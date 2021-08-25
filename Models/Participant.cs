// using System;
// using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;
// using System.Collections.Generic;

// namespace WeddingPlanner.Models
// {
//     public class Participant
//     {
//         [Key]
//         public int ParticipantId { get; set; }
//         [Required]
//         [MinLength(3)]
//         public string ParticipantFirstName { get; set; }
//         [Required]
//         [MinLength(3)]
//         public string ParticipantLastName { get; set; }
//         public List<WeddingParticipant> WeddingParticipants { get; set; }
//     }
// }
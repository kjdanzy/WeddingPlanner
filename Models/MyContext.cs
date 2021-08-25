using Microsoft.EntityFrameworkCore;

namespace WeddingPlanner.Models
{ 
    // the MyContext class representing a session with our MySQL 
    // database allowing us to query for or save data
    public class MyContext : DbContext 
    { 
        public MyContext(DbContextOptions options) : base(options) { }
        // the "LoginReg" table name will come from the DbSet variable name
        public DbSet<User> Users { get; set; }
        public DbSet<Wedding> Weddings { get; set; }
        // public DbSet<Participant> Participants { get; set; }
        public DbSet<WeddingParticipant> WeddingParticipants { get; set; }
    }
}
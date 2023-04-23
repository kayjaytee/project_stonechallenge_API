using Microsoft.EntityFrameworkCore;
using RockPaperScissorsAPI.Model;
using RockPaperScissorsAPI.Validation;

namespace RockPaperScissorsAPI.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) :base(options) { }

    // nameof dbset<> => Set<PlayerInvite>();

    public virtual DbSet<User> User { get; set; } = null!;
    public virtual DbSet<Game> Game { get; set; } = null!;
    public virtual DbSet<Message> Message { get; set; } = null!;
    public virtual DbSet<PlayerInvite> PlayerInvite { get; set; } = null!;
    public virtual DbSet<PlayerMoves> PlayerMoves { get; set; } = null!;
    public virtual DbSet<LoginUser> LoginUser { get; set; } = null!;
    public virtual DbSet<EmailUser> ResetPasswordUser { get; set; } = null!;
    public virtual DbSet<UserScore> UserScore{ get; set; } = null!;

    //Necessery to control Keys
    protected override void OnModelCreating(ModelBuilder modelbuilder)
    {
        modelbuilder.Entity<User>()
        .HasKey(x => new { x.UserID });

        modelbuilder.Entity<PlayerInvite>()
        .HasKey(x => new { x.InviteFromUserID, x.InviteToUserID });

        modelbuilder.Entity<PlayerMoves>()
        .HasKey(x => new { x.MoveID });

        modelbuilder.Entity<Game>()
        .HasKey(x => new { x.PlayerOneID, x.PlayerTwoID });

        modelbuilder.Entity<Message>()
        .HasKey(x => new { x.FromUserID, x.ToUserID });

        //

        modelbuilder.Entity<LoginUser>().HasNoKey();

        modelbuilder.Entity<EmailUser>().HasNoKey();

        modelbuilder.Entity<ResetPassword>().HasNoKey();

        modelbuilder.Entity<UserScore>()
            .HasKey(x => new { x.UserID });




    }


}

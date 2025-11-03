using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shared.Helper;
using Twilio.TwiML.Voice;

namespace Infrastructure.Data
{
     public class ApplicationDbContext : IdentityDbContext<User,Role,string >
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }




        public override int SaveChanges()
        {
            applyToAll();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            applyToAll();
            //return await base.SaveChangesAsync(cancellationToken);
            try
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($" SaveChangesAsync Error: {ex.Message}");
                Console.WriteLine($" Inner: {ex.InnerException?.Message}");
                throw; 
            }
        }


        private void applyToAll()
        {
            var entries = ChangeTracker.Entries<IAuditable>();
            var user = _httpContextAccessor.HttpContext?.User;
            string? currentUser = UserInfoHelper.GetUserId(user);
            var currentTime = DateTime.Now;

            foreach (var entry in entries)
            {
                if(entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedBy = currentUser;
                    entry.Entity.CreatedAt = currentTime;
                }
                else if(entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedBy = currentUser;
                    entry.Entity.UpdatedAt = currentTime;
                }
                else if(entry.State == EntityState.Deleted)
                {
                    entry.Entity.DeletedBy = currentUser;
                    entry.Entity.DeletedAt = currentTime;
                }             
            }
        }






        public DbSet<Movie> Movies { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<Rating> Rating { get; set; }
        public DbSet<Genre> Genre { get; set; }
        public DbSet<MovieGenre> MovieGenre { get; set; }
        public DbSet<Domain.Entities.Language> Language { get; set; }
        public DbSet<MovieLanguage> MovieLanguage { get; set; }
        public DbSet<GoogleUser> GoogleUser { get; set; }
        public DbSet<WatchList> WatchList { get; set; }

        public DbSet<RefreshToken> RefreshToken { get; set; }
        public DbSet<WaterBill> WaterBill { get; set; }
        public DbSet<ElectricityBill> ElectricityBill { get; set; }
        public DbSet<PaymentTransaction> PaymentTransaction { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillType> BillTypes { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<DeviceToken> DeviceToken { get; set; }

        public DbSet<Notification> Notification { get; set; }
        public DbSet<SMS> SMS { get; set; }
        public DbSet<DeleteRequest> DeleteRequest { get; set; }
        public DbSet<DeleteApproval> DeleteApproval { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Rename the Identity tables
            builder.Entity<User>().ToTable("Users");
            builder.Entity<Role>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");




            //builder.Entity<UserOtp>()
            //    .HasOne(uo => uo.user)
            //    .WithMany() 
            //    .HasForeignKey(uo => uo.userId)
            //    .IsRequired();




            builder.Entity<Comment>()
                .HasOne(c => c.Movie)
                .WithMany(m => m.Comment)
                .HasForeignKey(c => c.MovieId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comment)
                .HasForeignKey(c => c.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<Rating>()
                .HasOne(r => r.Movie)
                .WithMany(m => m.Rating)
                .HasForeignKey(r => r.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Rating>()
                .HasOne(r => r.User)
                .WithMany(u => u.Rating)
                .HasForeignKey(r => r.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<MovieGenre>()
                .HasOne(mg => mg.Movie)
                .WithMany(m => m.MovieGenre)
                .HasForeignKey(mg => mg.MovieId);

            builder.Entity<MovieGenre>()
                .HasOne(mg => mg.Genre)
                .WithMany(g => g.MovieGenre)
                .HasForeignKey(mg => mg.GenreId);

            builder.Entity<MovieGenre>()
                .HasKey(mg => new { mg.MovieId, mg.GenreId });


            builder.Entity<MovieLanguage>()
                .HasOne(ml => ml.Movie)
                .WithMany(m => m.MovieLanguage)
                .HasForeignKey(ml => ml.MovieId);

            builder.Entity<MovieLanguage>()
                .HasOne(ml => ml.Language)
                .WithMany(l => l.MovieLanguage)
                .HasForeignKey(ml => ml.LanguageId);

            builder.Entity<MovieLanguage>()
                .HasKey(ml => new { ml.MovieId, ml.LanguageId });


            builder.Entity<Movie>()
                .HasOne(m => m.User)
                .WithMany(u => u.Movie)
                .HasForeignKey(m => m.CreatedBy)     //FK
                .HasPrincipalKey(u => u.Id)          // PK in User
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<WatchList>()
                 .HasOne(w => w.User)
                 .WithMany(u => u.WatchList)
                 .HasForeignKey(w => w.UserId)
                 .HasPrincipalKey(u => u.Id)
                 .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<WatchList>()
                .HasOne(w => w.Movie)
                .WithMany(m => m.WatchList)
                .HasForeignKey(w => w.MovieId)
                .HasPrincipalKey(m => m.Id)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<WaterBill>()
              .HasOne(w => w.User)
             .WithMany(u => u.WaterBill)
             .HasForeignKey(w => w.UserId)
             .OnDelete(DeleteBehavior.Restrict);



            builder.Entity<WaterBill>()
                   .Property(w => w.Amount)
                   .HasColumnType("decimal(9,2)");


            builder.Entity<ElectricityBill>()
                 .HasOne(e => e.User)
                 .WithMany(u => u.ElectricityBill)
                 .HasForeignKey(e => e.UserId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ElectricityBill>()
                .Property(e => e.Amount)
                .HasColumnType("decimal(9,2)");

            builder.Entity<Bill>(entity =>
            {
                entity.HasKey(b => b.Id);

                entity.Property(b => b.TotalAmount)
                      .HasColumnType("decimal(18,2)");

                entity.HasOne(b => b.BillType)
                      .WithMany(bt => bt.Bills)
                      .HasForeignKey(b => b.BillTypeId);

                entity.HasOne(b => b.User)
                      .WithMany(u => u.Bills)
                      .HasForeignKey(b => b.UserId)
                      .HasPrincipalKey(u => u.Id);
            });


            builder.Entity<Organization>()
                .HasMany(o => o.Users)
                .WithOne(u => u.Organization)
                .HasForeignKey(u => u.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<DeleteRequest>()
                .HasMany(dr => dr.Approval)
                .WithOne(da => da.DeleteRequest)
                .HasForeignKey(da => da.DeleteRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<DeleteApproval>()
                .HasKey(da => da.ApprovalId);




        }



    }
}

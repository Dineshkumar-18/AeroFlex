using Microsoft.EntityFrameworkCore;
using AeroFlex.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace AeroFlex.Data
{
	public class ApplicationDbContext : DbContext
	{
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<UserRoleMapping> RoleMappings { get; set; }
        public DbSet<FlightOwner> FlightOwners { get; set; }
		public DbSet<Class> Classes { get; set; }
		public DbSet<Country> Countries { get; set; }
		public DbSet<Currency> Currencies { get; set; }
		public DbSet<Address> Addresses { get; set; }	
        public DbSet<Airline> Airlines { get; set; }
        public DbSet<Airport> Airports { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Flight> Flights { get; set; }
		public DbSet<FlightPricing> FlightsPricings { get; set; }
        public DbSet<FlightScheduleClass> FlightScheduleClasses { get; set; }
        public DbSet<FlightSchedule> FlightsSchedules { get; set; }
        public DbSet<FlightTax> FlightTaxes { get; set; }
        public DbSet<Passenger> Passengers { get; set; }    
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Refund> Refunds { get; set; }
		public DbSet<CancellationFee> CancellationFees { get; set; }
		public DbSet<CancellationInfo> CancellationInfos { get; set; }
        public DbSet<Seat> Seats { get; set; }
		public DbSet<SeatTypePricing> SeatTypePricings { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
		public DbSet<RefreshTokenInfo> RefreshTokenInfos { get; set; }

		public DbSet<CountryTax> CountryTaxes { get; set; }


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

            modelBuilder.Entity<User>()
            .ToTable("Users");

            modelBuilder.Entity<FlightOwner>()
                .ToTable("FlightOwners")
                .HasBaseType<User>(); // Configure inheritance relationship

            modelBuilder.Entity<Admin>()
                .ToTable("Admins")
                .HasBaseType<User>(); // Configure inheritance relationship

            //user entity
            modelBuilder.Entity<User>(entity =>
			{

               entity.HasOne(u => u.Address)   // Navigation from User to Address
        .WithOne(a => a.User)      // Navigation from Address to User
        .HasForeignKey<User>(u => u.AddressId)  // Explicitly set AddressId as FK
        .IsRequired(false)         // Make the relationship optional
        .OnDelete(DeleteBehavior.NoAction);


            });
			//booking entity
			modelBuilder.Entity<Booking>(entity =>
			{
				entity.HasOne(b => b.FlightSchedule)
				.WithMany(fs => fs.Bookings)
				.HasForeignKey(b => b.FlightScheduleId)
				.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(b => b.User)
				.WithMany(u => u.Bookings)
				.HasForeignKey(b => b.UserId)
				.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(b => b.FlightPricing)
				.WithMany(fp => fp.Bookings)
				.HasForeignKey(b => b.FlightPricingId)
				.OnDelete(DeleteBehavior.Restrict);
			});
			//passenger entity
			modelBuilder.Entity<Passenger>(entity =>
			{
				entity.HasOne(p => p.Booking)
			   .WithMany(b => b.Passengers)
			   .HasForeignKey(p => p.BookingId)
			   .OnDelete(DeleteBehavior.Cascade);
			});

			//Seat entity
			modelBuilder.Entity<Seat>(entity =>
			{
				entity.HasOne(s => s.FlightSchedule)
				.WithMany(fs => fs.Seats)
				.HasForeignKey(s => s.FlightScheduleId)
				.OnDelete(DeleteBehavior.Cascade);

				entity.HasOne(s => s.FlightScheduleClass)
				.WithMany(fsc => fsc.Seats)
				.HasForeignKey(s => s.FlightScheduleClassId)
				.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(s => s.Passenger)
				.WithOne(p => p.Seat)
				.OnDelete(DeleteBehavior.NoAction)
				.IsRequired(false);

				entity.HasOne(s => s.Booking)
				.WithMany(b => b.Seats)
				.HasForeignKey(s => s.BookingId)
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired(false);

				entity.HasOne(s => s.SeatTypePricing)
				.WithMany(stp => stp.Seats)
				.HasForeignKey(s => s.SeatTypePricingId)
				.OnDelete(DeleteBehavior.Restrict);

			});

			//flight entity
			modelBuilder.Entity<Flight>(entity =>
			{
				entity.HasOne(f => f.Airline)
				.WithMany(a => a.Flights)
				.HasForeignKey(f => f.AirlineId)
				.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(f => f.DepartureAirport)
				.WithMany(a => a.Departures)
				.HasForeignKey(f => f.DepartureAirportId)
				.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(f => f.ArrivalAirport)
				.WithMany(a => a.Arrivals)
				.HasForeignKey(f => f.ArrivalAirportId)
				.OnDelete(DeleteBehavior.Restrict);
			});


			modelBuilder.Entity<FlightTax>()
			.HasOne(ft => ft.Country)
			.WithMany(c => c.FlightTaxes)
			.HasForeignKey(ft => ft.CountryId);

			modelBuilder.Entity<FlightTax>()
				.HasOne(ft => ft.FlightClass)
				.WithMany(fc => fc.FlightTaxes)
				.HasForeignKey(ft => ft.ClassId);

			modelBuilder.Entity<FlightTax>()
				.HasOne(ft => ft.Currency)
				.WithMany(c => c.FlightTaxes)
				.HasForeignKey(ft => ft.CurrencyId);




			// FlightSchedule to Departure Airport relationship
			modelBuilder.Entity<FlightSchedule>()
		   .HasOne(fs => fs.DepartureAirport)
		   .WithMany(a => a.ScheduleDepartures)
		   .HasForeignKey(fs => fs.DepartureAirportId)
		   .OnDelete(DeleteBehavior.Restrict);

			// FlightSchedule to Arrival Airport relationship
			modelBuilder.Entity<FlightSchedule>()
				.HasOne(fs => fs.ArrivalAirport)
				.WithMany(a => a.ScheduleArrivals)
				.HasForeignKey(fs => fs.ArrivalAirportId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<FlightSchedule>()
				.HasOne(fs => fs.Flight)
				.WithMany(a => a.FlightSchedules)
				.HasForeignKey(fs => fs.FlightId)
				.OnDelete(DeleteBehavior.Restrict);


			//
			//
			//
			//
			//
			//
			//
			//
			//model 
			modelBuilder.Entity<FlightTax>(entity =>
			{

				entity.HasOne(ft => ft.Country)
				.WithMany(c => c.FlightTaxes)
				.HasForeignKey(ft => ft.CountryId)
				.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(ft => ft.FlightClass)
				.WithMany(c => c.FlightTaxes)
				.HasForeignKey(ft => ft.ClassId)
				.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(ft => ft.Currency)
				.WithMany(c => c.FlightTaxes)
				.HasForeignKey(ft => ft.CurrencyId)
				.OnDelete(DeleteBehavior.Restrict);

			});

			//Seat type pricing model

			modelBuilder.Entity<SeatTypePricing>(entity =>
			{
				entity.HasOne(stp => stp.FlightSchedule)
				.WithMany(fs => fs.SeatTypePricing)
				.HasForeignKey(stp => stp.FlightScheduleId)
				.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(stp => stp.FlightScheduleClass)
				.WithMany(fsc => fsc.SeatTypePricings)
				.HasForeignKey(stp => stp.FlightScheduleClassId)
				.OnDelete(DeleteBehavior.Restrict);

			});

			modelBuilder.Entity<CancellationInfo>(entity =>
			{
				entity.HasOne(ci=>ci.FlightSchedule)
				.WithMany(fs=>fs.CancellationInfos)
				.HasForeignKey(ci=>ci.CancellationId)
				.OnDelete(DeleteBehavior.NoAction);

				entity.HasOne(ci => ci.CancelledSeat)
				.WithOne(s => s.CancellationInfo)
				.OnDelete(DeleteBehavior.NoAction);

				entity.HasOne(ci=>ci.Passenger)
				.WithOne(p=>p.CancellationInfo)
				.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(ci => ci.CancellationFee)
				.WithOne(cf => cf.CancellationInfo)
				.OnDelete(DeleteBehavior.Restrict);

            });

          

            modelBuilder.Entity<UserRoleMapping>()
            .HasKey(urm => new { urm.UserId, urm.RoleId });

            modelBuilder.Entity<UserRoleMapping>()
                .HasOne(urm => urm.User)
                .WithMany(u => u.RoleMappings)
                .HasForeignKey(urm => urm.UserId);

            modelBuilder.Entity<UserRoleMapping>()
                .HasOne(urm => urm.Role)
                .WithMany(r=>r.UserRoleMappings)
                .HasForeignKey(urm => urm.RoleId);

        }

    }

}

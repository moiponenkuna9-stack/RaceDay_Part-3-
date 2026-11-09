using Microsoft.EntityFrameworkCore;
using RaceDay.Infrastructure;
using RaceDay.Models;

namespace RaceDay.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(RaceDayDbContext db, IPasswordService passwordService, ILogger logger)
    {
        try
        {
            await db.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            // Keep the API available so Swagger can be opened even when SQL Server/LocalDB
            // has not yet been installed or the connection string needs to be configured.
            logger.LogWarning(ex, "RaceDay database could not be initialized. Configure SQL Server/LocalDB and restart the API.");
            return;
        }
        if (await db.Users.AnyAsync()) return;

        var organiser1 = new User { FirstName = "Thabo", LastName = "Mokoena", Email = "thabo.organiser@raceday.test", Role = UserRole.Organiser, Phone = "0710000001", PasswordHash = passwordService.Hash("Password123!") };
        var organiser2 = new User { FirstName = "Lerato", LastName = "Maseko", Email = "lerato.organiser@raceday.test", Role = UserRole.Organiser, Phone = "0710000002", PasswordHash = passwordService.Hash("Password123!") };
        var participant1 = new User { FirstName = "Naledi", LastName = "Molefe", Email = "naledi.participant@raceday.test", Role = UserRole.Participant, Phone = "0710000003", PasswordHash = passwordService.Hash("Password123!") };
        var participant2 = new User { FirstName = "Kabelo", LastName = "Dlamini", Email = "kabelo.participant@raceday.test", Role = UserRole.Participant, Phone = "0710000004", PasswordHash = passwordService.Hash("Password123!") };
        db.Users.AddRange(organiser1, organiser2, participant1, participant2);
        await db.SaveChangesAsync();

        db.UserProfiles.AddRange(
            new UserProfile { UserId = participant1.UserId, DateOfBirth = new DateOnly(2004, 5, 14), Gender = "Female", EmergencyContactName = "Mpho Molefe", EmergencyContactPhone = "0721000003", Address = "Pretoria, Gauteng" },
            new UserProfile { UserId = participant2.UserId, DateOfBirth = new DateOnly(2003, 11, 2), Gender = "Male", EmergencyContactName = "Neo Dlamini", EmergencyContactPhone = "0721000004", Address = "Johannesburg, Gauteng" });

        var c1 = new Category { Name = "5 KM Fun Run", DistanceKm = 5, MaxParticipants = 500 };
        var c2 = new Category { Name = "10 KM Challenge", DistanceKm = 10, MaxParticipants = 750 };
        var c3 = new Category { Name = "21 KM Half Marathon", DistanceKm = 21.1m, MaxParticipants = 1000 };
        var c4 = new Category { Name = "42 KM Marathon", DistanceKm = 42.2m, MaxParticipants = 1200 };
        db.Categories.AddRange(c1, c2, c3, c4);
        await db.SaveChangesAsync();

        var today = DateTime.Today;
        var e1 = new Event { OrganiserId = organiser1.UserId, Name = "Pretoria Spring Run", Description = "Community road race through central Pretoria.", EventDate = today.AddDays(30), Location = "Pretoria National Botanical Garden", DistanceKm = 10, EventType = EventType.Run, RegistrationOpen = today.AddDays(-30), RegistrationClose = today.AddDays(20), Status = EventStatus.Open };
        var e2 = new Event { OrganiserId = organiser1.UserId, Name = "Johannesburg City Challenge", Description = "Road-running event for recreational and competitive runners.", EventDate = today.AddDays(50), Location = "Mary Fitzgerald Square", DistanceKm = 21.1m, EventType = EventType.Run, RegistrationOpen = today.AddDays(-20), RegistrationClose = today.AddDays(40), Status = EventStatus.Open };
        var e3 = new Event { OrganiserId = organiser2.UserId, Name = "Gauteng Heritage Cycle", Description = "A community cycling event.", EventDate = today.AddDays(70), Location = "Cradle of Humankind Visitor Centre", DistanceKm = 42.2m, EventType = EventType.Cycle, RegistrationOpen = today.AddDays(-10), RegistrationClose = today.AddDays(60), Status = EventStatus.Open };
        db.Events.AddRange(e1, e2, e3);
        await db.SaveChangesAsync();

        db.EventCategories.AddRange(
            new EventCategory { EventId = e1.EventId, CategoryId = c1.CategoryId, EntryFee = 80 },
            new EventCategory { EventId = e1.EventId, CategoryId = c2.CategoryId, EntryFee = 120 },
            new EventCategory { EventId = e1.EventId, CategoryId = c3.CategoryId, EntryFee = 180 },
            new EventCategory { EventId = e2.EventId, CategoryId = c1.CategoryId, EntryFee = 90 },
            new EventCategory { EventId = e2.EventId, CategoryId = c2.CategoryId, EntryFee = 140 },
            new EventCategory { EventId = e2.EventId, CategoryId = c3.CategoryId, EntryFee = 200 },
            new EventCategory { EventId = e3.EventId, CategoryId = c2.CategoryId, EntryFee = 150 },
            new EventCategory { EventId = e3.EventId, CategoryId = c3.CategoryId, EntryFee = 220 },
            new EventCategory { EventId = e3.EventId, CategoryId = c4.CategoryId, EntryFee = 300 });
        await db.SaveChangesAsync();
    }
}

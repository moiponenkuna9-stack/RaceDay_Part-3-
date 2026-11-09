using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using RaceDay.DTOs;
using RaceDay.Models;

namespace RaceDay.Tests;

public class RaceDayApiTests : IClassFixture<RaceDayApiFactory>
{
    private readonly RaceDayApiFactory factory;
    public RaceDayApiTests(RaceDayApiFactory factory) => this.factory = factory;

    private HttpClient CreateClient() => factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions { HandleCookies = true });

    [Fact]
    public async Task RegisterParticipant_Succeeds()
    {
        var client = CreateClient();
        var email = $"test{Guid.NewGuid():N}@example.com";
        var response = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest { FirstName = "Test", LastName = "Runner", Email = email, Password = "Password123!", Role = UserRole.Participant });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task LoginWithWrongPassword_IsRejected()
    {
        var client = CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest { Email = "naledi.participant@raceday.test", Password = "WrongPassword!" });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ParticipantCanViewOwnProfileAfterLogin()
    {
        var client = CreateClient();
        var login = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest { Email = "naledi.participant@raceday.test", Password = "Password123!" });
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        var response = await client.GetAsync("/api/profile");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ParticipantCannotCreateEvent()
    {
        var client = CreateClient();
        await client.PostAsJsonAsync("/api/auth/login", new LoginRequest { Email = "naledi.participant@raceday.test", Password = "Password123!" });
        var response = await client.PostAsJsonAsync("/api/events", new CreateEventRequest { Name = "Blocked", EventDate = DateTime.UtcNow.AddDays(30), Location = "Pretoria", DistanceKm = 5, EventType = EventType.Run, RegistrationOpen = DateTime.UtcNow.AddDays(-1), RegistrationClose = DateTime.UtcNow.AddDays(20) });
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task OrganiserCanCreateEvent()
    {
        var client = CreateClient();
        await client.PostAsJsonAsync("/api/auth/login", new LoginRequest { Email = "thabo.organiser@raceday.test", Password = "Password123!" });
        var response = await client.PostAsJsonAsync("/api/events", new CreateEventRequest { Name = "Test Organiser Event", EventDate = DateTime.UtcNow.AddDays(30), Location = "Pretoria", DistanceKm = 10, EventType = EventType.Run, RegistrationOpen = DateTime.UtcNow.AddDays(-1), RegistrationClose = DateTime.UtcNow.AddDays(20) });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task ParticipantCanEnrolInEvent()
    {
        var client = CreateClient();
        await client.PostAsJsonAsync("/api/auth/login", new LoginRequest { Email = "naledi.participant@raceday.test", Password = "Password123!" });
        var response = await client.PostAsJsonAsync("/api/events/1/enrolments", new CreateEnrolmentRequest { CategoryId = 1 });
        Assert.True(response.StatusCode is HttpStatusCode.Created or HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task OrganiserCannotUseParticipantEnrolmentEndpoint()
    {
        var client = CreateClient();
        await client.PostAsJsonAsync("/api/auth/login", new LoginRequest { Email = "thabo.organiser@raceday.test", Password = "Password123!" });
        var response = await client.PostAsJsonAsync("/api/events/1/enrolments", new CreateEnrolmentRequest { CategoryId = 1 });
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task SwaggerIsAvailable()
    {
        var client = CreateClient();
        var response = await client.GetAsync("/swagger/index.html");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

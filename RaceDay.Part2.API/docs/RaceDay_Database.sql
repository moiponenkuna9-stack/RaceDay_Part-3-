/*
  RaceDay Part 2 - SQL Server reference script
  Code-First EF Core is the source of the production schema.
  This script is provided for SSMS verification and matches the implemented model.
*/

IF DB_ID('RaceDayDB_Part2') IS NOT NULL
BEGIN
    ALTER DATABASE RaceDayDB_Part2 SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE RaceDayDB_Part2;
END;
GO

CREATE DATABASE RaceDayDB_Part2;
GO
USE RaceDayDB_Part2;
GO

CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(500) NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Organiser','Participant')),
    Phone NVARCHAR(30) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

CREATE TABLE UserProfiles (
    ProfileId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL UNIQUE,
    DateOfBirth DATE NULL,
    Gender NVARCHAR(30) NULL,
    EmergencyContactName NVARCHAR(100) NULL,
    EmergencyContactPhone NVARCHAR(30) NULL,
    Address NVARCHAR(300) NULL,
    CONSTRAINT FK_UserProfiles_User FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);

CREATE TABLE Events (
    EventId INT IDENTITY(1,1) PRIMARY KEY,
    OrganiserId INT NOT NULL,
    Name NVARCHAR(150) NOT NULL,
    Description NVARCHAR(500) NULL,
    EventDate DATETIME2 NOT NULL,
    Location NVARCHAR(200) NOT NULL,
    DistanceKm DECIMAL(6,2) NOT NULL,
    EventType NVARCHAR(20) NOT NULL CHECK (EventType IN ('Run','Walk','Cycle')),
    RegistrationOpen DATETIME2 NOT NULL,
    RegistrationClose DATETIME2 NOT NULL,
    Status NVARCHAR(20) NOT NULL CHECK (Status IN ('Draft','Open','Closed','Completed','Cancelled')),
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Events_Organiser FOREIGN KEY (OrganiserId) REFERENCES Users(UserId)
);

CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE,
    DistanceKm DECIMAL(6,2) NOT NULL,
    MaxParticipants INT NOT NULL
);

CREATE TABLE EventCategories (
    EventId INT NOT NULL,
    CategoryId INT NOT NULL,
    EntryFee DECIMAL(10,2) NOT NULL DEFAULT 0,
    PRIMARY KEY (EventId, CategoryId),
    CONSTRAINT FK_EventCategories_Event FOREIGN KEY (EventId) REFERENCES Events(EventId) ON DELETE CASCADE,
    CONSTRAINT FK_EventCategories_Category FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId)
);

CREATE TABLE Enrolments (
    EnrolmentId INT IDENTITY(1,1) PRIMARY KEY,
    ParticipantId INT NOT NULL,
    EventId INT NOT NULL,
    CategoryId INT NOT NULL,
    EnrolmentDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    Status NVARCHAR(20) NOT NULL CHECK (Status IN ('Active','Cancelled','Completed')),
    RaceNumber INT NOT NULL,
    CONSTRAINT UQ_Enrolments_ParticipantEvent UNIQUE (ParticipantId, EventId),
    CONSTRAINT UQ_Enrolments_EventRaceNumber UNIQUE (EventId, RaceNumber),
    CONSTRAINT FK_Enrolments_Participant FOREIGN KEY (ParticipantId) REFERENCES Users(UserId),
    CONSTRAINT FK_Enrolments_EventCategory FOREIGN KEY (EventId, CategoryId) REFERENCES EventCategories(EventId, CategoryId)
);

CREATE TABLE Results (
    ResultId INT IDENTITY(1,1) PRIMARY KEY,
    EnrolmentId INT NOT NULL UNIQUE,
    FinishTime TIME(0) NULL,
    Position INT NULL,
    ResultStatus NVARCHAR(20) NOT NULL CHECK (ResultStatus IN ('Finished','DNF','DNS','DSQ')),
    RecordedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Results_Enrolment FOREIGN KEY (EnrolmentId) REFERENCES Enrolments(EnrolmentId) ON DELETE CASCADE
);
GO

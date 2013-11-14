CREATE TABLE [dbo].[Experiences] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (255)   NOT NULL,
    [Description]      NVARCHAR (255)   NOT NULL,
    [Start]            DATETIME         NOT NULL,
    [End]              DATETIME         NULL,
    [Location]         NVARCHAR (255)   NULL,
    [CoverImageUrl]    NVARCHAR (255)   NULL,
    [Details]          NVARCHAR (MAX)   NULL,
    [Notes]            NVARCHAR (MAX)   NULL,
    [Created]          DATETIME         NULL,
    [ExperienceTypeID] UNIQUEIDENTIFIER NOT NULL,
    [UserID]           UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FKA3A530596C067905] FOREIGN KEY ([ExperienceTypeID]) REFERENCES [dbo].[ExperienceTypes] ([ID]),
    CONSTRAINT [FKA3A530598ACB21CD] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([ID])
);


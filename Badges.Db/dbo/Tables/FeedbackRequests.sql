CREATE TABLE [dbo].[FeedbackRequests] (
    [ID]           UNIQUEIDENTIFIER NOT NULL,
    [Message]      NVARCHAR (255)   NULL,
    [Created]      DATETIME         NOT NULL,
    [Viewed]       DATETIME         NULL,
    [Response]     NVARCHAR (255)   NULL,
    [ResponseDate] DATETIME         NULL,
    [InstructorID] UNIQUEIDENTIFIER NOT NULL,
    [ExperienceID] UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FKA71F2616D85FF60F] FOREIGN KEY ([ExperienceID]) REFERENCES [dbo].[Experiences] ([ID]),
    CONSTRAINT [FKA71F2616FAE76590] FOREIGN KEY ([InstructorID]) REFERENCES [dbo].[Instructors] ([ID])
);


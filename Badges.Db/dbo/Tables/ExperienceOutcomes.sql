CREATE TABLE [dbo].[ExperienceOutcomes] (
    [ID]           UNIQUEIDENTIFIER NOT NULL,
    [Rating]       FLOAT (53)       NULL,
    [Notes]        NVARCHAR (MAX)   NULL,
    [Created]      DATETIME         NULL,
    [ExperienceID] UNIQUEIDENTIFIER NOT NULL,
    [OutcomeID]    UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK5CFFA1AB6C3973A] FOREIGN KEY ([OutcomeID]) REFERENCES [dbo].[Outcomes] ([ID]),
    CONSTRAINT [FK5CFFA1ABD85FF60F] FOREIGN KEY ([ExperienceID]) REFERENCES [dbo].[Experiences] ([ID])
);


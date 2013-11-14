CREATE TABLE [dbo].[BadgeFulfillments] (
    [ID]                UNIQUEIDENTIFIER NOT NULL,
    [Comment]           NVARCHAR (140)   NULL,
    [BadgeSubmissionID] UNIQUEIDENTIFIER NOT NULL,
    [BadgeCriteriaID]   UNIQUEIDENTIFIER NOT NULL,
    [ExperienceID]      UNIQUEIDENTIFIER NULL,
    [SupportingWorkID]  UNIQUEIDENTIFIER NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FKE56D583229AA914] FOREIGN KEY ([BadgeSubmissionID]) REFERENCES [dbo].[BadgeSubmissions] ([ID]),
    CONSTRAINT [FKE56D5832920F4EDF] FOREIGN KEY ([BadgeCriteriaID]) REFERENCES [dbo].[BadgeCriterias] ([ID]),
    CONSTRAINT [FKE56D5832BCAD331C] FOREIGN KEY ([SupportingWorkID]) REFERENCES [dbo].[SupportingWorks] ([ID]),
    CONSTRAINT [FKE56D5832D85FF60F] FOREIGN KEY ([ExperienceID]) REFERENCES [dbo].[Experiences] ([ID])
);


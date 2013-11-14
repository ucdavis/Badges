CREATE TABLE [dbo].[SupportingWorks] (
    [ID]           UNIQUEIDENTIFIER NOT NULL,
    [Description]  NVARCHAR (255)   NOT NULL,
    [Name]         NVARCHAR (255)   NULL,
    [ContentId]    UNIQUEIDENTIFIER NULL,
    [ContentType]  NVARCHAR (255)   NULL,
    [Url]          NVARCHAR (255)   NULL,
    [Notes]        NVARCHAR (MAX)   NULL,
    [Created]      DATETIME         NOT NULL,
    [Type]         NVARCHAR (255)   NOT NULL,
    [ExperienceID] UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FKC181041DD85FF60F] FOREIGN KEY ([ExperienceID]) REFERENCES [dbo].[Experiences] ([ID])
);




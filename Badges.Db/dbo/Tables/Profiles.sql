CREATE TABLE [dbo].[Profiles] (
    [ID]        UNIQUEIDENTIFIER NOT NULL,
    [FirstName] NVARCHAR (255)   NOT NULL,
    [LastName]  NVARCHAR (255)   NOT NULL,
    [Email]     NVARCHAR (255)   NOT NULL,
    [ImageUrl]  NVARCHAR (255)   NULL,
    [Bio]       NVARCHAR (MAX)   NULL,
    [Goals]     NVARCHAR (MAX)   NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);


CREATE TABLE [dbo].[Outcomes] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [Name]        NVARCHAR (255)   NULL,
    [ImageUrl]    NVARCHAR (255)   NULL,
    [Description] NVARCHAR (MAX)   NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);


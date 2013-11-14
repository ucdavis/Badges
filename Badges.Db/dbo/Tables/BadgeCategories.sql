CREATE TABLE [dbo].[BadgeCategories] (
    [ID]       UNIQUEIDENTIFIER NOT NULL,
    [Name]     NVARCHAR (255)   NOT NULL,
    [ImageUrl] NVARCHAR (255)   NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);


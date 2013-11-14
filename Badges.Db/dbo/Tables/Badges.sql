CREATE TABLE [dbo].[Badges] (
    [ID]              UNIQUEIDENTIFIER NOT NULL,
    [Name]            NVARCHAR (64)    NOT NULL,
    [Description]     NVARCHAR (140)   NULL,
    [ImageUrl]        NVARCHAR (255)   NOT NULL,
    [CreatedOn]       DATETIME         NOT NULL,
    [Approved]        BIT              NOT NULL,
    [UserID]          UNIQUEIDENTIFIER NOT NULL,
    [BadgeCategoryID] UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK18AF090D8ACB21CD] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK18AF090D9AEDD0FA] FOREIGN KEY ([BadgeCategoryID]) REFERENCES [dbo].[BadgeCategories] ([ID])
);


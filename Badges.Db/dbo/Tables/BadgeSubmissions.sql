CREATE TABLE [dbo].[BadgeSubmissions] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [Reflection]  NVARCHAR (MAX)   NULL,
    [CreatedOn]   DATETIME         NOT NULL,
    [AwardedOn]   DATETIME         NULL,
    [Approved]    BIT              NOT NULL,
    [Submitted]   BIT              NOT NULL,
    [SubmittedOn] DATETIME         NULL,
    [UserID]      UNIQUEIDENTIFIER NOT NULL,
    [BadgeID]     UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK2E3661468ACB21CD] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK2E366146D448E5D8] FOREIGN KEY ([BadgeID]) REFERENCES [dbo].[Badges] ([ID])
);


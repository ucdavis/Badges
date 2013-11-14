CREATE TABLE [dbo].[Instructors] (
    [ID]         UNIQUEIDENTIFIER NOT NULL,
    [FirstName]  NVARCHAR (255)   NULL,
    [LastName]   NVARCHAR (255)   NOT NULL,
    [Email]      NVARCHAR (255)   NULL,
    [Identifier] NVARCHAR (255)   NOT NULL,
    [UserID]     UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FKD736B2008ACB21CD] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([ID]),
    UNIQUE NONCLUSTERED ([Identifier] ASC)
);


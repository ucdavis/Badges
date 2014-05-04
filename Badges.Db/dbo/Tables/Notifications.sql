CREATE TABLE [dbo].[Notifications] (
    [ID]      UNIQUEIDENTIFIER NOT NULL,
    [Pending] BIT              NOT NULL,
    [Created] DATETIME         NOT NULL,
    [Message] NVARCHAR (MAX)   NULL,
    [UserID]  UNIQUEIDENTIFIER NOT NULL,
    [Subject] NVARCHAR(250) NOT NULL, 
    [Type] INT NOT NULL, 
    [Metadata] NVARCHAR(MAX) NULL, 
    PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FKFEFC42618ACB21CD] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([ID])
);


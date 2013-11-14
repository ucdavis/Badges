CREATE TABLE [dbo].[Permissions] (
    [User_id] UNIQUEIDENTIFIER NOT NULL,
    [Role_id] NVARCHAR (255)   NOT NULL,
    PRIMARY KEY CLUSTERED ([User_id] ASC, [Role_id] ASC),
    CONSTRAINT [FKEA223C4CA8D3D50A] FOREIGN KEY ([Role_id]) REFERENCES [dbo].[Roles] ([ID]),
    CONSTRAINT [FKEA223C4CD18638C5] FOREIGN KEY ([User_id]) REFERENCES [dbo].[Users] ([ID])
);


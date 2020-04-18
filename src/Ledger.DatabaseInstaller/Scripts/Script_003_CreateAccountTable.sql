use Ledger;
go

create table [dbo].[Account]
(
     [Id] [uniqueidentifier] not null
    ,[UserId] [uniqueidentifier] not null
    ,[Name] [nvarchar](255) not null
    ,[CreatedTimestamp] [datetime2](7) not null
    ,[UpdatedTimestamp] [datetime2](7) not null
    ,constraint [PK_Account] primary key clustered ([Id])
    ,constraint [FK_Account_User] foreign key ([UserId]) references [dbo].[User]([Id])
);
go

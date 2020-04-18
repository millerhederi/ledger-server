use Ledger;
go

create table [dbo].[User]
(
     [Id] [uniqueidentifier] not null
    ,[UserName] [nvarchar](100) not null
    ,[CreatedTimestamp] [datetime2](7) not null
    ,[UpdatedTimestamp] [datetime2](7) not null
    ,constraint [PK_User] primary key clustered ([Id])
);
go

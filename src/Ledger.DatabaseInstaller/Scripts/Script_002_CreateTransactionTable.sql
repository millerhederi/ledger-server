use Ledger;
go

create table [dbo].[Transaction]
(
     [Id] [uniqueidentifier] not null
    ,[UserId] [uniqueidentifier] not null
    ,[PostedDate] [date] not null
    ,[Description] [nvarchar](255) null
    ,[Amount] [decimal](9,2) not null
    ,[CreatedTimestamp] [datetime2](7) not null
    ,[UpdatedTimestamp] [datetime2](7) not null
    ,constraint [PK_Transaction] primary key clustered ([Id])
    ,constraint [FK_Transaction_User] foreign key ([UserId]) references [dbo].[User]([Id])
);
go

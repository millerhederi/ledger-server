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

declare @UtcNow [datetime2](7) = GetUtcDate();

insert into [dbo].[User] values
    ('da1ac815-ec39-4ca8-a8f3-7f9856384e32', 'user', @UtcNow, @UtcNow);

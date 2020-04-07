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

declare @UtcNow [datetime2](7) = GetUtcDate();

insert into [dbo].[Transaction] values
     ('fdcb5bc6-e59f-4b1d-85f7-dc819f5b6c05', 'da1ac815-ec39-4ca8-a8f3-7f9856384e32', '3/26/2020', 'Groceries', 24.87, @UtcNow, @UtcNow)
    ,('5546337b-e2e8-4f21-9754-ec9106826c59', 'da1ac815-ec39-4ca8-a8f3-7f9856384e32', '3/28/2020', 'Netflix', 13.00, @UtcNow, @UtcNow)
go
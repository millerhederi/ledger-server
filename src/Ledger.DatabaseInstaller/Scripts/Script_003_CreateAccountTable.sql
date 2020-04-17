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

declare @UtcNow [datetime2](7) = GetUtcDate();
declare @UserId [uniqueidentifier] = 'da1ac815-ec39-4ca8-a8f3-7f9856384e32'

insert into [dbo].[Account] values
     ('d7e7cee1-bbff-4b1e-9592-3fe10134bef7', @UserId, 'assets:checking:boa', @UtcNow, @UtcNow)
    ,('0549dcd0-24d7-4cce-9fc7-70c200edfec4', @UserId, 'expenses:food', @UtcNow, @UtcNow)
    ,('1fac80c5-8b65-4c16-9af5-d0a6b36a79e6', @UserId, 'expenses:housing', @UtcNow, @UtcNow)
    ,('46b7390d-02bb-4422-8b85-481bee1ee69e', @UserId, 'expenses:transportation', @UtcNow, @UtcNow)
    ,('218dbf03-eaa1-4d21-9950-e1a0794b2d87', @UserId, 'liabilities:cc:visa', @UtcNow, @UtcNow)
go
use Ledger;
go

create table [dbo].[Account]
(
     [Id] [uniqueidentifier] not null
    ,[UserId] [uniqueidentifier] not null
    ,[ParentAccountId] [uniqueidentifier] null
    ,[Name] [nvarchar](255) not null
    ,[CreatedTimestamp] [datetime2](7) not null
    ,[UpdatedTimestamp] [datetime2](7) not null
    ,constraint [PK_Account] primary key clustered ([Id])
    ,constraint [FK_Account_ParentAccountId] foreign key ([ParentAccountId]) references [dbo].[Account]([Id])
    ,constraint [FK_Account_User] foreign key ([UserId]) references [dbo].[User]([Id])
);
go

declare @UtcNow [datetime2](7) = GetUtcDate();
declare @UserId [uniqueidentifier] = 'da1ac815-ec39-4ca8-a8f3-7f9856384e32'

-- assets
--     checking
--         boa
--         chase
-- expenses
--     food
--     housing
--     transportation
-- liabilities
--     credit_card
--         visa
insert into [dbo].[Account] values
     ('d7e7cee1-bbff-4b1e-9592-3fe10134bef7', @UserId, null, 'assets', @UtcNow, @UtcNow)
    ,('aa42cd0b-0905-42eb-9c5d-3ad68ac0a028', @UserId, 'd7e7cee1-bbff-4b1e-9592-3fe10134bef7', 'checking', @UtcNow, @UtcNow)
    ,('544b8933-8811-4f64-a98a-4ef066a0fef8', @UserId, 'aa42cd0b-0905-42eb-9c5d-3ad68ac0a028', 'boa', @UtcNow, @UtcNow)
    ,('2915e1a0-9320-4459-a50a-e8bdb881f191', @UserId, 'aa42cd0b-0905-42eb-9c5d-3ad68ac0a028', 'chase', @UtcNow, @UtcNow)
    ,('1a130fc5-9f9c-4aa1-8be3-25ca543b0ce6', @UserId, null, 'expenses', @UtcNow, @UtcNow)
    ,('0549dcd0-24d7-4cce-9fc7-70c200edfec4', @UserId, '1a130fc5-9f9c-4aa1-8be3-25ca543b0ce6', 'food', @UtcNow, @UtcNow)
    ,('1fac80c5-8b65-4c16-9af5-d0a6b36a79e6', @UserId, '1a130fc5-9f9c-4aa1-8be3-25ca543b0ce6', 'housing', @UtcNow, @UtcNow)
    ,('46b7390d-02bb-4422-8b85-481bee1ee69e', @UserId, '1a130fc5-9f9c-4aa1-8be3-25ca543b0ce6', 'transportation', @UtcNow, @UtcNow)
    ,('1f7fece2-642f-47d8-9f04-42e818bef47c', @UserId, null, 'liabilities', @UtcNow, @UtcNow)
    ,('41bac36a-d01c-4237-88a8-5cc355331de5', @UserId, '1f7fece2-642f-47d8-9f04-42e818bef47c', 'credit_card', @UtcNow, @UtcNow)
    ,('218dbf03-eaa1-4d21-9950-e1a0794b2d87', @UserId, '41bac36a-d01c-4237-88a8-5cc355331de5', 'visa', @UtcNow, @UtcNow)
go
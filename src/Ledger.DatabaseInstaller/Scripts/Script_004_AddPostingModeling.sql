use Ledger;
go

alter table [dbo].[Transaction] drop column [Amount];

create table [dbo].[Posting]
(
     [Id] [uniqueidentifier] not null
    ,[TransactionId] [uniqueidentifier] not null
    ,[AccountId] [uniqueidentifier] not null
    ,[Amount] [decimal](9, 2) not null
    ,[CreatedTimestamp] [datetime2](7) not null
    ,[UpdatedTimestamp] [datetime2](7) not null
    ,constraint [PK_Posting] primary key clustered ([Id])
    ,constraint [FK_Posting_TransactionId] foreign key ([TransactionId]) references [dbo].[Transaction]([Id])
    ,constraint [FK_Posting_AccountId] foreign key ([AccountId]) references [dbo].[Account]([Id])
);
go
create table [RR_Identity].[RefreshToken]
(
    [Id]              int identity (1, 1) not null,
    [UserId]          int                 not null,
    [Token]           nvarchar(max)       not null,
    [ExpiresOn]       datetime2           not null,
    [CreatedOn]       datetime2           not null,
    [CreatedByIp]     nvarchar(39)        not null,
    [RevokedOn]       datetime2           null,
    [RevokedByIp]     nvarchar(39)        null,
    [ReplacedByToken] nvarchar(max)       null,
    constraint [PK_RefreshToken] primary key clustered ([Id] asc),
    constraint [FK_RefreshToken_User] foreign key ([UserId]) references [RR_Identity].[User] ([Id]) on delete cascade
)

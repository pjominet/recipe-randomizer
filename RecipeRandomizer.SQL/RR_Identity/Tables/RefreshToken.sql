create table [RR_Identity].[RefreshToken]
(
    [Id]              int identity (1, 1) not null,
    [UserId]          int                 not null,
    [Token]           nvarchar(max)       not null,
    [Expires]         datetime2           not null,
    [Created]         datetime2           not null,
    [CreatedByIp]     nvarchar(39)        not null,
    [Revoked]         datetime2           null,
    [RevokedByIp]     nvarchar(39)        null,
    [ReplacedByToken] datetime2           null,
    constraint [PK_RefreshToken] primary key clustered ([Id] asc),
    constraint [FK_Recipe_Cost] foreign key ([UserId]) references [RR_Identity].[User] ([Id]) on delete cascade
)

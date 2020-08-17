create table [RR_Identity].[User]
(
    [Id]           int identity (1, 1) not null,
    [FirstName]    nvarchar(max)       not null,
    [LastName]     nvarchar(max)       not null,
    [Email]        nvarchar(max)       not null,
    [PasswordHash] binary(64)          not null,
    [PasswordSalt] binary(128)         not null,
    constraint [PK_User] primary key clustered ([Id] asc)
)

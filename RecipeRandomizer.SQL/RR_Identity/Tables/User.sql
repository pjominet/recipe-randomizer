create table [RR_Identity].[User]
(
    [Id]                  int identity (1, 1) not null,
    [RoleId]              int                 not null,
    [FirstName]           nvarchar(max)       not null,
    [LastName]            nvarchar(max)       not null,
    [Email]               nvarchar(max)       not null,
    [PasswordHash]        nvarchar(max)       not null,
    [HasAcceptedTerms]    bit                 not null default 0,
    [VerificationToken]   nvarchar(max)       null,
    [VerifiedOn]          datetime2           null,
    [ResetToken]          nvarchar(max)       null,
    [ResetTokenExpiresOn] datetime2           null,
    [PasswordResetOn]     datetime2           null,
    [CreatedOn]           datetime2           not null,
    [UpdatedOn]           datetime2           null,
    constraint [PK_User] primary key clustered ([Id] asc),
    constraint [FK_User_Role] foreign key ([RoleId]) references [RR_Identity].[Role] ([Id]) on delete no action
)

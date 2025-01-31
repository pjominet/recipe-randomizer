﻿create table [RR_Identity].[User]
(
    [Id]                  int identity (1, 1) not null,
    [RoleId]              int                 not null,
    [Username]            nvarchar(max)       not null,
    [Email]               nvarchar(max)       not null,
    [PasswordHash]        nvarchar(max)       not null,
    [HasAcceptedTerms]    bit                 not null default 0,
    [ProfileImageUri]     nvarchar(max)       null,
    [VerificationToken]   nvarchar(max)       null,
    [VerifiedOn]          datetime2           null,
    [ResetToken]          nvarchar(max)       null,
    [ResetTokenExpiresOn] datetime2           null,
    [PasswordResetOn]     datetime2           null,
    [CreatedOn]           datetime2           not null,
    [UpdatedOn]           datetime2           null,
    [LockedOn]            datetime2           null,
    [LockedById]          int                 null,
    constraint [PK_User] primary key clustered ([Id] asc),
    constraint [FK_User_Role] foreign key ([RoleId]) references [RR_Identity].[Role] ([Id]) on delete no action,
    constraint [FK_User_LockedBy] foreign key ([LockedById]) references [RR_Identity].[User] ([Id]) on delete no action,
)

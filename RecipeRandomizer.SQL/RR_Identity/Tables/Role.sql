create table [RR_Identity].[Role]
(
    [Id]    int         not null,
    [Label] nvarchar(8) not null,
    constraint [PK_Role] primary key clustered ([Id] ASC)
)

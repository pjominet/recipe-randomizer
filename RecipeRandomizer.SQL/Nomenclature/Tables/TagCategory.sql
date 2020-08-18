create table [Nomenclature].[TagCategory]
(
    [Id]    int          not null,
    [Label] nvarchar(32) not null,
    constraint [PK_TagCategory] primary key clustered ([Id] ASC)
)

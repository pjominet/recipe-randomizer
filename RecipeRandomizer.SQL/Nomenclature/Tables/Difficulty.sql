create table [Nomenclature].[Difficulty]
(
    [Id]    int          not null,
    [Label] nvarchar(32) not null,
    constraint [PK_Difficulty] primary key clustered ([Id] ASC)
)

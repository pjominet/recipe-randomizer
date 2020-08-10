create table [Nomenclature].[Difficulty]
(
    [Id]    int identity (1, 1) not null,
    [Label] nvarchar(32)        not null,
    constraint [PK_Difficulty] primary key clustered ([Id] ASC)
)

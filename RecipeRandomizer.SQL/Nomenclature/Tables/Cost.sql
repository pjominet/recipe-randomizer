create table [Nomenclature].[Cost]
(
    [Id]    int          not null,
    [Label] nvarchar(32) not null,
    constraint [PK_Cost] primary key clustered ([Id] ASC)
)

create table [Nomenclature].[Cost]
(
    [Id]    int identity (1, 1) not null,
    [Label] nvarchar(32)        not null,
    constraint [PK_Cost] primary key clustered ([Id] ASC)
)

create table [Nomenclature].[Quantity]
(
    [Id]          int          not null,
    [Label]       nvarchar(32) not null,
    [Description] nvarchar(64) null,
    constraint [PK_Quantity] primary key clustered ([Id] ASC)
)

create table [Nomenclature].[QuantityUnit]
(
    [Id]          int          not null,
    [Label]       nvarchar(32) not null,
    [Description] nvarchar(64) null,
    constraint [PK_QuantityUnit] primary key clustered ([Id] ASC)
)

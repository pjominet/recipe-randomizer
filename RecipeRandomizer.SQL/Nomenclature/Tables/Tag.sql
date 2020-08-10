create table [Nomenclature].[Tag]
(
    [Id]            int identity (1, 1) not null,
    [Label]         nvarchar(32)        not null,
    [TagCategoryId] int                 not null,
    constraint [PK_Tag] primary key clustered ([Id] asc),
    constraint [FK_Tag_TagCategory] foreign key ([TagCategoryId]) references [Nomenclature].[TagCategory] ([Id])
)

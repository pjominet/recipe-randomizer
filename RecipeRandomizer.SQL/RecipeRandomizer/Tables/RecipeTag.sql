create table [RecipeRandomizer].[RecipeTag]
(
    [RecipeId] int not null,
    [TagId]    int not null,
    constraint [PK_RecipeTag] primary key clustered ([RecipeId], [TagId]),
    constraint [FK_RecipeTag_Recipe] foreign key ([RecipeId]) references [RecipeRandomizer].[Recipe] ([Id]) on delete no action,
    constraint [FK_RecipeTag_Tag] foreign key ([TagId]) references [Nomenclature].[Tag] ([Id]) on delete no action
)

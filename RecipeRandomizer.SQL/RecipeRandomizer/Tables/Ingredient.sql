create table [RecipeRandomizer].[Ingredient]
(
    [Id]         int identity (1, 1) not null,
    [QuantityId] int                 not null,
    [RecipeId]   int                 not null,
    [Name]       nvarchar(32)        not null,
    constraint [PK_Ingredient] primary key clustered ([Id] asc),
    constraint [FK_Ingredient_Quantity] foreign key ([QuantityId]) references [Nomenclature].[Quantity] ([Id]),
    constraint [FK_Ingredient_Recipe] foreign key ([RecipeId]) references [RecipeRandomizer].[Recipe] ([Id])
)

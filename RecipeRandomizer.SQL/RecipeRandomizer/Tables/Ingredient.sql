create table [RecipeRandomizer].[Ingredient]
(
    [Id]             int identity (1, 1) not null,
    [QuantityUnitId] int                 not null,
    [RecipeId]       int                 not null,
    [Name]           nvarchar(32)        not null,
    [Quantity]       int                 not null,
    constraint [PK_Ingredient] primary key clustered ([Id] asc),
    constraint [FK_Ingredient_Quantity] foreign key ([QuantityUnitId]) references [Nomenclature].[Quantity] ([Id]) on delete no action,
    constraint [FK_Ingredient_Recipe] foreign key ([RecipeId]) references [RecipeRandomizer].[Recipe] ([Id]) on delete cascade
)

create table [RecipeRandomizer].[RecipeLike]
(
    [RecipeId] int not null,
    [UserId]   int not null,
    constraint [PK_RecipeLike] primary key clustered ([RecipeId], [UserId]),
    constraint [FK_RecipeLike_Recipe] foreign key ([RecipeId]) references [RecipeRandomizer].[Recipe] ([Id]) on delete no action,
    constraint [FK_RecipeLike_User] foreign key ([UserId]) references [RR_Identity].[User] ([Id]) on delete no action
)

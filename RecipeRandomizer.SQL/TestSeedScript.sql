begin transaction
    set identity_insert [RecipeRandomizer].[Recipe] on
    merge [RecipeRandomizer].[Recipe] as target
    using (values (1, null, N'Test Recipe', N'Do not cook this', null, 7, 2, 2, 15, 30, N'None', getdate(), getdate(), null)
    ) as source ([Id], [UserId], [Name], [Description], [ImgUri], [NumberOfPeople], [CostId], [DifficultyId],
                 [PrepTime], [CookTime], [Preparation], [CreatedOn], [LastUpdatedOn], [DeletedOn])
    on (target.[Id] = source.[Id])
    when matched then
        update
        set [UserId]         = source.[UserId],
            [Name]           = source.[Name],
            [Description]    = source.[Description],
            [ImgUri]         = source.[ImgUri],
            [NumberOfPeople] = source.[NumberOfPeople],
            [CostId]         = source.[CostId],
            [DifficultyId]   = source.[DifficultyId],
            [PrepTime]       = source.[PrepTime],
            [CookTime]       = source.[CookTime],
            [Preparation]    = source.[Preparation],
            [CreatedOn]      = source.[CreatedOn],
            [LastUpdatedOn]  = source.[LastUpdatedOn],
            [DeletedOn]      = source.[DeletedOn]
    when not matched then
        insert ([Id], [UserId], [Name], [Description], [ImgUri], [NumberOfPeople], [CostId], [DifficultyId],
                [PrepTime], [CookTime], [Preparation], [CreatedOn], [LastUpdatedOn], [DeletedOn])
        values (source.[Id], source.[UserId], source.[Name], source.[Description], source.[ImgUri], source.[NumberOfPeople], source.[CostId], source.[DifficultyId],
                source.[PrepTime], source.[CookTime], source.[Preparation], source.[CreatedOn], source.[LastUpdatedOn], source.[DeletedOn])
    when not matched by source then
        delete;
    set identity_insert [RecipeRandomizer].[Recipe] off

    set identity_insert [RecipeRandomizer].[Ingredient] on
    merge [RecipeRandomizer].[Ingredient] as target
    using (values (1, 1, 1, N'Salt', 50),
                  (2, 2, 1, N'Sugar', 100),
                  (3, 3, 1, N'Meat', 150),
                  (4, 4, 1, N'Chocolate', 200)
    ) as source ([Id], [QuantityId], [RecipeId], [Name], [Quantity])
    on (target.[Id] = source.[Id])
    when matched then
        update
        set [QuantityId] = source.[QuantityId],
            [RecipeId]   = source.[RecipeId],
            [Name]       = source.[Name],
            [Quantity]   = source.[Quantity]
    when not matched then
        insert ([Id], [QuantityId], [RecipeId], [Name], [Quantity])
        values (source.[Id], source.[QuantityId], source.[RecipeId], source.[Name], source.[Quantity])
    when not matched by source then
        delete;
    set identity_insert [RecipeRandomizer].[Ingredient] off

    set identity_insert [RecipeRandomizer].[RecipeTag] on
    merge [RecipeRandomizer].[RecipeTag] as target
    using (values (1, 7),
                  (1, 7),
                  (1, 8)
    ) as source ([RecipeId], [TagId])
    on (target.[RecipeId] = source.[RecipeId] and target.[TagId] = source.[TagId])
    when matched then
        update
        set [RecipeId] = source.[RecipeId],
            [TagId]    = source.[TagId]
    when not matched then
        insert ([RecipeId], [TagId])
        values (source.[RecipeId], source.[TagId])
    when not matched by source then
        delete;
    set identity_insert [RecipeRandomizer].[RecipeTag] off
commit transaction;

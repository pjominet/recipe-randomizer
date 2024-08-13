begin transaction
    set identity_insert [RecipeRandomizer].[Recipe] on
    merge [RecipeRandomizer].[Recipe] as target
    using (values (1,  null,
                   N'Test Recipe',
                   N'<h4>Description</h4><p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Morbi eu sapien pulvinar, faucibus lorem quis, malesuada mauris. Donec a egestas sem. Cras odio dolor, facilisis et gravida sed, bibendum ut velit. In nunc ex, hendrerit id tincidunt et, suscipit sed massa. In molestie est et nibh consectetur, sed placerat arcu dictum. Vestibulum lectus massa, pharetra vel leo ultricies, iaculis ultrices orci. Maecenas tempor ipsum sed efficitur molestie. Vivamus fringilla dolor quam, suscipit bibendum dui pellentesque et. Aliquam vitae turpis eget massa vulputate porttitor at ac ipsum. Fusce pellentesque varius magna at vestibulum. Mauris in convallis diam. Sed a dapibus turpis, quis dictum diam.</p>',
                   null, 7, 2, 2, 15, 30,
                   N'<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Morbi eu sapien pulvinar, faucibus lorem quis, malesuada mauris. Donec a egestas sem. Cras odio dolor, facilisis et gravida sed, bibendum ut velit. In nunc ex, hendrerit id tincidunt et, suscipit sed massa. In molestie est et nibh consectetur, sed placerat arcu dictum. Vestibulum lectus massa, pharetra vel leo ultricies, iaculis ultrices orci. Maecenas tempor ipsum sed efficitur molestie. Vivamus fringilla dolor quam, suscipit bibendum dui pellentesque et. Aliquam vitae turpis eget massa vulputate porttitor at ac ipsum. Fusce pellentesque varius magna at vestibulum. Mauris in convallis diam. Sed a dapibus turpis, quis dictum diam.</p>',
                   getutcdate(),
                   getutcdate(),
                   null)
    ) as source ([Id], [UserId], [Name], [Description], [ImageUri], [NumberOfPeople], [CostId], [DifficultyId],
                 [PrepTime], [CookTime], [Preparation], [CreatedOn], [UpdatedOn], [DeletedOn])
    on (target.[Id] = source.[Id])
    when matched then
        update
        set [UserId]         = source.[UserId],
            [Name]           = source.[Name],
            [Description]    = source.[Description],
            [ImageUri]       = source.[ImageUri],
            [NumberOfPeople] = source.[NumberOfPeople],
            [CostId]         = source.[CostId],
            [DifficultyId]   = source.[DifficultyId],
            [PrepTime]       = source.[PrepTime],
            [CookTime]       = source.[CookTime],
            [Preparation]    = source.[Preparation],
            [CreatedOn]      = source.[CreatedOn],
            [UpdatedOn]      = source.[UpdatedOn],
            [DeletedOn]      = source.[DeletedOn]
    when not matched then
        insert ([Id], [UserId], [Name], [Description], [ImageUri], [NumberOfPeople], [CostId], [DifficultyId],
                [PrepTime], [CookTime], [Preparation], [CreatedOn], [UpdatedOn], [DeletedOn])
        values (source.[Id], source.[UserId], source.[Name], source.[Description], source.[ImageUri], source.[NumberOfPeople], source.[CostId], source.[DifficultyId],
                source.[PrepTime], source.[CookTime], source.[Preparation], source.[CreatedOn], source.[UpdatedOn], source.[DeletedOn])
    when not matched by source then
        delete;
    set identity_insert [RecipeRandomizer].[Recipe] off

    set identity_insert [RecipeRandomizer].[Ingredient] on
    merge [RecipeRandomizer].[Ingredient] as target
    using (values (1, 1, 1, N'Salt', 50),
                  (2, 2, 1, N'Sugar', 100),
                  (3, 3, 1, N'Meat', 150),
                  (4, 4, 1, N'Chocolate', 200)
    ) as source ([Id], [QuantityUnitId], [RecipeId], [Name], [Quantity])
    on (target.[Id] = source.[Id])
    when matched then
        update
        set [QuantityUnitId] = source.[QuantityUnitId],
            [RecipeId]       = source.[RecipeId],
            [Name]           = source.[Name],
            [Quantity]       = source.[Quantity]
    when not matched then
        insert ([Id], [QuantityUnitId], [RecipeId], [Name], [Quantity])
        values (source.[Id], source.[QuantityUnitId], source.[RecipeId], source.[Name], source.[Quantity])
    when not matched by source then
        delete;
    set identity_insert [RecipeRandomizer].[Ingredient] off

    merge [RecipeRandomizer].[RecipeTag] as target
    using (values (1, 2),
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
commit transaction;

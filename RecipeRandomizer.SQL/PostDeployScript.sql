begin transaction

    merge [Nomenclature].[Cost] as target
    using (values (1, N'Cheap'),
                  (2, N'Average'),
                  (3, N'Expensive')
    ) as source ([Id], [Label])
    on (target.[Id] = source.[Id])
    when matched then
        update set [Label] = source.[Label]
    when not matched then
        insert ([Id], [Label])
        values (source.[Id], source.[Label])
    when not matched by source then
        delete;

    merge [Nomenclature].[Difficulty] as target
    using (values (1, N'Easy'),
                  (2, N'Average'),
                  (3, N'Hard')
    ) as source ([Id], [Label])
    on (target.[Id] = source.[Id])
    when matched then
        update set [Label] = source.[Label]
    when not matched then
        insert ([Id], [Label])
        values (source.[Id], source.[Label])
    when not matched by source then
        delete;

    merge [Nomenclature].[Quantity] as target
    using (values (1, N'mg', N'milligram'),
                  (2, N'g', N'gram'),
                  (3, N'kg', N'kilogram'),
                  (4, N'ml', N'millilitre'),
                  (5, N'l', N'litre'),
                  (6, N'pinch', N'~350mg'),
                  (7, N'tablespoon', N'~25ml'),
                  (8, N'teaspoon', N'~5ml'),
                  (9, N'cup', N'~235ml')
    ) as source ([Id], [Label], [Description])
    on (target.[Id] = source.[Id])
    when matched then
        update set [Label] = source.[Label], [Description] = source.[Description]
    when not matched then
        insert ([Id], [Label], [Description])
        values (source.[Id], source.[Label], source.[Description])
    when not matched by source then
        delete;

    merge [Nomenclature].[TagCategory] as target
    using (values (1, N'Diet'),
                  (2, N'FoodType'),
                  (3, N'Cuisine')
    ) as source ([Id], [Label])
    on (target.[Id] = source.[Id])
    when matched then
        update set [Label] = source.[Label]
    when not matched then
        insert ([Id], [Label])
        values (source.[Id], source.[Label])
    when not matched by source then
        delete;

    set identity_insert [Nomenclature].[Tag] on
    merge [Nomenclature].[Tag] as target
    using (values (1, 1, N'Vegetarian'),
                  (2, 1, N'Vegan'),
                  (3, 2, N'Appetizer'),
                  (4, 2, N'Dish'),
                  (5, 2, N'Dessert'),
                  (6, 2, N'Cocktail'),
                  (7, 2, N'Fastfood'),
                  (8, 3, N'European'),
                  (9, 3, N'Arabic'),
                  (10, 3, N'Asian'),
                  (11, 3, N'Japanese'),
                  (12, 3, N'French'),
                  (13, 3, N'Mediterranean'),
                  (14, 3, N'Greek'),
                  (15, 3, N'German'),
                  (16, 3, N'Chinese'),
                  (17, 3, N'Vietnamese'),
                  (18, 3, N'African')
    ) as source ([Id], [TagCategoryId], [Label])
    on (target.[Id] = source.[Id])
    when matched then
        update set [TagCategoryId] = source.[TagCategoryId], [Label] = source.[Label]
    when not matched then
        insert ([Id], [TagCategoryId], [Label])
        values (source.[Id], source.[TagCategoryId], source.[Label])
    when not matched by source then
        delete;
    set identity_insert [Nomenclature].[Tag] off

    merge [RR_Identity].[Role] as target
    using (values (1, N'Admin'),
                  (2, N'User')
    ) as source ([Id], [Label])
    on (target.[Id] = source.[Id])
    when matched then
        update set [Label] = source.[Label]
    when not matched then
        insert ([Id], [Label])
        values (source.[Id], source.[Label])
    when not matched by source then
        delete;

commit transaction;

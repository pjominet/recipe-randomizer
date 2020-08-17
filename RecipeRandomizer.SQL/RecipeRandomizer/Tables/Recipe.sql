create table [RecipeRandomizer].[Recipe]
(
    [Id]             int identity (1, 1) not null,
    [UserId]         int                 null,
    [Name]           nvarchar(256)       not null,
    [Description]    nvarchar(max)       null,
    [ImageUri]       nvarchar(max)       null,
    [NumberOfPeople] int                 not null,
    [CostId]         int                 not null,
    [DifficultyId]   int                 not null,
    [PrepTime]       int                 not null,
    [CookTime]       int                 not null,
    [Preparation]    nvarchar(max)       not null,
    [DateCreated]    datetime2           not null,
    [LastUpdated]    datetime2           not null,
    [DeletedOn]      datetime2           null,
    constraint [PK_Recipe] primary key clustered ([Id] asc),
    constraint [FK_Recipe_Cost] foreign key ([CostId]) references [Nomenclature].[Cost] ([Id]) on delete no action,
    constraint [FK_Recipe_Difficulty] foreign key ([DifficultyId]) references [Nomenclature].[Difficulty] ([Id]) on delete no action,
    constraint [FK_Recipe_User] foreign key ([UserId]) references [RR_Identity].[User] ([Id]) on delete no action
);

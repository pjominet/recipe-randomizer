create table [RecipeRandomizer].[Recipe]
(
    [Id]             int identity (1, 1) not null,
    [Name]           nvarchar(256)       not null,
    [Description]    nvarchar(512)       null,
    [ImageUri]       nvarchar(128)       null,
    [NumberOfPeople] int                 not null,
    [CostId]         int                 not null,
    [DifficultyId]   int                 not null,
    [PrepTime]       int                 not null,
    [CookTime]       int                 not null,
    [Preparation]    nvarchar(max)       not null,
    [DateCreated]    datetime2           not null,
    [LastUpdated]    datetime2           not null,
    [IsDeleted]      bit                 not null default 0,
    constraint [PK_Recipe] primary key clustered ([Id] asc),
    constraint [FK_Recipe_Cost] foreign key ([CostId]) references [Nomenclature].[Cost] ([Id]),
    constraint [FK_Recipe_Difficulty] foreign key ([DifficultyId]) references [Nomenclature].[Difficulty] ([Id])
);

using MigrationSplitter;

var migrations = new List<MigrationInfo>()
{
    

    
    new ("l_pizza_ru", "production",  0, 466898547),
    new ("pizza_ru", "production",  0, 466898547),
    // new ("pizza_kz", "production_kz",  0, 29027251),
    // new ("pizza_uz", "production_uz",  0, 905185),
    // new ("pizza_kg", "production_kg",  0, 3121860),
    // new ("pizza_by", "production_by",  0, 9008898),
    //
    // new ("pizza_de", "production_de",  0, 57492),
    //
    // new ("pizza_ee", "production_ee",  0, 1919761),
    //
    // new ("pizza_lt", "production_lt",  0, 2733248),
    //
    // new ("pizza_ng", "production_ng",  0, 439586),
    //
    // new ("pizza_pl", "production_pl",  0, 107),
    //
    // new ("pizza_ro", "production_ro",  0, 4987055),
    //
    // new ("pizza_si", "production_si",  0, 261578),

};

var template = @"use {database};
update ordercomposition_customization
set CompositionComponentsUUId=(select componentslinks_history.UUId from componentslinks_history where componentslinks_history.Id = ordercomposition_customization.CompositionComponentsId)
where OrderCompositionId between {min} and {max} and CompositionComponentsUUId is null
";

foreach (var migration in migrations)
{
   Splitter.GenerateFiles(template, migration);   
}

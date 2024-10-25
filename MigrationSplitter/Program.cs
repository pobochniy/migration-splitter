using MigrationSplitter;

var migrations = new List<MigrationInfo>()
{
    
   new MigrationInfo("pizza_ru", "production",  0,  322525639),
   new MigrationInfo("pizza_ae", "production_ae",  0,  30000),
   new MigrationInfo("pizza_am", "production_am",  0,  247472),
   new MigrationInfo("pizza_az", "production_az",  0,  24086),
   new MigrationInfo("pizza_bg", "production_bg",  0,  349),
   new MigrationInfo("pizza_by", "production_by",  0,  13352032),
   new MigrationInfo("pizza_cy", "production_cy",  0,  111665),
   new MigrationInfo("pizza_de", "production_de",  0,  233438),
   new MigrationInfo("pizza_ee", "production_ee",  0,  2123891),
   new MigrationInfo("pizza_ge", "production_ge",  0,  106894),
   new MigrationInfo("pizza_hr", "production_hr",  0,  15371),
   new MigrationInfo("pizza_id", "production_id",  0,  20),
   new MigrationInfo("pizza_kg", "production_kg",  0,  5973743),
   new MigrationInfo("pizza_kz", "production_kz",  0,  32405294),
   new MigrationInfo("pizza_lt", "production_lt",  0,  2374013),
   new MigrationInfo("pizza_ng", "production_ng",  0,  1240095),
   new MigrationInfo("pizza_pl", "production_pl",  0,  484532),
   new MigrationInfo("pizza_ro", "production_ro",  0,  3733290),
   new MigrationInfo("pizza_rs", "production_rs",  0,  23260),
   new MigrationInfo("pizza_si", "production_si",  0,  423973),
   new MigrationInfo("pizza_tj", "production_tj",  0,  798203),
   new MigrationInfo("pizza_tr", "production_tr",  0,  418826),
   new MigrationInfo("pizza_uz", "production_uz",  0,  1903693),
   new MigrationInfo("pizza_vn", "production_vn",  0,  2099784),
   new MigrationInfo("doner_ru", "doner_ru",  0,  3379138),
   new MigrationInfo("drinkit_ru", "drinkit_ru",  0,  3197021),
   new MigrationInfo("drinkit_ae", "drinkit_ae",  0,  136255),
   new MigrationInfo("drinkit_kz", "drinkit_kz",  0,  392525),
};

var template = @"use {database};
update orders o
set o.StartDateTimeCooking = (select IF(year(max(oc.StartDateTimeCooking)) < 2000, o.DateOfReceipt, max(oc.StartDateTimeCooking)) from ordercomposition oc where oc.OrderId = o.Id and oc.UnitId = o.UnitId)
where o.Id between {min} and {max} and o.StartDateTimeCooking is null;";

foreach (var migration in migrations)
{
   Splitter.GenerateFiles(template, migration);   
}

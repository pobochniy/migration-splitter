# Executing scripts on multiple databases

## Setup

1. Fill `appSettings.{environment}.json` by scheme
```json 
{
  "Databases": [
    {
      "name": "human-readable database name for local use (ex: pizza-ae)",
      "ConnectionString": "connection string to database"
    }
  ]
}
```

## Usage

1. Run 
``` bash 
dotnet run --project MigrationRowGenerator.csproj -- yandex --pizza_ru
```
where `{environment}` your specified name used at `appSettings.{environment}.json`.<br/><br/>
Also, all other arguments can be defined in `appSettings.json`.
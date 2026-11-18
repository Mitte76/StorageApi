# StorageApi

StorageApi är ett ASP.NET Core Web API för hantering av lagerdata och produkter. API:t använder Entity Framework Core och Microsoft SQL Server för att lagra, filtrera och redovisa lagerstatistik.

## Funktioner

- Skapa, läsa, uppdatera och ta bort produkter
- Filtrera produkter efter kategori och namn
- Hämta statistik för hela lagret
- Beräkna totalt lagervärde och medelpris
- Utforska API:t via Swagger i utvecklingsmiljö

## Teknisk översikt

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core 10
- SQL Server / LocalDB
- Swagger (Swashbuckle)

## Förutsättningar

- .NET 10 SDK
- SQL Server Express LocalDB eller en annan tillgänglig SQL Server-instans
- Visual Studio 2026 eller Visual Studio Code

## Kom igång

1. Klona projektet:

```powershell
git clone <repository-url>
cd StorageApi
```

2. Kontrollera anslutningssträngen i `StorageApi/appsettings.json`:

```json
"ConnectionStrings": {
  "StorageApiContext": "Server=(localdb)\\mssqllocaldb;Database=StorageApiContext;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

För en annan SQL Server-instans ska `StorageApiContext`-värdet ändras till motsvarande anslutningssträng.

3. Skapa eller uppdatera databasen:

```powershell
dotnet ef database update --project StorageApi/StorageApi.csproj
```

4. Starta API:t:

```powershell
dotnet run --project StorageApi/StorageApi.csproj
```

När programmet körs i utvecklingsläge finns Swagger UI vanligtvis på:

- `https://localhost:7187/swagger`
- `http://localhost:5150/swagger`

Portarna kan även visas i konsolen vid uppstart.

## API-endpoints

Basvägen för alla produktanrop är `/api/Products`.

| Metod | Endpoint | Beskrivning |
|---|---|---|
| `GET` | `/api/Products` | Hämtar produkter. De valfria query-parametrarna `category` och `name` filtrerar resultatet med exakta matchningar. |
| `GET` | `/api/Products/getCopilot` | Alternativ implementation för att hämta och filtrera produkter. Accepterar samma valfria query-parametrar; `category` matchas exakt medan `name` matchas om produktnamnet innehåller söktexten. |
| `GET` | `/api/Products/{id}` | Hämtar en produkt via id |
| `GET` | `/api/Products/stats` | Hämtar lagerstatistik |
| `GET` | `/api/Products/stats2` | Hämtar lagerstatistik via en inläst produktlista |
| `GET` | `/api/Products/stats3` | Hämtar lagerstatistik via gruppering |
| `POST` | `/api/Products` | Skapar en ny produkt |
| `PUT` | `/api/Products/{id}` | Uppdaterar en befintlig produkt |
| `DELETE` | `/api/Products/{id}` | Tar bort en produkt |

Exempel på filtrering: `/api/Products?category=Elektronik&name=Dator` eller `/api/Products/getCopilot?category=Elektronik&name=Dator`.

## Exempel

### Skapa en produkt

```http
POST /api/Products
Content-Type: application/json

{
  "name": "Trådlöst tangentbord",
  "price": 499,
  "category": "Tillbehör",
  "shelf": "A12",
  "count": 10,
  "description": "Tangentbord med Bluetooth"
}
```

### Uppdatera en produkt

```http
PUT /api/Products/1
Content-Type: application/json

{
  "id": 1,
  "name": "Trådlöst tangentbord",
  "price": 449,
  "category": "Tillbehör",
  "shelf": "A12",
  "count": 8,
  "description": "Uppdaterad produktbeskrivning"
}
```

## Produktmodell

| Fält | Typ | Begränsning |
|---|---|---|
| `id` | `int` | Genereras av databasen |
| `name` | `string` | Obligatoriskt, högst 100 tecken |
| `price` | `int` | Mellan 0 och 1000 |
| `category` | `string` | Obligatoriskt |
| `shelf` | `string` | Obligatoriskt |
| `count` | `int` | Mellan 0 och 499 |
| `description` | `string` | Valfritt, högst 500 tecken |

## Lagerstatistik

Statistikobjektet innehåller:

- `totalProducts`: totalt antal enheter i lager
- `totalInventoryValue`: summan av pris gånger antal
- `averagePrice`: genomsnittligt produktpris

## Databasmigrationer

Migrationerna finns i katalogen `StorageApi/Migrations`. När datamodellen ändras kan en ny migration skapas med:

```powershell
dotnet ef migrations add <MigrationNamn> --project StorageApi/StorageApi.csproj
```

Tillämpa därefter migrationen:

```powershell
dotnet ef database update --project StorageApi/StorageApi.csproj
```

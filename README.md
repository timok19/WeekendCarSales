# Weekend Car Sales

Tato aplikace slouží k importu prodejů vozů z XML souborů, jejich trvalému uložení do databáze a následnému generování přehledů o víkendových prodejích. Umožňuje uživateli snadno sledovat prodejní výkon v klíčových dnech týdne a automatizovaně počítat ceny s DPH.

## Architektura

Aplikace je navržena podle principů **Clean Architecture** a je rozdělena do čtyř hlavních vrstev: Presentation (WPF), Application (Business Logic & CQRS), Infrastructure (Data Access & Services) a Core (Domain Model).

## Jak spustit aplikaci?

Ke spuštění aplikace na lokálním stroji budete potřebovat následující:

- **Windows 10/11** (WPF vyžaduje Windows Runtime).
- **.NET 10 SDK** (nebo novější).
- **IDE** (Rider, Visual Studio 2022 nebo VS Code s C# Dev Kit).

### Kroky pro lokální spuštění:

1. Naklonujte repozitář:
   ```powershell
   git clone https://github.com/timok19/WeekendCarSales.git
   ```
2. Přejděte do kořenového adresáře projektu:
   ```powershell
   cd WeekendCarSales
   ```
3. Obnovte závislosti a sestavte aplikaci:
   ```powershell
   dotnet restore
   dotnet build
   ```
4. Spusťte aplikaci:
   ```powershell
   dotnet run --project src\WeekendCarSales.Presentation\WeekendCarSales.Presentation.csproj
   ```

### Vnější závislosti

Aplikace má minimální externí závislosti díky použití vestavěných technologií:

- **SQLite**: Používá se jako lokální databázové úložiště. Databázový soubor (`weekend-car-sales.db`) se automaticky vytvoří v adresáři `Data` po prvním spuštění.
- **XML Soubory**: Pro testování je k dispozici vzorový soubor v `src\WeekendCarSales.Presentation\Data\sales-data.xml`.

## Aplikace

Aplikace využívá moderní prvky WPF pro UX-friendly rozhraní. Hlavní funkce zahrnují:
- **Import XML**: Validace a import dat o prodejích.
- **Víkendové reporty**: Automatické filtrování prodejů (sobota/neděle) a seskupování podle modelů.
- **Value Objects**: Přesná práce s měnou a sazbami DPH v doménové vrstvě.
- **Unit Tests**: Pokrytí klíčové logiky pomocí XUnit, Shouldly a NSubstitute.


# Developer Checklist – CLI Build Fixes

> Purpose: Resolve all build errors and warnings in the **EzDbCodeGen.Cli** solution so the CLI compiles & runs successfully.
> Target date: **_ASAP_**

---

## 1  – Project & Reference Hygiene

- [ ] Ensure **TemplateEngine** project *references* the required interface & core projects.
  - `EzDbCodeGen.TemplateEngine`
    - Add project refs (if missing):
      ```bash
      dotnet add src/EzDbCodeGen.TemplateEngine/EzDbCodeGen.TemplateEngine.csproj reference \
        src/EzDbCodeGen.TemplateEngine.Interfaces/EzDbCodeGen.TemplateEngine.Interfaces.csproj \
        src/EzDbCodeGen.Core/EzDbCodeGen.Core.csproj
      ```
  - Verify CLI already references **TemplateEngine** (transitive ok).
- [ ] Run `dotnet restore` at sln root – ensure *no* package-version conflicts.

## 2  – Fix Namespace / Using Issues

| File | Missing namespaces | Action |
|------|--------------------|--------|
| `TemplateEngine/Helpers/HandlebarsRelationshipHelpers.cs` | `EzDbCodeGen.TemplateEngine.Interfaces`<br>`EzDbCodeGen.Core.TemplateEngine.Helpers` | Add `using` lines + fully-qualify interfaces if ambiguity remains |
| `HandlebarsStringFormatHelpers.cs` | same as above + ensure `IStringFormatHelpers` | Add `using` lines |
| `HandlebarsTypeConversionHelpers.cs` | same as above + ensure `ITypeConversionHelpers` | Add `using` lines |

> General rule: **every class implementing an interface must import its exact namespace.**

## 3  – Synchronise Interface Contracts

For each helper class, verify *all* interface members are implemented **exactly** as declared.

1. Open the interface under `Core/TemplateEngine/Helpers/*` or `TemplateEngine.Interfaces/*`.
2. Compare signatures (return types, nullability, default params) with the implementing class.
3. Add missing methods / adjust signatures in the helper class. 
4. If a helper doesn’t need certain interface methods, implement explicitly throwing `NotImplementedException` (temporary) – better than compiler error.

### Checklist per helper
- [ ] `HandlebarsRelationshipHelpers` implements `IRelationshipHelpers` **and** `IHelperRegistration`.
- [ ] `HandlebarsStringFormatHelpers` implements `IStringFormatHelpers` **and** `IHelperRegistration`.
- [ ] `HandlebarsTypeConversionHelpers` implements `ITypeConversionHelpers` **and** `IHelperRegistration`.

## 4  – Nullable Reference Type Warnings

- [ ] At **top of every file** generating CS8632 warnings add:
  ```csharp
  #nullable enable
  ```
- [ ] Alternatively, add once per project in `.csproj`:
  ```xml
  <PropertyGroup>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  ```
- [ ] Audit helper method params/returns – mark with `?` or remove `?` to satisfy compiler.

## 5  – Compile‐Test Cycle

1. Run **incremental builds** after each logical fix:
   ```bash
   dotnet build src/EzDbCodeGen.Cli/EzDbCodeGen.Cli.csproj
   ```
2. Address *new* errors as they surface (often cascading).
3. When zero **errors**, ensure **warnings** are ≤ agreed threshold.

### Smoke Test

- [ ] Execute CLI `dotnet run --project src/EzDbCodeGen.Cli -- --help` – should print usage.
- [ ] Execute a real command (e.g., `schema-info`) against local SQL instance.

## 6  – QA / Regression

- [ ] Run **unit tests** in `/tests` folder:
  ```bash
  dotnet test
  ```
- [ ] Verify that core comparison & code-generation tests still pass.

## 7  – Code Cleanup (optional but recommended)

- [ ] Use JetBrains ReSharper CLT for automatic clean-ups:
  ```bash
  jb cleanupcode EzDbCodeGen.sln
  jb inspectcode EzDbCodeGen.sln --no-build --output=resharper-report.xml
  ```
- [ ] Review duplicates via `jb dupfinder`.

---

### Completion Criteria

- `dotnet build EzDbCodeGen.sln` completes **without errors**.
- CLI binary can execute `--help`, `generate`, `schema-info`, etc.
- All existing tests pass.
- Critical warnings (nullable, obsolete APIs) addressed or justified.

---

*Prepared for the development team – please check off each item as you proceed.*

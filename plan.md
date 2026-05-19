# TreinoSport — Refactor, Best Practices & New Features Plan

> **Scope:** Backend (`TreinoSportAPI` — ASP.NET Core 7, C#) + Frontend (`TreinoSportWeb` — Angular 19; "C:\Users\linco\OneDrive\Documents\Repositorio\TreinoSportWeb")
> **Date:** 2026-04-30

---

## Table of Contents

1. [Backend Refactor & Best Practices](#1-backend-refactor--best-practices)
2. [Backend New Feature — CT Search by Location](#2-backend-new-feature--ct-search-by-location)
3. [Backend Unit Tests](#3-backend-unit-tests)
4. [Frontend Completion & Best Practices](#4-frontend-completion--best-practices)
5. [Frontend New Feature — CT Search (Aluno)](#5-frontend-new-feature--ct-search-aluno)
6. [Execution Order](#6-execution-order)

---

## 1. Backend Refactor & Best Practices

### 1.1 Migrate Data Layer from Raw SQL to Dapper

**Current state:** `BaseMapper` + `SqlServerConnection` wrap raw ADO.NET with manual `DataTableReader` parsing. Every mapper builds parameter strings by hand (`@obj0`, `@obj1`) with no type safety.

**Plan:**
- Add NuGet package `Dapper` to `TreinoSportAPI.csproj`.
- Rewrite `BaseMapper` to expose a `IDbConnection` factory instead of raw `SqlServerConnection`.
- Rewrite each mapper method to use `connection.QueryAsync<T>()` / `connection.ExecuteAsync()` with named, strongly-typed parameters.
- Keep existing SQL queries — Dapper does not force ORM conventions, so queries remain under full control.
- Delete `SqlServerConnection.cs` and `BaseMapper.cs` after migration; replace with a single `SqlConnectionFactory.cs` that reads the connection string from `IConfiguration`.
- Files affected: `Mappers/Connection/BaseMapper.cs`, `Mappers/Connection/SqlServerConnection.cs`, `Mappers/ContaMapper.cs`, `Mappers/LoginMapper.cs`, `Mappers/TreinoMapper.cs`.

### 1.2 Introduce Repository Interfaces

**Current state:** Services depend on concrete mapper classes, making them untestable in isolation.

**Plan:**
- Create `Mappers/Interfaces/` folder.
- Add interfaces: `IContaMapper`, `ILoginMapper`, `ITreinoMapper`, `ITreinoMapperNoSQL`.
- Have each concrete mapper implement its interface.
- Update `Program.cs` DI registrations to bind interface → concrete: e.g. `services.AddScoped<IContaMapper, ContaMapper>()`.
- Update all service constructors to depend on interfaces, not concrete classes.

### 1.3 Standardize Error Handling with Global Middleware

**Current state:** Each controller has its own try/catch block calling `UtilEnvironment.InternalServerError()`. This is copy-pasted across all controllers.

**Plan:**
- Create `Middleware/GlobalExceptionMiddleware.cs` implementing `IMiddleware`.
- The middleware catches `APIException` (returns 400 with `ApiError` and `IsPublicMessage: true`) and all other exceptions (returns 500 with a generic `ApiError`).
- Register it in `Program.cs` via `app.UseMiddleware<GlobalExceptionMiddleware>()`.
- Remove all try/catch blocks from controllers; let exceptions propagate.
- Remove `UtilEnvironment.InternalServerError()` and `UtilEnvironment.IsPublicMessageCheck()` — they become redundant.
- Files affected: all 4 controllers, `Utilities/UtilEnvironment.cs`, `Program.cs`.

### 1.4 Introduce Service Interfaces

**Current state:** Controllers depend on concrete service classes, preventing mocking in tests.

**Plan:**
- Create `Services/Interfaces/IContaService.cs`, `ILoginService.cs`, `ITreinoService.cs`, `IAuthService.cs`.
- Existing `IEmailService.cs` already follows this pattern — replicate it for all services.
- Update `Program.cs` DI to bind interfaces → concretes.
- Update all controller constructors to depend on interfaces.

### 1.5 Move JWT Secret to Environment Variable

**Current state:** The JWT signing key is hardcoded in `appsettings.json` and committed to git.

**Plan:**
- Remove the `Jwt:Key` value from `appsettings.json` (leave the key name, set value to empty string or placeholder).
- Read it from an environment variable `JWT_SECRET` using `Environment.GetEnvironmentVariable("JWT_SECRET")` in `Program.cs`.
- Update `Dockerfile` to document that this env var must be provided at runtime.
- Add `JWT_SECRET` to `.gitignore`-protected `.env` file (for local dev) and document in `README.md`.

### 1.6 Add Input Validation with Data Annotations

**Current state:** Models (`Conta`, `Treino`) have no validation attributes. Invalid payloads reach service and mapper layers.

**Plan:**
- Add `[Required]`, `[MaxLength]`, `[EmailAddress]`, `[Range]` attributes to `Models/Conta.cs` and `Models/Treino.cs`.
- Enable automatic model validation response in `Program.cs` via `services.AddControllers().ConfigureApiBehaviorOptions(...)` to return 400 on invalid model state.
- This removes the need for manual null/empty checks in services for basic validation.

### 1.7 Fix MongoDB Timezone Hack

**Current state:** `TreinoMapperNoSQL.cs` applies a manual `-2 hour` offset on insert/update to compensate for timezone differences.

**Plan:**
- Store all `DateTime` values as UTC explicitly using `DateTime.UtcNow` and `DateTimeKind.Utc`.
- Configure MongoDB driver to serialize/deserialize `DateTime` as UTC: set `BsonSerializer.RegisterSerializer(new DateTimeSerializer(DateTimeKind.Utc))` in `MongoDBConnection.cs`.
- Remove the manual `-2` hour offset in `TreinoMapperNoSQL.cs`.
- Apply timezone conversion only at the API response layer (or handle it on the frontend) rather than in the data access layer.

### 1.8 Project Structure Cleanup

- Rename `MapperNoSQL/` to `Mappers/NoSQL/` for consistency.
- Move `Utilities/UtilEnvironment.cs` static members that remain after the middleware refactor into a proper `Infrastructure/Configuration.cs` class, injected via DI.
- Create `Mappers/Interfaces/` and `Services/Interfaces/` as described above.
- Add XML doc comments (`/// <summary>`) to all public service and mapper methods.

---

## 2. Backend New Feature — CT Search by Location

### 2.1 Add Location Fields to CONTA

**Database migration:**
```sql
ALTER TABLE CONTA
  ADD Latitude  FLOAT NULL,
      Longitude FLOAT NULL,
      Cep       VARCHAR(9) NULL;
```

- `Latitude` / `Longitude`: populated when the user grants browser geolocation.
- `Cep`: fallback CEP (postal code, format `00000-000`), populated via ViaCEP lookup when geolocation is denied.
- Add a computed column or stored procedure `dbo.fn_DistanciaKm(@lat1, @lon1, @lat2, @lon2)` using the Haversine formula for distance ordering.

**Model update:**
- Add `Latitude?`, `Longitude?`, `Cep?` to `Models/Conta.cs`.

### 2.2 Update Account Registration & Profile

- Add `Latitude`, `Longitude`, `Cep` to the registration and update payloads.
- Update `ContaMapper` (`InserirConta`, `AtualizarConta`) to include the new fields.
- Update `ContaService` and `UsuarioController` to accept and persist location on registration (`PUT /api/usuario/cadastrar`) and update (`PATCH /api/usuario/atualizar`).

### 2.3 New CT Search Endpoint

**New endpoint:** `GET /api/usuario/ct/buscar`

**Query parameters:**
| Param | Type | Description |
|---|---|---|
| `latitude` | double? | User's current latitude |
| `longitude` | double? | User's current longitude |
| `cep` | string? | Fallback CEP when coordinates are unavailable |
| `modalidade` | string? | Optional filter by sport type |
| `raio` | int? | Search radius in km (default: 20) |

**Logic:**
1. If `latitude` + `longitude` are provided, query SQL Server with Haversine distance ordering, filtered by `raio`.
2. If only `cep` is provided, call the ViaCEP external API (`https://viacep.com.br/ws/{cep}/json/`) to resolve coordinates, then proceed as above.
3. Return a list of matching CTs (name, description, modalidades offered, distance in km).
4. Endpoint is public (no `[Authorize]` required — allow unauthenticated browsing).

**New files:**
- `Services/Interfaces/IUsuarioService.cs` — add `BuscarCTs(double? lat, double? lng, string? cep, string? modalidade, int raio)`.
- `Services/UsuarioService.cs` — implement CT search logic, including ViaCEP HTTP call.
- `Mappers/Interfaces/IContaMapper.cs` — add `BuscarCTsPorLocalizacao(double lat, double lng, int raio, string? modalidade)`.
- `Mappers/ContaMapper.cs` — implement the Haversine SQL query.

**ViaCEP integration:**
- Add `HttpClient` registration in `Program.cs`: `services.AddHttpClient<UsuarioService>()`.
- ViaCEP returns `{ "logradouro": ..., "localidade": ..., "lat": ..., "lng": ... }` — parse and use lat/lng.
- Handle ViaCEP errors (invalid CEP, service unavailable) gracefully with `APIException`.

---

## 3. Backend Unit Tests

### 3.1 Setup Test Project

- Create `TreinoSportAPI.Tests/TreinoSportAPI.Tests.csproj` as an xUnit project in the same solution.
- Add to `TreinoSportAPI.sln`.
- NuGet packages:
  - `xunit` + `xunit.runner.visualstudio`
  - `Moq` — mocking library for interface dependencies
  - `FluentAssertions` — readable assertions
  - `Microsoft.NET.Test.Sdk`

### 3.2 Test Coverage Targets (Services Layer)

**`ContaService` tests** — `Tests/Services/ContaServiceTests.cs`
| Test | Scenario |
|---|---|
| `CadastrarConta_EmailJaExiste_ThrowsAPIException` | Duplicate email → throws `APIException` with public message |
| `CadastrarConta_Sucesso_ChamaMapperInserir` | Valid payload → `IContaMapper.InserirConta()` is called once |
| `RedefinirSenha_TokenInvalido_ThrowsAPIException` | Bad token → `APIException` |
| `RedefinirSenha_TokenValido_AtualizaSenha` | Good token → mapper update called |
| `EnviarTokenSenha_EmailNaoExiste_ThrowsAPIException` | Unknown email → `APIException` |
| `EnviarTokenSenha_EmailExiste_EnviaEmail` | Known email → `IEmailService.SendPasswordCode()` called |

**`TreinoService` tests** — `Tests/Services/TreinoServiceTests.cs`
| Test | Scenario |
|---|---|
| `CriarTreino_LimiteAlunosNegativo_ThrowsAPIException` | Invalid limit → exception |
| `CriarTreino_Sucesso_PersisteSqlEMongo` | Valid treino → both `ITreinoMapper` and `ITreinoMapperNoSQL` called |
| `AdicionarAluno_TreinoLotado_ThrowsAPIException` | Treino at capacity → exception |
| `AdicionarAluno_AlunoJaInscrito_ThrowsAPIException` | Duplicate enrollment → exception |
| `RemoverAluno_AlunoNaoInscrito_ThrowsAPIException` | Aluno not in treino → exception |
| `DeletarTreino_NaoPertenceAoCT_ThrowsAPIException` | Wrong CT deletes → exception |
| `MarcarPresenca_HorarioInexistente_ThrowsAPIException` | Bad horario code → exception |

**`AuthService` tests** — `Tests/Services/AuthServiceTests.cs`
| Test | Scenario |
|---|---|
| `Autenticar_CredenciaisInvalidas_ThrowsAPIException` | Wrong credentials → exception |
| `Autenticar_CredenciaisValidas_RetornaToken` | Valid credentials → non-null JWT string |
| `Autenticar_TokenContemRole` | Token claims include `role` = "CT" or "Aluno" |

**`UsuarioService` tests (CT Search)** — `Tests/Services/UsuarioServiceTests.cs`
| Test | Scenario |
|---|---|
| `BuscarCTs_SemCoordenadas_UsaViaCep` | No lat/lng → ViaCEP called |
| `BuscarCTs_CepInvalido_ThrowsAPIException` | ViaCEP returns error → exception |
| `BuscarCTs_ComCoordenadas_RetornaListaOrdenada` | Valid lat/lng → mapper called, list returned |

### 3.3 Test Helpers

- `Tests/Helpers/MockBuilders.cs` — static factory methods to build `Conta`, `Treino`, `Horario` test objects.
- `Tests/Helpers/FakeHttpMessageHandler.cs` — for mocking `HttpClient` in ViaCEP tests.

---

## 4. Frontend Completion & Best Practices

### 4.1 Add HTTP Interceptor for Auth Headers

**Current state:** Every service method manually adds `Authorization: Bearer <token>` to the request.

**Plan:**
- Create `src/app/core/interceptors/auth.interceptor.ts`.
- The interceptor reads the token from `localStorage` and clones every outgoing request with the `Authorization` header.
- Register it in `app.config.ts` via `provideHttpClient(withInterceptors([authInterceptor]))`.
- Remove manual header injection from `TreinoService` and any other service.
- Move the `core/` folder from empty to containing: `interceptors/`, `guards/`.

### 4.2 Fix AuthGuard — Token Expiry Validation

**Current state:** `auth.guard.ts` only checks `localStorage.getItem('token') !== null`. An expired token passes the guard.

**Plan:**
- In `auth.guard.ts`, decode the token using `JwtHelperService` from `@auth0/angular-jwt`.
- Check `jwtHelper.isTokenExpired(token)` — if expired, clear localStorage and redirect to `/login`.
- Also validate the token is not malformed (wrap in try/catch).

### 4.3 Fix Logout — Clear Token

**Current state:** The "Sair" link in `lateral-menu` routes to `/login` but does not clear the token.

**Plan:**
- In `lateral-menu.component.ts`, add a `logout()` method that calls `localStorage.removeItem('token')` before navigating.
- Bind the "Sair" link to `(click)="logout()"` instead of a raw `routerLink`.

### 4.4 Fix TreinoService Stale Token Bug

**Current state:** `getTreinos()` and `getTreino(id)` capture the token at service construction time. If the user logs in after the service instantiates, these calls use an empty token.

**Plan:**
- After adding the HTTP interceptor (4.1), remove all token retrieval from `TreinoService` entirely — the interceptor handles it.
- If the interceptor is not implemented first, fix as a minimum: read the token inside each method at call time (`localStorage.getItem('token')`), not in the constructor.

### 4.5 Fix Environment File Typo & Add Production Config

**Current state:** `src/environments/enviroment.ts` (missing `n`). No production environment file.

**Plan:**
- Rename `enviroment.ts` → `environment.ts` and update all imports.
- Create `src/environments/environment.prod.ts` with `production: true` and a production `apiUrl` placeholder.
- Configure `angular.json` `fileReplacements` under the `production` build configuration to swap `environment.ts` for `environment.prod.ts`.
- Replace hardcoded LAN IP (`192.168.15.4`) with a configurable base URL from the environment file.

### 4.6 Centralize API URL Usage

**Current state:** `ApiService` has the `apiUrl`, but `TreinoService` and `AuthService` bypass it and hardcode the URL internally.

**Plan:**
- Ensure all services use `ApiService` (or inject `environment.apiUrl` directly) — no hardcoded URLs outside the environment file.
- Refactor `TreinoService` and `AuthService` to use `ApiService` generic methods or inject `environment` for the base URL.

### 4.7 Implement Home Aluno Page

**Current state:** `home-aluno.component.ts` is an empty shell.

**Plan:**
- Display the list of treinos the student is enrolled in (`GET /api/treino/aluno/todos`).
- Each treino card shows: name, modalidade, CT name, days/times, and a "Ver detalhes" button.
- Add a search/filter input to filter the displayed list by name or modalidade.
- Add a navigation link to the CT Search page (new feature, see section 5).
- Add `TreinoService.getTreinosAluno()` method calling `GET /api/treino/aluno/todos`.

### 4.8 Implement Editar Treino Modal

**Current state:** `editar-treino.component.ts` exists but is completely empty.

**Plan:**
- Implement a form pre-populated with the current treino's data (nome, descricao, modalidade, limiteAlunos, dataVencimento).
- On submit, call `PATCH /api/treino/ct/detalhes` with the updated data.
- Wire the modal into `DialogService.openEditarTreino(treino: Treino)`.
- Add an "Editar" button to `GerenciamentoTreinoComponent` that opens this modal.
- Add `TreinoService.atualizarTreino(treino: Treino)` calling `PATCH /api/treino/ct/detalhes`.

### 4.9 Implement Gerenciamento de Horários (Editar Horários)

**Current state:** The backend has `PATCH /api/treino/ct/horarios` and `GET /api/treino/ct/horarios` but there is no frontend UI to use them.

**Plan:**
- Add an "Editar Horários" section or modal in `GerenciamentoTreinoComponent`.
- Allow CT to add/remove days and time slots from an existing treino.
- Call `TreinoService.atualizarHorarios()` on save.

### 4.10 Implement Marcar/Remover Presença (CT view)

**Current state:** The attendance modals show present students but there is no UI for a CT to mark/remove attendance.

**Plan:**
- In `ListaPresencaComponent`, if the logged-in user is a CT, show a toggle button next to each student.
- Call `PATCH /api/treino/aluno/presenca/marcar` or `PATCH /api/treino/aluno/presenca/remover` accordingly.
- Add corresponding methods to `TreinoService`.

### 4.11 Add Reactive Form Validation Feedback

**Current state:** `CadastroComponent` has form validation but error messages are not always shown. `CriarTreinoComponent` has minimal validation feedback.

**Plan:**
- Add `<mat-error>` elements tied to each `FormControl`'s error state in both forms.
- Ensure all required fields show inline errors on submit attempt.

---

## 5. Frontend New Feature — CT Search (Aluno)

### 5.1 New Page: Buscar CTs

**Route:** `/buscar-cts` (accessible to authenticated Alunos; also consider public access for discovery)

**Component:** `src/app/components/buscar-cts/buscar-cts.component.ts`

**Flow:**
1. On page load, the browser prompts for geolocation permission via `navigator.geolocation.getCurrentPosition()`.
2. If granted: store lat/lng, call the search API immediately with current coordinates.
3. If denied: show a CEP input field. On submit, call the search API with the CEP.
4. Results are displayed as Material cards, sorted nearest-first, showing:
   - CT name and description
   - Distance (e.g., "3.2 km")
   - Modalidades offered (as chips)
   - A "Ver detalhes" or "Entrar em contato" CTA (future scope)
5. A `<mat-select>` filter for modalidade narrows results without re-fetching (client-side filter on the loaded list). Re-fetch only if the user changes location or radius.
6. A radius slider (10 km / 20 km / 50 km) allows expanding the search.

### 5.2 New Service Method

- Add `UsuarioService.buscarCTs(params: BuscarCTsParams): Observable<CTResult[]>`.
- `BuscarCTsParams`: `{ latitude?: number, longitude?: number, cep?: string, modalidade?: string, raio?: number }`.
- New model: `src/app/models/ct-result.model.ts` with `{ codigo, nome, descricao, distanciaKm, modalidades }`.

### 5.3 Navigation Integration

- Add a "Buscar CTs" link in `LateralMenuComponent` visible only to Aluno role (check `UserStateService.role`).
- Add a prominent "Buscar CTs" button on the `HomeAlunoComponent`.
- Add route `{ path: 'buscar-cts', component: BuscarCtsComponent, canActivate: [AuthGuard] }` to `app.routes.ts`.

### 5.4 Location Permission UX

- Show a clear, friendly prompt explaining why location is needed before triggering the browser dialog.
- If location is denied, the CEP fallback input appears with a ViaCEP-powered auto-fill of the city name as confirmation feedback.
- Display an `<mat-spinner>` during the API call.
- Display `<mat-error>` or a snackbar for API errors (invalid CEP, no results found).

---

## 6. Execution Order

The recommended implementation sequence to minimize rework:

```
Phase 1 — Backend Foundation (enables testability)
  1.1  Introduce service interfaces (IContaService, ITreinoService, IAuthService)
  1.2  Introduce mapper interfaces (IContaMapper, ILoginMapper, ITreinoMapper, ITreinoMapperNoSQL)
  1.3  Migrate data layer to Dapper
  1.4  Global exception middleware (remove controller try/catch)
  1.5  Move JWT secret to environment variable
  1.6  Add input validation (Data Annotations)
  1.7  Fix MongoDB UTC timezone handling
  1.8  Project structure cleanup

Phase 2 — Backend New Feature
  2.1  DB migration: add Latitude, Longitude, Cep to CONTA
  2.2  Update registration/profile endpoints with location fields
  2.3  Implement CT search endpoint with Haversine + ViaCEP fallback

Phase 3 — Backend Tests
  3.1  Create test project (xUnit + Moq + FluentAssertions)
  3.2  ContaService tests
  3.3  TreinoService tests
  3.4  AuthService tests
  3.5  UsuarioService / CT Search tests

Phase 4 — Frontend Best Practices
  4.5  Fix environment file (rename + prod config)
  4.6  Centralize API URL
  4.1  Add HTTP interceptor
  4.2  Fix AuthGuard token expiry
  4.3  Fix logout
  4.4  Fix TreinoService stale token bug
  4.11 Add reactive form validation feedback

Phase 5 — Frontend Feature Completion
  4.7  Implement Home Aluno
  4.8  Implement Editar Treino modal
  4.9  Implement Gerenciamento de Horários UI
  4.10 Implement Marcar/Remover Presença UI

Phase 6 — Frontend New Feature (CT Search)
  5.2  Models & service method for CT search
  5.3  Navigation integration (route + menu link)
  5.1  BuscarCTs component (geolocation + CEP fallback + results UI)
  5.4  Location permission UX polish
```

---

## Notes & Open Items

- **CORS:** `Program.cs` currently whitelists hardcoded LAN IPs. For any deployment, replace with a configurable allowed-origins list read from `appsettings.json`.
- **JWT Expiry (30 min):** Consider adding refresh token support in a later phase; for now, the AuthGuard fix (4.2) will correctly kick users out when the token expires.
- **`treino-sport: "file:"` in package.json:** Remove this self-referencing entry — it is a leftover artifact.
- **`core/` folder (frontend):** After adding the interceptor and moving the guard, the `core/` folder will have content. Keep it as the home for cross-cutting Angular concerns.
- **Background Service (`RenovarAulasBackground`):** No changes needed, but add a unit test to verify the nightly cleanup logic once the test project is in place.

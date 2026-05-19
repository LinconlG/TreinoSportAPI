# TreinoSport — Task Tracker

> Generated from `plan.md` on 2026-04-30
> **Backend:** `C:\Users\linco\OneDrive\Documents\Repositorio\TreinoSportAPI`
> **Frontend:** `C:\Users\linco\OneDrive\Documents\Repositorio\TreinoSportWeb`

---

## Phase 1 — Backend Foundation (enables testability)

- [x] **1.1** Introduce Service Interfaces (`IContaService`, `ITreinoService`, `IAuthService`, `IEmailService` already exists — replicate pattern)
- [x] **1.2** Introduce Mapper Interfaces (`IContaMapper`, `ILoginMapper`, `ITreinoMapper`, `ITreinoMapperNoSQL`) and update DI + service constructors
- [x] **1.3** Migrate Data Layer from Raw SQL/ADO.NET to Dapper (add NuGet, rewrite `BaseMapper`, all concrete mappers, delete `SqlServerConnection.cs`)
- [x] **1.4** Global Exception Middleware (`Middleware/GlobalExceptionMiddleware.cs`), remove controller try/catch blocks
- [x] **1.5** Move JWT Secret to Environment Variable (`JWT_SECRET` env var, update `appsettings.json` + `Program.cs` + `Dockerfile`)
- [x] **1.6** Add Input Validation with Data Annotations to `Models/Conta.cs` and `Models/Treino.cs`; configure `ApiBehaviorOptions`
- [x] **1.7** Fix MongoDB Timezone Hack — store UTC, configure BsonSerializer, remove manual `-2h` offset in `TreinoMapperNoSQL.cs`
- [x] **1.8** Project Structure Cleanup — rename `MapperNoSQL/` → `Mappers/NoSQL/`, create `Mappers/Interfaces/` and `Services/Interfaces/`, add XML doc comments

---

## Phase 2 — Backend New Feature (CT Search by Location)

- [x] **2.1** DB Migration — add `Latitude`, `Longitude`, `Cep` columns to `CONTA` table; add Haversine SQL function `dbo.fn_DistanciaKm`; update `Models/Conta.cs`
- [x] **2.2** Update Account Registration & Profile — update `ContaMapper`, `ContaService`, `UsuarioController` to accept and persist location fields
- [x] **2.3** New CT Search Endpoint `GET /api/usuario/ct/buscar` — Haversine query, ViaCEP fallback, `IUsuarioService`, `UsuarioService`, `IContaMapper.BuscarCTsPorLocalizacao`

---

## Phase 3 — Backend Unit Tests

- [x] **3.1** Setup Test Project (`TreinoSportAPI.Tests`) — xUnit, Moq, FluentAssertions, add to solution
- [x] **3.2** `ContaService` Tests — 6 scenarios (duplicate email, success insert, token invalid/valid, email not found/found)
- [x] **3.3** `TreinoService` Tests — 7 scenarios (invalid limit, success persist, lotado, duplicate enroll, aluno not found, wrong CT delete, bad horario)
- [x] **3.4** `AuthService` Tests — 3 scenarios (invalid creds, valid creds return token, token contains role)
- [x] **3.5** `UsuarioService` / CT Search Tests — 3 scenarios (no coords → ViaCEP, invalid CEP, valid coords → ordered list); helpers `MockBuilders.cs` + `FakeHttpMessageHandler.cs`

---

## Phase 4 — Frontend Best Practices

- [x] **4.5** Fix Environment File — rename `enviroment.ts` → `environment.ts`, create `environment.prod.ts`, configure `angular.json` fileReplacements, replace hardcoded IP
- [x] **4.6** Centralize API URL — ensure all services use `environment.apiUrl`; refactor `TreinoService` and `AuthService`
- [x] **4.1** Add HTTP Interceptor for Auth Headers (`core/interceptors/auth.interceptor.ts`), register in `app.config.ts`
- [x] **4.2** Fix AuthGuard — decode JWT, check expiry with `JwtHelperService`, clear localStorage on expiry
- [x] **4.3** Fix Logout — add `logout()` method in `lateral-menu.component.ts` that clears token before navigating
- [x] **4.4** Fix TreinoService Stale Token Bug — remove constructor-time token capture; rely on interceptor (or read at call time)
- [x] **4.11** Add Reactive Form Validation Feedback — `<mat-error>` in `CadastroComponent` and `CriarTreinoComponent`

---

## Phase 5 — Frontend Feature Completion

- [x] **4.7** Implement Home Aluno Page — list enrolled treinos, search/filter, "Buscar CTs" button, `TreinoService.getTreinosAluno()`
- [x] **4.8** Implement Editar Treino Modal — pre-populated form, `PATCH /api/treino/ct/detalhes`, wire into `DialogService`, "Editar" button in `GerenciamentoTreinoComponent`
- [x] **4.9** Implement Gerenciamento de Horários UI — add/remove days & time slots, call `TreinoService.atualizarHorarios()`
- [x] **4.10** Implement Marcar/Remover Presença (CT view) — toggle button in `ListaPresencaComponent`, call mark/remove attendance endpoints

---

## Phase 6 — Frontend New Feature (CT Search)

- [x] **5.2** Models & Service Method — `ct-result.model.ts`, `UsuarioService.buscarCTs()`, `BuscarCTsParams` interface
- [x] **5.3** Navigation Integration — route `buscar-cts`, menu link (Aluno only), button on `HomeAlunoComponent`
- [x] **5.1** BuscarCTs Component — geolocation prompt, CEP fallback input, results as Material cards, modalidade filter, radius slider
- [x] **5.4** Location Permission UX Polish — friendly prompt, spinner, snackbar errors, ViaCEP city name confirmation

---

## Phase 7 — Cleanup & Hardening

- [x] **7.1** CORS: move hardcoded LAN IPs to `appsettings.json` `Cors:AllowedOrigins` array; read via `IConfiguration` in `Program.cs`
- [x] **7.2** `package.json`: remove self-referencing `"treino-sport": "file:"` entry
- [x] **7.3** `RenovarAulasBackground`: inject `ITreinoMapperNoSQL` via constructor DI instead of `new TreinoMapperNoSQL(...)`
- [x] **7.4** `RenovarAulasBackground`: unit test — verify `ReiniciarPresencas` clears only yesterday's DayOfWeek horários (mocked `ITreinoMapperNoSQL`)
- [x] **7.5** `ApiService.put`: add optional `HttpParams` parameter; fix `putTreinoAluno` URL-string hack in `TreinoService`
- [x] **7.6** Remove unused `loginPath` field in `LateralMenuComponent`
- [x] **7.7** Consolidate JWT decoding — replace `@auth0/angular-jwt` / `JwtHelperService` in `UserStateService` with native `atob`; uninstall the package
- [x] **7.8** Add `.env.example` to backend root documenting `JWT_SECRET`, `ConnectionStrings__DataBaseConnection`, `ConnectionStrings__MongoDB`

---

## Phase 8 — Critical Bug Fixes (from sugest.md)

- [x] **8.1** Fix `getTreinosAluno` missing `codigoUsuario` param — read `CodigoConta` from JWT claims and pass to `GET /api/treino/aluno/todos`
- [x] **8.2** Fix attendance marking — change `marcarPresenca`/`removerPresenca` to use `codigoAluno` (int) instead of email; pass full schedule as request body; add "Adicionar presença" button
- [x] **8.3** Fix `atualizarHorarios` payload — send `codigoTreino` as query param and `List<DiaDaSemana>` as body
- [x] **8.4** Fix CEP geocoding — integrate Nominatim to convert CEP → coordinates in `UsuarioService.ResolverCoordsViaCep()`
- [x] **8.5** Fix background job attendance reset — call `AtualizarDiasHorarios()` after clearing `AlunosPresentes` in `RenovarAulasBackground` (done in 7.3)
- [x] **8.6** Add `Modalidades` field to `CTResult` — JOIN `TREINO` table in CT search query and aggregate modalidades per CT

---

## Phase 9 — High-Priority Improvements (from sugest.md)

- [x] **9.1** Profile Edit page — `ProfileComponent` at `/perfil`, pre-populated form, calls `PATCH /api/usuario/atualizar`
- [x] **9.2** CT location at registration — add geolocation/CEP step to `CadastroComponent` when `isCentroTreinamento` is checked
- [x] **9.3** Password reset UI — `EsqueciSenhaComponent` at `/esqueci-senha` with 3-step flow; wire "Esqueci minha senha" link
- [x] **9.4** Remove-student button in Gerenciamento Treino — `removerAluno()` in `TreinoService`, confirmation dialog, × button in template
- [x] **9.5** Delete Treino also deletes MongoDB document — call `DeletarHorarios(codigoTreino)` in `TreinoService.DeletarTreino()`

---

## Phase 10 — Backend Security & Hardening (from sugest.md)

- [x] **10.1** Password hashing — BCrypt on registration and login (`BCrypt.Net-Next` NuGet)
- [x] **10.2** Add `[Authorize]` to `PATCH /api/usuario/atualizar` and validate caller matches `codigoConta`
- [x] **10.3** Rate limiting on `PUT /api/usuario/senha/envio` — max 3 requests per email per 15 min
- [x] **10.4** Fix `Horario.Codigo` uniqueness — use UUID or server-assigned ID instead of client-side `horarios.Length + 1`
- [x] **10.5** Swagger JWT bearer config — add `AddSecurityDefinition`/`AddSecurityRequirement` in `Program.cs`; add XML doc comments to controllers

---

_Legend: `[ ]` pending · `[~]` in progress · `[x]` done · `[!]` failed/blocked_

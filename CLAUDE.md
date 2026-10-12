# CLAUDE.md

Dayardan (ExamTrainer): a multiple-choice exam practice app with multilingual content and AI feedback on mistakes. Production domain: `dayardan.xyz`. See `README.md` for the full feature list, setup and endpoint table.

## Layout

- `api/`: ASP.NET Core 9 Web API, EF Core 9 + SQL Server, Identity + JWT. Single project (`api.csproj`, root namespace `api`), no test project.
- `client/`: React 19 + TypeScript + Vite 8 SPA, React Router v7, plain CSS (one `.css` file per component or page).
- `api/wwwroot/`: **build output of the client**, committed to git. The API serves it with `MapFallbackToFile("index.html")`.
- `api/Data.zip`: an old snapshot of source folders, not used by the build. Ignore it.

## Commands

```bash
# API (from api/) – http://localhost:5041, Swagger at /swagger
dotnet run
dotnet ef migrations add <Name>
dotnet ef database update

# Client (from client/) – http://localhost:5173
npm run dev
npm run lint
npm run build     # tsc -b && vite build -> writes straight into ../api/wwwroot (emptyOutDir)
```

There are no automated tests. Check changes with `dotnet build`, `npm run lint` and `npm run build`.

## Configuration

- Secrets are read from user secrets (`UserSecretsId` in `api.csproj`) or environment variables (`JWT__SigningKey`, ...). `appsettings.json` keeps blank placeholders; `appsettings.Production.json` is gitignored. Never commit real values.
- Keys: `ConnectionStrings:DefaultConnection`, `JWT:Issuer`, `JWT:Audience`, `JWT:SigningKey`, `Groq:ApiKey`. The code reads `Jwt:*`; .NET config keys are case-insensitive, so both spellings work. `Gemini:ApiKey` is unused (left over from before the switch to Groq).
- The client reads **`VITE_API_BASE_URL`** (`client/.env` = `http://localhost:5041`; `.env.production` is empty, so production uses same-origin relative URLs). The README says `VITE_API_URL`, which is wrong.
- CORS origins are hardcoded in `Program.cs`. Add new frontend origins there.

## Backend conventions

- Request flow: Controller → Repository (`Repositories/Interfaces` + `Implementations`) → `ApplicationDbContext`. Business logic beyond CRUD goes in `Services/`. Register new repositories and services as scoped in `Program.cs`.
- Entities map to DTOs through hand-written static extension methods in `Mappers/` (no AutoMapper).
- Some entity classes (`Question`, `Subject`, `AnswerOption`, the `*Translation` classes) are in the **global namespace**; `AppUser`, `ExamAttempt` and `UserAnswer` are in `api.Models`. Follow whichever the neighbouring file does.
- **Translations model:** `Subject`, `Question` and `AnswerOption` have no text of their own. Text lives in `*Translation` rows keyed by `LanguageCode` (`en` / `ru` / `ky`). Read endpoints take an optional `?lang=` and fall back to the first translation. Admin endpoints (`.../admin`) return every translation.
- Question updates replace the whole object: `QuestionRepository.UpdateAsync` clears and re-adds the translations and answer options.
- Roles are `Admin` and `User`, seeded with `HasData` in `ApplicationDbContext`. `DbSeeder.SeedAdminAsync` runs at startup and creates the admin account.
- Exams: `GET /api/exam/start/{subjectId}` picks 30 random questions and returns every language at once (`ExamStartMultilingualDto`), so the client can switch language mid-exam. `POST /api/exam/submit` works without logging in; if a token is present, the attempt is linked to that user.
- AI feedback (`StatisticsService`): calls Groq's OpenAI-compatible chat API (`meta-llama/llama-4-scout-17b-16e-instruct`, JSON mode) with a Russian or Kyrgyz prompt, normalizes the result into `ExamFeedbackDto`, and caches it as JSON in `ExamAttempt.AiFeedback`. `force=true` regenerates it. Only `ru` and `ky` prompts exist; the client sends `ru` when the UI language is `en`.
- After changing the model, add a migration. Migrations live in `api/Migrations/`.

## Frontend conventions

- All HTTP calls go through `client/src/services/api.ts` (`apiFetch`, plus `apiFetchMultipart` for uploads). It attaches the JWT from `localStorage`, dispatches `api:unauthorized` on a 401, and replaces every server error message with a generic one.
- Shared types are in `client/src/types/index.ts`. Keep them in sync with the API DTOs (camelCase).
- Auth state lives in `AuthContext`: the token and username are kept in `localStorage`, and `isAdmin` is decoded from the JWT role claim. Language lives in `LanguageContext`, defaults to `ru`, and is saved in `localStorage`.
- UI strings go in `client/src/i18n/strings.ts`, with entries for all three languages, accessed through `useT()`. Some pages still have inline `lang === 'ru' && ...` strings; move them to `strings.ts` when you touch them.
- Pages go in `src/pages/`, reusable pieces in `src/components/`, and routes are registered in `App.tsx`.

## Deployment

Production runs on **MonsterASP.NET** (Premium Single plan, Windows/IIS, EU datacenter; the subscription renews yearly in October). To reach the control panel, log in at https://www.monsterasp.net.

- One website serves `https://dayardan.xyz` and `www`. Let's Encrypt SSL is managed in the panel.
- The MSSQL database is hosted on MonsterASP as well. It is only reachable from the hosted site, not from a local machine, and the panel makes daily automatic backups.
- Domain: registered at **Namecheap** (expires 2027-05-07). DNS is managed in Namecheap's Advanced DNS, with A records for `@` and `www` that point at the MonsterASP server.
- Production config: `api/appsettings.Production.json` (gitignored) is deployed with the app. It holds the connection string, JWT key and Groq key. Server names and credentials are kept there and in the panel, not in the repo.

To publish, run `npm run build` in `client/` first if the SPA changed, then from `api/`:

```bash
dotnet publish -c Release -p:PublishProfile=MonsterASP -p:Password=<web deploy password>
```

Or use Rider: right-click `api` → Publish → choose the `MonsterASP` profile. The profile is `api/Properties/PublishProfiles/MonsterASP.pubxml` (gitignored). It sets `SkipExtraFilesOnServer=true` so that server-only files such as `wwwroot/uploads` survive deploys. The Web Deploy password is in the `.publishSettings` file downloaded from the panel and is never stored in the repo. When the app fails to start (HTTP 500.30), the panel's **Logs** page shows the startup exception.

History: until about July 2026 the app ran on a SmarterASP.NET free trial, which expired and deleted the site. `api/Dockerfile` (port 8080) exists but has never been used in production.

## Known issues and future work

Security and correctness, highest priority first:

1. **Seeded admin credentials are hardcoded** in `api/Data/Seed/DbSeeder.cs` and committed. Move them to configuration and change that password. Old commits also contain `appsettings.Production.json` with real secrets (removed in `3b032f1`, but still in history), so rotate the JWT signing key, the Groq key and the DB password.
2. **Correct answers leak to the client.** `ExamStartDto` and the public `GET /api/subjects/{id}/questions` both include `IsCorrect`. Remove it from student-facing DTOs; correctness should only be visible to admins and after submission (`ExamAttemptDetailDto`).
3. **The AI feedback rate limit is client-side only**: `DAILY_LIMIT` in `ExamFeedbackPage.tsx` is stored in `localStorage`, so anyone can bypass it, and `force=true` calls Groq every time. Enforce it on the server (ASP.NET rate limiter or a per-user counter in the DB).
4. `SubmitExam` does not check that the questions belong to `request.SubjectId` and trusts the client-supplied `SubjectId` and `TotalQuestions`. It also loads each question separately (N+1 queries).
5. `FileUploadService.DeleteFileAsync` builds a path from a user-supplied URL without restricting it to `wwwroot/uploads` (path traversal; mitigated only by the Admin-only route). Image validation checks the file extension only.
6. `ExamController.GetExamFeedback` returns raw exception messages, including Groq error bodies, to the client. `AccountController.Register` returns the whole exception object with a 500. `StatisticsService` writes full Groq responses with `Console.WriteLine`.
7. `QuestionController.GetQuestions` has a duplicated `[HttpGet]` attribute and no upper bound on `pageSize`. `StartExam` loads every question of the subject into memory before shuffling.
8. `TokenService` uses `DateTime.Now` (should be `UtcNow`). `Login` compares `Email == input.ToLower()` rather than using the normalized email.

Features and infrastructure:

- No tests. Add an xUnit project for the API (start with scoring in `SubmitExam` and `NormalizeStudyRoadmap`) and Vitest for the client.
- No English AI prompt, and the perfect-score message is hardcoded in Russian or Kyrgyz.
- No CI. A GitHub Actions workflow that runs `dotnet build`, `npm run lint` and `npm run build` would cover the main checks.
- Move upload storage to blob storage or a mounted volume.
- Fix the README: it uses the wrong env var name and still describes copying `dist/` by hand, but the build already writes into `api/wwwroot`.
- Delete leftovers: `api/Data.zip`, the `Gemini` config key, the ngrok `allowedHosts` in `vite.config.ts`, and the Swagger title `"Demo API"`.

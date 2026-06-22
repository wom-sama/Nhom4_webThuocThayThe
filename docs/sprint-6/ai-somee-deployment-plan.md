# S6 AI and Somee Free deployment plan

Audit date: 2026-06-21.

## Provider facts

Somee Free currently publishes these limits:

- 150 MB web storage.
- 5 GB monthly transfer.
- One SQL Server database with 30 MB data and 30 MB log.
- Forced advertising and shared hosting.
- ASP.NET Core 10 is listed as supported.

Source: https://somee.com/FreeAspNetHosting.aspx

Somee does not publish a guaranteed CPU, memory, concurrent-user limit or SLA for the Free plan.
Therefore, `10 concurrent users` is an engineering demo baseline for this project, not an official
Somee capacity claim. PERF10 uses 10 virtual users, 250 ms pacing, 120 mixed public requests, p95
at most 1.5 seconds and at most 1 percent errors. A real-host test must be rerun after deployment.

## AI boundary

The external provider is Google Gemini `gemini-2.5-flash`, called only when the user requests an
explanation. The deterministic recommendation service remains the source of score, stock and safety
alerts. AI has no database access, no tool calls, no write permission and no approval permission.

The request contains only generic drug/candidate fields and deterministic reasons. It excludes email,
display name, clinical note and patient safety profile. Free-tier data may be used to improve Google
products, so no personal or clinical record may be sent.

Provider references:

- Models: https://ai.google.dev/gemini-api/docs/models
- API keys: https://ai.google.dev/gemini-api/docs/api-key
- Structured output: https://ai.google.dev/gemini-api/docs/structured-output
- Rate limits: https://ai.google.dev/gemini-api/docs/rate-limits
- Pricing/data-use table: https://ai.google.dev/gemini-api/docs/pricing

Google states that limits depend on project and usage tier and must be read in AI Studio. New AI Studio
keys are authorization keys; unrestricted standard keys were rejected starting 2026-06-19. The API key
must be stored only as a runtime secret and never in Git, Jira comments, logs or browser JavaScript.

## Runtime settings

```text
AI__Gemini__Enabled=true
AI__Gemini__ApiKey=<Google AI Studio authorization key>
AI__Gemini__Model=gemini-2.5-flash
```

Docker maps the untracked `.env` values `AI_GEMINI_ENABLED`, `GEMINI_API_KEY` and
`AI_GEMINI_MODEL` to these settings. The provider times out after 8 seconds, caches a generic
explanation for 15 minutes and falls back to rule-based text for disabled, timeout, 429, 5xx,
blocked or malformed responses. The endpoint permits five requests per minute per IP.

## Somee release procedure

1. Run all unit, integration, acceptance, security and performance suites.
2. Run `powershell -File .\scripts\Publish-Somee.ps1`.
3. Confirm the publish directory is below 145 MB, leaving at least 5 MB headroom.
4. Create the Somee site and SQL database, then apply `database/schema.sql`.
5. Configure `ConnectionStrings__PharmacyDatabase` and the AI settings outside source control.
6. Upload the generated zip contents through the Somee control panel/FTP.
7. Verify `/health`, public search, login/RBAC, AI fallback and one live no-PII AI request.
8. Run PERF10 against the real URL at a low-traffic time and record p95/errors/transfer in Jira.
9. If the host cannot protect runtime secrets, keep AI disabled or use a secret-capable host.

The 5 GB estimate in PERF10 covers application response bodies only. Somee-injected advertisements,
TLS overhead, browser cache misses and Gemini traffic are outside that estimate, so actual monthly
transfer must be observed in the Somee panel.

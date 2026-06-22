# ASP.NET Core localization architecture

Jira story: `N4WTT-209`

Sub-task: `N4WTT-218`

## Decision

The application uses one supported UI culture, `vi-VN`, for S8. The architecture still uses standard
ASP.NET Core localization services so another reviewed culture can be added without replacing visible
strings throughout Razor views.

This document defines the implementation contract. It does not unlock UI implementation before the
Figma design gate in `N4WTT-212` is approved.

## Current-state evidence

The `develop` baseline at `d912530` has these localization risks:

- `Views/Shared/_Layout.cshtml` declares `<html lang="en">`.
- Razor views contain Vietnamese text without diacritics and several English fallback pages.
- `AddControllersWithViews()` is registered without `AddLocalization`, view localization or
  DataAnnotations localization.
- default validation attributes therefore produce framework-provided English messages.
- currency uses ad-hoc `VND` suffixes and dates are formatted independently in each view.
- UI strings, persisted workflow values and domain data are not separated explicitly.

These findings are the baseline for `N4WTT-226`; they are not treated as already fixed.

## Culture contract

| Setting | S8 value |
| --- | --- |
| Default culture | `vi-VN` |
| Default UI culture | `vi-VN` |
| Supported cultures | `vi-VN` only |
| URL culture segment | Not used in S8 |
| Culture cookie/query override | Not exposed in S8 |
| Root document language | `vi` from `CurrentUICulture.TwoLetterISOLanguageName` |
| Date | `dd/MM/yyyy` |
| Date and time | `dd/MM/yyyy HH:mm` |
| Currency | `vi-VN`, zero fractional digits |
| Source encoding | UTF-8 |

Using one culture prevents a query string or stale browser preference from creating mixed-language
screens. A culture selector requires a separate product decision and complete resources for every
supported locale.

## Service and middleware design

The implementation PR adds localization to the existing MVC registration:

```csharp
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services
    .AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (_, factory) =>
            factory.Create(typeof(SharedResource));
    });

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var vietnamese = new CultureInfo("vi-VN");
    options.DefaultRequestCulture = new RequestCulture(vietnamese);
    options.SupportedCultures = [vietnamese];
    options.SupportedUICultures = [vietnamese];
    options.ApplyCurrentCultureToResponseHeaders = true;
    options.RequestCultureProviders.Clear();
});
```

`UseRequestLocalization()` is resolved from `IOptions<RequestLocalizationOptions>` and runs after
static files but before routing, authentication and endpoints:

```csharp
var localizationOptions = app.Services
    .GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(localizationOptions);
app.UseRouting();
```

The layout derives the language attribute from the active UI culture:

```cshtml
@using System.Globalization
<html lang="@CultureInfo.CurrentUICulture.TwoLetterISOLanguageName">
```

## Resource ownership and layout

Marker classes keep resource lookups testable and avoid coupling domain services to view paths.

```text
Localization/
  SharedResource.cs
  AuthenticationResource.cs
  DrugSearchResource.cs
  InventoryResource.cs
  ExpertReviewResource.cs
  ReportsResource.cs
  AiExplanationResource.cs
Resources/
  SharedResource.vi-VN.resx
  AuthenticationResource.vi-VN.resx
  DrugSearchResource.vi-VN.resx
  InventoryResource.vi-VN.resx
  ExpertReviewResource.vi-VN.resx
  ReportsResource.vi-VN.resx
  AiExplanationResource.vi-VN.resx
```

| Resource | Owns | Does not own |
| --- | --- | --- |
| `SharedResource` | navigation, common actions, generic loading/error states | feature-specific clinical wording |
| `AuthenticationResource` | login, access denied, session and credential messages | account identifiers or secrets |
| `DrugSearchResource` | search, availability, recommendation labels and score reasons | drug names or immutable source data |
| `InventoryResource` | warehouse, batch, quantity, expiry and inventory validation | external warehouse identifiers |
| `ExpertReviewResource` | decision labels, notes, confirmation and audit feedback | persisted status codes |
| `ReportsResource` | metrics, source status, backup and audit presentation | audit action/entity identifiers |
| `AiExplanationResource` | prompt boundary, optional AI disclosure, fallback, rate-limit and error | deterministic score computation |

Resource keys use stable semantic names such as `Navigation.DrugSearch`,
`Inventory.Validation.ExpiryBeforeImport` and `Ai.Disclosure.NotMedicalAdvice`. They do not use the
Vietnamese sentence itself as a key.

## Translation boundaries

The following data remains unchanged and is rendered as supplied by the domain or external source:

- brand names, INN/active-ingredient names and manufacturer names;
- ATC codes, medicine IDs, batch numbers, warehouse IDs and source-system identifiers;
- role codes, authorization policy names and persisted workflow status codes;
- audit action/entity codes and API contract property names.

UI adapters map persisted codes to resource keys. No migration renames role or status values merely to
add Vietnamese diacritics.

Descriptions, usage instructions and contraindications in seed/reference data require a separate data
quality review. A localized label must never disguise missing or unverified clinical data.

## DataAnnotations strategy

View models receive explicit localized display names and validation messages. S8 must not rely on the
framework's current-language defaults.

```csharp
[Display(Name = "Drug.Name")]
[Required(ErrorMessage = "Validation.Required")]
public string Name { get; set; } = string.Empty;
```

`AddDataAnnotationsLocalization` resolves these keys through `SharedResource`. Feature-specific
cross-field rules such as `ExpiryDate > ImportedDate` belong in a custom validation attribute with the
message key `Inventory.Validation.ExpiryBeforeImport`. Client-side and server-side validation must
render the same reviewed text.

## Formatting strategy

- Store dates, numbers and money as typed values, never localized strings.
- Format dates at the presentation boundary using the culture contract.
- Render prices using a single helper or display template so every view emits the same currency form.
- Keep measurement units attached to source data; use a nonbreaking space between values and units in
  visible output where supported.
- Test the production Windows/IIS output because currency spacing can differ by runtime globalization
  data.

## AI localization boundary

`AiExplanationResource` owns user-visible disclosure, loading, fallback, rate-limit and error copy.
Prompt templates are isolated from ordinary UI resources so a copy edit cannot silently change the AI
contract.

The AI response is optional explanatory text. It cannot change candidate ordering, score, stock,
warnings or authorization. When Gemini is unavailable, the deterministic explanation remains available
in Vietnamese. Prompts and logs must not contain credentials, patient identifiers or unredacted medical
records.

## Test gates

The implementation is accepted only when all checks pass:

1. Unit: resource keys required by marker classes resolve for `vi-VN`.
2. Unit: inventory cross-field and numeric validation returns the canonical Vietnamese messages.
3. Integration: a request without culture hints uses `vi-VN` and emits `Content-Language: vi-VN`.
4. Acceptance: every HTML response has `lang="vi"` and no English error/privacy scaffold remains.
5. Acceptance: date, date/time, number and currency examples match the locale contract.
6. Acceptance: authentication, access denied, validation, empty, loading, error, offline and rate-limit
   states use reviewed Vietnamese copy.
7. Static scan: new user-facing Razor literals outside approved resource calls fail CI, with an explicit
   allowlist for proper nouns and immutable identifiers.
8. Accessibility: localized accessible names, tooltips and validation summaries are announced in the
   expected order.
9. Responsive: the longest reviewed strings fit at `390 x 844` and 200% zoom without overlap or
   horizontal overflow.
10. Regression: persisted role/status values and API contracts remain unchanged.

## Rollout sequence

1. `N4WTT-217`: approve glossary and state microcopy.
2. `N4WTT-218`: approve this architecture and resource ownership.
3. `N4WTT-212`: approve the exact Figma version.
4. `N4WTT-220` and `N4WTT-226`: add localization infrastructure, resources and `lang` fix in one
   reviewable implementation branch.
5. `N4WTT-219`: implement only the locked visual frames and component behavior.
6. `N4WTT-221` and `N4WTT-222`: execute visual, accessibility, keyboard and cross-browser gates.

Any new visible string after step 3 requires a resource key and a Jira-linked change request. A missing
translation is a release defect, not a reason to fall back silently to unaccented Vietnamese or English.

## Architecture acceptance

- [x] Default and supported culture behavior is explicit.
- [x] Resource ownership and stable key naming are defined.
- [x] DataAnnotations and cross-field validation strategy is defined.
- [x] Domain data, persisted codes and translatable UI boundaries are defined.
- [x] AI prompts, AI visible copy and deterministic fallback are separated.
- [x] Formatting and production-runtime verification are defined.
- [x] Unit, integration, acceptance, accessibility and responsive gates are defined.
- [x] Rollout remains blocked by the Figma approval gate.

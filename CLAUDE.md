# Eventually

A C# library that publishes one uniform CloudEvents 1.0 message for every mutation of persisted data, over a transport chosen by dependency injection (NATS, AMQP).

The spec, plan and tasks live in `.specify/features/eventually-core/`. Read them before changing behaviour. Open design questions are in `open-questions.md`; don't settle them in code.

## Working rules

- Spec-kit and TDD: each task lands its failing test first, then the code, then a refactor.
- Prefer pure static methods. Side effects (transport, logging, clock, id generation) stay at the edges behind interfaces.
- Expected failures are values. Use `Result<T>`, `Option<T>` and `Error` from StandSure.Functional; don't write your own result or option types, and don't throw for validation.
- `Match` is failure-first, with named arguments: `result.Match(onFailure: ..., onSuccess: ...)`, `option.Match(onNone: ..., onSome: ...)`.
- The StandSure.Functional API summary is in `docs/standsure-functional.md`.
- Error codes are lowercase dotted, under `event.` (for example `event.entity_type.colon`). Validation failures come back as one `event.invalid` error with a child per problem.
- Warnings are errors, nullable is on, and banned APIs are in `BannedSymbols.txt`.

## Settled design (Nov 25)

- Wire format: structured JSON, `application/cloudevents+json`.
- `type` is the constant `standsure.eventually`; callers can't change it.
- `subject` is `{entitytype}:{action}:{entityid}`. A colon in the entity type is rejected; ids may contain colons.
- Extension attributes: `entitytype`, `entityid`, `action`, `tenantid` (unset when there is no tenant).
- Verbs are lowercase tokens: none, create, update, patch, archive, delete, purge.
- Topic: `standsure.eventually.{entitytype}.{action}.{entityid}.{tenant}`, each part percent-encoded; no tenant is `_`.
- Consumers fetch fresh data. The optional `data` is for audit only.
- No acks. A failed publish is the caller's call: the library logs at Error and runs an optional `OnUndelivered` handler.

## Build

- .NET 11 SDK (see `global.json`). StandSure.Functional comes from the stand-sure-org GitHub Packages feed, which needs a token with `read:packages`.
- `dotnet test` runs the xUnit v3 tests.
- CI uses the Oui-DELIVER reusable workflows in `.github/workflows/`.

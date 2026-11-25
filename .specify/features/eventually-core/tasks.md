# Eventually core: tasks

Each task is red, green, refactor: the test lands first and fails, then the code.

## Phase 0: setup

- [ ] T001 Scaffold: slnx, global.json (11.0.100), Directory.Build.props, Directory.Packages.props, BannedSymbols.txt, nuget.config with nuget.org and the stand-sure-org feed, CLAUDE.md with the consumer block, docs/standsure-functional.md copied from the library.
- [ ] T002 CI through Oui-DELIVER's reusable .NET workflows, copied from stand-sure-functional.

## Phase 1: pure core (story 1)

- [ ] T003 Test: each ChangeVerb maps to and from its token; an unknown token is Validation event.verb.unknown.
- [ ] T004 Test: entity types and ids are any non-empty strings, dots and Unicode included; empty ones fail validation.
- [ ] T005 Test: Create builds every attribute, extensions included, from a fixed EntityChange and EventContext.
- [ ] T006 Test: several bad fields give one Validation error whose Errors hold every child.
- [ ] T007 Test: Tenant.None means no tenantid; Tenant.Of sets it; data over the cap is event.data.too\_large.
- [ ] T007a Test: a none change builds like any other, with the context's new id and time; two none events for the same entity have distinct ids and non-decreasing times.
- [ ] T008 Implement Create, Tenant and the verb rules to green.

## Phase 2: wire format and topics (stories 1, 2, 6)

- [ ] T009 Test: ToJson writes a valid structured CloudEvent (required attributes, extensions, data only when present), and null-valued attributes read back as unset.
- [ ] T010 Test: FromJson round trip over generated events; each malformed document gives its own Validation code and never throws.
- [ ] T011 Test: TopicFor encodes each part to one token (. \* > # % whitespace), is reversible, uses \_ for no tenant, and encodes a real tenant named \_.
- [ ] T011a Test: the filter builder for entity-only, entity-and-id, verb-only and tenant-only selections, each checked against the topics it should and shouldn't match, dotted entity types included.
- [ ] T012 Implement to green.

## Phase 3: publisher and subscriber (stories 1, 2, 4, 5)

- [ ] T013 Test: publish through the in-memory transport with a fixed TimeProvider sends exactly one message on the expected topic.
- [ ] T014 Test: a transport failure becomes Unavailable; cancellation propagates; nothing else escapes.
- [ ] T014a Test: with no handler, a transport failure writes one log entry at Error with id, type, subject, topic, kind and code, and no data (fake logger).
- [ ] T014b Test: a configured OnUndelivered runs once after the log entry with the event, topic and error; a throwing handler is logged at Error and PublishAsync still returns the transport failure.
- [ ] T014c Test: a Validation failure is returned without logging or calling the handler.
- [ ] T015 Test: publishing to a topic with no subscribers succeeds.
- [ ] T016 Test: a throwing handler is counted and not redelivered; a malformed message is dropped and counted.
- [ ] T017 Implement publisher, subscriber, undelivered handling and in-memory transport to green.

## Phase 4: DI (story 3)

- [ ] T018 Test: AddEventually().UseInMemory() resolves the publisher; no transport or two transports fail at startup.

## Phase 5: NATS (stories 3, 5)

- [ ] T019 Extract the abstract contract suite and run it against in-memory.
- [ ] T020 NATS adapter passes the same suite against a NATS server in CI.
- [ ] T021 Benchmark: publish to an unsubscribed subject vs build and serialize.

## Phase 6: finish

- [ ] T022 Telemetry: spans and counters, with tests on a listener.
- [ ] T023 docs/llm-api-summary.md and a sample, so agents reach for the library.
- [ ] T024 AMQP adapter against the same suite (later).


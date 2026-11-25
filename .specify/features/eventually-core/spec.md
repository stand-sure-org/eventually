# Eventually core: spec

## User stories

1. **Publish a change (P1).** As a service author, right after I persist a mutation I publish one change event with one call, naming the entity, its id, the verb and the tenant, without describing the changed data.
   - Given entity type StandSure.Billing.Invoice, id 42, verb update, tenant t1, when I publish, then exactly one CloudEvent goes out with type standsure.eventually, subject StandSure.Billing.Invoice:update:42, my service's source, a fresh id, the current time, extension attributes entitytype StandSure.Billing.Invoice, entityid 42, action update and tenantid t1, and no data, as CloudEvents JSON on topic standsure.eventually.StandSure%2EBilling%2EInvoice.update.42.t1.
   - Given no tenant, the event carries no tenantid attribute at all.
   - Given an invalid entity name or an empty id, publish returns a Validation failure listing every problem, and nothing goes out.
2. **React to a change (P1).** As a consumer, I subscribe by entity type, entity id, verb or tenant (wildcards allowed), get the event, and fetch fresh data myself.
   - Given a subscription to invoice changes of any id and verb, when an invoice.update is published, then my handler runs once with the parsed event.
   - Given a malformed message on the topic, my handler does not run; the message is dropped and counted.
3. **Swap the backend (P1).** As an operator, I move from in-memory to NATS (and later AMQP) by changing the DI registration only.
   - The same contract test suite passes unchanged against every transport.
4. **Fire and forget (P1).** Publish returns once the backend has taken the message, or failed to. There is no consumer ack, nack or redelivery in the API.
   - Given a subscriber whose handler throws, the exception is caught, logged and counted, and the message is not redelivered.
5. **Cheap when nobody listens (P2).** Most topics have no subscribers, so publishing needs no topic setup and costs about as much as building and serializing the envelope.
   - Given a topic with no subscribers, publish succeeds and no topic or queue is created.
6. **Audit data (P2).** As an audit-log consumer, I receive an optional JSON data field when the producer attaches one.
   - Given a change with audit data, the event has datacontenttype application/json and the data round-trips unchanged; without it, the event has no data.
7. **Ask for reprocessing, or send a heartbeat (P2).** As a caller, I publish verb none to say nothing was persisted: either a heartbeat, or a request that consumers fetch and reprocess the entity now.
   - Given a none event for invoice 42, it gets a fresh id and the current time like any other event, so consumers ordering by time never see it go backwards; an earlier event is never re-sent.
8. **Know when publishing fails (P1).** As an operator, I see every event that couldn't be handed to the backend, and I can plug in my own handling.
   - Given the backend is down and no handler is configured, publish returns Unavailable and one structured log entry at Error names the event id, type, subject, topic and error code, never the data.
   - Given a configured handler, it also runs with the event, topic and error; if the handler itself fails, that is logged at Error too and nothing is thrown.

## Functional requirements

- **FR-001 Envelope.** Each event is a CloudEvents 1.0 event: specversion 1.0; id is a UUIDv7 string (time-ordered, unique per source); source is one configured URI reference per producing service; type is always standsure.eventually, set by the library and not configurable, since source already says who sent it; subject is {entitytype}:{action}:{entityid}, so prefix filters can pick an entity type, one verb of it, or one entity; time is UTC from TimeProvider.
- **FR-002 Extension attributes.** The structured envelope data lives in flat CloudEvents extension attributes, the one place the library reads it from: entitytype, entityid and action (all String), and tenantid (String, unset when there is no tenant). Consumers never parse type, subject or the topic to get these. Extension values are scalars only (String, Integer, Boolean, URI, Timestamp, Binary), so nothing nested goes there.
- **FR-003 Verbs.** A fixed set with lowercase wire tokens: create, update, patch, archive, delete, purge, none.
- **FR-004 Names and ids are strings.** Entity type names may contain dots (for example StandSure.Billing.Invoice), and every id is an opaque string. Only two things fail validation: an empty value, and a colon in an entity type, which would make the subject ambiguous. Ids may contain colons (for example urn:isbn:0451450523), because the id comes last in the subject. The topic encodes everything (FR-015).
- **FR-005 Data.** Optional JSON only, for audit. Off by default; over a configured size cap is a Validation failure.
- **FR-006 Pure construction.** Building, validating, naming the topic, serializing and parsing are pure functions returning Result, so each is tested with fixed inputs and no mocks.
- **FR-007 Errors.** Every expected failure is a StandSure.Functional Error: Validation for bad input, Unavailable when the backend can't take the message, Unexpected for anything else. The library throws nothing but OperationCanceledException.
- **FR-008 Publish API.** PublishAsync(change, ct) returns Task of Result of Unit. Success means the backend took the message at the configured confirmation level, never that a consumer got it.
- **FR-009 Subscribe API.** Subscribe(filter, handler) returns a Result holding a disposable subscription. Parse failures and handler exceptions are counted and dropped.
- **FR-010 Transports.** One port, IEventTransport. Adapters: in-memory (tests), NATS core, then AMQP. Exactly one is registered; zero or two fails at startup.
- **FR-011 No provisioning.** Publishing never creates topics, streams or queues. NATS core drops a subject nobody is subscribed to; on AMQP a topic exchange without the mandatory flag drops unroutable messages.
- **FR-012 Telemetry.** Same pattern as StandSure.Functional: ActivitySource and Meter named StandSure.Eventually, counters for published, publish failures, received and dropped.
- **FR-013 Library use.** No hand-rolled Result or error types. The repo's CLAUDE.md carries the consumer block from stand-sure-functional and a copy of its API summary, since agents only used the library when pointed to it.
- **FR-014 Verb none.** none means no persisted mutation. It serves as a heartbeat, or as a caller's request that consumers re-fetch and reprocess the entity. It always carries a new id and the current time, so it never breaks time ordering, and the library offers no way to re-send an old event.
- **FR-015 Topic order and encoding.** Topics follow expected search and subscription, in the same order as the subject: standsure.eventually.{entitytype}.{action}.{entityid}.{tenant}. Each of the last four parts is one token: characters that NATS or AMQP treat as separators or wildcards (. \* > # whitespace) and % itself are percent-encoded, so StandSure.Billing.Invoice becomes StandSure%2EBilling%2EInvoice. The CloudEvent attributes always hold the raw strings; only the topic is encoded. A missing tenant is the token \_ (a real tenant named \_ is encoded as %5F).
- **FR-016 Failed publishes.** What a failed publish means for the update is the caller's decision, and the docs say so plainly (see the plan). The library returns the failure, writes one structured log entry at Error (event id, type, subject, topic, error kind and code; never data) through ILogger with the LoggerMessage source generator, and then runs an optional caller-supplied Func of UndeliveredEvent, CancellationToken and Task. Handler failures are logged at Error and never thrown. Validation failures are returned only, since they are caller bugs, not delivery problems.
- **FR-017 Tenant on purpose.** Callers say Tenant.Of(id) or Tenant.None explicitly, so a forgotten tenant can't look the same as an entity that has none. Both shapes reach the wire as set or unset tenantid.

## Out of scope

Guaranteed or exactly-once delivery, consumer acks, replay or event sourcing, change diffs (other than audit data), schema registry.

## Success criteria

- **SC-001:** One contract suite, green against in-memory and NATS, with no producer or consumer code change between them.
- **SC-002:** Round trip holds for every generated event:

```latex
\mathrm{FromJson}\circ\mathrm{ToJson} = \mathrm{Success}\circ\mathrm{id}
```

- **SC-003:** A benchmark shows publishing to an unsubscribed NATS subject costs within a small constant of building and serializing the envelope.


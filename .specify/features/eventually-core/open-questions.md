# Open questions

Decided Nov 25:

- **Name:** Eventually. Package StandSure.Eventually; repo stand-sure-org/stand-sure-eventually (you said yes; it needs creating on GitHub). No clash on nuget.org for StandSure.Eventually; an unrelated 2016 event-sourcing package owns the bare id Eventually.
- **Verb none:** a heartbeat, or a caller's request to reprocess without breaking time ordering.
- **Extension attributes:** entitytype, entityid, action and tenantid.
- **Subject:** {entitytype}:{action}:{entityid}, for prefix filtering by entity type, verb or one entity.
- **Type:** the constant standsure.eventually, with no reversed-domain prefix.
- **Topic order:** standsure.eventually.{entitytype}.{action}.{entityid}.{tenant}, noun then verb then id, since NATS and RabbitMQ both match left to right.
- **Verb casing:** lowercase everywhere.
- **Failed publish:** the caller decides; the library returns the failure, logs at Error and runs an optional handler.
- **Wire:** CloudEvents structured JSON.
- **Ids:** all strings; entity types may contain dots but not colons; the topic encodes.

Still open:

1. **Heartbeat vs reprocess:** should a none event say which it is, for example with a reason extension?
2. **Confirmation level:** for callers who roll back on failure, should the NATS adapter offer a server round trip or JetStream publish acknowledgement, and which is the default?
3. **Tenant:** unset tenantid on the wire with an explicit Tenant.None in code, and \_ in the topic (my pick), or a real sentinel value?
4. **Source:** one URI reference per service, for example urn:standsure:billing?
5. **Audit data:** new state, a diff, or before and after; the size cap; and whether audit gets a durable path despite no-ack.
6. **Functional version:** wait for the stable stand-sure-functional tag, or start on 0.1.0-rc-2.


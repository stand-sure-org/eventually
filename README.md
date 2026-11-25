# Eventually

One uniform [CloudEvents 1.0](https://github.com/cloudevents/spec) message for every mutation of persisted data.

Every save, update or delete publishes the same small event: which entity changed, how, and for which tenant. Subscribers fetch fresh data themselves, so the message never carries state that can go stale. Most topics have no subscribers, and that's fine.

```text
type     standsure.eventually
subject  StandSure.Billing.Invoice:update:42
topic    standsure.eventually.StandSure%2EBilling%2EInvoice.update.42.t1
```

Status: early. The core event model and its failing tests are in; the publisher and transports come next. See `.specify/features/eventually-core/` for the spec, plan and tasks.

Built on [StandSure.Functional](https://github.com/stand-sure-org/stand-sure-functional).

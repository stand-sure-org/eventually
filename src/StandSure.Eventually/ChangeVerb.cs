namespace StandSure.Eventually;

/// <summary>What happened to the entity. The wire token is the lowercase name.</summary>
public enum ChangeVerb
{
    /// <summary>Nothing was persisted: a heartbeat, or a request that consumers fetch and reprocess the entity now.</summary>
    None,
    Create,
    Update,
    Patch,
    Archive,
    Delete,
    Purge,
}

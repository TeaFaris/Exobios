public sealed class DamageSource
{
    public readonly Entity Source;

    public readonly string DeathMessage;

    public DamageSource(Entity source, string deathMessage = default)
    {
        Source = source;
        DeathMessage = deathMessage;
    }
}
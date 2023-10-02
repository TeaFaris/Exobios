using System;

public sealed class DamageSource
{
    public readonly Entity Source;
    public readonly Entity Target;

    private readonly string deathMessageFormat;

    public string FormattedDeathMessage => string.Format(deathMessageFormat, Source.ToString(), Target.ToString());

    /// <param name="source">Entity that damaged.</param>
    /// <param name="target">Entity who was damaged.</param>
    /// <param name="deathMessageFormat">
    /// Supports formatting,
    /// where {0} is <see cref="Target"/> => <see cref="Entity.ToString"/> (damaged entity)
    /// and {1} <see cref="Source"/> => <see cref="Entity.ToString"/> (entity that damaged it).
    /// </param>
    public DamageSource(Entity source, Entity target, string deathMessageFormat = "{0} was killed by {1}.")
    {
        Source = source;
        Target = target;

        this.deathMessageFormat = deathMessageFormat;
    }
}
using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class ReadOnlyPropertyAttribute : PropertyAttribute
{

}
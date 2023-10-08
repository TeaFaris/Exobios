using System.Linq;
using System.Reflection;
using System;

public static class SmartEnumExtensions
{
    public static TEnum[] GetValues<TEnum, TID>()
        where TEnum : ISmartEnum<TID>
        where TID : struct
    {
        Type EnumType = typeof(TEnum);

        FieldInfo[] fields = EnumType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
        
        return fields
            .Select(x => x.GetValue(null))
            .OfType<TEnum>()
            .ToArray();
    }
    public static int GetLength<TEnum, TID>()
        where TEnum : ISmartEnum<TID>
        where TID : struct
    {
        Type enumType = typeof(TEnum);
        FieldInfo[] fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
        return fields.Length;
    }

    public static TEnumStruct? GetByID<TEnumStruct, TID>(TID ID)
        where TEnumStruct : struct, ISmartEnum<TID>
        where TID : struct
    {
        var values = GetValues<TEnumStruct, TID>();

        TEnumStruct[] enums = Array.FindAll(values, x => x.ID.Equals(ID));
        if (!enums.Any())
        {
            return null;
        }

        return enums[0];
    }

    public static TEnumClass? GetByID<TEnumClass, TID>(TID id, bool isClass = true)
        where TEnumClass : class, ISmartEnum<TID>
        where TID : struct
    {
        var values = GetValues<TEnumClass, TID>();
        return Array.Find(values, x => x.ID.Equals(id));
    }
}
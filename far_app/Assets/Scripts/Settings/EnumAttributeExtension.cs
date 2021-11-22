using System;
using System.Reflection;
using System.Collections.Generic;

public class EnumName : Attribute
{
    public string Name { get; protected set; }

    public EnumName(string inName)
    {
        Name = inName;
    }
}

static class EnumNameExtensions
{
    public static List<string> GetAllNames(this Type inEnum)
    {
        Array enumValuesArray = Enum.GetValues(inEnum);
        List<string> enumNames = new List<string>();
        foreach (object enumValueObject in enumValuesArray)
        {
            Enum enumValue = enumValueObject as Enum;
            enumNames.Add(enumValue.GetName());
        }

        return enumNames;
    }

    public static string GetName(this Enum inEnum)
    {
        EnumName enumName = GetEnumNameAttribute(inEnum);
        return enumName.Name;
    }

    private static EnumName GetEnumNameAttribute(object inObject)
    {
        Type type = inObject.GetType();
        FieldInfo fieldInfo = type.GetField(inObject.ToString());
        EnumName[] attributes = fieldInfo.GetCustomAttributes(
            typeof(EnumName), false) as EnumName[];

        if (attributes.Length > 0)
            return attributes[0];
        else
            throw new ArgumentException($"Object '{inObject}' does not have EnumName attribute");
    }
}
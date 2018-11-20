using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class EnumDescriptionAttribute : Attribute
{
    public string Description
    {
        private set;
        get;
    }

    public EnumDescriptionAttribute(string description)
    {
        this.Description = description;
    }
}
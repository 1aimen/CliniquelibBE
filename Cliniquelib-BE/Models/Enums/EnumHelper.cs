using System;
using System.ComponentModel;
using System.Reflection;

namespace Cliniquelib_BE.Models.Enums
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            return value.GetType()
                        .GetField(value.ToString())!
                        .GetCustomAttribute<DescriptionAttribute>()?
                        .Description ?? value.ToString();
        }

        public static T GetEnumFromDescriptionString<T>(string description) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                var attribute = field.GetCustomAttribute<DescriptionAttribute>();
                if (attribute != null && attribute.Description == description || field.Name == description)
                    return (T)field.GetValue(null)!;
            }
            throw new ArgumentException($"Unknown enum description: {description}");
        }
    }
}

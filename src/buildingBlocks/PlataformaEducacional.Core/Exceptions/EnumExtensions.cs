using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace PlataformaEducacional.Core.Exceptions;

public static class EnumExtensions
{
    /// <summary>
    /// Retrieves the description associated with an enumeration value.
    /// </summary>
    /// <param name="enumValue">The enumeration value for which to retrieve the description.</param>
    /// <returns>The description specified in the <see cref="DescriptionAttribute"/> applied to the enumeration value, or the
    /// string representation of the enumeration value if no description is defined.</returns>
    public static string GetDescription<TEnum>(this TEnum enumValue) where TEnum : struct, Enum
    {
        var field = enumValue.GetType().GetField(enumValue.ToString());

        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();

        return attribute?.Description ?? enumValue.ToString();
    }

    /// <summary>
    /// Returns a comma-separated string of all the names in the enum type.
    /// </summary>
    public static string GetAllNamesCommaSeparated<TEnum>(this TEnum enumValue) where TEnum : struct, Enum
    {
        // Get all the names as a string array
        string[] names = Enum.GetNames(typeof(TEnum));

        // Join them with a comma and a space
        return String.Join(", ", names);
    }

    /// <summary>
    /// Retrieves the display name of the specified enumeration value.
    /// </summary>
    /// <remarks>This method uses reflection to retrieve the <see
    /// cref="System.ComponentModel.DataAnnotations.DisplayAttribute"/> associated with the enumeration value. If no
    /// such attribute is found, the method returns the string representation of the enumeration value.</remarks>
    /// <param name="enumValue">The enumeration value for which to retrieve the display name.</param>
    /// <returns>The display name specified by the <see cref="System.ComponentModel.DataAnnotations.DisplayAttribute"/> applied
    /// to the enumeration value, or the enumeration value's name as a string if no display attribute is present.</returns>
    public static string GetDisplayName<TEnum>(this TEnum enumValue) where TEnum : struct, Enum
    {
        var field = enumValue.GetType().GetField(enumValue.ToString());

        var attribute = field?.GetCustomAttributes(typeof(DisplayAttribute), false)
                             .Cast<DisplayAttribute>()
                             .FirstOrDefault();

        return attribute?.Name ?? enumValue.ToString();
    }
}

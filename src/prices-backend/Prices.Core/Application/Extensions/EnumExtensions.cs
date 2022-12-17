using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Prices.Core.Application.Extensions;

public static class EnumExtensions
{
    public static string ToDescription(this Enum obj)
    {
        try
        {
            var fieldInfo = obj.GetType().GetField(obj.ToString());

            if (fieldInfo == null)
                return "Unknown";

            var attributeArray = fieldInfo.GetCustomAttributes(false);

            foreach (var attribute in attributeArray)
            {
                switch (attribute)
                {
                    case DescriptionAttribute descriptionAttribute:
                        return descriptionAttribute.Description;
                    case DisplayAttribute displayAttribute:
                        return displayAttribute.Name!;
                    case XmlEnumAttribute displayAttribute:
                        return displayAttribute.Name!;
                }
            }
            return obj.ToString();
        }
        catch (NullReferenceException)
        {
            return "Unknown";
        }
    }
}
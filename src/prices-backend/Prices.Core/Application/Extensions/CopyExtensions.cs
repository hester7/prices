using System.Xml.Serialization;

namespace Prices.Core.Application.Extensions
{
    public static class CopyExtensions
    {
        public static T DeepCopyXml<T>(this T self)
        {
            using var ms = new MemoryStream();
            var s = new XmlSerializer(typeof(T));
            s.Serialize(ms, self);
            ms.Position = 0;
            return (T)s.Deserialize(ms)!;
        }
    }
}

using FluentNHibernate.Mapping;

namespace Badges.Core
{
    public static class MappingExtensions
    {
        public static PropertyPart StringMaxLength(this PropertyPart propertyPart){
            return propertyPart.CustomType("StringClob");
        }
    }
}
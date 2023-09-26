using Contracts;
using Entities.LinkModels;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Service.DataShaping
{
    public class DataShaper<T> : IDataShaper<T> where T : class 
    {
        public PropertyInfo[] properties { get; set; }
        public DataShaper()
        {
            properties = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        }
        public IEnumerable<ShapedEntity>ShapeData(IEnumerable<T>entities, string fieldsString)
        {
            var requiredProperties = GetRequiredProperties(fieldsString);
            return FetchData(entities, requiredProperties);
        }
        public ShapedEntity ShapeData(T entity, string fieldsString)
        {
            var requiredProperties = GetRequiredProperties(fieldsString);
            return FetchDataForEntity(entity, requiredProperties);
        }
        private IEnumerable<PropertyInfo>GetRequiredProperties(string fieldsString)
        {
            var requiredProperties = new List<PropertyInfo>();
            if (!string.IsNullOrWhiteSpace(fieldsString))
            {
                var fields = fieldsString.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var field in fields)
                {
                    var property = properties.FirstOrDefault(pi => pi.Name.Equals(field.Trim(), StringComparison.InvariantCultureIgnoreCase));
                    if (property == null)
                    {
                        continue;
                    }
                    requiredProperties.Add(property);
                }
            }
            else
            {
                requiredProperties = properties.ToList();
            }
            return requiredProperties;
        }
        public IEnumerable<ShapedEntity> FetchData(IEnumerable<T> entities, IEnumerable<PropertyInfo> requiredProperties)
        {
            var shapedData = new List<ShapedEntity>();
            foreach (var entity in entities)
            {
                var shapedObject = FetchDataForEntity(entity, requiredProperties);
                shapedData.Add(shapedObject);
            }
            return shapedData;
        }
        private ShapedEntity FetchDataForEntity(T entity, IEnumerable<PropertyInfo> requiredProperties)
        {
            var shapedObject = new ShapedEntity();

            foreach (var property in requiredProperties)
            {
                var objectPropertyValue = property.GetValue(entity);
                shapedObject.ExpandoObject.TryAdd(property.Name, objectPropertyValue);
            }
            var objectProperty = entity.GetType().GetProperty("Id");
            shapedObject.Id = (Guid)objectProperty.GetValue(entity);
            
            

            return shapedObject;
        }
        private void WriteLinksToXml(string key, object value, XmlWriter writer)
        {
            writer.WriteStartElement(key);
            if (value.GetType() == typeof(List<Link>))
            {
                foreach (var val in value as List<Link>)
                {
                    writer.WriteStartElement(nameof(Link));
                    WriteLinksToXml(nameof(val.Href), val.Href, writer);
                    WriteLinksToXml(nameof(val.Rel), val.Href, writer);
                    WriteLinksToXml(nameof(val.Method), val.Method, writer);
                    writer.WriteEndElement();
                }
            }
            else
            {
                writer.WriteString(value.ToString());
            }
            writer.WriteEndElement();
        }

    }

    
}

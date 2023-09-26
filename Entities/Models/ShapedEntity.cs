using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class ShapedEntity
    {
        public class Entity
        {
            [Column("EmployeeId")]
            public Guid Id { get; set; }
            [Required(ErrorMessage = "Employee name is a required field")]
            [MaxLength(60, ErrorMessage = "Maximum length for the name is 60 characters")]
            public string? Name { get; set; }
            [Required(ErrorMessage = "Age is a required field")]
            public int Age { get; set; }

            [Required(ErrorMessage = "Employee Position is a required field")]
            [MaxLength(20, ErrorMessage = "Maximum length for the position is 20 characters")]
            public string? Position { get; set; }

            [ForeignKey(nameof(Company))]
            public Guid CompanyId { get; set; }
            public Company? Company { get; set; }




            //public IDictionary<string, object> TryAdd(string name, object val)
            //{
            //    var dictionary = new Dictionary<string, object>();
            //    return dictionary;
            //}

            //public static IDictionary<TKey, TValue> TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
            //{
            //    dictionary.Add(key, value);
            //    return dictionary;
            //}



        }
        public ShapedEntity()
        {
            ExpandoObject = new ExpandoObject();
            //Entity = new Entity();
        }
        public Guid Id { get; set; }
        public ExpandoObject ExpandoObject { get; set; }
        //public Entity Entity { get; set; }
    }
}

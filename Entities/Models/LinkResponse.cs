using Entities.LinkModels;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class LinkResponse
    {
        public bool HasLinks { get; set; }
        public List<ExpandoObject> ShapedEntities { get; set; }
        public LinkCollectionWrapper<ExpandoObject> LinkEntities { get; set; }
        public LinkResponse()
        {
            LinkEntities = new LinkCollectionWrapper<ExpandoObject>();
            ShapedEntities = new List<ExpandoObject>();
        }
    }
}

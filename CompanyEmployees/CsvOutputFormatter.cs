using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Shared.DataTranasferObject;
using System.Text;

namespace CompanyEmployees
{
    public class CsvOutputFormatter : TextOutputFormatter
    {
        public CsvOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv"));
            SupportedEncodings.Add(System.Text.Encoding.UTF8);
            SupportedEncodings.Add(System.Text.Encoding.Unicode);
        }
        protected override bool CanWriteType(Type? type)
        {
            if (typeof(CompanyDto).IsAssignableFrom(type)||typeof(IEnumerable<CompanyDto>).IsAssignableFrom(type))
            {
                return base.CanWriteType(type);
            }
            return false;

          
        }
        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var response = context.HttpContext.Response;
            var buffer = new StringBuilder();
            if (context.Object is IEnumerable<CompanyDto>)
            {
                foreach (var company in (IEnumerable<CompanyDto>)context.Object)
                {
                   
                    FormatCsv(buffer, company);
                }
            }
            else
            {
                FormatCsv(buffer, (CompanyDto)context.Object);
            }
            await response.WriteAsync(buffer.ToString());
        }
        private static void FormatCsv(StringBuilder buffer, CompanyDto company)
        {
            //buffer.AppendLine("{");
            //buffer.AppendLine($"id:{company.Id}");
            //buffer.AppendLine($"Name:{company.Name}");
            //buffer.AppendLine($"FullAddress:{company.FullAddress}");
            //buffer.AppendLine("}");

            buffer.AppendLine($"{company.Id},\"{company.Name},\"{company.FullAddress}\"");
        }
    }
}

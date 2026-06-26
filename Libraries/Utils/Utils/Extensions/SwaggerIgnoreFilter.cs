using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
namespace Utils.Extensions
{
    public class SwaggerIgnoreFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var apiDescription in context.ApiDescriptions)
            {
                var actionAttributes = apiDescription.ActionDescriptor.EndpointMetadata;
                if (actionAttributes.Any(a => a is SwaggerIgnoreAttribute))
                {
                    var key = "/" + apiDescription.RelativePath.TrimEnd('/');
                    swaggerDoc.Paths.Remove(key);
                }
            }
        }
    }

    public class IgnoreSchemaFilter : ISchemaFilter
    {
        private readonly string[] _ignoredSchemas;

        public IgnoreSchemaFilter(params string[] ignoredSchemas)
        {
            _ignoredSchemas = ignoredSchemas;
        }

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (_ignoredSchemas.Contains(context.Type.Name))
            {
                context.SchemaRepository.Schemas.Remove(context.Type.Name);
            }
        }
    }



}

namespace Utils.Extensions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SwaggerIgnoreAttribute : Attribute
    {
    }


    [AttributeUsage(AttributeTargets.Class)]
    public class SwaggerIgnoreSchemaAttribute : Attribute
    {
    }



}

using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TaskManager.API.Filters
{
  public class AuthorizeOperationFilter : IOperationFilter
  {
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
      // Check if the endpoint has [Authorize] attribute
      var hasAuthorize = context.MethodInfo.DeclaringType!.GetCustomAttributes(true)
          .Union(context.MethodInfo.GetCustomAttributes(true))
          .OfType<AuthorizeAttribute>()
          .Any();

      // Check if the endpoint has [AllowAnonymous] attribute
      var hasAllowAnonymous = context.MethodInfo.DeclaringType!.GetCustomAttributes(true)
          .Union(context.MethodInfo.GetCustomAttributes(true))
          .OfType<AllowAnonymousAttribute>()
          .Any();

      // Only add security requirement if [Authorize] is present and [AllowAnonymous] is NOT present
      if (hasAuthorize && !hasAllowAnonymous)
      {
        operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] { }
                        }
                    }
                };
      }
    }
  }
}
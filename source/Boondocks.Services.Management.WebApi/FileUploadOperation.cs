using System.Linq;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Boondocks.Services.Management.WebApi
{
    public class FileUploadOperation : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.OperationId.ToLower() == "v1applicationversionspost")
            {
                //operation.Parameters.Clear();

                var parameter = operation.Parameters.FirstOrDefault(p => p.Name == "file");

                if (parameter != null)
                {
                    operation.Parameters.Remove(parameter);
                }

                operation.Parameters.Add(new NonBodyParameter
                {
                    Name = "file",
                    In = "formData",
                    Description = "Upload File",
                    Required = true,
                    Type = "file"
                });
                operation.Consumes.Add("multipart/form-data");
            }
        }
    }
}
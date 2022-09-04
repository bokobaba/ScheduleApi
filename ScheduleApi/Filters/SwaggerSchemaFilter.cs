﻿using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ScheduleApi.Filters {
    public class SwaggerSchemaFilter : ISchemaFilter {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context) {
            context.SchemaRepository.Schemas.Remove("Employee");
            context.SchemaRepository.Schemas.Remove("Request");
            context.SchemaRepository.Schemas.Remove("Schedule");
            context.SchemaRepository.Schemas.Remove("User");
            context.SchemaRepository.Schemas.Remove("UserRefreshToken");
        }
    }
}

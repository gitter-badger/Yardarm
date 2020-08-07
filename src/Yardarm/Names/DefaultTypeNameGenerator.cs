﻿using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.OpenApi.Models;
using Yardarm.Generation;
using Yardarm.Generation.Api;

namespace Yardarm.Names
{
    public class DefaultTypeNameGenerator : ITypeNameGenerator
    {
        private readonly ITypeGeneratorRegistry _typeGeneratorRegistry;

        public DefaultTypeNameGenerator(ITypeGeneratorRegistry typeGeneratorRegistry)
        {
            _typeGeneratorRegistry = typeGeneratorRegistry ?? throw new ArgumentNullException(nameof(typeGeneratorRegistry));
        }

        public TypeSyntax GetName(LocatedOpenApiElement element)
        {
            return GetNameInternal(element)
                         ?? throw new InvalidOperationException("Element does not have a type name.");
        }

        protected virtual TypeSyntax? GetNameInternal(LocatedOpenApiElement element) =>
            element switch
            {
                LocatedOpenApiElement<OpenApiOperation> operationElement => GetOperationName(operationElement),
                LocatedOpenApiElement<OpenApiRequestBody> requestBodyElement => GetRequestBodyName(requestBodyElement),
                LocatedOpenApiElement<OpenApiResponse> responseElement => GetResponseName(responseElement),
                LocatedOpenApiElement<OpenApiSchema> schemaElement => GetSchemaName(schemaElement),
                _ => element.Parents.Count > 0 ? GetNameInternal(element.Parents[0]) : null
            };

        protected virtual TypeSyntax GetOperationName(LocatedOpenApiElement<OpenApiOperation> element) =>
            _typeGeneratorRegistry.Get(element).GetTypeName();

        protected virtual TypeSyntax GetRequestBodyName(LocatedOpenApiElement<OpenApiRequestBody> element) =>
            _typeGeneratorRegistry.Get(element).GetTypeName();

        protected virtual TypeSyntax GetResponseName(LocatedOpenApiElement<OpenApiResponse> element) =>
            _typeGeneratorRegistry.Get(element).GetTypeName();

        protected virtual TypeSyntax GetSchemaName(LocatedOpenApiElement<OpenApiSchema> element) =>
            _typeGeneratorRegistry.Get(element).GetTypeName();
    }
}

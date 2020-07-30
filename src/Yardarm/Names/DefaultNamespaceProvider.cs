﻿using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.OpenApi.Models;

namespace Yardarm.Names
{
    public class DefaultNamespaceProvider : INamespaceProvider
    {
        private readonly NameSyntax _rootNamespace;

        public DefaultNamespaceProvider(YardarmGenerationSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            _rootNamespace = SyntaxFactory.ParseName(settings.RootNamespace);
        }

        public virtual NameSyntax GetSchemaNamespace(OpenApiSchema schema) => GetModelNamespace();

        protected virtual NameSyntax GetModelNamespace() =>
            SyntaxFactory.QualifiedName(_rootNamespace, SyntaxFactory.IdentifierName("Models"));
    }
}
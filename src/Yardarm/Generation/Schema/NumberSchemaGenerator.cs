﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.OpenApi.Models;
using Yardarm.Spec;

namespace Yardarm.Generation.Schema
{
    public class NumberSchemaGenerator : ITypeGenerator
    {
        private TypeSyntax? _nameCache;

        public TypeSyntax TypeName => _nameCache ??= GetTypeName();

        private readonly ILocatedOpenApiElement<OpenApiSchema> _schemaElement;

        public NumberSchemaGenerator(ILocatedOpenApiElement<OpenApiSchema> schemaElement)
        {
            _schemaElement = schemaElement ?? throw new ArgumentNullException(nameof(schemaElement));
        }

        protected virtual TypeSyntax GetTypeName() =>
            (_schemaElement.Element.Type, _schemaElement.Element.Format) switch
            {
                (_, "int32") => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                (_, "integer") => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                (_, "int") => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                (_, "int64") => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.LongKeyword)),
                (_, "byte") => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ByteKeyword)),
                ("integer", _) => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.LongKeyword)),
                ("number", "decimal") => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.DecimalKeyword)),
                ("number", "float") => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.FloatKeyword)),
                ("number", _) => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.DoubleKeyword)),
                _ => SyntaxFactory.IdentifierName("dynamic")
            };

        public SyntaxTree? GenerateSyntaxTree() => null;

        public IEnumerable<MemberDeclarationSyntax> Generate() =>
            Enumerable.Empty<MemberDeclarationSyntax>();
    }
}

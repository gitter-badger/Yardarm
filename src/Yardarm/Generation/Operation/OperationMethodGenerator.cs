﻿using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.OpenApi.Models;
using Yardarm.Generation.MediaType;
using Yardarm.Generation.Request;
using Yardarm.Generation.Tag;
using Yardarm.Helpers;
using Yardarm.Spec;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Yardarm.Generation.Operation
{
    public class OperationMethodGenerator : IOperationMethodGenerator
    {
        public const string UrlVariableName = "url";
        public const string RequestMessageVariableName = "requestMessage";

        private readonly IMediaTypeSelector _mediaTypeSelector;

        protected GenerationContext Context { get; }

        public OperationMethodGenerator(GenerationContext context, IMediaTypeSelector mediaTypeSelector)
        {
            _mediaTypeSelector = mediaTypeSelector ?? throw new ArgumentNullException(nameof(mediaTypeSelector));
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public BlockSyntax Generate(LocatedOpenApiElement<OpenApiOperation> operation) =>
            Block(GenerateStatements(operation));

        protected virtual IEnumerable<StatementSyntax> GenerateStatements(LocatedOpenApiElement<OpenApiOperation> operation)
        {
            yield return MethodHelpers.ThrowIfArgumentNull(TagTypeGenerator.RequestParameterName);

            yield return GenerateUrlVariable(operation);

            yield return GenerateRequestMessageVariable(operation);

            yield return ExpressionStatement(AddHeadersMethodGenerator.InvokeAddHeaders(
                IdentifierName(TagTypeGenerator.RequestParameterName),
                IdentifierName(RequestMessageVariableName)));

            LocatedOpenApiElement<OpenApiRequestBody>? requestBody = operation.GetRequestBody();
            if (requestBody != null)
            {
                var statement = GenerateRequestBodyConfiguration(requestBody);
                if (statement != null)
                {
                    yield return statement;
                }
            }

            // Placeholder until we actually do the request
            yield return ThrowStatement(ObjectCreationExpression(
                        QualifiedName(IdentifierName("System"), IdentifierName("NotImplementedException")))
                    .WithArgumentList(ArgumentList()))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        }

        protected virtual StatementSyntax GenerateUrlVariable(LocatedOpenApiElement<OpenApiOperation> operation) =>
            MethodHelpers.LocalVariableDeclarationWithInitializer(UrlVariableName,
                BuildUriMethodGenerator.InvokeBuildUri(
                    IdentifierName(TagTypeGenerator.RequestParameterName)));

        protected virtual StatementSyntax GenerateRequestMessageVariable(
            LocatedOpenApiElement<OpenApiOperation> operation)
        {
            return MethodHelpers.LocalVariableDeclarationWithInitializer(RequestMessageVariableName,
                ObjectCreationExpression(WellKnownTypes.System.Net.Http.HttpRequestMessage.Name)
                    .AddArgumentListArguments(Argument(GetRequestMethod(operation)),
                        Argument(IdentifierName(UrlVariableName))));
        }

        protected virtual StatementSyntax? GenerateRequestBodyConfiguration(
            LocatedOpenApiElement<OpenApiRequestBody> requestBody)
        {
            LocatedOpenApiElement<OpenApiMediaType>? mediaType = _mediaTypeSelector.Select(requestBody);
            if (mediaType == null)
            {
                return null;
            }

            var configurationBlock = Block(
                ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                    SyntaxHelpers.MemberAccess(RequestMessageVariableName, "Content"),
                    ObjectCreationExpression(WellKnownTypes.System.Net.Http.StringContent.Name)
                        .AddArgumentListArguments(
                            Argument(SyntaxHelpers.StringLiteral("")),
                            Argument(SyntaxHelpers.MemberAccess("System", "Text", "Encoding", "UTF8")),
                            Argument(SyntaxHelpers.StringLiteral(mediaType.Key))))));

            return MethodHelpers.IfNotNull(
                SyntaxHelpers.MemberAccess(TagTypeGenerator.RequestParameterName,
                    RequestTypeGenerator.BodyPropertyName),
                configurationBlock);
        }

        protected virtual ExpressionSyntax GetRequestMethod(LocatedOpenApiElement<OpenApiOperation> operation) =>
            operation.Key switch
            {
                "Delete" => QualifiedName(WellKnownTypes.System.Net.Http.HttpMethod.Name, IdentifierName("Delete")),
                "Get" => QualifiedName(WellKnownTypes.System.Net.Http.HttpMethod.Name, IdentifierName("Get")),
                "Head" => QualifiedName(WellKnownTypes.System.Net.Http.HttpMethod.Name, IdentifierName("Head")),
                "Options" => QualifiedName(WellKnownTypes.System.Net.Http.HttpMethod.Name, IdentifierName("Options")),
                "Patch" => QualifiedName(WellKnownTypes.System.Net.Http.HttpMethod.Name, IdentifierName("Patch")),
                "Post" => QualifiedName(WellKnownTypes.System.Net.Http.HttpMethod.Name, IdentifierName("Post")),
                "Put" => QualifiedName(WellKnownTypes.System.Net.Http.HttpMethod.Name, IdentifierName("Put")),
                "Trace" => QualifiedName(WellKnownTypes.System.Net.Http.HttpMethod.Name, IdentifierName("Trace")),
                _ => ObjectCreationExpression(WellKnownTypes.System.Net.Http.HttpMethod.Name).AddArgumentListArguments(
                    Argument(SyntaxHelpers.StringLiteral(operation.Key.ToUpperInvariant())))
            };
    }
}
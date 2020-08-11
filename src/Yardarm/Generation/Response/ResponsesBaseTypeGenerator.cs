﻿using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Yardarm.Helpers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Yardarm.Generation.Response
{
    public class ResponsesBaseTypeGenerator : TypeGeneratorBase
    {
        private const string BaseClassName = "ResponseBase";
        private const string MessageProperty = "Message";

        public ResponsesBaseTypeGenerator(GenerationContext context)
            : base(context)
        {
        }

        public override TypeSyntax GetTypeName()
        {
            var ns = Context.NamespaceProvider.GetRootNamespace();

            return QualifiedName(ns, IdentifierName(BaseClassName));
        }

        public override IEnumerable<MemberDeclarationSyntax> Generate()
        {
            ClassDeclarationSyntax declaration = ClassDeclaration(BaseClassName)
                .AddBaseListTypes(
                    SimpleBaseType(WellKnownTypes.IDisposable()))
                .AddModifiers(
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.AbstractKeyword))
                .AddMembers(
                    GenerateConstructor(),
                    GenerateMessageProperty(),
                    GenerateIsSuccessStatusCodeProperty(),
                    GenerateStatusCodeProperty(),
                    GenerateDisposeMethod());

            yield return declaration;
        }

        #region Constructors

        private ConstructorDeclarationSyntax GenerateConstructor() =>
            ConstructorDeclaration(BaseClassName)
                .AddModifiers(Token(SyntaxKind.PublicKeyword))
                .AddParameterListParameters(
                    Parameter(Identifier("message")).WithType(WellKnownTypes.HttpResponseMessage()))
                .WithBody(Block(
                    ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                        IdentifierName(MessageProperty),
                        SyntaxHelpers.ParameterWithNullCheck("message")))
                    ));

        #endregion

        #region Properties

        private PropertyDeclarationSyntax GenerateMessageProperty() =>
            PropertyDeclaration(WellKnownTypes.HttpResponseMessage(), Identifier(MessageProperty))
                .AddModifiers(Token(SyntaxKind.PublicKeyword))
                .AddAccessorListAccessors(
                    AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));

        private PropertyDeclarationSyntax GenerateIsSuccessStatusCodeProperty() =>
            PropertyDeclaration(PredefinedType(Token(SyntaxKind.BoolKeyword)), Identifier("IsSuccessStatusCode"))
                .AddModifiers(Token(SyntaxKind.PublicKeyword))
                .WithExpressionBody(ArrowExpressionClause(
                    MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(MessageProperty), IdentifierName("IsSuccessStatusCode"))));

        private PropertyDeclarationSyntax GenerateStatusCodeProperty() =>
            PropertyDeclaration(WellKnownTypes.HttpStatusCode(), Identifier("StatusCode"))
                .AddModifiers(Token(SyntaxKind.PublicKeyword))
                .WithExpressionBody(ArrowExpressionClause(
                    MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(MessageProperty), IdentifierName("StatusCode"))));

        #endregion

        #region Methods

        private MethodDeclarationSyntax GenerateDisposeMethod() =>
            MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier("Dispose"))
                .AddModifiers(
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.VirtualKeyword))
                .WithBody(Block().AddStatements(ExpressionStatement(
                    InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName("Message"),
                        IdentifierName("Dispose"))))));

        #endregion
    }
}
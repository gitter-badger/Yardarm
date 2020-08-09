﻿using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Yardarm.Generation
{
    public interface ITypeGenerator
    {
        void Preprocess();

        TypeSyntax GetTypeName();

        SyntaxTree? GenerateSyntaxTree();

        IEnumerable<MemberDeclarationSyntax> Generate();
    }
}
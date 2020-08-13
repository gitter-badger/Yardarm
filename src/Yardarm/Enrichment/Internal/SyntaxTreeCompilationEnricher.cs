﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Yardarm.Generation;

namespace Yardarm.Enrichment.Internal
{
    internal class SyntaxTreeCompilationEnricher : ICompilationEnricher
    {
        private readonly IList<ISyntaxTreeGenerator> _generators;

        public int Priority => CompilationEnrichmentPriority.SyntaxTreeGeneration;

        public SyntaxTreeCompilationEnricher(IEnumerable<ISyntaxTreeGenerator> generators)
        {
            _generators = generators.ToArray();
        }

        public ValueTask<CSharpCompilation> EnrichAsync(CSharpCompilation target,
            CancellationToken cancellationToken = default) =>
            new ValueTask<CSharpCompilation>(target
                .AddSyntaxTrees(_generators
                    .AsParallel()
                    .AsUnordered()
                    .WithCancellation(cancellationToken)
                    .SelectMany(p => p.Generate())
                    .ToArray()));
    }
}

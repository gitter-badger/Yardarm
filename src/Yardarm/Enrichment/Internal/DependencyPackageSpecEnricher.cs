﻿using System.Collections.Generic;
using System.Linq;
using NuGet.Packaging;
using NuGet.ProjectModel;
using Yardarm.Packaging;

namespace Yardarm.Enrichment.Internal
{
    internal class DependencyPackageSpecEnricher : IPackageSpecEnricher
    {
        private readonly IDependencyGenerator[] _dependencyGenerators;

        public DependencyPackageSpecEnricher(IEnumerable<IDependencyGenerator> dependencyGenerators)
        {
            _dependencyGenerators = dependencyGenerators.ToArray();
        }

        public int Priority => 0;

        public PackageSpec Enrich(PackageSpec packageSpec)
        {
            packageSpec.Dependencies.AddRange(_dependencyGenerators
                .SelectMany(p => p.GetDependencies()));

            return packageSpec;
        }
    }
}

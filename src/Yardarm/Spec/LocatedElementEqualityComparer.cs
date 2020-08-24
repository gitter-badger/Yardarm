﻿using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;

namespace Yardarm.Spec
{
    internal class LocatedElementEqualityComparer<T> : IEqualityComparer<ILocatedOpenApiElement<T>>
        where T : IOpenApiElement
    {
        public bool Equals(ILocatedOpenApiElement<T>? x, ILocatedOpenApiElement<T>? y)
        {
            if (x == null)
            {
                return y == null;
            }

            if (y == null)
            {
                return false;
            }

            if (x.Element is IOpenApiReferenceable referenceableX && y.Element is IOpenApiReferenceable referenceableY)
            {
                if (referenceableX.Reference != null)
                {
                    if (referenceableY.Reference == null)
                    {
                        // Can't be equal if one is a reference and the other is not
                        return false;
                    }

                    return referenceableX.Reference.ReferenceV3 == referenceableY.Reference.ReferenceV3;
                }
            }

            // Neither are references, so compare the paths

            if (!ReferenceEquals(x.Element, y.Element) || x.Key != y.Key || (x.Parent is null) != (y.Parent is null))
            {
                return false;
            }

            return x.Parent is null || Equals(x.Parent, y.Parent);
        }

        public int GetHashCode(ILocatedOpenApiElement<T> obj)
        {
            if (obj.Element is IOpenApiReferenceable referenceable && referenceable.Reference != null)
            {
                return referenceable.Reference.ReferenceV3.GetHashCode();
            }
            else
            {
                var hashCode = new HashCode();

                hashCode.Add(obj.Element);
                hashCode.Add(obj.Key);
                hashCode.Add(obj.Parent);

                return hashCode.ToHashCode();
            }
        }
    }
}

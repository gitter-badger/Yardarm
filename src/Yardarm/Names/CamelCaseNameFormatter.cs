﻿using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Yardarm.Names
{
    public class CamelCaseNameFormatter : INameFormatter
    {
        public static CamelCaseNameFormatter Instance { get; } = new CamelCaseNameFormatter();

        [return: NotNullIfNotNull("name")]
        public virtual string? Format(string? name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }

            var builder = new StringBuilder(name!.Length);

            bool first = true;
            bool nextCapital = false;
            foreach (char ch in name)
            {
                if (first)
                {
                    first = false;

                    if (char.IsLetter(ch))
                    {
                        builder.Append(char.ToLowerInvariant(ch));
                        continue;
                    }
                }

                if (char.IsLetter(ch))
                {
                    if (nextCapital)
                    {
                        builder.Append(char.ToUpperInvariant(ch));
                        nextCapital = false;
                    }
                    else
                    {
                        builder.Append(ch);
                    }
                }
                else if (char.IsDigit(ch))
                {
                    builder.Append(ch);
                    nextCapital = true;
                }
                else
                {
                    nextCapital = true;
                }
            }

            return builder.ToString();
        }
    }
}

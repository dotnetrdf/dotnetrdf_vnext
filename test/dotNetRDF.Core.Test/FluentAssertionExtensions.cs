using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions.Primitives;
using FluentAssertions.Numeric;
using FluentAssertions.Execution;

namespace VDS.RDF
{
    public static class FluentAssertionExtensions
    {
        public static void BeEqualTo(this ObjectAssertions assertions, object value, string because, params object[] reasonArgs)
        {
            Execute.Assertion.ForCondition(assertions.Subject.Equals(value)).BecauseOf(because, reasonArgs).FailWith("Expected object to be equal to {0}{reason}", null);
        }

        public static void NotBeEqualTo(this ObjectAssertions assertions, object value, string because, params object[] reasonArgs)
        {
            Execute.Assertion.ForCondition(!assertions.Subject.Equals(value)).BecauseOf(because, reasonArgs).FailWith("Expected object to not be equal to {0}{reason}", null);
        }

        public static void BeEqualTo<T>(this ComparableTypeAssertions<T> assertions, T value, string because, params object[] reasonArgs)
        {
            Execute.Assertion.ForCondition(assertions.Subject.Equals(value)).BecauseOf(because, reasonArgs).FailWith("Expected object to be equal to {0}{reason}", null);
        }

        public static void NotBeEqualTo<T>(this ComparableTypeAssertions<T> assertions, T value, string because, params object[] reasonArgs)
        {
            Execute.Assertion.ForCondition(!assertions.Subject.Equals(value)).BecauseOf(because, reasonArgs).FailWith("Expected object to not be equal to {0}{reason}", null);
        }

    }
}

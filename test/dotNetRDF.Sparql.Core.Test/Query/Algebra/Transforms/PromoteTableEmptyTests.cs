using Xunit;

namespace VDS.RDF.Query.Algebra.Transforms
{
    public class PromoteTableEmptyTests
        : AbstractAlgebraTransformTests
    {
        protected override IAlgebraTransform CreateInstance()
        {
            return new PromoteTableEmpty();
        }

        [Fact]
        public void PromoteTableEmptyJoin1()
        {
            IAlgebra lhs = Table.CreateEmpty();
            IAlgebra rhs = Table.CreateUnit();

            IAlgebra join = Join.CreateDirect(lhs, rhs);
            Assert.IsType(typeof(Join), join);

            CheckTransform(join, lhs);
        }

        [Fact]
        public void PromoteTableEmptyJoin2()
        {
            IAlgebra lhs = Table.CreateUnit();
            IAlgebra rhs = Table.CreateEmpty();

            IAlgebra join = Join.CreateDirect(lhs, rhs);
            Assert.IsType(typeof(Join), join);

            CheckTransform(join, rhs);
        }

        [Fact]
        public void PromoteTableEmptyJoin3()
        {
            IAlgebra lhs = Table.CreateEmpty();
            IAlgebra rhs = Table.CreateEmpty();

            IAlgebra join = Join.CreateDirect(lhs, rhs);
            Assert.IsType(typeof(Join), join);

            CheckTransform(join, lhs);
        }

        [Fact]
        public void PromoteTableEmptyJoin4()
        {
            IAlgebra lhs = Table.CreateUnit();
            IAlgebra rhs = Join.CreateDirect(Table.CreateUnit(), Table.CreateEmpty());

            IAlgebra join = Join.CreateDirect(lhs, rhs);
            Assert.IsType(typeof(Join), join);

            CheckTransform(join, Table.CreateEmpty());
        }
    }
}

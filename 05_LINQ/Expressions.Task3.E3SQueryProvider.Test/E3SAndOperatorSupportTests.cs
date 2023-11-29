using System;
using System.Linq;
using System.Linq.Expressions;
using Expressions.Task3.E3SQueryProvider.Models.Entities;
using Xunit;

namespace Expressions.Task3.E3SQueryProvider.Test
{
    public class E3SAndOperatorSupportTests
    {
        #region SubTask 3: AND operator support

        [Fact]
        public void TestAndQueryable()
        {
            var translator = new ExpressionToFtsRequestTranslator();
            Expression<Func<IQueryable<EmployeeEntity>, IQueryable<EmployeeEntity>>> expression
                = query => query.Where(e => e.Workstation == "EPRUIZHW006" && e.Manager.StartsWith("John") && e.Office == "Kyiv");

            string translated = translator.Translate(expression);
            Assert.Equal("Workstation:(EPRUIZHW006) AND Manager:(John*)", translated);
        }

        [Fact]
        public void ComplexAndQueryTest()
        {
            var translator = new ExpressionToFtsRequestTranslator();
            Expression<Func<EmployeeEntity, bool>> expression
                = employee => employee.Workstation == "EPRUIZHW006" && employee.Manager.StartsWith("John") && employee.Office == "Kyiv";

            string translated = translator.Translate(expression);
            Assert.Equal("Workstation:(EPRUIZHW006) AND Manager:(John*) AND Office:(Kyiv)", translated);
        }

        [Fact]
        public void NoAndOperatorTest()
        {
            var translator = new ExpressionToFtsRequestTranslator();
            Expression<Func<EmployeeEntity, bool>> expression
                = employee => employee.Workstation == "EPRUIZHW006";

            string translated = translator.Translate(expression);
            Assert.Equal("Workstation:(EPRUIZHW006)", translated);
        }

        #endregion
    }
}

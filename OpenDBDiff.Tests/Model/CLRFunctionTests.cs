using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDBDiff.SqlServer.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Options;

namespace OpenDBDiff.Tests.Model.Tests
{
    [TestClass]
    public class CLRFunctionTests
    {
        private static CLRFunction CreateFunction()
        {
            Database database = new Database();
            database.Options = new SqlOption();
            CLRFunction function = new CLRFunction(database)
            {
                Name = "RegexGroupValuesByName",
                Owner = "dbo",
                AssemblyName = "RegexFunctions",
                AssemblyClass = "UserDefinedFunctions",
                AssemblyMethod = "RegexGroupValuesByName",
                AssemblyExecuteAs = "CALLER",
            };
            function.Parameters.Add(new Parameter { Name = "@input", Type = "nvarchar", Size = -1 });
            return function;
        }

        [TestMethod]
        public void ToSql_ScalarFunction_ShouldReturnScalarType()
        {
            CLRFunction function = CreateFunction();
            function.ReturnType.Type = "nvarchar";
            function.ReturnType.Size = 4000;

            string sql = function.ToSql();

            StringAssert.Contains(sql, "CREATE FUNCTION [dbo].[RegexGroupValuesByName] (@input [nvarchar](max))");
            StringAssert.Contains(sql, "RETURNS  [nvarchar](4000) WITH EXECUTE AS CALLER");
        }

        [TestMethod]
        public void ToSql_TableValuedFunction_ShouldReturnTable()
        {
            CLRFunction function = CreateFunction();
            function.IsTableValued = true;
            function.ReturnColumns.Add(new Parameter { Name = "[GroupName]", Type = "nvarchar", Size = 128 });
            function.ReturnColumns.Add(new Parameter { Name = "[Value]", Type = "nvarchar", Size = -1 });

            string sql = function.ToSql();

            StringAssert.Contains(sql, "RETURNS TABLE ([GroupName] [nvarchar](128),[Value] [nvarchar](max)) WITH EXECUTE AS CALLER");
            StringAssert.Contains(sql, "EXTERNAL NAME [RegexFunctions].[UserDefinedFunctions].[RegexGroupValuesByName]");
        }
    }
}

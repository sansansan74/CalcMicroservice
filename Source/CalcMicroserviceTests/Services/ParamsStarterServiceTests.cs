namespace CalcMicroservice.Services.Tests
{
    [TestClass()]
    public class ParamsStarterServiceTests
    {
        [TestMethod()]
        public void ContainsModuleTestInclude()
        {
            string[] args = ["STARTMODULES='+MOD1 +MOD2'", "p1=v1"];

            // Act
            var ps = new ParamsStarterService();
            ps.Init(args);

            // Assert
            Assert.IsTrue(ps.ContainsModule("MOD1"));
            Assert.IsTrue(ps.ContainsModule("MOD2"));
            Assert.IsFalse(ps.ContainsModule("MOD3"));
        }

        [TestMethod()]
        public void ContainsModuleTestExclude()
        {
            string[] args = ["STARTMODULES='-MOD1 -MOD2'", "p1=v1"];

            // Act
            var ps = new ParamsStarterService();
            ps.Init(args);

            // Assert
            Assert.IsFalse(ps.ContainsModule("MOD1"));
            Assert.IsFalse(ps.ContainsModule("MOD2"));
            Assert.IsTrue(ps.ContainsModule("MOD3"));
        }



        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ContainsModuleTestIncludeExcludeMix()
        {
            string[] args = ["STARTMODULES='+MOD1 -MOD2'", "p1=v1"];

            // Act
            var ps = new ParamsStarterService();
            ps.Init(args);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ContainsModuleTestFromEnvironmentVariableAndArgsMix()
        {
            Environment.SetEnvironmentVariable("STARTMODULES", "+MOD1 +MOD2");
            string[] args = ["STARTMODULES='+MOD1'", "p1=v1"];

            try
            {
                var ps = new ParamsStarterService();
                ps.Init(args);
            }
            finally
            {
                Environment.SetEnvironmentVariable("STARTMODULES", "");
            }
        }

        [TestMethod()]
        public void ContainsModuleTestFromEnvironmentVariable()
        {
            Environment.SetEnvironmentVariable("STARTMODULES", "-MOD1 -MOD2");

            try
            {
                string[] args = ["p1=v1"];
                // Act
                var ps = new ParamsStarterService();
                ps.Init(args);

                // Assert
                Assert.IsFalse(ps.ContainsModule("MOD1"));
                Assert.IsFalse(ps.ContainsModule("MOD2"));
                Assert.IsTrue(ps.ContainsModule("MOD3"));
            }
            finally
            {
                Environment.SetEnvironmentVariable("STARTMODULES", "");
            }
        }

        [TestMethod()]
        public void ContainsModuleTestExcludeAll()
        {
            string[] args = ["STARTMODULES=''", "p1=v1"];

            // Act
            var ps = new ParamsStarterService();
            ps.Init(args);

            // Assert
            Assert.IsTrue(ps.ContainsModule("MOD1"));
            Assert.IsTrue(ps.ContainsModule("MOD2"));
            Assert.IsTrue(ps.ContainsModule("MOD3"));
        }
    }
}
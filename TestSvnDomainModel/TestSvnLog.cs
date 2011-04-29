using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SvnDomainModel;

namespace TestSvnDomainModel
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class TestSvnLog
    {
        #region "MsTest Stuff"
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
        #endregion

        private Svn mSvnLog;

         [TestInitialize()]
         public void MyTestInitialize() {
             mSvnLog = new Svn(string.Empty, string.Empty, string.Empty);
         }
        

        [TestMethod]
        public void TestRevisionsReturnedAsASingleRangeIfEveryRevisionIsSelected()
        {
            var Revisions = mSvnLog.GetRevisionRange(new string[] { "r1", "r2", "r3" }, new string[] { "r1", "r2", "r3" });

            Assert.AreEqual(1, Revisions.Count());
            Assert.AreEqual("r1:r3", Revisions[0]);
        }

        [TestMethod]
        public void TestSkippingARevisionResultsInTwoNoneRangesBeingReturned()
        {
            var Revisions = mSvnLog.GetRevisionRange(new string[] { "r1", "r2", "r3" }, new string[] { "r1", "r3" });

            Assert.AreEqual(2, Revisions.Count());
            Assert.AreEqual("r1", Revisions[0]);
            Assert.AreEqual("r3", Revisions[1]);
        }

        [TestMethod]
        public void TestSkippingLastResultsInOneRange()
        {
            var Revisions = mSvnLog.GetRevisionRange(new string[] { "r1", "r2", "r3" }, new string[] { "r1", "r2" });

            Assert.AreEqual(1, Revisions.Count());
            Assert.AreEqual("r1:r2", Revisions[0]);
        }

        [TestMethod]
        public void TestSkippingMiddleCanCreateTwoRanges()
        {
            var Revisions = mSvnLog.GetRevisionRange(new string[] { "r1", "r2", "r3", "r4", "r5" }, new string[] { "r1", "r2", "r4", "r5" });

            Assert.AreEqual(2, Revisions.Count());
            Assert.AreEqual("r1:r2", Revisions[0]);
            Assert.AreEqual("r4:r5", Revisions[1]);
        }

        [TestMethod]
        [ExpectedException(typeof(SvnProcess.SvnException))]
        public void TestSvnProncessThrowsAnErrorWhenTheStandardErrorLogHasSomthingInIt()
        {
            var proc = new SvnProcess("-asdf", "netstat");
            proc.ExecuteCommand();
        }

        [TestMethod]
        public void TestWhenOnlySingleRevisionSelectedItGetsTurnedIntoRevisionRange()
        {
            var Revisions = mSvnLog.GetRevisionRange(new string[] { "r1", "r2", "r3" }, new string[] { "r1" });

            Assert.AreEqual("r1:r1", Revisions[0]);
        }

        [TestMethod]
        public void TestRangeCreatedWhenFirstRevisionSkipped()
        {
            var Revisions = mSvnLog.GetRevisionRange(new string[] { "r1", "r20", "r21", "r22"}, new string[] { "r20", "r21", "r22"});

            Assert.AreEqual(1, Revisions.Count());
            Assert.AreEqual("r20:r22", Revisions[0]);
        }
    }
}

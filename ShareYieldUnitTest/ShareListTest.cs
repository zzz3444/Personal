using System;
using ShareYield.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace ShareYieldUnitTest
{
    [TestClass]
    public class ShareListTest
    {
        [TestMethod]
        public void TestGetShares()
        {
            ShareList shares = new ShareList();
            Assert.IsInstanceOfType(shares.getShares(), typeof(List<Share>));
            Assert.IsTrue(shares.getShares().Count == 17);
        }
    }
}

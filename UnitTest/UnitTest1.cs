using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kogler.SerialCOM
{
    [TestClass]
    public class BisModelTest
    {
        
        
        [TestMethod]
        public void ReadDataTest()
        {
            var bis = new BisModel();
            bis.AddSampleData();

            Assert.AreEqual(bis.Entries.Count, 5);
        }
    }
}

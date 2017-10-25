using Microsoft.VisualStudio.TestTools.UnitTesting;
using StratumMiner.MiningService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StratumMiner.Tests
{
    [TestClass]
    public class UInt256Tests
    {
        [TestMethod]
        public void VerifyZeroValue()
        {
            var number = UInt256.Parse("0");

            Assert.IsTrue(number.Data.Length == UInt256.Size);
            Assert.IsFalse(number.Data.Any(x => x != 0));

        }
        [TestMethod]
        public void VerifyOneValue()
        {
            var number = UInt256.Parse("1");
            Assert.IsTrue(number.Data.Length == UInt256.Size);
            Assert.IsTrue(number.Data.First() == 1);
        }

        [TestMethod]
        public void VerifyValue_FF()
        {
            var number = UInt256.Parse("FF");
            Assert.IsTrue(number.Data.Length == UInt256.Size);
            Assert.IsTrue(number.Data.First() == 255);

        }
        [TestMethod]
        public void VerifyValue_02FF()
        {
            var number = UInt256.Parse("02FF");
            Assert.IsTrue(number.Data.Length == UInt256.Size);
            Assert.IsTrue(number.Data[0] == 0xFF);
            Assert.IsTrue(number.Data[1] == 0x02);
        }
        [TestMethod]
        public void VerifyValue_0302FF()
        {
            var number = UInt256.Parse("0302FF");
            Assert.IsTrue(number.Data.Length == UInt256.Size);
            Assert.IsTrue(number.Data[0] == 0xFF);
            Assert.IsTrue(number.Data[1] == 0x02);
            Assert.IsTrue(number.Data[2] == 0x03);
        }
        [TestMethod]
        public void VerifyValue_040302FF()
        {
            var number = UInt256.Parse("040302FF");
            Assert.IsTrue(number.Data.Length == UInt256.Size);
            Assert.IsTrue(number.Data[0] == 0xFF);
            Assert.IsTrue(number.Data[1] == 0x02);
            Assert.IsTrue(number.Data[2] == 0x03);
            Assert.IsTrue(number.Data[3] == 0x04);
        }
        [TestMethod]
        public void VerifyValueFromByteArray()
        {
            var number = new UInt256(new byte[] { 1 });
            Assert.IsTrue(number.Data[0] == 1);
        }




    }
}

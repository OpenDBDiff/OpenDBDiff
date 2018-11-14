﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDBDiff.Schema.SQLServer.Generates.Generates.Util;

namespace OpenDBDiff.Schema.SQLServer.Generates.Generates.Utils.Tests
{
    [TestClass]
    public class ByteToHexEncoderTests
    {
        [TestMethod]
        public void EncodesBytesIntoHex()
        {
            byte[] bytes = { 0, 1, 2, 4, 8, 16, 32, 64, 128, 255 };

            var hex = ByteToHexEncoder.ByteArrayToHex(bytes);

            Assert.AreEqual("0x000102040810204080FF", hex);
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void fifteenThousandUsersHR()
        {
            for (int i = 0; i < 15000;i++ )
            {
                BLL.Test.TestingClass.loginTest("000000","Resco@1234");
            }
        }
        [TestMethod]
        public void tenThousandUsersHR()
        {
            for (int i = 0; i < 10000; i++)
            {
                BLL.Test.TestingClass.loginTest("000000", "Resco@1234");
            }
        }
        [TestMethod]
        public void oneThousandUsersHR()
        {
            for (int i = 0; i < 1000; i++)
            {
                BLL.Test.TestingClass.loginTest("000000", "Resco@1234");
            }
        }
        [TestMethod]
        public void fifteenThousandUsersLM()
        {
            for (int i = 0; i < 15000; i++)
            {
                BLL.Test.TestingClass.loginTest("510502", "Hbl@1234");
            }
        }
        [TestMethod]
        public void tenThousandUsersLM()
        {
            for (int i = 0; i < 10000; i++)
            {
                BLL.Test.TestingClass.loginTest("510502", "Hbl@1234");
            }
        }
        [TestMethod]
        public void oneThousandUsersLM()
        {
            for (int i = 0; i < 1000; i++)
            {
                BLL.Test.TestingClass.loginTest("510502", "Hbl@1234");
            }
        }
    }
}





















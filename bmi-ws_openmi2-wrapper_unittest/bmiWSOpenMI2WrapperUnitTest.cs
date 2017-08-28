using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using bmiFlaskOpenmi2;

namespace UnitTestProject2
{
    [TestClass]
    public class bmiWSOpenMI2WrapperUnitTest
    {
        [TestMethod]
        public void testInitialise()
        {
            bmiFlaskOpenMI2Wrapper comp = new bmiFlaskOpenMI2Wrapper();
            //todo fail test if response isn't success
        }

        [TestMethod]
        public void testInitialiseUpdate()
        {
            bmiFlaskOpenMI2Wrapper comp = new bmiFlaskOpenMI2Wrapper();
            comp.Initialise("met_base");
            comp.Update();
            //todo fail test if response isn't success
        }

        [TestMethod]
        public void testInitialiseStatusUpdateStatus()
        {
            bmiFlaskOpenMI2Wrapper comp = new bmiFlaskOpenMI2Wrapper();
            comp.Initialise("met_base");
            string status = comp.Ping();
            comp.Update();
            status = comp.Ping();
            //todo fail test if response isn't success
        }

        [TestMethod]
        public void testInitialiseUpdateFinsh()
        {
            bmiFlaskOpenMI2Wrapper comp = new bmiFlaskOpenMI2Wrapper();
            comp.Initialise("met_base");
            comp.Update();
            comp.Finish();
            string status = comp.Ping();
            //todo fail test if response isn't success
        }
        
        [TestMethod]
        public void testInitialiseGetDoublesFinsh()
        {
            bmiFlaskOpenMI2Wrapper comp = new bmiFlaskOpenMI2Wrapper();
            comp.Initialise("met_base");
            Double[] d = comp.GetDoubles("snowpack__depth", -99.99); //Missing value not used 
            comp.Finish();
            string status = comp.Ping();
            //todo fail test if response isn't success
        }
    }
}

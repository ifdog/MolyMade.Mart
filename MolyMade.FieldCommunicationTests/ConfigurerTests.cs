using Microsoft.VisualStudio.TestTools.UnitTesting;
using MolyMade.FieldCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolyMade.FieldCommunication.Tests
{
    [TestClass()]
    public class ConfigurerTests
    {
        [TestMethod()]
        public void DisposeTest()
        {
            Configurer c = new Configurer();
            Assert.IsNotNull(c.Load());
        }
    }
}
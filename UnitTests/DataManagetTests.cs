using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using StockViewApp;
using NUnit.Gui;
using System.Reflection;
using UnitTests.Mocks;

namespace UnitTests
{

    [TestFixture]
    public class DataManagetTests
    {

        [Test]
        public void DMGetStocksTest()
        {
            IFinanceDataBusinessLogic dataBL = new MockFinanceBusinessLogic();
            DataManager dataManager = new DataManager("NSE", dataBL);
            Assert.AreSame(dataManager.DataBL, dataBL);
        }


    }
}

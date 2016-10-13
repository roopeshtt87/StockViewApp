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

    static class UniTestsStartup
    {
        [STAThread]
        static void Main()
        {
            AppEntry.Main(new[] { Assembly.GetExecutingAssembly().Location });
        }
    }

    [TestFixture]
    public class  MainWindowViewModelTests
    {

        [Test]
        public void VMCreationTest()
        {
            var vm = new MainWindowViewModel("NSE");
            Assert.AreEqual(vm.Exchange, "NSE");
            Assert.IsNotNull(vm.dataManager);
            Assert.IsNotNull(vm.dataManager.DataBL);
        }

    }
}

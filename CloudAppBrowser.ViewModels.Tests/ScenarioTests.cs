using System;
using System.Linq;
using CloudAppBrowser.Core;
using CloudAppBrowser.ViewModels.Services.Eureka;
using NUnit.Framework;

namespace CloudAppBrowser.ViewModels.Tests
{
    [TestFixture]
    public class ScenarioTests
    {
        [Test]
        public void TestEnvironmentCreation()
        {
            AppBrowserViewModel appBrowserViewModel = new AppBrowserViewModel(new AppBrowser(), new MockViewContext());

            appBrowserViewModel.MainForm.AddEnvironment();
            appBrowserViewModel.Environments[0].AddDockerService();
            appBrowserViewModel.Environments[0].AddEurekaService();

            Assert.That(appBrowserViewModel.Environments[0].Services.Select(s => s.ModuleName).ToArray(), Is.EqualTo(new string[] {"Docker", "Eureka"}));
            Assert.That(appBrowserViewModel.MainForm.ModulesTree.Modules.Select(s => s.ModuleName).ToArray(), Is.EqualTo(new string[] {"New Environment"}));
            Assert.That(appBrowserViewModel.MainForm.ModulesTree.Modules[0].GetSubModules().Select(s => s.ModuleName).ToArray(), Is.EqualTo(new string[] {"Docker", "Eureka"}));
            Assert.That(appBrowserViewModel.MainForm.ModulesTree.SelectedModule, Is.InstanceOf(typeof(EurekaServiceViewModel)));
        }

        private class MockViewContext : ViewContext
        {
            public override void Invoke(Action action)
            {
                action();
            }

            public override void MessageBox(string message, string caption)
            {
                Console.WriteLine($"{caption}:{message}");
            }

            public override bool ShowDialog(object viewModel)
            {
                return true;
            }

            public override object CreatePanel(object viewModel)
            {
                throw new NotImplementedException();
            }
        }
    }
}
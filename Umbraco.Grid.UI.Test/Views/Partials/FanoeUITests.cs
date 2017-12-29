using Moq;
using NUnit.Framework;
using Umbraco.Core.Dictionary;
using Umbraco.Grid.UI.Test.Utilities;
using Umbraco.Web;
using ASP;
using RazorGenerator.Testing;

namespace Umbraco.Grid.Fanoe.Tests
{
    [TestFixture()]
    public class FanoeUITests
    {
        private ContextMocker _contextMocker;
        private UmbracoHelper _helper;
        private _Views_Partials_Grid_Fanoe_cshtml _compiledView;

        [SetUp]
        public void Init()
        {
            _contextMocker = new ContextMocker();
            var dictionary = new Mock<ICultureDictionary>();
            dictionary.Setup(x => x[It.IsAny<string>()]).Returns("some translated value");
            _helper = _contextMocker.GetMockedUmbracoHelper(dictionary.Object);
            _compiledView = new _Views_Partials_Grid_Fanoe_cshtml(_helper);
            _compiledView.ViewContext = _contextMocker.GetViewContext();
        }


        [Test()]
        public void FanoeUITests_Get_Partial_View()
        {
            var html = _compiledView.RenderAsHtml(new object());
            //var msg = html.DocumentNode.Descendants("p").FirstOrDefault().InnerText;

            //Assert.IsTrue(!string.IsNullOrEmpty(msg));
            //Assert.IsTrue(!string.IsNullOrWhiteSpace(msg));
            //Assert.IsTrue(msg.ToLower().Contains("unavailable"));
        }
    }
}

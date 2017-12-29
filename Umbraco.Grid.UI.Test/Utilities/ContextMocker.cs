using Moq;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Logging;
using Umbraco.Core.Profiling;
using Umbraco.Core.Services;
using Umbraco.Core.Models;
using Umbraco.Core.Dictionary;
using System.Collections;
using System.Web;
using Umbraco.Web;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;
using System.Web.Security;

namespace Umbraco.Grid.UI.Test.Utilities
{
    public class ContextMocker
    {
        public ContextMocker(string[] qKeys = null)
        {
            ILogger loggerMock = Mock.Of<ILogger>();
            IProfiler profilerMock = Mock.Of<IProfiler>();
            IDictionary dictionary = Mock.Of<IDictionary>();

            var session = new Mock<HttpSessionStateBase>();
            var contextBaseMock = new Mock<HttpContextBase>();
            contextBaseMock.Setup(ctx => ctx.Session).Returns(session.Object);
            contextBaseMock.SetupGet(ctx => ctx.Items).Returns(dictionary);

            if (qKeys != null)
            {
                var requestMock = new Mock<HttpRequestBase>();
                requestMock.SetupGet(r => r.Url).Returns(new System.Uri("http://imaginary.com"));
                var mockedQstring = new Mock<NameValueCollection>();
                foreach (var k in qKeys)
                {
                    mockedQstring.Setup(r => r.Get(It.Is<string>(s => s.Contains(k) == true))).Returns("example_value_" + k);
                }

                requestMock.SetupGet(r => r.QueryString).Returns(mockedQstring.Object);
                contextBaseMock.SetupGet(p => p.Request).Returns(requestMock.Object);
            }

            WebSecurity webSecurityMock = new Mock<WebSecurity>(null, null).Object;
            IUmbracoSettingsSection umbracoSettingsSectionMock = Mock.Of<IUmbracoSettingsSection>();

            this.HttpContextBaseMock = contextBaseMock.Object;
            this.ApplicationContextMock = new ApplicationContext(CacheHelper.CreateDisabledCacheHelper(), new ProfilingLogger(loggerMock, profilerMock));
            this.UmbracoContextMock = UmbracoContext.EnsureContext(contextBaseMock.Object, this.ApplicationContextMock, webSecurityMock, umbracoSettingsSectionMock, Enumerable.Empty<IUrlProvider>(), true);
        }

        public ApplicationContext ApplicationContextMock { get; private set; }
        public UmbracoContext UmbracoContextMock { get; private set; }
        public HttpContextBase HttpContextBaseMock { get; private set; }

        //TODO: add mockings as params
        public UmbracoHelper GetMockedUmbracoHelper(ICultureDictionary dictionary)
        {
            UmbracoHelper helper = new UmbracoHelper(
            this.UmbracoContextMock,
            Mock.Of<IPublishedContent>(),
            Mock.Of<ITypedPublishedContentQuery>(query => query.TypedContent(It.IsAny<int>()) == Mock.Of<IPublishedContent>(content => content.Id == 7)),
            Mock.Of<IDynamicPublishedContentQuery>(),
            Mock.Of<ITagQuery>(),
            Mock.Of<IDataTypeService>(),
            new UrlProvider(this.UmbracoContextMock, Enumerable.Empty<IUrlProvider>()),
            dictionary,
            Mock.Of<IUmbracoComponentRenderer>(),
            new MembershipHelper(this.UmbracoContextMock, Mock.Of<MembershipProvider>(), Mock.Of<RoleProvider>()));

            return helper;
        }

        public ViewContext GetViewContext()
        {
            var controllecContext = new Mock<ControllerContext>();
            controllecContext.SetupGet(x => x.HttpContext).Returns(this.HttpContextBaseMock);
            TextWriter textWriter = new StreamWriter(new MemoryStream());
            var viewContext = new ViewContext(controllecContext.Object, new Mock<IView>().Object, new ViewDataDictionary(), new TempDataDictionary(), textWriter);

            return viewContext;
        }
    }
}

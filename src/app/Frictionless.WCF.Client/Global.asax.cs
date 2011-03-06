using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.ServiceModel;
using Frictionless.WCF.Interface;
using Autofac.Integration.Wcf;
using Frictionless.WCF.Client.Helpers;

namespace Frictionless.WCF.Client {
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication {

		private ServiceOptions ServiceOptions { get; set; }

		public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
			filters.Add(new HandleErrorAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes) {
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
					"Default", // Route name
					"{controller}/{action}/{id}", // URL with parameters
					new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
			);

		}

		protected void Application_Start() {

			ServiceOptions = ServiceOptions.LoadServiceOptions();

			AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);


			AppDomain currentDomain = AppDomain.CurrentDomain;
			currentDomain.AssemblyResolve += new ResolveEventHandler(ResolveAssembly);


			var builder = new ContainerBuilder();
			builder.RegisterControllers(typeof(MvcApplication).Assembly);

			//configure the service
			if (ServiceOptions.UseEmbeddedService)
				ConfigureEmbeddedService(builder, ServiceOptions.ServiceLibraryPath + "\\Frictionless.WCF.Service.dll");
			else
				ConfigureWcfService(builder, ServiceOptions.ServiceUrl);

			var container = builder.Build();
			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
			Frictionless.WCF.Interface.IService1 service = container.Resolve<Frictionless.WCF.Interface.IService1>();
		}

		/// <summary>
		/// This method uses refelection to load the service dynamically. It finds a class called AutoWirer and executes the COnfigure method on it.
		/// </summary>
		/// <param name="containerBuilder"></param>
		/// <param name="serviceDll"></param>
		private void ConfigureEmbeddedService(ContainerBuilder containerBuilder, string serviceDll) {
			Assembly serviceImplimentation = Assembly.LoadFrom(serviceDll);
			Type autowirerType = serviceImplimentation.GetTypes().FirstOrDefault(x => x.Name == "AutoWirer");
			var configureInfo = autowirerType.GetMethod("Configure");
			configureInfo.Invoke(null, new object[] { containerBuilder });
		}

		private void ConfigureWcfService(ContainerBuilder containerBuilder, string serviceUrl) {

			containerBuilder.Register(c => new ChannelFactory<IService1>(
				new System.ServiceModel.BasicHttpBinding(),
				new EndpointAddress(serviceUrl)))
				.SingleInstance();

			containerBuilder.Register(c => c.Resolve<ChannelFactory<IService1>>().CreateChannel())
				.UseWcfSafeRelease();
		}

		private Assembly ResolveAssembly(object obj, ResolveEventArgs args) {
			Assembly result = null;
			if (args.RequestingAssembly.GetName().Name == "Service") {
				result = Assembly.Load(ServiceOptions.ServiceLibraryPath + "\\" + args.Name);
			}
			return result;
		}
	}
}
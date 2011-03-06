using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Reflection;

namespace Frictionless.WCF.Client.Helpers {
	public class ServiceOptions {
		public bool UseEmbeddedService { get; set; }
		public string ServiceLibraryPath { get; set; }
		public string ServiceUrl { get; set; }

		public static ServiceOptions LoadServiceOptions() {
			ServiceOptions options = new ServiceOptions();

			bool embedService = false;
			bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["EmbedService"], out embedService);
			options.UseEmbeddedService = embedService;

			string servicePath = System.Configuration.ConfigurationManager.AppSettings["ServicePath"] ?? "";
			string currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
			currentPath = currentPath.Replace("file:\\", "");
			string serviceDirectory = new DirectoryInfo(currentPath + "\\" + servicePath).FullName;
			options.ServiceLibraryPath = serviceDirectory;

			options.ServiceUrl = System.Configuration.ConfigurationManager.AppSettings["ServiceUrl"] ?? "";

			return options;
		}

	}
}
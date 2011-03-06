using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Frictionless.WCF.Interface;
using Frictionless.WCF.Service;

namespace Frictionless.WCF.ServiceImpl {
	public static class AutoWirer {
		public static void Configure(ContainerBuilder builder) {
			builder.RegisterType<Service1>().As<IService1>();
		}
	}
}
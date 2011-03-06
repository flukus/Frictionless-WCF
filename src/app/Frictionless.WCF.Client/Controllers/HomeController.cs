using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frictionless.WCF.Interface;

namespace Frictionless.WCF.Client.Controllers {
	public class HomeController : Controller {

		private readonly IService1 _Service1;
		private IService1 Service1 { get { return _Service1; } }

		public HomeController(IService1 service) {
			_Service1 = service;
		}

		public ActionResult Index() {
			string data = Service1.GetData(1337);
			ViewBag.valueFromService = data;
			return View(ViewBag);
		}

	}
}

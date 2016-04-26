using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform;
using MvvmCross.Platform.Core;
using MvvmCross.Platform.IoC;
using static System.Console;

namespace WindowsConsoleApp
{
	class Program
	{
		static void Main(string[] args)
		{
			MvxSimpleIoCContainer.Initialize();
			Mvx.RegisterSingleton(typeof(ITestService), () => TestServiceProxy.GetTestService());

			var service = Mvx.Resolve<ITestService>();
			var result = service.GetVersion();

			WriteLine("ITestService returned: {0}", result);
			WriteLine("Everything works fine.\r\nHit ENTER to quit.");
			ReadLine();
		}
	}

	public interface ITestService
	{
		string GetVersion();
	}

	// creates a transparent proxy that implements ITestService
	public class TestServiceProxy : RealProxy
	{
		public static ITestService GetTestService()
		{
			return new TestServiceProxy().GetTransparentProxy() as ITestService;
		}

		public TestServiceProxy() : base(typeof(ITestService))
		{
		}

		public override IMessage Invoke(IMessage msg)
		{
			var message = msg as IMethodCallMessage;
			switch (message.MethodName)
			{
				case "GetVersion":
					return new ReturnMessage("Version 1.0 alpha", null, 0, null, message);

				case "GetType":
					return new ReturnMessage(typeof(ITestService), null, 0, null, message);
			}

			return new ReturnMessage(new NotImplementedException(), message);
		}
	}
}

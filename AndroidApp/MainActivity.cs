using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using MvvmCross.Platform.IoC;
using MvvmCross.Platform;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using MvvmCross.Platform.Exceptions;

namespace AndroidApp
{
	[Activity(Label = "AndroidApp", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		int count = 1;

		static MainActivity()
		{
			MvxSimpleIoCContainer.Initialize();
			Mvx.RegisterSingleton(typeof(ITestService), () => TestServiceProxy.GetTestService());
		}

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button>(Resource.Id.MyButton);

			button.Click += (s, e) =>
			{
				count++;

				try
				{
					var service = Mvx.Resolve<ITestService>();
					var result = service.GetVersion();

					button.Text = $"ITestService returned: {result} ({count})";
					Toast.MakeText(this, "Everything works fine!", ToastLength.Long).Show();
				}
				catch (MvxException ex)
				{
					button.Text = ex.Message;
					Toast.MakeText(this, "Resolution failed.", ToastLength.Long).Show();
				}
			};
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


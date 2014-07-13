﻿
namespace Anycmd.EDI.MessageDispatcher {
	using Anycmd.Web;
	using Application;
	using Ef;
	using Exceptions;
	using Host;
	using Host.EDI;
	using Host.EDI.Handlers;
	using Host.EDI.Handlers.Distribute;
	using Logging;
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Reflection;
	using System.Security.Principal;
	using System.Threading;

	/// <summary>
	/// <remarks>
	/// 设计不能在本体库服务器之外运行本程序
	/// </remarks>
	/// </summary>
	class Program {
		static void Main(string[] args) {

			//程序没有运行
			try {
				// 当前用户是管理员的时候，直接启动应用程序，如果不是管理员，
				// 则使用启动对象启动程序，以确保使用管理员身份运行
				WindowsIdentity identity = WindowsIdentity.GetCurrent();
				var principal = new WindowsPrincipal(identity);
				if (principal.IsInRole(WindowsBuiltInRole.Administrator)) {
					Run();
				}
				else {
					//创建启动对象
					var startInfo = new System.Diagnostics.ProcessStartInfo();
					//设置运行文件
					startInfo.FileName = Assembly.GetExecutingAssembly().Location;
					//设置启动参数
					startInfo.Arguments = String.Join(" ", args);
					//设置启动动作,确保以管理员身份运行
					startInfo.Verb = "runas";
					//如果不是管理员，则启动UAC
					System.Diagnostics.Process.Start(startInfo);
				}
			}
			catch (Exception ex) {
				LoggingService.Error(ex);
				Console.WriteLine(ex.Message);
				Console.WriteLine("按任意键退出");
				Console.ReadKey();
			}
		}

		#region Run
		private static void Run() {
			Console.WriteLine("正在启动服务...");
			#region boot
			EfContext.InitStorage(new SimpleEfContextStorage());
			// 环境初始化
			AppHost appHost = new DefaultAppHost();
			var container = appHost.Container;
			container.AddService(typeof(ILoggingService), new log4netLoggingService(appHost));
			container.AddService(typeof(IUserSessionStorage), new WebUserSessionStorage());
			container.AddService(typeof(IOperationLogStorage), new WebOperationLogStorage());
			appHost.Init();
			new DefaultNodeHost(appHost).Init();
			appHost.RegisterRepository(new List<string>
			{
				"EDIEntities",
				"ACEntities",
				"InfraEntities",
				"IdentityEntities"
			}, typeof(AppHost).Assembly);
			appHost.RegisterEDICore();
			string processID = ConfigurationManager.AppSettings["ProcessID"];
			ProcessDescriptor process;
			if (!NodeHost.Instance.Processs.TryGetProcess(new Guid(processID), out process)) {
				throw new CoreException("非法的分发器标识" + processID);
			}
			bool createdNew;
			// 使用本体标识从而限制一个本体只能有一个分发器进程
			string globalGuid = string.Format("Global\\{0}_Dispatcher", process.Process.OntologyID);
			var mutex = new Mutex(true, globalGuid, out createdNew);
			if (!createdNew) {
				//程序正在运行
				Console.WriteLine("绑定到本本体的程序已在运行……");
				Console.WriteLine("无需重复启动。按任意键退出本窗口");
				Console.ReadKey();
				return;
			}
			#endregion

			IDispatcher dispatcher = appHost.GetRequiredService<IDispatcherFactory>().CreateDispatcher(process);
			Console.Title = process.Title;
			if (process.IsRuning()) {
				//程序正在运行
				Console.WriteLine("绑定到本本体的程序已在运行……");
				Console.WriteLine("无需重复启动。按任意键退出本窗口");
				Console.ReadKey();
				return;
			}

			var serviceHost = new ServiceSelfHost(process);
			serviceHost.Init();
			string words = "命令提示：开始:start 停止:stop 退出:exit 帮助:help\n"
				+ string.Format("监听地址：{0}", process.WebApiBaseAddress);
			Console.WriteLine(words);

			AtachEventMethod(dispatcher);
			dispatcher.Start();

			#region 控制
			bool isRuning = false;
			while (true) {
				string arg = "";
				arg = Console.ReadLine().Trim().ToLower();

				switch (arg) {
					case "start":
						dispatcher.Start();
						isRuning = true;
						break;
					case "stop":
						dispatcher.Stop();
						isRuning = false;
						break;
					case "exit":
						dispatcher.Stop();
						isRuning = false;
						Environment.Exit(0);
						break;
					case "?":
					case "/?":
					case "help":
						Console.WriteLine(words);
						if (isRuning) {
							Console.WriteLine("运行中...");
						}
						break;
					case "":
						break;
					default:
						Console.WriteLine("不支持的命令");
						break;
				}
			}
			#endregion
		}
		#endregion

		private static void AtachEventMethod(IDispatcher sender) {
			sender.Starting += sender_Starting;
			sender.Stopping += sender_Stopping;
			sender.Stoped += sender_Stoped;
			sender.Distributing += sender_Sending;
			sender.Distributed += sender_Sended;
			sender.Sleepping += sender_Sleepping;
			sender.Waked += sender_Waked;
			sender.Error += sender_Error;
		}

		#region event methods
		static void sender_Waked(object sender, EventArgs e) {
			Console.WriteLine("醒来" + DateTime.Now.ToString());
		}

		private static void sender_Starting(object sender, EventArgs e) {
			var s = sender as IDispatcher;
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(string.Format("发送者:{0}", s.Id.ToString()));
			Console.WriteLine("开始工作" + DateTime.Now.ToString());
		}

		private static void sender_Stopping(object sender, StoppingEventArgs e) {
			Console.WriteLine("正在结束...");
		}

		private static void sender_Stoped(object sender, EventArgs e) {
			Console.WriteLine("停止工作" + DateTime.Now.ToString());
		}

		private static void sender_Sending(object sender, DistributingEventArgs e) {
			var sendStrategy = sender as IDispatcher;
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine();
			Console.Write("----------------------------------");
			Console.Write("发送开始");
			Console.Write("----------------------------------");
			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(string.Format("目标节点 {0} 转移策略 {1}", e.Context.ClientAgent.Name, e.Context.ClientAgent.Transfer.Name));
			Console.WriteLine(string.Format("目标地址 {0}", e.Context.Responder.Transfer.GetAddress(e.Context.Responder)));
			Console.ForegroundColor = ConsoleColor.Gray;
		}

		private static void sender_Sended(object sender, DistributedEventArgs e) {
			var dispatcher = sender as IDispatcher;
			if (dispatcher == null) {
				throw new CoreException();
			}
			Console.ForegroundColor = ConsoleColor.Green;
			if (!e.Context.Result.IsSuccess) {
				Console.ForegroundColor = ConsoleColor.Red;
			}
			Console.WriteLine(string.Format("响应 {0} {1}", e.Context.Result.ReasonPhrase, e.Context.Result.Description));
			Console.WriteLine(string.Format("状态码:{0}\n说明:{1}",
				e.Context.Result.Status.ToString(), e.Context.Result.Description));
			if (!e.Context.Result.IsSuccess) {
				Console.ForegroundColor = ConsoleColor.Green;
			}
			Console.WriteLine(string.Format("完成发送时间戳:{0}\n本次发送耗时长:{1}", e.DistributedOn.ToString(), e.TimeSpan.ToString()));
			Console.WriteLine(string.Format(
				"累计发送{0}条，成功{1}条，失败{2}条",
				dispatcher.SucessCount + dispatcher.FailCount,
				dispatcher.SucessCount,
				dispatcher.FailCount));
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write("----------------------------------");
			Console.Write("发送结束");
			Console.Write("----------------------------------");
			Console.ForegroundColor = ConsoleColor.Gray;
		}

		private static void sender_Sleepping(object sender, SleepingEventArgs e) {
			Console.WriteLine();
			Console.WriteLine(string.Format("没有待上传数据！休眠{0}秒...", (e.SleepTimeSpan / 1000).ToString()));
			Console.ForegroundColor = ConsoleColor.Gray;
		}

		private static void sender_Error(object sender, ExceptionEventArgs e) {
			if (e.Exception != null) {
				Console.WriteLine();
				Console.WriteLine(e.Exception.Message);
				if (!e.ExceptionHandled) {
					LoggingService.Error(e.Exception);
				}
			}
		}
		#endregion
	}
}

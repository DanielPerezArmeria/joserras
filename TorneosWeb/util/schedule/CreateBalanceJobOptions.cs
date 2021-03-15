using CronScheduler.Extensions.Scheduler;
using System;

namespace TorneosWeb.util.schedule
{
	public class CreateBalanceJobOptions : SchedulerOptions
	{
		public CreateBalanceJobOptions()
		{
			CronSchedule = "0 0 7 ? * MON";
			CronTimeZone = "Central Standard Time (Mexico)";
		}

	}

}
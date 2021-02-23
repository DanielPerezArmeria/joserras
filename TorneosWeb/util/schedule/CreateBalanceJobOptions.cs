using CronScheduler.Extensions.Scheduler;

namespace TorneosWeb.util.schedule
{
	public class CreateBalanceJobOptions : SchedulerOptions
	{
		public CreateBalanceJobOptions()
		{
			CronSchedule = "0 0 7 ? * MON";
		}

	}

}
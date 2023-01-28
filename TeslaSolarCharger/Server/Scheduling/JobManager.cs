using Quartz;
using Quartz.Spi;
using TeslaSolarCharger.Server.Scheduling.Jobs;
using TeslaSolarCharger.Shared.Contracts;

namespace TeslaSolarCharger.Server.Scheduling;

public class JobManager
{
    private readonly ILogger<JobManager> _logger;
    private readonly IJobFactory _jobFactory;
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly IConfigurationWrapper _configurationWrapper;

    private IScheduler _scheduler;


#pragma warning disable CS8618
    public JobManager(ILogger<JobManager> logger, IJobFactory jobFactory, ISchedulerFactory schedulerFactory, IConfigurationWrapper configurationWrapper)
#pragma warning restore CS8618
    {
        _logger = logger;
        _jobFactory = jobFactory;
        _schedulerFactory = schedulerFactory;
        _configurationWrapper = configurationWrapper;
    }

    public async Task StartJobs()
    {
        _logger.LogTrace("{Method}()", nameof(StartJobs));
        _scheduler = _schedulerFactory.GetScheduler().GetAwaiter().GetResult();
        _scheduler.JobFactory = _jobFactory;

        var chargingValueJob = JobBuilder.Create<ChargingValueJob>().Build();
        var carStateCachingJob = JobBuilder.Create<ConfigJsonUpdateJob>().Build();
        var powerDistributionAddJob = JobBuilder.Create<PowerDistributionAddJob>().Build();
        var handledChargeFinalizingJob = JobBuilder.Create<HandledChargeFinalizingJob>().Build();
        var mqttReconnectionJob = JobBuilder.Create<MqttReconnectionJob>().Build();
        var newVersionCheckJob = JobBuilder.Create<NewVersionCheckJob>().Build();
        var spotPriceJob = JobBuilder.Create<SpotPriceJob>().Build();

        var chargingValueJobUpdateIntervall = _configurationWrapper.ChargingValueJobUpdateIntervall();

        var chargingValueTrigger =
            TriggerBuilder.Create().WithSchedule(SimpleScheduleBuilder.RepeatSecondlyForever((int)chargingValueJobUpdateIntervall.TotalSeconds)).Build();

        var carStateCachingTrigger = TriggerBuilder.Create()
            .WithSchedule(SimpleScheduleBuilder.RepeatMinutelyForever(3)).Build();

        var powerDistributionAddTrigger = TriggerBuilder.Create()
            .WithSchedule(SimpleScheduleBuilder.RepeatSecondlyForever(16)).Build();

        var handledChargeFinalizingTrigger = TriggerBuilder.Create()
            .WithSchedule(SimpleScheduleBuilder.RepeatMinutelyForever(9)).Build();

        var mqttReconnectionTrigger = TriggerBuilder.Create()
            .WithSchedule(SimpleScheduleBuilder.RepeatSecondlyForever(54)).Build();

        var newVersionCheckTrigger = TriggerBuilder.Create()
            .WithSchedule(SimpleScheduleBuilder.RepeatHourlyForever(47)).Build();

        var spotPricePlanningTrigger = TriggerBuilder.Create()
            .WithSchedule(SimpleScheduleBuilder.RepeatHourlyForever(1)).Build();

        var triggersAndJobs = new Dictionary<IJobDetail, IReadOnlyCollection<ITrigger>>
        {
            {chargingValueJob,  new HashSet<ITrigger> { chargingValueTrigger }},
            {carStateCachingJob, new HashSet<ITrigger> {carStateCachingTrigger}},
            {powerDistributionAddJob, new HashSet<ITrigger> {powerDistributionAddTrigger}},
            {handledChargeFinalizingJob, new HashSet<ITrigger> {handledChargeFinalizingTrigger}},
            {mqttReconnectionJob, new HashSet<ITrigger> {mqttReconnectionTrigger}},
            {newVersionCheckJob, new HashSet<ITrigger> {newVersionCheckTrigger}},
            {spotPriceJob, new HashSet<ITrigger> {spotPricePlanningTrigger}},
        };

        await _scheduler.ScheduleJobs(triggersAndJobs, false).ConfigureAwait(false);

        await _scheduler.Start().ConfigureAwait(false);
    }

    public async Task StopJobs()
    {
        await _scheduler.Shutdown(true).ConfigureAwait(false);
    }
}

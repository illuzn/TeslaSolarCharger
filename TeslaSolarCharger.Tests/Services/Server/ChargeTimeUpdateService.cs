﻿using System;
using TeslaSolarCharger.Shared.Contracts;
using TeslaSolarCharger.Shared.Dtos.Settings;
using Xunit;
using Xunit.Abstractions;

namespace TeslaSolarCharger.Tests.Services.Server;

public class ChargeTimeUpdateService : TestBase
{
    public ChargeTimeUpdateService(ITestOutputHelper outputHelper)
        : base(outputHelper)
    {
    }

    [Fact]
    public void DoesSetShouldStartTimesCorrectly()
    {
        var car = new Car();
        var chargeTimeUpdateService = Mock.Create<TeslaSolarCharger.Server.Services.ChargeTimeUpdateService>();
        var dateTime = new DateTime(2022, 12, 15, 10, 0, 0, DateTimeKind.Local);

        car.CarState.ShouldStopChargingSince = dateTime;
        car.CarState.EarliestSwitchOff = dateTime;

        Mock.Mock<IDateTimeProvider>().Setup(d => d.Now()).Returns(dateTime);
        var timeSpanUntilSwitchOn = TimeSpan.FromMinutes(5);
        Mock.Mock<IConfigurationWrapper>().Setup(c => c.TimespanUntilSwitchOn()).Returns(timeSpanUntilSwitchOn);
        chargeTimeUpdateService.SetEarliestSwitchOnToNowWhenNotAlreadySet(car);

        Assert.Equal(dateTime, car.CarState.ShouldStartChargingSince);
        Assert.Equal(dateTime + timeSpanUntilSwitchOn, car.CarState.EarliestSwitchOn);
    }

    [Fact]
    public void DoesSetShouldStopTimesCorrectly()
    {
        var car = new Car();
        var chargeTimeUpdateService = Mock.Create<TeslaSolarCharger.Server.Services.ChargeTimeUpdateService>();
        var dateTime = new DateTime(2022, 12, 15, 10, 0, 0, DateTimeKind.Local);

        car.CarState.ShouldStartChargingSince = dateTime;
        car.CarState.EarliestSwitchOn = dateTime;

        Mock.Mock<IDateTimeProvider>().Setup(d => d.Now()).Returns(dateTime);
        var timeSpanUntilSwitchOn = TimeSpan.FromMinutes(5);
        Mock.Mock<IConfigurationWrapper>().Setup(c => c.TimespanUntilSwitchOff()).Returns(timeSpanUntilSwitchOn);
        chargeTimeUpdateService.SetEarliestSwitchOffToNowWhenNotAlreadySet(car);

        Assert.Equal(dateTime, car.CarState.ShouldStopChargingSince);
        Assert.Null(car.CarState.ShouldStartChargingSince);
        Assert.Equal(dateTime + timeSpanUntilSwitchOn, car.CarState.EarliestSwitchOff);
        Assert.Null(car.CarState.EarliestSwitchOn);
    }
}

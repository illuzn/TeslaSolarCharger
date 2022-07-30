﻿using TeslaSolarCharger.Shared.Dtos.BaseConfiguration;

namespace TeslaSolarCharger.Shared.Contracts;

public interface IConfigurationWrapper
{
    string CarConfigFileFullName();
    TimeSpan ChargingValueJobUpdateIntervall();
    TimeSpan PvValueJobUpdateIntervall();
    string MqqtClientId();
    string MosquitoServer();
    string? CurrentPowerToGridUrl();
    string? CurrentInverterPowerUrl();
    string? CurrentPowerToGridJsonPattern();
    decimal CurrentPowerToGridCorrectionFactor();
    string TeslaMateApiBaseUrl();
    List<int> CarPriorities();
    string GeoFence();
    TimeSpan TimespanUntilSwitchOn();
    TimeSpan TimespanUntilSwitchOff();
    int PowerBuffer();
    string? TelegramBotKey();
    string? TelegramChannelId();
    string? CurrentInverterPowerJsonPattern();
    string? CurrentPowerToGridXmlPattern();
    string? CurrentInverterPowerXmlPattern();
    string? CurrentPowerToGridXmlAttributeHeaderName();
    string? CurrentPowerToGridXmlAttributeHeaderValue();
    string? CurrentPowerToGridXmlAttributeValueName();
    string? CurrentInverterPowerXmlAttributeHeaderName();
    string? CurrentInverterPowerXmlAttributeHeaderValue();
    string? CurrentInverterPowerXmlAttributeValueName();
    string TeslaMateDbServer();
    int TeslaMateDbPort();
    string TeslaMateDbDatabaseName();
    string TeslaMateDbUser();
    string TeslaMateDbPassword();
    string BaseConfigFileFullName();

    Task<DtoBaseConfiguration> GetBaseConfigurationAsync();
    Task SaveBaseConfiguration(DtoBaseConfiguration baseConfiguration);
    Task<bool> IsBaseConfigurationJsonRelevant();
    Task UpdateBaseConfigurationAsync(DtoBaseConfiguration dtoBaseConfiguration);
    Dictionary<string, string> CurrentPowerToGridHeaders();
    Dictionary<string, string> CurrentInverterPowerHeaders();
    decimal CurrentInverterPowerCorrectionFactor();
    string ConfigFileDirectory();

    /// <summary>
    /// Baseconfig file name (excluding path)
    /// </summary>
    /// <returns></returns>
    string GetBaseConfigFileName();
}
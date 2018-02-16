namespace Boondocks.Agent.Interfaces
{
    using Services.Contracts.Interfaces;

    internal interface IDeviceConfigurationProvider
    {
        IDeviceConfiguration GetDeviceConfiguration();
    }
}
namespace Boondocks.Agent.Interfaces
{
    using Services.Contracts.Interfaces;

    internal interface IDeviceConfigurationProvider
    {
        bool Exists();

        IDeviceConfiguration GetDeviceConfiguration();
    }
}
namespace Boondocks.Agent.Model
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Domain;
    using Newtonsoft.Json;
    using Services.Device.Contracts;

    internal class OperationalStateProvider
    {
        private readonly PathFactory _pathFactory;

        public OperationalStateProvider(PathFactory pathFactory)
        {
            _pathFactory = pathFactory ?? throw new ArgumentNullException(nameof(pathFactory));

            try
            {
                if (File.Exists(pathFactory.OperationStatePath))
                {
                    //Grab the json from disk
                    var json = File.ReadAllText(pathFactory.OperationStatePath);

                    //Deserialize it.
                    State = JsonConvert.DeserializeObject<DeviceOperationalState>(json);
                }
            }
            catch (Exception)
            {
                //TODO: Log this
            }

            if (State == null) State = new DeviceOperationalState();
        }

        public DeviceOperationalState State { get; }

        public void Save()
        {
            //Make sure the directory exists.
            Directory.CreateDirectory(_pathFactory.AgentStatusDirectory);

            //Serial the state
            var json = JsonConvert.SerializeObject(State, Formatting.Indented);

            //Write it out
            File.WriteAllText(_pathFactory.OperationStatePath, json);
        }
    }
}
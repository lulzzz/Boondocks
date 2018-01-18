using System;
using System.IO;
using Newtonsoft.Json;

namespace Boondocks.Agent.Model
{
    public class OperationalStateProvider
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
                    string json = File.ReadAllText(pathFactory.OperationStatePath);

                    //Deserialize it.
                    State = JsonConvert.DeserializeObject<DeviceOperationalState>(json);
                }
            }
            catch (Exception)
            {
                //TODO: Log this
            }

            if (State == null)
            {
                //Create a default state.
                State = new DeviceOperationalState();
            }
        }

        public void Save()
        {
            //Make sure the directory exists.
            Directory.CreateDirectory(_pathFactory.SupervisorStatusDirectory);

            //Serial the state
            var json = JsonConvert.SerializeObject(State, Formatting.Indented);

            //Write it out
            File.WriteAllText(_pathFactory.OperationStatePath, json);
        }

        public DeviceOperationalState State { get; }
    }
}
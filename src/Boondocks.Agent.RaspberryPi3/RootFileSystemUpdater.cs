namespace Boondocks.Agent.RaspberryPi3
{
    using System;
    using System.IO;
    using Serilog;

    public class RootFileSystemUpdater
    {
        private readonly ILogger _logger;
        private const string PartitionA = "/dev/mmcblk0p2";
        private const string PartitionB = "/dev/mmcblk0p3";

        private const string CommandLinePath = "/mnt/boot/cmdline.txt";

        private const string ImageVersionInfoPath = "/mnt/boot/image-version-info";

        //private static readonly string[] Partitions = new string[]
        //{
        //    PartitionA,
        //    PartitionB
        //};

        public RootFileSystemUpdater(ILogger logger)
        {
            _logger = logger.ForContext(GetType());
        }

        public int? GetCurrentPartition()
        {
            if (!File.Exists(CommandLinePath))
            {
                _logger.Error("Unable to find cmdline.txt at {CmdLinePath}", CommandLinePath);
                return null;
            }

            string commandLineContents = File.ReadAllText(CommandLinePath);

            if (string.IsNullOrWhiteSpace(commandLineContents))
            {
                _logger.Error("The cmdline.txt file had no contents.");
                return null;
            }

            if (commandLineContents.Contains(PartitionA))
                return 0;

            if (commandLineContents.Contains(PartitionB))
                return 1;

            _logger.Error("Unable to determine which root file system partition is in use.");
            
            return null;
        }

        /// <summary>
        /// e.g. "resin-image-raspberrypi3-20180123113228"
        /// </summary>
        /// <returns></returns>
        public string GetImageVersionInfo()
        {
            if (!File.Exists(ImageVersionInfoPath))
            {
                _logger.Error("Unable to find image-version-info at {ImgVerInfoPath}.", ImageVersionInfoPath);
                return null;
            }

            return File.ReadAllText(ImageVersionInfoPath);
        }

        public void Update()
        {
            //Figure out what partition we're using

            //download docker image

            //Export the docker image

            //extract the 'image.tar' file

            //Delete the export file

            //extract the 'root-file-system.img' file.

            //Delete the 'image.tar' file

            //dd the contents of 'root-file-system.img' to the other partition.

            //check the contents of that partition to make sure it's correct

            //update cmdline.txt to point to the correct partition

            //remove the docker image

            //reboot the host
        }
    }
}
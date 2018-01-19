#===========================================================
# We'll use this to build the .NET Core code.
#===========================================================
FROM microsoft/dotnet AS corebuildstep

#copy the code to the image
COPY ../source/* ./build

#publish the bloody agent
RUN dotnet publish -r linux-arm -f netcoreapp2.0 /build/Boondocks.Agent/Boondocks.Agent.csproj

#===========================================================
# Now start making the image that will run on the pi
#===========================================================
FROM resin/raspberrypi3-debian

RUN [ "cross-build-start" ]

# Install the packages necessary for .NET Core
RUN apt-get -y install libunwind8 gettext wget

# Download the nightly binaries for .NET Core 2
RUN wget https://dotnetcli.blob.core.windows.net/dotnet/Runtime/release/2.0.0/dotnet-runtime-latest-linux-arm.tar.gz

# Create a folder to hold the .NET Core 2 installation
RUN mkdir /opt/dotnet

# Unzip the dotnet zip into the dotnet installation folder
RUN tar -xvf dotnet-runtime-latest-linux-arm.tar.gz -C /opt/dotnet

# set up a symbolic link to a directory on the path so we can call dotnet
RUN ln -s /opt/dotnet/dotnet /usr/local/bin

#copy the built files over to the device.
COPY --from=corebuildstep /build/Boondocks.Agent/bin/linux-arm/Release* /opt/boondocks/

#start up the application
CMD ["dotnet", "/opt/boondocks/Boondocks.Agent.dll"]

RUN [ "cross-build-end" ]  
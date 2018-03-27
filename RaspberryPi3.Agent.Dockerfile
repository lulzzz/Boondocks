#===========================================================
# We'll use this to build the .NET Core code.
#===========================================================
FROM microsoft/dotnet:sdk AS core-build-step

#copy the code to the image
COPY ./src/ /build/

#move to the build directory
WORKDIR /build/

#restore and publish the bloody agent
RUN dotnet restore -s https://api.nuget.org/v3/index.json \
	&& dotnet publish -r linux-arm -f netcoreapp2.0 /build/Boondocks.Agent.RaspberryPi3/Boondocks.Agent.RaspberryPi3.csproj

#===========================================================
# Now start making the image that will run on the pi
#===========================================================
FROM boondocks/resin-dotnet:resin-raspberrypi3-dotnet206

RUN [ "cross-build-start" ]

#copy the built files over to the device.
COPY --from=core-build-step /build/Boondocks.Agent.RaspberryPi3/bin/Debug/netcoreapp2.0/linux-arm/publish/ /opt/boondocks/

#copy the entry file
COPY ./src/Boondocks.Agent.Base/scripts/entry.sh /usr/bin/entry.sh

#mark this as executable
RUN chmod +x /usr/bin/entry.sh

ENTRYPOINT [ "/usr/bin/entry.sh" ]

RUN [ "cross-build-end" ]  

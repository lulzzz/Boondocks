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
FROM resin/raspberrypi3-debian

RUN [ "cross-build-start" ]

#update it! & install the packages necessary for .NET Core
RUN apt-get update && apt-get -y install libunwind8 gettext wget

# Install .NET Core
ENV DOTNET_VERSION 2.0.5
ENV DOTNET_DOWNLOAD_URL https://dotnetcli.blob.core.windows.net/dotnet/Runtime/$DOTNET_VERSION/dotnet-runtime-$DOTNET_VERSION-linux-arm.tar.gz
ENV DOTNET_DOWNLOAD_SHA 73F66386D844CBEEF2AE55AE4DA9C3701E27FA18F1FC335A5E9CAF50D239938088F223B46114776A52182CF457A4C68318E5CF6A17CC4EABC7BFF02353AFEF7E

RUN curl -SL $DOTNET_DOWNLOAD_URL --output dotnet.tar.gz \
    && echo "$DOTNET_DOWNLOAD_SHA dotnet.tar.gz" | sha512sum -c - \
    && mkdir -p /usr/share/dotnet \
    && tar -zxf dotnet.tar.gz -C /usr/share/dotnet \
    && rm dotnet.tar.gz \
    && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet
	
#copy the built files over to the device.
COPY --from=core-build-step /build/Boondocks.Agent.RaspberryPi3/bin/Debug/netcoreapp2.0/linux-arm/publish/ /opt/boondocks/

#copy the entry file
COPY ./src/Boondocks.Agent/scripts/entry.sh /usr/bin/entry.sh

#start up the application
#CMD ["dotnet", "/opt/boondocks/Boondocks.Agent.dll"]

RUN [ "cross-build-end" ]  

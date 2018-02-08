#===========================================================
# We'll use this to build the .NET Core code.
#===========================================================
FROM microsoft/dotnet:sdk AS core-build-step

#copy the code to the image
COPY ./source/ /build/

#move to the build directory
WORKDIR /build/

#restore and publish the bloody agent
RUN dotnet restore -s https://api.nuget.org/v3/index.json \
	&& dotnet publish -r linux-arm -f netcoreapp2.0 /build/Boondocks.Agent/Boondocks.Agent.csproj

#===========================================================
# Now start making the image that will run on the pi
#===========================================================
FROM resin/raspberrypi3-debian

RUN [ "cross-build-start" ]

#update it! & install the packages necessary for .NET Core
RUN apt-get update && apt-get -y install libunwind8 gettext wget

# Download the nightly binaries for .NET Core 2
# Create a folder to hold the .NET Core 2 installation
# Unzip the dotnet zip into the dotnet installation folder
# set up a symbolic link to a directory on the path so we can call dotnet
#RUN wget https://dotnetcli.blob.core.windows.net/dotnet/Runtime/release/2.0.0/dotnet-runtime-latest-linux-arm.tar.gz \ 
#	&& mkdir /opt/dotnet \ 
#	&& tar -xvf dotnet-runtime-latest-linux-arm.tar.gz -C /opt/dotnet \
#	&& ln -s /opt/dotnet/dotnet /usr/local/bin

# Install .NET Core
ENV DOTNET_VERSION 2.0.5
ENV DOTNET_DOWNLOAD_URL https://dotnetcli.blob.core.windows.net/dotnet/Runtime/$DOTNET_VERSION/dotnet-runtime-$DOTNET_VERSION-linux-x64.tar.gz
ENV DOTNET_DOWNLOAD_SHA 21D54E559C5130BB3F8C38EADACB7833EC90943F71C4E9C8FA2D53192313505311230B96F1AFEB52D74D181D49CE736B83521754E55F15D96A8756921783CD33

RUN curl -SL $DOTNET_DOWNLOAD_URL --output dotnet.tar.gz \
    && echo "$DOTNET_DOWNLOAD_SHA dotnet.tar.gz" | sha512sum -c - \
    && mkdir -p /usr/share/dotnet \
    && tar -zxf dotnet.tar.gz -C /usr/share/dotnet \
    && rm dotnet.tar.gz \
    && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet

#copy the built files over to the device.
COPY --from=core-build-step /build/Boondocks.Agent/bin/Debug/netcoreapp2.0/linux-arm/publish/ /opt/boondocks/

#start up the application
CMD ["dotnet", "/opt/boondocks/Boondocks.Agent.dll"]

RUN [ "cross-build-end" ]  

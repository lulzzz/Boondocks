#===========================================================
# BUILD: Using Docker images containing .net core SDK
#===========================================================
FROM microsoft/dotnet:2.0-sdk AS core-build-step

# Copy the code to the image
COPY ./src/Boondocks.Base/ /build/src/Boondocks.Base
COPY ./src/Boondocks.Device/Components /build/src/Boondocks.Device/Components
COPY ./src/Boondocks.Device/Boondocks.Device.WebApi /build/src/Boondocks.Device/Boondocks.Device.WebApi

# Build and Publish the WebApi service host
WORKDIR /build/src/Boondocks.Device

RUN dotnet publish \
    --configuration Debug \
    --framework netcoreapp2.0 \
    --source https://api.nuget.org/v3/index.json \
    /build/src/Boondocks.Device/Boondocks.Device.WebApi/Boondocks.Device.WebApi.csproj 

#===========================================================
# CREATE: Image using non-sdk verison of .net base image
#===========================================================
FROM microsoft/aspnetcore:2.0
	
# Copy the built files over to the device.
COPY --from=core-build-step /build/src/Boondocks.Device/Boondocks.Device.WebApi/bin/Debug/netcoreapp2.0/publish/ /opt/boondocks/device-api

# Expose the web port
EXPOSE 80

WORKDIR /opt/boondocks/device-api

CMD ["dotnet", "Boondocks.Device.WebApi.dll"]

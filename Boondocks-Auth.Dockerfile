#===========================================================
# BUILD: Using Docker images containing .net core SDK
#===========================================================
FROM microsoft/dotnet:2.0-sdk AS core-build-step

# Copy the code to the image
COPY ./src/Boondocks.Base/ /build/src/Boondocks.Base
COPY ./src/Boondocks.Auth/Components /build/src/Boondocks.Auth/Components
COPY ./src/Boondocks.Auth/Boondocks.Auth.WebApi /build/src/Boondocks.Auth/Boondocks.Auth.WebApi

# Build and Publish the WebApi service host
WORKDIR /build/src/Boondocks.Auth

RUN dotnet publish \
    --configuration Debug \
    --framework netcoreapp2.0 \
    --source https://api.nuget.org/v3/index.json \
    /build/src/Boondocks.Auth/Boondocks.Auth.WebApi/Boondocks.Auth.WebApi.csproj 

#===========================================================
# CREATE: Image using non-sdk verison of .net base image
#===========================================================
FROM microsoft/aspnetcore:2.0
	
# Copy the built files over to the device.
COPY --from=core-build-step /build/src/Boondocks.Auth/Boondocks.Auth.WebApi/bin/Debug/netcoreapp2.0/publish/ /opt/boondocks/auth-api

# Expose the web port
EXPOSE 80

WORKDIR /opt/boondocks/auth-api

CMD ["dotnet", "Boondocks.Auth.WebApi.dll"]


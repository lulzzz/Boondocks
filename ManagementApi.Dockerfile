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
	&& dotnet publish -r ubuntu.16.04-x64 -f netcoreapp2.0 /build/Boondocks.Services.Management.WebApi/Boondocks.Services.Management.WebApi.csproj

#===========================================================
# Now start making the actual image
#===========================================================
FROM microsoft/dotnet:runtime
	
#copy the built files over to the device.
COPY --from=core-build-step /build/Boondocks.Services.Management.WebApi/bin/Debug/netcoreapp2.0/ubuntu.16.04-x64/publish/ /opt/boondocks/

#Expose the web port
EXPOSE 80

#move to our application folder
WORKDIR /opt/boondocks/

#start up the application
CMD ["dotnet", "Boondocks.Services.Management.WebApi.dll"]
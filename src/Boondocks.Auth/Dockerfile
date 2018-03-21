FROM microsoft/aspnetcore

COPY dist /app
COPY certs/auth-private.pfx /app/certs/auth-private.pfx

WORKDIR /app

EXPOSE 80/tcp
ENTRYPOINT ["dotnet", "Boondocks.Auth.WebApi.dll"]

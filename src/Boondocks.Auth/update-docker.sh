# Delete the current image and container:

docker rm boondocks.auth --force
docker rmi boondocks/auth --force

# Publish the dotnet core application so the binaries can be
# copied to the image:
rm dist -rf
dotnet publish ./src/Boondocks.Auth.WebApi/Boondocks.Auth.WebApi.csproj --output ../../dist

# Build the container:
docker build . -t boondocks/auth -f Dockerfile

# Create a container from the image with the needed environment variables:
docker create -p 5050:80 \
	-e my:config='Docker Config' \
	-e ASPNETCORE_ENVIRONMENT='Development' \
	-e boondocks__auth__jwt__issuer='http://localhost' \
	-e boondocks__auth__jwt__audience='http://localhost' \
	-e boondocks__auth__jwt__validForMinutes='30' \
	-e boondocks__auth__jwt__certificateFilePath='certs/auth-private.pfx' \
	-e boondocks__auth__ldap__DomainName='CAPTIVE_AIRE' \
	-e boondocks__auth__ldap__Connection='2008dc1.captiveaire.com' \
	-e boondocks__auth__ldap__Port='389' \
	--name boondocks.auth boondocks/auth \
	--mount type=bind,source="$(pwd)"/certs,target=/certs \


docker start boondocks.auth

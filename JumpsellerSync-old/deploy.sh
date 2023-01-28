#!/bin/bash

sudo service js-main stop
sudo service js-linkstore stop
sudo service js-intcomex stop
sudo service js-tecnoglobal stop
sudo service js-nexsys stop

dotnet publish --configuration Release -o /var/www/JumpsellerSync/Main/ \
	./JumpsellerSync.RestApi.Frontend/JumpsellerSync.RestApi.FrontEnd.csproj

dotnet publish --configuration Release -o /var/www/JumpsellerSync/Linkstore/ \
        ./JumpsellerSync.RestApi.Provider.Linkstore/JumpsellerSync.RestApi.Provider.Linkstore.csproj

dotnet publish --configuration Release -o /var/www/JumpsellerSync/Intcomex/ \
        ./JumpsellerSync.RestApi.Provider.Intcomex/JumpsellerSync.RestApi.Provider.Intcomex.csproj

dotnet publish --configuration Release -o /var/www/JumpsellerSync/Tecnoglobal/ \
        ./JumpsellerSync.RestApi.Provider.Tecnoglobal/JumpsellerSync.RestApi.Provider.Tecnoglobal.csproj

dotnet publish --configuration Release -o /var/www/JumpsellerSync/Nexsys/ \
        ./JumpsellerSync.RestApi.Provider.Nexsys/JumpsellerSync.RestApi.Provider.Nexsys.csproj

sudo service js-main start
sudo service js-linkstore start
sudo service js-intcomex start
sudo service js-tecnoglobal start
sudo service js-nexsys start

FROM mcr.microsoft.com/dotnet/aspnet:6.0
COPY Apsitvarkom.Api/bin/Release/net6.0/publish/ App/
WORKDIR /App
EXPOSE 80
ENTRYPOINT ["dotnet", "Apsitvarkom.Api.dll"]
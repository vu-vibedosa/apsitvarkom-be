FROM mcr.microsoft.com/dotnet/aspnet:7.0
COPY Apsitvarkom.Api/bin/Release/net7.0/publish/ App/
WORKDIR /App
EXPOSE 80
ENTRYPOINT ["dotnet", "Apsitvarkom.Api.dll"]
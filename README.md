# apsitvarkom-be

Apsitvarkom back end dotnet web API

## Dependencies

- PostgreSQL (expects `ConnectionStrings:ApsitvarkomDatabase` in configuration)
- Google Maps [Geocoding API](https://developers.google.com/maps/documentation/geocoding/overview) (expects `Geocoding:ApiKey` in configuration).

## Running locally

1. Run `docker-compose -f dev-stack.yml up` (requires [Docker](https://www.docker.com/))
   1. This starts [PostgreSQL](https://www.postgresql.org/) on port `5432`. Database data will _be saved_ if you shut down the stack.
   2. This starts [Adminer](https://www.adminer.org/) on port `8080`. Visit [`http://localhost:8080`](http://localhost:8080) and login in with `username: postgres` and `password: devpassword`.
2. Run `dotnet user-secrets set "Geocoding:ApiKey" "<OUR_API_KEY>"`
   1. Replace `<OUR_API_KEY>` with the development api key from our google sheet yourself and keep it as a secret.
   2. This has to be done only once
3. Start the web API (either `dotnet run` or by clicking "Play" in Visual Studio)
   1. Visit swagger at http://localhost:5125/swagger

## CICD

- Every PR to `staging` branch runs CI automatically that builds, tests and reports coverage in the PR.
- Every merge/push to `staging` branch runs CICD that automatically builds, tests, builds docker image, pushes it to an [Artifact Registry](https://cloud.google.com/artifact-registry) and then deploys it in [Cloud Run](https://cloud.google.com/run). See "Environments" on GitHub to access `staging` environment url.

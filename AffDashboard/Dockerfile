# Étape 1 : Build l'application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copie les fichiers solution et projet
COPY *.sln .
COPY AffDashboard/*.csproj ./AffDashboard/

# Restaure les dépendances
RUN dotnet restore

# Copie tout le reste du projet
COPY . .

# Publie l'application en Release
RUN dotnet publish -c Release -o /app/out

# Étape 2 : Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copie depuis l'étape précédente
COPY --from=build /app/out .

# Lance l'application
ENTRYPOINT ["dotnet", "AffDashboard.dll"]

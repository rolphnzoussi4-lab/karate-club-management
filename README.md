# Karate Club Management

Application ASP.NET Core MVC pour la gestion d'un club de karaté avec Entity Framework Core et SQL Server.

## Fonctionnalités
- Gestion des membres, entraîneurs, cours, examens et paiements
- Authentification avec rôles Administrateur, Entraîneur et Secrétaire
- Tableau de bord avec statistiques de membres, grades, revenus et événements
- Gestion des présences et des résultats d'examens
- Génération de reçus PDF
- CRUD complet pour toutes les entités
- Bootstrap 5 pour l'interface utilisateur
- Tests unitaires avec xUnit

## Architecture
- Presentation : `src/KarateClubManagement.Web`
- Business : `src/KarateClubManagement.Business`
- Data Access : `src/KarateClubManagement.Data`
- Tests : `src/KarateClubManagement.Tests`

## Prérequis
- .NET 8 SDK
- Docker Desktop (recommandé sur macOS / Linux)
- SQL Server 2022 via Docker ou un serveur SQL accessible
- Git

## Installation rapide sur macOS / Linux
1. Démarrer SQL Server dans Docker :
   ```bash
   docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrong!Passw0rd" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
   ```
2. Vérifier que le conteneur tourne :
   ```bash
   docker ps
   ```
3. Restaurer les dépendances :
   ```bash
   dotnet restore
   ```
4. Démarrer l'application sur HTTP :
   ```bash
   dotnet run --project src/KarateClubManagement.Web/KarateClubManagement.Web.csproj --urls http://localhost:5173
   ```
5. Ouvrir dans Safari :
   `http://localhost:5173`

## Configuration SQL Server
Le projet utilise par défaut un serveur SQL accessible sur :
`Server=localhost,1433;Database=KarateClubDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;MultipleActiveResultSets=true`

## Comptes d'accès par défaut
- Admin : `admin@karateclub.com` / `Admin123!`
- Sécrétaire : `secretary@karateclub.com` / `Secret123!`
- Entraîneur : `coach@karateclub.com` / `Coach123!`

## Base de données
Le projet utilise `EnsureCreated()` au démarrage pour créer la base de données si elle n'existe pas.
Les scripts SQL sont disponibles dans le dossier `database/`.

## Déploiement GitHub
Le dépôt est prêt pour publication et continuation du développement.

## Licence
MIT

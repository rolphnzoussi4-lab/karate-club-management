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
- SQL Server 2019+ ou SQL Server Express
- Git
- Visual Studio 2022 / VS Code / Rider

## Installation
1. Cloner le dépôt
2. Créer la base de données SQL Server
3. Configurer la chaîne de connexion dans `src/KarateClubManagement.Web/appsettings.json`
4. Restaurer les dépendances :
   ```bash
   dotnet restore
   ```
5. Lancer l'application :
   ```bash
   dotnet run --project src/KarateClubManagement.Web/KarateClubManagement.Web.csproj
   ```

## Base de données
Le projet utilise `EnsureCreated()` au démarrage pour créer la base de données si elle n'existe pas.
Les scripts SQL sont disponibles dans le dossier `database/`.

## Comptes d'accès par défaut
- Admin : `admin@karateclub.com` / `Admin123!`
- Sécrétaire : `secretary@karateclub.com` / `Secret123!`
- Entraîneur : `coach@karateclub.com` / `Coach123!`

## Déploiement GitHub
Le dépôt est préparé pour être publié sur GitHub avec un commit initial et la structure du projet prête à l'emploi.

## Documentation technique
- `database/create_database.sql`
- `src/KarateClubManagement.Web/Program.cs`
- `src/KarateClubManagement.Data/KarateClubDbContext.cs`
- `src/KarateClubManagement.Business/Services/`

## Licence
MIT

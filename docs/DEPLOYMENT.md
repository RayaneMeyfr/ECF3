# Guide de déploiement – BookHub

## Prérequis

- .NET 8 SDK
- Docker Desktop
- Visual Studio 2022 / VS Code / Rider
- Git

## Étapes de déploiement

### 1. Cloner le projet
```bash
git clone <url-du-depot>
cd BookHub
```

### 2. Démarrer les conteneurs Docker
```bash
# Lancer tous les services en arrière-plan
docker-compose up -d

# Vérifier le statut des conteneurs
docker-compose ps
```
> Cette étape crée et démarre les conteneurs pour : PostgreSQL, CatalogService, UserService, LoanService, API Gateway et le frontend BlazorClient.

### 3. Créer les migrations et mettre à jour la base de données
Pour chaque microservice, naviguer dans le répertoire du service et exécuter :
```bash
# CatalogService
cd src/Services/BookHub.CatalogService
dotnet ef migrations add InitialCreate
dotnet ef database update

# UserService
cd ../BookHub.UserService
dotnet ef migrations add InitialCreate
dotnet ef database update

# LoanService
cd ../BookHub.LoanService
dotnet ef migrations add InitialCreate
dotnet ef database update
```
> Assurez-vous que la chaîne de connexion PostgreSQL dans `appsettings.json` est correcte.

### 4. Lancer les microservices et le frontend
```bash
docker-compose up -d
```
- Frontend Blazor : [http://localhost:8080](http://localhost:8080)  
- API Gateway : [http://localhost:5000](http://localhost:5000)

### 5. Utilisateurs de test
| Email | Mot de passe | Rôle |
|-------|--------------|------|
| admin@bookhub.com | Admin123! | Admin |
| librarian@bookhub.com | Librarian123! | Bibliothécaire |
| user@bookhub.com | User123! | Membre |

### 6. Arrêter les services
```bash
docker-compose down
```

### 7. Réinitialiser les volumes (DB) si nécessaire
```bash
docker volume prune -f
```
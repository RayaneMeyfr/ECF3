# Architecture – BookHub

## 1. Vue d’ensemble

BookHub est une plateforme de gestion de bibliothèque numérique basée sur une architecture microservices.  
Elle est composée de plusieurs services indépendants développés en .NET 8, ainsi que d’une interface utilisateur en Blazor WebAssembly.

L’ensemble des communications entre le frontend et les microservices passe par une API Gateway, qui agit comme point d’entrée unique du système.

### Composants principaux :
- BlazorClient (Frontend)
- API Gateway
- CatalogService
- UserService
- LoanService
- Base de données PostgreSQL

---

## 2. Architecture globale (Microservices)

Chaque microservice est responsable d’un domaine fonctionnel précis :

| Service | Responsabilité |
|--------|----------------|
| CatalogService | Gestion du catalogue de livres |
| UserService | Gestion des utilisateurs et authentification |
| LoanService | Gestion des emprunts |
| API Gateway | Routage des requêtes et point d’entrée unique |
| BlazorClient | Interface utilisateur |
| PostgreSQL | Stockage des données |

### Flux principaux :
1. L’utilisateur interagit avec l’application Blazor WebAssembly.
2. Le frontend envoie ses requêtes à l’API Gateway.
3. L’API Gateway redirige les requêtes vers le microservice concerné.
4. Chaque microservice accède à la base de données PostgreSQL via Entity Framework Core.

---

## 3. Architecture Hexagonale (Ports & Adapters)

Chaque microservice est structuré selon le principe de l’architecture hexagonale (Ports & Adapters).

Cette architecture vise à :
- Isoler le cœur métier des dépendances techniques
- Faciliter les tests unitaires
- Améliorer la maintenabilité et l’évolutivité du code

### Structure d’un microservice :

#### BookHub.CatalogService :
```text
Service/
├── Domain/
│   ├── Entities/
│   └── Ports/
├── Application/
│   └── Services/
├── Infrastructure/
│   └── Persistence/
│       └── Repositories/
└── Api/
    └── Controllers/
```

#### BookHub.UserService :
```text
Service/
├── Domain/
│   ├── Entities/
│   └── Ports/
├── Application/
│   └── Services/
├── Infrastructure/
│   └── Persistence/
│       └── Repositories/
└── Api/
    └── Controllers/
```

#### BookHub.LoanService :
```text
Service/
├── Domain/
│   ├── Entities/
│   └── Ports/
├── Application/
│   └── Services/
├── Infrastructure/
│   ├── HttpClients/
│   ├── Persistence/
│   │   └── Repositories/
│   └── Security/
└── Api/
    └── Controllers/
```
---

## 4. Rôle des différentes couches

### Domain
- Contient la logique métier pure
- Ne dépend d’aucune technologie
- Définit les entités et les interfaces (Ports)

### Application
- Implémente les cas d’utilisation
- Orchestre les appels entre le domaine et l’infrastructure
- Ne contient pas de logique technique

### Infrastructure
- Implémente les ports définis dans le domaine
- Contient les accès à la base de données (EF Core)
- Contient les clients HTTP et la sécurité (JWT)

### API
- Point d’entrée HTTP du microservice
- Expose les endpoints REST
- Convertit les requêtes HTTP en appels applicatifs

---

## 5. Technologies utilisées

| Composant | Technologie |
|----------|-------------|
| Backend | .NET 8 – ASP.NET Core Web API |
| Frontend | Blazor WebAssembly |
| Base de données | PostgreSQL |
| ORM | Entity Framework Core 8 |
| Authentification | JWT |
| Conteneurisation | Docker, Docker Compose |
| Tests | xUnit, Moq, FluentAssertions |

---

## 6. Sécurité

L’authentification est basée sur des tokens JWT.  
Le UserService est responsable de l’émission des tokens après authentification.

Les autres services vérifient les tokens transmis par l’API Gateway afin de contrôler l’accès aux ressources en fonction des rôles (Admin, Bibliothécaire, Membre).

---
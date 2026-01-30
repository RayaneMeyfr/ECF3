# API Reference – BookHub

Ce document liste les endpoints REST disponibles pour chaque microservice de la plateforme BookHub.

---

## Catalog Service

Endpoints pour la gestion du catalogue de livres.

| Méthode | Endpoint | Rôle | Description |
|---------|---------|------|------------|
| GET | `/api/books` | Tous | Liste tous les livres |
| GET | `/api/books/{id}` | Tous | Détails d’un livre par ID |
| GET | `/api/books/search?term=` | Tous | Recherche de livres par mot-clé |
| POST | `/api/books` | Admin | Créer un nouveau livre |
| PUT | `/api/books/{id}` | Admin | Modifier un livre existant |
| DELETE | `/api/books/{id}` | Admin | Supprimer un livre |

---

## User Service

Endpoints pour la gestion des utilisateurs et authentification.

| Méthode | Endpoint | Rôle | Description |
|---------|---------|------|------------|
| POST | `/api/users/register` | Tous | Inscription d’un nouvel utilisateur |
| POST | `/api/users/login` | Tous | Connexion et obtention d’un token JWT |
| GET | `/api/users/{id}` | Tous | Récupérer le profil d’un utilisateur |
| GET | `/api/users` | Admin | Liste tous les utilisateurs |

---

## Loan Service

Endpoints pour la gestion des emprunts de livres.

| Méthode | Endpoint | Rôle | Description |
|---------|---------|------|------------|
| POST | `/api/loans` | Tous | Créer un nouvel emprunt |
| GET | `/api/loans/{id}` | Tous | Détails d’un emprunt |
| GET | `/api/loans/user/{userId}` | Tous | Liste des emprunts d’un utilisateur |
| PUT | `/api/loans/{id}/return` | Tous | Retour d’un livre |
| GET | `/api/loans/overdue` | Admin / Bibliothécaire | Liste des emprunts en retard |

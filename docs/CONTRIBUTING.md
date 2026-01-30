# Contribution au projet

## Documentation ajoutée
- **ARCHITECTURE.md** : description complète de l'architecture du projet.
- **API_REFERENCE.md** : documentation de toutes les routes API.
- **DEPLOYMENT.md** : guide pour déployer le projet.

## LoanService : routes implémentées
- **Create** : création d'un nouvel emprunt avec vérification des conditions (nombre maximal, disponibilité).
- **Return** : rendre un emprunt et mettre à jour la disponibilité du livre.
- **Overdue** : lister tous les emprunts en retard.

## API Gateway
- Mise en place complète avec **Ocelot**.
- Configuration des routes vers les microservices.
- Sécurité et routage centralisés.

## Frontend
- Page **BookDetail** avec le composant **BookCard** pour afficher les détails d'un livre.
- Page **MyLoans** pour que l'utilisateur voie ses emprunts.
- Composant **LoanStatus** pour afficher l'état des emprunts (Active, Returned, Overdue) avec badges.
- Bouton pour **rendre un emprunt** depuis MyLoans, mise à jour du statut et de la disponibilité.
- Intégration du **LoanService** côté frontend pour gérer les appels API.

## Bilan
- Documentation complète ajoutée.
- API Gateway fonctionnelle.
- Routes manquantes de LoanService implémentées.
- Frontend enrichi avec gestion des emprunts et rendu.
- Quelques bugs mineurs restent à corriger.

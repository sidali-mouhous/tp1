# FTELMarketApi

API REST ASP.NET Core pour un marketplace de produits, avec interface web simple, gestion des catégories et persistance en base de données.

## Vue d’ensemble

Cette application permet de gérer un catalogue de produits avec :

- CRUD sur les produits
- CRUD sur les catégories
- base SQLite par défaut pour démarrer rapidement
- support PostgreSQL via Docker
- endpoint Swagger pour tester l’API
- interface front simple pour gérer le catalogue dans le navigateur

## Fonctionnalités

- Consultation, création, modification et suppression de produits
- Consultation, création, modification et suppression de catégories
- Relation 1-N entre catégories et produits
- Validation basique des entrées
- Persistance avec Entity Framework Core
- Données initiales chargées automatiquement au démarrage

## Prérequis

- .NET SDK 10
- Docker Desktop (optionnel si vous utilisez PostgreSQL)
- Visual Studio 2022 ou 2026 recommandé

## Démarrage rapide

1. Restaurer les paquets NuGet :

```bash
dotnet restore
```

2. Lancer l’application :

```bash
dotnet run --urls http://localhost:5099
```

3. Ouvrir les URLs suivantes dans le navigateur :

- http://localhost:5099/ → interface du catalogue
- http://localhost:5099/swagger → Swagger UI
- http://localhost:5099/api/catalog → API produits
- http://localhost:5099/api/categories → API catégories

## Configuration de la base de données

Par défaut, le projet utilise SQLite.

Le fichier de configuration est géré dans le code avec la variable `DatabaseProvider` et la chaîne de connexion `DefaultConnection`.

### SQLite (par défaut)

Aucune configuration supplémentaire n’est nécessaire.

### PostgreSQL avec Docker

1. Démarrer la base PostgreSQL :

```bash
docker compose up -d
```

2. Définir le provider PostgreSQL :

Windows PowerShell :

```powershell
setx DatabaseProvider PostgreSQL
setx ConnectionStrings__DefaultConnection "Host=localhost;Port=5432;Database=ftelmarket;Username=postgres;Password=postgres"
```

3. Appliquer les migrations et relancer l’API :

```bash
dotnet ef database update
dotnet run --urls http://localhost:5099
```

## Structure du projet

```text
FTELMarketApi/
├── Controllers/
│   ├── CatalogController.cs
│   └── CategoriesController.cs
├── Data/
│   ├── AppDbContext.cs
│   └── DbInitializer.cs
├── Models/
│   ├── ApiContracts.cs
│   ├── Category.cs
│   └── Product.cs
├── Migrations/
├── Properties/
├── wwwroot/
│   ├── app.js
│   ├── index.html
│   └── styles.css
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
├── FTELMarketApi.csproj
├── FTELMarketApi.http
├── ftelmarket.db
├── README.md
└── buildlog.txt
```

## Endpoints principaux

### Produits

- GET /api/catalog
- GET /api/catalog/{id}
- POST /api/catalog
- PUT /api/catalog/{id}
- DELETE /api/catalog/{id}

### Catégories

- GET /api/categories
- GET /api/categories/{id}
- POST /api/categories
- PUT /api/categories/{id}
- DELETE /api/categories/{id}

## Exemple de requêtes HTTP

### Lister les produits

```bash
curl http://localhost:5099/api/catalog
```

### Créer une catégorie

```bash
curl -X POST http://localhost:5099/api/categories \
  -H "Content-Type: application/json" \
  -d "{\"name\":\"Electroménager\"}"
```

### Créer un produit

```bash
curl -X POST http://localhost:5099/api/catalog \
  -H "Content-Type: application/json" \
  -d "{\"name\":\"Aspirateur\",\"description\":\"Aspirateur sans fil\",\"price\":149.99,\"quantityInStock\":10,\"categoryId\":1}"
```

## Tests rapides

Pour vérifier le bon fonctionnement :

```bash
dotnet build
curl http://localhost:5099/api/status
curl http://localhost:5099/api/catalog
```

## Remarques

- Le prix est géré en `decimal` pour éviter les erreurs d’arrondi liées à l’argent.
- Les dates sont stockées en UTC pour une meilleure cohérence multi-fuseau horaire.
- Les catégories ont un nom unique pour éviter les doublons.

## Déploiement / publication GitHub

Pour publier le projet sur GitHub :

```bash
git init
git add .
git commit -m "Initial commit"
git branch -M main
git remote add origin <URL_DU_REPO_GITHUB>
git push -u origin main
```

Remplacez `<URL_DU_REPO_GITHUB>` par l’URL de votre dépôt GitHub public.

## Licence

Ce projet est fourni à titre pédagogique pour le TP de formation.

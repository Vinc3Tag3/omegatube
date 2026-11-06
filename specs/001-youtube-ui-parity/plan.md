# Implementation Plan: Parité de navigation et de lecture avec YouTube

**Branch**: `001-youtube-ui-parity` | **Date**: 2026-11-04 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `/specs/001-youtube-ui-parity/spec.md`

## Summary

Rapprocher la consultation et la lecture du site YouTube, sur la télécommande, sans mini-lecteur ni courts métrages. Le travail étend le cœur C++ déjà responsable de YouTube et les écrans NUI déjà en place (accueil, recherche, chaîne, playlist, lecteur).

L'accueil sans compte utilise le client WEB anonyme. Les listes du compte restent sur le client TV authentifié. Les pastilles, Explorer et les onglets de chaîne qui bougent sont des identifiants dans `remote-config.json`. Les actions de compte (playlist, historique, notifications, commentaires) renvoient les jetons trouvés dans les réponses, plus une édition de playlist `WL` / `LL` pour la bibliothèque. Le masquage « Pas intéressé » est d'abord local à l'application ouverte. La file, les recherches récentes et le mélange d'une playlist sont locaux. Les chapitres sortent du player déjà téléchargé. Les commentaires sont un appel `next` WEB séparé.

Ordre de livraison, chaque tranche livrable seule :

1. Cartes, focus, emplacements d'attente, Reprendre, pastilles, accueil sans compte.
2. Bloc d'informations du lecteur, lecture automatique visible, carte de fin, partage par code.
3. Recherche pleine page, filtres, recherches récentes.
4. Guide (Vous, Explorer, chaînes suivies) et abonnements filtrés.
5. Bibliothèque compte : historique groupé, WL, LL, playlists, menu de carte.
6. Pages chaîne et playlist, notifications, aléatoire.
7. Chapitres, description riche, file.
8. Commentaires en lecture.

## Technical Context

**Language/Version**: C++17 (`core/`), C# / .NET TizenFX NUI API 8 (`app/OmegaTube/`)

**Primary Dependencies**: Cœur existant (Mbed TLS, nlohmann/json, client InnerTube TVHTML5 + WEB), Tizen NUI, Return YouTube Dislike déjà branché pour les compteurs. Pas de dépendance nouvelle.

**Storage**: Fichiers JSON déjà dans le dossier de données de l'application (`config.json`, `positions.json`, cache de flux). Ajouts : `recent-searches.json` (dix entrées), champs d'affichage dans `positions.json`. File de lecture et masquages « Pas intéressé » : mémoire, vidés à la fermeture.

**Testing**: `core/tools/ytcheck` étendu, dont `ytcheck --selfcheck` pour l'encodage des filtres, les temps de description et le seuil Reprendre. Interface : parcours télévision de [quickstart.md](quickstart.md). Pas de projet de test C# (aucun n'existe).

**Target Platform**: Samsung Tizen 6.0 (API 8) et plus récent, écran 1920×1080, télécommande. Le cœur YouTube se vérifie aussi sur l'hôte via `ytcheck`.

**Project Type**: Application télévision native (coquille C# + bibliothèque C++), un seul utilisateur devant l'écran.

**Performance Goals**: Le déplacement du focus reste immédiat. Le texte et les images des cartes continuent de se remplir hors du chemin de la touche, via `UiWork`. Tout appel réseau reste hors du fil UI, comme `YouTube.cs` aujourd'hui. L'aléatoire d'une playlist peut attendre la fin du chargement ; l'écran doit le montrer.

**Constraints**: Un seul décodeur, donc une seule vidéo affichée. Pas d'aperçu dans la miniature. Le jeton de compte ne part pas vers le client WEB. Les logs et le rapport de bug ne gagnent pas de jeton. Les sections personnelles du rail et les réglages lecteur existants restent. Les compteurs « je n'aime pas » restent affichés.

**Scale/Scope**: Un téléviseur, un compte. Huit parcours de la spec. Flux paginés. Commentaires paginés. Playlists lues en entier seulement pour l'aléatoire.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

`.specify/memory/constitution.md` est le modèle amont non adopté : titres `[PRINCIPLE_1_NAME]`, version `[CONSTITUTION_VERSION]`, aucune règle en vigueur. Les exemples commentés (librairie d'abord, CLI, tests d'abord) ne sont pas une constitution.

- Avant recherche : **PASS**. Aucune règle à violer, aucune dérogation.
- Après conception (relecture de `research.md`, `data-model.md`, `contracts/`) : **PASS**. Le plan ne crée pas de projet ni de couche. Il reste dans `core/` et `app/OmegaTube/`. Si une constitution est ratifiée plus tard, ce gate est à refaire.

## Project Structure

### Documentation (this feature)

```text
specs/001-youtube-ui-parity/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   ├── shell-json.md
│   └── screens.md
└── tasks.md              # /speckit-tasks — pas créé ici
```

### Source Code (repository root)

```text
app/OmegaTube/
├── OmegaTube.cs              # pile d'écrans, retour
├── NativeCore.cs             # nouveaux P/Invoke
├── I18n.cs                   # libellés en, fr, es
├── Config/AppConfig.cs       # Autoplay et HomeRows inchangés de rôle
├── Data/YouTube.cs           # modèles et appels
├── Data/LocalStore.cs        # positions enrichies, recherches récentes
└── Ui/
    ├── Theme.cs              # contour de focus
    ├── Card.cs               # avatar, badges, meta, focus
    ├── Rail.cs               # guide, avatars
    ├── HomeScreen.cs         # pastilles, Reprendre, entrées du guide
    ├── SearchScreen.cs       # clavier repliable, filtres
    ├── FeedGrid.cs           # menu maintenu, groupes d'historique
    ├── ShelfList.cs          # Reprendre en tête
    ├── ChannelScreen.cs      # en-tête, onglets, tri, notifications
    ├── PlaylistScreen.cs     # en-tête, aléatoire, vidéo en cours
    └── PlayerScreen.cs       # infos, fin, chapitres, description, file, commentaires, partage

core/
├── include/omega/core.h      # commentaires JSON et nouvelles fonctions
├── src/core.cpp              # wrappers C
├── src/yt/innertube.h
├── src/yt/innertube.cpp      # parse et appels
└── tools/ytcheck.cpp         # preuves hôte

remote-config.json            # home, homeChips, explore, channelTabs
```

**Structure Decision**: Un seul produit déjà découpé en `app/OmegaTube` (écrans, focus, textes) et `core` (réseau, JSON YouTube). Les écrans nouveaux ne sont pas des projets : Vous, Explorer, commentaires, description et file sont des états ou des pages dans les fichiers UI existants, ajoutés seulement si un fichier dépasse la lisibilité du fichier d'à côté. Pas de dossier `src/` ou `frontend/` générique.

## Complexity Tracking

Aucune violation de constitution. Tableau de dérogation non utilisé.

## Design post-recherche

Décisions détaillées : [research.md](research.md). Modèle : [data-model.md](data-model.md).

Points qui contraignent l'implémentation :

- Le browse TV authentifié et le browse WEB anonyme ne se mélangent pas dans un même appel.
- `omega_yt_search` gagne un argument de filtres. L'encodage protobuf vit dans le cœur, avec un self-check.
- `WL`, `LL` et le mélange local réutilisent la pagination playlist déjà là.
- Les mutations fragiles (historique, notifications, j'aime de commentaire, pas intéressé côté compte) copient la commande de la réponse. Le masquage visuel « Pas intéressé » ne dépend pas de cette commande pour respecter la spec pendant que l'application reste ouverte.
- Le lecteur n'est pas recréé pour montrer les infos. Retour depuis le lecteur détruit la vidéo.
- `remote-config.json` porte les browse ids et les `params` d'onglets, pas les jetons par utilisateur.

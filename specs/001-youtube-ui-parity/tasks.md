---
description: "Task list for YouTube UI parity feature"
---

# Tasks: Parité de navigation et de lecture avec YouTube

**Input**: Design documents from `/specs/001-youtube-ui-parity/`

**Prerequisites**: [plan.md](plan.md), [spec.md](spec.md), [research.md](research.md), [data-model.md](data-model.md), [contracts/](contracts/)

**Tests**: Pas de suite C# demandée. Validation hôte via `core/tools/ytcheck.cpp` et parcours TV dans [quickstart.md](quickstart.md).

**Organization**: Une phase par user story (US1–US8). Chaque phase est livrable et testable seule après la phase Foundational.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: fichiers différents, pas de dépendance sur une tâche incomplète du même lot
- **[USn]**: user story de [spec.md](spec.md)

## Path Conventions

- Cœur : `core/src/yt/`, `core/include/omega/core.h`, `core/src/core.cpp`, `core/tools/ytcheck.cpp`
- Interface : `app/OmegaTube/`
- Config distante : `remote-config.json`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Clés de config et outil hôte pour prouver les contrats avant / pendant l’UI.

- [x] T001 Ajouter dans `remote-config.json` les blocs `home.signedOutBrowseId` (défaut `FEtrending`), `homeChips[]` (`id`, `label`, `browseId`), `explore[]`, `channelTabs` (`videos`, `videosPopular`, `live`, `playlists`, `about`) sans retirer les clés InnerTube existantes
- [x] T002 [P] Documenter dans `specs/001-youtube-ui-parity/contracts/shell-json.md` les clés ajoutées si le fichier diverge après T001 (alignement doc ↔ JSON embarqué)
- [x] T003 Ajouter l’option `--selfcheck` dans `core/tools/ytcheck.cpp` avec sortie 0 et stubs pour encodage filtres recherche, parse temps description, seuil Reprendre (>10 s, >30 s restantes) à implémenter en T004–T006

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Champs de carte partagés, stockage local étendu, masquage session, focus thème. **Aucune user story avant la fin de cette phase.**

- [x] T004 Implémenter les self-checks réels dans `core/tools/ytcheck.cpp` (`--selfcheck`) : encodage protobuf filtres, regex temps `h:mm:ss`/`m:ss`, règle Reprendre identique à [data-model.md](data-model.md)
- [x] T005 Étendre `omega::yt::FeedItem` dans `core/src/yt/innertube.h` (`channel_avatar`, `verified`, `badge`, `item_count`, `feedback` tokens optionnels) et sérialiser ces champs dans le JSON renvoyé par le cœur (`core/src/core.cpp`)
- [x] T006 [P] Parser avatar, badge vérifié/nouveau/première/direct, `itemCount` playlist/mix et jetons `feedback` dans `parse_video_renderer`, `parse_lockup`, `parse_tile` dans `core/src/yt/innertube.cpp`
- [x] T007 [P] Étendre `VideoItem` et `YouTube.ToItems` dans `app/OmegaTube/Data/YouTube.cs` pour lire les nouveaux champs JSON ; étendre `LocalStore.ItemsJson` / cache flux dans `app/OmegaTube/Data/LocalStore.cs` pour persister avatar, badge, `itemCount`, `watched`
- [x] T008 Enrichir `LocalStore.SavePosition` dans `app/OmegaTube/Data/LocalStore.cs` avec `title`, `channel`, `thumbnail`, `channelAvatar` (quand connus) ; exposer `LocalStore.ListResumeCandidates()` triés par `at` décroissant avec règle d’éligibilité Reprendre
- [x] T009 Créer `app/OmegaTube/Data/SessionHide.cs` : ensembles en mémoire vidéos « Pas intéressé » et chaînes « Ne plus recommander », effacés au redémarrage ; méthode `Filter(IEnumerable<VideoItem>)` pour les listes
- [x] T010 [P] Ajouter constantes contour focus (couleur, épaisseur) dans `app/OmegaTube/Ui/Theme.cs` ; retirer l’inversion gris clair du bloc info carte comme style de sélection principal
- [x] T011 [P] Refactoriser `app/OmegaTube/Ui/Card.cs` : appliquer contour sur `thumb` au focus ; conserver texte clair sur fond sombre ; titre max deux lignes (`MultiLine` + hauteur) sans recouvrir durée ni barre progression

**Checkpoint**: `ytcheck --selfcheck` vert ; cartes peuvent afficher les nouveaux champs dès qu’une liste les fournit.

---

## Phase 3: User Story 1 — Reconnaître l’accueil YouTube (Priority: P1) 🎯 MVP

**Goal**: Accueil lisible type site : cartes complètes, skeletons, Reprendre, pastilles, accueil public sans compte.

**Independent Test**: Parcours quickstart § TV points 1–5 (sans compte, cartes, attente, Reprendre, pastilles).

### Implementation for User Story 1

- [ ] T012 [US1] Implémenter browse WEB anonyme (sans Bearer) pour `remote-config.json` → `home.signedOutBrowseId` dans `core/src/yt/innertube.cpp` + wrapper `omega_yt_home_anon` dans `core/include/omega/core.h` et `core/src/core.cpp` (même forme JSON que recherche / étagères)
- [ ] T013 [P] [US1] Ajouter `omega_yt_browse` réponse `chips[]` pour `FEwhat_to_watch` quand InnerTube les fournit ; sinon l’interface lira `homeChips` depuis config (`core/src/yt/innertube.cpp`)
- [ ] T014 [P] [US1] Déclarer `YtHomeAnon` dans `app/OmegaTube/NativeCore.cs` et méthodes `YouTube.HomeAnon` / `YouTube.HomeChips` dans `app/OmegaTube/Data/YouTube.cs`
- [ ] T015 [US1] Afficher avatar chaîne, badge vérifié, badges live/nouveau/première, repères playlist/mix et `itemCount` dans `app/OmegaTube/Ui/Card.cs` (`Fill`) ; badges absents = pas de placeholder cassé
- [ ] T016 [US1] Ajouter rangée skeleton (cartes grises sans texte) dans `app/OmegaTube/Ui/FeedGrid.cs` et `app/OmegaTube/Ui/ShelfList.cs` pendant chargement ; remplacer par erreur + Réessayer si échec réseau
- [ ] T017 [US1] Construire étagère « Reprendre » en tête dans `app/OmegaTube/Ui/HomeScreen.cs` + `app/OmegaTube/Ui/ShelfList.cs` via `LocalStore.ListResumeCandidates()` ; masquer l’étagère si liste vide
- [ ] T018 [US1] UI pastilles au-dessus des étagères dans `app/OmegaTube/Ui/HomeScreen.cs` : « Tout » + chips ; filtrage par `browseId` sans quitter l’accueil ; état vide dédié par pastille
- [ ] T019 [US1] Brancher accueil sans compte sur `YouTube.HomeAnon` dans `app/OmegaTube/Ui/HomeScreen.cs` (`BuildSections` / loader `home`) au lieu d’un écran vide-only
- [ ] T020 [P] [US1] Libellés I18n (`Reprendre`, pastilles, badges, skeleton, retry) dans `app/OmegaTube/I18n.cs` (en source, fr, es)
- [ ] T021 [US1] Ajouter `build/host/ytcheck --home-anon` dans `core/tools/ytcheck.cpp` ; au moins une vidéo, pas de `signed out`

**Checkpoint**: MVP accueil — quickstart §1–5 OK sur TV.

---

## Phase 4: User Story 2 — Lire une vidéo avec le contexte du site (Priority: P1)

**Goal**: Bloc infos sans pause, À suivre vertical, autoplay visible, carte fin annulable, partage QR, compteurs je n’aime pas conservés.

**Independent Test**: quickstart § TV points 13 (partie infos/autoplay/fin) et §6 lecteur sans commentaires/chapitres.

### Implementation for User Story 2

- [ ] T022 [US2] Ajouter panneau « Informations » dans `app/OmegaTube/Ui/PlayerScreen.cs` : titre, ligne chaîne (avatar, abonnés), abonnement, like/dislike avec compteurs RYD, aperçu description, sans mettre en pause la vidéo
- [ ] T023 [US2] Remplacer la rangée horizontale de suggestions par liste verticale « À suivre » (≥3 entrées si disponibles) dans le panneau infos de `app/OmegaTube/Ui/PlayerScreen.cs`, alimentée par `YouTube.Related` existant
- [ ] T024 [US2] Interrupteur « Lecture automatique » dans le panneau infos lié à `AppConfig.Player.Autoplay` dans `app/OmegaTube/Config/AppConfig.cs` et persistance via `AppConfig.Save`
- [ ] T025 [US2] Grande carte « À suivre » + compte à rebours annulable en fin de VOD dans `app/OmegaTube/Ui/PlayerScreen.cs` ; respecter autoplay off (aucune suite) ; direct sans compte à rebours replay
- [ ] T026 [P] [US2] Action Partager : QR `https://www.youtube.com/watch?v=` via `NativeCore.QrEncode` / flux existant dans `app/OmegaTube/Ui/PlayerScreen.cs`
- [ ] T027 [P] [US2] Libellés I18n panneau infos, autoplay, carte fin, partager dans `app/OmegaTube/I18n.cs`

**Checkpoint**: US1 + US2 — lecture avec contexte site testable seule.

---

## Phase 5: User Story 3 — Chercher comme sur le site (Priority: P2)

**Goal**: Recherches récentes (max 10), grille pleine page, types mixtes, filtres cumulables.

**Independent Test**: quickstart § TV point 6 et commandes hôte `--search` filtrée.

### Implementation for User Story 3

- [ ] T028 [US3] Encoder filtres recherche (date, duration, type, sort, live, hd, subtitles) en protobuf dans `core/src/yt/innertube.cpp` ; étendre `Client::search` et `omega_yt_search` dans `core/include/omega/core.h` / `core/src/core.cpp` avec argument JSON filtres (`null` = aucun)
- [ ] T029 [US3] Parser résultats `kind: channel` dans `core/src/yt/innertube.cpp` et les exposer dans le JSON recherche
- [ ] T030 [P] [US3] `NativeCore.YtSearch` + surcharge filtres et mapping dans `app/OmegaTube/Data/YouTube.cs`
- [ ] T031 [US3] Persister `recent-searches.json` (max 10, antéchronologique, retirer une entrée / vider) dans `app/OmegaTube/Data/LocalStore.cs`
- [ ] T032 [US3] Refondre layout `app/OmegaTube/Ui/SearchScreen.cs` : recherches récentes → suggestions → clavier ; après recherche, masquer clavier et `FeedGrid` pleine hauteur ; bouton modifier requête
- [ ] T033 [US3] UI filtres cumulables + état « aucun filtre » dans `app/OmegaTube/Ui/SearchScreen.cs` ; message zéro résultat
- [ ] T034 [US3] Carte chaîne et navigation vers `ChannelScreen` ; playlist → `PlaylistScreen` ; vidéo → lecteur dans `app/OmegaTube/Ui/SearchScreen.cs` / `FeedRow`
- [ ] T035 [P] [US3] Libellés filtres et recherches récentes dans `app/OmegaTube/I18n.cs`
- [ ] T036 [US3] Commande `ytcheck --search "<q>" --duration long` (ou flags équivalents) dans `core/tools/ytcheck.cpp`

**Checkpoint**: Recherche indépendante de l’accueil enrichi.

---

## Phase 6: User Story 4 — Abonnements, historique et bibliothèque (Priority: P2)

**Goal**: Abonnements filtrés, Vous (historique, WL, LL, playlists), menu carte, actions compte.

**Independent Test**: quickstart § TV points 7–10.

### Implementation for User Story 4

- [ ] T037 [US4] Implémenter `omega_yt_account_action` dans `core/include/omega/core.h`, `core/src/core.cpp`, `core/src/yt/innertube.cpp` (`addToPlaylist`, `createPlaylist`, `feedback`, `notification`) selon [contracts/shell-json.md](contracts/shell-json.md)
- [ ] T038 [US4] Parser `groups[]` et `commands` pause/clear pour `FEhistory` dans `core/src/yt/innertube.cpp` ; conserver `items` plat pour cache
- [ ] T039 [P] [US4] P/Invoke et wrappers `YouTube.AccountAction`, `YouTube.HistoryGroups` dans `app/OmegaTube/NativeCore.cs` et `app/OmegaTube/Data/YouTube.cs`
- [ ] T040 [US4] Créer `app/OmegaTube/Ui/YouScreen.cs` : entrées Historique, À regarder plus tard (`WL`), Vidéos aimées (`LL`), Playlists (`FElibrary`) ; intégrer au rail via `app/OmegaTube/Ui/HomeScreen.cs`
- [ ] T041 [US4] Vue historique groupée + recherche interne + retirer une entrée + pause + effacement confirmé dans `app/OmegaTube/Ui/FeedGrid.cs` ou composant dédié sous `app/OmegaTube/Ui/`
- [ ] T042 [US4] Abonnements : bande avatars (chaînes guide, voir US5 T046 si guide pas prêt — stub liste vide) + flux chronologique `FEsubscriptions` `items` + filtres Tout / Non vues / En direct dans `app/OmegaTube/Ui/HomeScreen.cs` / `FeedGrid.cs`
- [ ] T043 [US4] Menu carte (maintien touche) : WL, enregistrer playlist, Pas intéressé, Ne plus recommander, file ; appliquer `SessionHide` + appels compte dans `app/OmegaTube/Ui/FeedGrid.cs` ; sans compte = message connexion, pas de succès fictif
- [ ] T044 [P] [US4] Libellés bibliothèque, historique, menu carte dans `app/OmegaTube/I18n.cs`
- [ ] T045 [US4] Étendre `ytcheck` : `--playlist-page WL`, `--playlist-page LL` après `--signin` dans `core/tools/ytcheck.cpp`

**Checkpoint**: Parcours compte bibliothèque + menu carte.

---

## Phase 7: User Story 5 — Guide du site (Priority: P2)

**Goal**: Rail Accueil, Recherche, Abonnements, Vous, Explorer, sections perso, chaînes suivies, Compte, Réglages.

**Independent Test**: quickstart § TV point 7 (sections guide + Explorer).

### Implementation for User Story 5

- [ ] T046 [US5] Implémenter `omega_yt_guide` (client TV + token) → `channels[]` dans `core/src/yt/innertube.cpp`, `core/include/omega/core.h`, `core/src/core.cpp`
- [ ] T047 [P] [US5] `YouTube.GuideChannels` dans `app/OmegaTube/Data/YouTube.cs` + `NativeCore.cs`
- [ ] T048 [US5] Reconstruire `BuildSections` / ordre rail dans `app/OmegaTube/Ui/HomeScreen.cs` : Accueil, Recherche, Abonnements, Vous, Explorer, `HomeRows` visibles, chaînes suivies (avatar+nom → `ChannelScreen`), Compte, Réglages ; ne pas effacer sections perso
- [ ] T049 [US5] Créer `app/OmegaTube/Ui/ExploreScreen.cs` : liste thèmes depuis `remote-config.json` `explore[]` ; browse id par entrée ; vide / erreur dédiés
- [ ] T050 [US5] Brancher bande avatars Abonnements sur `GuideChannels` dans `app/OmegaTube/Ui/HomeScreen.cs` (complète T042)
- [ ] T051 [P] [US5] Afficher photo compte sur entrée Compte du rail dans `app/OmegaTube/Ui/Rail.cs` / `HomeScreen.cs` depuis `YouTube.Account`
- [ ] T052 [P] [US5] Libellés Explorer et entrées guide dans `app/OmegaTube/I18n.cs`
- [ ] T053 [US5] Commande `ytcheck --guide` après `--signin` dans `core/tools/ytcheck.cpp`

**Checkpoint**: Navigation guide complète ; sections perso conservées.

---

## Phase 8: User Story 6 — Chaîne et playlist (Priority: P3)

**Goal**: Page chaîne (bannière, onglets, tri, notifications) ; page playlist (en-tête, aléatoire, surlignage en cours).

**Independent Test**: quickstart § TV points 11–12.

### Implementation for User Story 6

- [ ] T054 [US6] Étendre `Client::channel` avec argument onglet + `channelTabs` params depuis config dans `core/src/yt/innertube.cpp` ; en-tête `banner`, `handle`, `description`, `notifications` JSON selon contrat
- [ ] T055 [US6] Enrichir première page `playlist_page` avec `owner`, `thumbnail`, `count` dans `core/src/yt/innertube.cpp` / JSON
- [ ] T056 [P] [US6] Signatures C# mises à jour dans `app/OmegaTube/NativeCore.cs` et `app/OmegaTube/Data/YouTube.cs`
- [ ] T057 [US6] Refondre `app/OmegaTube/Ui/ChannelScreen.cs` : bannière, @handle, onglets Accueil/Vidéos/En direct/Playlists/À propos, tri récent/populaire onglet Vidéos, notifications Toutes/Personnalisées/Aucune + message si jetons `unknown`
- [ ] T058 [US6] Refondre `app/OmegaTube/Ui/PlaylistScreen.cs` : illustration, propriétaire, count, Enregistrer, surlignage `playingVideoId`, Aléatoire (charger toutes pages puis Fisher-Yates, attente visible)
- [ ] T059 [US6] Exposer niveau notifications chaîne dans JSON lecteur (`omega_player_formats`) et panneau infos `PlayerScreen.cs` pour la chaîne en cours
- [ ] T060 [P] [US6] Libellés onglets chaîne, notifications, playlist aléatoire dans `app/OmegaTube/I18n.cs`

**Checkpoint**: Pages chaîne/playlist alignées contrat écrans.

---

## Phase 9: User Story 7 — Chapitres, description, file (Priority: P3)

**Goal**: Marqueurs chapitres, description dépliée cliquable, file session prioritaire sur suggestions.

**Independent Test**: quickstart § TV points 13–14 et 17 (file).

### Implementation for User Story 7

- [ ] T061 [US7] Extraire chapitres player (`macroMarkers` / barre décorée) dans `core/src/yt/innertube.cpp` lors du `player()` ; ajouter `chapters[]` au JSON `omega_player_formats` dans `core/src/core.cpp`
- [ ] T062 [US7] Liste chapitres + ticks barre progression + seek ±1 s dans `app/OmegaTube/Ui/PlayerScreen.cs`
- [ ] T063 [US7] Panneau description dépliée scrollable ; parser temps description et liens/mots-dièse ; ouvrir vidéo/chaîne/playlist/recherche ou QR lien externe dans `app/OmegaTube/Ui/PlayerScreen.cs`
- [ ] T064 [US7] File de lecture en mémoire dans `app/OmegaTube/Ui/PlayerScreen.cs` : voir, ajouter, retirer, réordonner ; consommer avant suggestions ; vide au prochain lancement app
- [ ] T065 [P] [US7] Brancher « Ajouter à la file » du menu carte (T043) vers file lecteur si vidéo en cours ou file globale session dans `app/OmegaTube/Data/` ou `PlayerScreen`
- [ ] T066 [P] [US7] Libellés chapitres, description, file dans `app/OmegaTube/I18n.cs`

**Checkpoint**: Chapitres + file testables sans commentaires.

---

## Phase 10: User Story 8 — Commentaires (Priority: P3)

**Goal**: Lire commentaires, tri, réponses, j’aime commentaire ; états disabled/failed.

**Independent Test**: quickstart § TV point 15 et `ytcheck --comments`.

### Implementation for User Story 8

- [ ] T067 [US8] Implémenter `omega_yt_comments(videoId, continuation)` WEB dans `core/src/yt/innertube.cpp`, `core/include/omega/core.h`, `core/src/core.cpp` (`state`, `sort`, `comments[]`, pagination, `likeToken`)
- [ ] T068 [US8] `YouTube.Comments` + modèles commentaire dans `app/OmegaTube/Data/YouTube.cs` ; j’aime via `AccountAction` + `likeToken`
- [ ] T069 [US8] Panneau commentaires dans `app/OmegaTube/Ui/PlayerScreen.cs` : liste, tri pertinent/récent, réponses repliables, j’aime si connecté, messages disabled vs failed+retry
- [ ] T070 [P] [US8] Libellés commentaires dans `app/OmegaTube/I18n.cs`
- [ ] T071 [US8] Commande `ytcheck --comments <videoId>` dans `core/tools/ytcheck.cpp`

**Checkpoint**: Tous parcours spec couverts côté fonctionnalité.

---

## Phase 11: Polish & Cross-Cutting Concerns

**Purpose**: Retour navigation FR-041, validation bout en bout, cohérence cache.

- [ ] T072 Vérifier pile Retour : lecteur masque vidéo et garde progression carte ; page au-dessus du lecteur revient à ±1 s (`app/OmegaTube/OmegaTube.cs`, `app/OmegaTube/Ui/PlayerScreen.cs`)
- [ ] T073 [P] Appliquer `SessionHide.Filter` à toutes les listes flux (`FeedGrid`, `ShelfList`, recherche) dans `app/OmegaTube/Ui/`
- [ ] T074 [P] Passer les parcours [quickstart.md](quickstart.md) (hôte + TV) et corriger les écarts documentés dans `specs/001-youtube-ui-parity/quickstart.md` si les commandes divergent
- [ ] T075 Revue I18n : aucune chaîne utilisateur nouvelle sans entrée fr/es dans `app/OmegaTube/I18n.cs`

---

## Dependencies & Execution Order

### Phase Dependencies

```text
Phase 1 Setup
    ↓
Phase 2 Foundational  ← BLOCKS toutes les user stories
    ↓
Phase 3 US1 (P1) ──→ MVP recommandé (stop ici pour demo)
Phase 4 US2 (P1) ──→ souvent enchaîné avec US1 (lecteur)
    ↓
Phase 5 US3, Phase 6 US4, Phase 7 US5  (P2, ordre flexible après US1+US2 si guide requis pour avatars abonnements : US5 avant fin US4 bande avatars — T050 après T046)
    ↓
Phase 8 US6, Phase 9 US7, Phase 10 US8  (P3)
    ↓
Phase 11 Polish
```

### User Story Dependencies

| Story | Dépend de | Note |
| --- | --- | --- |
| US1 | Foundational | MVP seul |
| US2 | Foundational, cartes US1 utiles mais pas bloquant | Lecteur existant |
| US3 | Foundational | Indépendant |
| US4 | Foundational, T037 actions compte | Bande abonnements complète après T046 (US5) |
| US5 | Foundational | US4 T042 partiel sans guide |
| US6 | Foundational, cartes navigation | |
| US7 | US2 panneau infos | File menu carte T065 → T043 |
| US8 | US2 panneau infos | |

### Parallel Opportunities

- **Phase 1**: T002 ∥ T001 après T001 écrit
- **Phase 2**: T006 ∥ T007 ∥ T010 ∥ T011 après T005
- **US1**: T013 ∥ T014 ∥ T020 après T012 ; T021 après T012
- **US2**: T026 ∥ T027 ∥ T024 après T022
- **US3**: T029 ∥ T030 ∥ T035 après T028
- **US4**: T039 ∥ T044 ∥ T045 après T037
- **US5**: T047 ∥ T051 ∥ T052 après T046
- **US6**: T056 ∥ T060 après T054–T055
- **Polish**: T073 ∥ T074 ∥ T075

---

## Parallel Example: User Story 1

```bash
# Après T012 (home anon core):
# Agent A: T013 chips parse + T014 bindings
# Agent B: T020 I18n + T015 Card badges UI
# Agent C: T021 ytcheck --home-anon
# Séquentiel ensuite: T016 skeletons → T017 Reprendre → T018 pastilles → T019 signed-out branch
```

---

## Implementation Strategy

### MVP First (User Story 1 only)

1. Phase 1 + Phase 2
2. Phase 3 (T012–T021)
3. **STOP** — valider quickstart §1–5 sur TV
4. Demo / merge partiel

### Incremental Delivery (plan.md)

1. US1 → US2 (consultation + lecture)
2. US3 recherche
3. US5 guide puis US4 bibliothèque (T046 avant T050)
4. US6 pages
5. US7 puis US8 lecteur avancé
6. Polish

### Suggested MVP Scope

**Phase 1 + Phase 2 + Phase 3 (US1)** — 21 tâches jusqu’à T021, accueil reconnaissable sans implémenter le nouveau lecteur infos.

---

## Notes

- Ne pas logger de jetons OAuth / feedback dans `BugReport` (contrainte plan).
- `remote-config.json` sur `main` est lu à distance : tester aussi la copie embarquée au build.
- Un browse id mort se corrige dans config, pas par repli silencieux vers un autre thème.
- Commit logique : fin de chaque checkpoint user story.

# Contrat: JSON entre le cœur et l'interface

L'interface ne parle pas à YouTube directement. Chaque appel reste une fonction C bloquante qui renvoie une chaîne JSON à libérer, comme aujourd'hui. Les champs déjà consommés restent. Les champs nouveaux sont omis ou vides quand l'information n'existe pas : l'interface ne doit pas casser sur une ancienne réponse.

Les erreurs restent `{"error":"..."}`. `signed out` garde ce libellé quand un appel de compte part sans jeton.

## Cartes (`items[]`)

Champs ajoutés, tous optionnels :

```json
{
  "channelAvatar": "https://...",
  "verified": true,
  "badge": "live|new|premiere",
  "itemCount": 12,
  "feedback": { "notInterested": "...", "dontRecommend": "...", "removeHistory": "..." }
}
```

`badge` absent et `live: true` veulent déjà dire « direct ». `itemCount` concerne `kind` `playlist`.

## Accueil

- Sans compte : nouvel appel, même forme que la recherche (`items` ou `shelves`), browse id pris dans `remote-config.json` → `home.signedOutBrowseId` (défaut prévu `FEtrending`). Pas de jeton.
- Avec compte : `omega_yt_browse("FEwhat_to_watch")` inchangé, plus :

```json
{ "chips": [ { "id": "music", "label": "Musique", "browseId": "FEmusic" } ] }
```

`chips` absent = l'interface utilise `homeChips` du remote-config. La pastille Tout n'est pas renvoyée : l'interface l'ajoute toujours.

## Recherche

`omega_yt_search` accepte les filtres. Extension de signature côté C : la requête reste le texte, les filtres sont un JSON compact dans un argument nouveau (`null` = aucun filtre), pour ne pas surcharger le texte de la requête.

```json
{
  "date": "any|hour|today|week|month|year",
  "duration": "any|short|medium|long",
  "type": "any|video|channel|playlist",
  "sort": "relevance|date|views",
  "live": false,
  "hd": false,
  "subtitles": false
}
```

Les cartes chaîne dans `items` :

```json
{ "id": "UCxxxx", "kind": "channel", "title": "Nom", "channelAvatar": "https://...", "subscribers": "1,2 M" }
```

`kind: "channel"` n'ouvre pas le lecteur.

## Guide et Explorer

Nouvel appel `omega_yt_guide` (compte requis) :

```json
{
  "channels": [ { "id": "UCxxxx", "title": "Nom", "avatar": "https://..." } ]
}
```

Explorer n'a pas d'appel dédié : `omega_yt_browse` sur les ids de `remote-config.json` → `explore[]` (`id`, `label`, `browseId`). Sans compte, ces browse ids publics passent par le même chemin que l'accueil anonyme (WEB, sans jeton). Avec compte, le client TV reste utilisé s'il renvoie des éléments ; sinon repli WEB sans jeton. Le choix est interne au cœur. L'interface ne reçoit qu'un flux ou une erreur.

## Bibliothèque

- Historique : `omega_yt_browse("FEhistory")` gagne `groups` :

```json
{ "groups": [ { "title": "Aujourd'hui", "items": [ {} ] } ], "commands": { "pause": "...", "clear": "..." } }
```

Si `groups` est présent, l'interface l'affiche. `items` plat peut rester pour le cache, identique à la concaténation des groupes.

- `WL` et `LL` : `omega_yt_playlist_page("WL"|"LL", continuation)`, forme actuelle, plus l'en-tête playlist ci-dessous.
- Liste des playlists : `omega_yt_browse("FElibrary")` avec des `items` de `kind: "playlist"`.

Mutations, nouvel appel `omega_yt_account_action` :

```json
{ "action": "addToPlaylist|createPlaylist|feedback|notification", "playlistId": "WL", "videoId": "...", "name": "...", "token": "...", "channelId": "...", "level": "all|personalized|none" }
```

Réponse `{"ok":true}` ou `{"error":"..."}`. `createPlaylist` réussit avec `{"ok":true,"playlistId":"..."}`.

`feedback` envoie `token` tel que reçu (`notInterested`, `dontRecommend`, `removeHistory`, `pause`, `clear`). L'effacement d'historique n'est appelé qu'après confirmation dans l'interface : le cœur ne confirme pas lui-même.

## Chaîne

`omega_yt_channel` gagne un argument d'onglet `home|videos|videosPopular|live|playlists|about` (chaîne vide = vidéos récentes, comportement actuel).

En-tête étendu sur la première page :

```json
{
  "title": "",
  "avatar": "",
  "banner": "",
  "handle": "@nom",
  "subscribers": "",
  "description": "",
  "notifications": {
    "level": "all|personalized|none|unknown",
    "tokens": { "all": "...", "personalized": "...", "none": "..." }
  }
}
```

`about` renvoie `items` vide et la description longue dans `description`.

## Playlist

Première page de `omega_yt_playlist_page`, champs d'en-tête ajoutés :

```json
{ "title": "", "owner": "", "thumbnail": "", "count": 42 }
```

`count` absent si inconnu. L'interface peut afficher le nombre de cartes déjà chargées avec « + » tant qu'il y a une continuation, comme aujourd'hui, mais préfère `count` quand il est là.

## Lecteur

Le JSON déjà produit avec les formats gagne :

```json
{
  "chapters": [ { "title": "Intro", "startMs": 0 } ],
  "notifications": { "level": "unknown", "tokens": {} }
}
```

`description` existe déjà. Les chapitres absents = tableau vide. La durée de la vidéo sert à rejeter un chapitre ou un temps de description hors plage ; ce rejet est fait dans l'interface.

`omega_yt_details` continue d'exposer `likes` et `dislikes` (Return YouTube Dislike). Ce contrat ne les retire pas.

## Commentaires

Nouvel appel `omega_yt_comments(videoId, continuation)` :

```json
{
  "state": "ready|disabled",
  "sort": { "top": "token", "newest": "token" },
  "comments": [
    {
      "id": "",
      "author": "",
      "text": "",
      "published": "",
      "likes": 0,
      "liked": false,
      "replyCount": 0,
      "repliesToken": "",
      "likeToken": ""
    }
  ],
  "continuation": ""
}
```

`continuation` vide en entrée = première page, tri par défaut (pertinents). Pour changer de tri ou ouvrir les réponses, l'interface renvoie le jeton correspondant comme `continuation`.

`state: "disabled"` = commentaires fermés, `comments` vide, pas une erreur. Une panne réseau reste `{"error":"..."}`.

Aimer : `omega_yt_account_action` avec `action: "feedback"` et le `likeToken`. Pas de second appel spécial.

## Config distante ajoutée

Dans `remote-config.json`, sans retirer les clés actuelles :

```json
{
  "home": { "signedOutBrowseId": "FEtrending" },
  "homeChips": [ { "id": "music", "label": "Music", "browseId": "FEmusic" } ],
  "explore": [ { "id": "trending", "label": "Trending", "browseId": "FEtrending" } ],
  "channelTabs": { "videos": "EgZ2aWRlb3PyBgQKAjoA", "videosPopular": "", "live": "", "playlists": "", "about": "" }
}
```

Implémentation Phase 1 (2026-11-04) : `homeChips` inclut aussi `{ "id": "all", "browseId": "" }` (pastille « Tout » côté UI). Les clés `explore` / `homeChips` portent des libellés anglais source pour `T()`. Un browse id qui renvoie une erreur est un échec visible (message + réessayer), pas un repli vers un autre thème.

## Invariants

- Aucun appel de compte sans jeton ne renvoie `ok: true`.
- Le jeton n'est pas écrit dans les JSON renvoyés à l'interface, ni dans les logs existants (déjà filtrés par le rapport de bug).
- Les nouveaux champs ne changent pas le sens de `continuation` : une chaîne opaque à renvoyer telle quelle.

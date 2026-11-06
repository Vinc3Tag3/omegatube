# Research: Parité de navigation et de lecture avec YouTube

Décisions pour `specs/001-youtube-ui-parity/spec.md`. Aucun point de la spec n'est laissé ouvert. Les identifiants YouTube qui bougent vivent dans `remote-config.json`, comme les clients InnerTube déjà utilisés.

## 1. Où vit le travail

- **Decision**: Étendre le cœur C++ (`core/src/yt`, API C `core/include/omega/core.h`) et l'interface C# existante (`app/OmegaTube`). Pas de nouveau projet, pas de nouvelle couche.
- **Rationale**: La coquille NUI ne parle à YouTube que par le JSON du cœur. L'accueil, la recherche, la chaîne, la playlist et le lecteur existent déjà.
- **Alternatives considered**: Refaire les écrans dans une autre pile graphique. Rejeté : l'application est un client Tizen NUI, et le lecteur matériel ne se déplace pas.

## 2. Clients YouTube

- **Decision**: Garder les clients déjà en config. Le client `browse` (TVHTML5, jeton du compte) sert aux listes du compte. Le client `search` (WEB, visiteur) sert à la recherche, aux pages chaîne, aux playlists et à l'accueil sans compte. Ne jamais envoyer le jeton au client WEB, ni un appel TV sans jeton.
- **Rationale**: `omega_yt_browse` échoue avec `signed out` sans compte, et le commentaire de config dit que seul le client TV accepte le jeton. La recherche WEB fonctionne déjà sans compte.
- **Alternatives considered**: Un seul client WEB authentifié pour tout. Rejeté : le jeton TV n'est pas accepté par ce client, et le changer casse l'accueil actuel.

## 3. Accueil sans compte et pastilles

- **Decision**: Sans compte, un browse WEB sans jeton sur l'identifiant public `FEtrending`, affiché comme une seule étagère « Tendances » si YouTube ne renvoie pas de rangées. Avec compte, garder `FEwhat_to_watch`. Les pastilles personnalisées sont lues dans la réponse (`chipCloudChipRenderer`) quand elles existent. Sinon, pastilles fixes de `remote-config.json` (`homeChips`), chacune pointant vers un browse id. Le jeu de repli est Tout, Musique, En direct, Jeux, Actualités, Sport.
- **Rationale**: La spec exige un accueil peuplé sans compte (SC-001) et des pastilles même si le compte n'en fournit pas.
- **Alternatives considered**: Laisser l'accueil vide et seulement changer le texte. Rejeté : contredit FR-010. Inventer des pastilles en relançant des recherches libres. Rejeté : moins stable qu'un browse id corrigible dans le JSON.

## 4. Étagère Reprendre

- **Decision**: Construire la rangée dans l'interface, en tête. Une vidéo y entre si la position locale ou le pourcentage du compte est au-delà de 10 secondes et à plus de 30 secondes de la fin. Dédupliquer par id. Trier par le visionnage local le plus récent, puis par l'ordre du compte. Étendre `positions.json` avec le titre, la chaîne, la miniature et l'avatar connus au moment de la sauvegarde : aujourd'hui seuls `pos`, `dur` et `at` sont stockés, donc une vidéo vue ici mais absente du flux n'aurait pas de carte.
- **Rationale**: Le seuil reprend la règle déjà écrite dans `LocalStore.SavePosition` (à peine commencé / presque fini). Le flux d'accueil ne contient pas toutes les vidéos reprises.
- **Alternatives considered**: Déplacer seulement une étagère dont le titre ressemble à « Reprendre ». Rejeté : le titre dépend de la langue et rate les lectures faites uniquement sur ce téléviseur.

## 5. Carte, focus, avatars

- **Decision**: Ajouter sur l'élément de flux, quand la réponse les porte : avatar de chaîne, vérifié, badge (nouveau, première, direct), nombre d'éléments d'une playlist. La carte les affiche. La sélection devient un contour clair sur la miniature ; le bloc de texte ne s'inverse plus en gris clair. L'avatar se charge avec le même remplissage différé que la miniature (`UiWork`). S'il manque, la ligne meta reste alignée, sans trou.
- **Rationale**: `parse_video_renderer` et `parse_lockup` ne gardent pas l'avatar ni le badge vérifié. Le focus actuel (`CardInfoFocused`) contredit FR-006.
- **Alternatives considered**: Toujours afficher un avatar générique. Rejeté : la spec demande l'avatar seulement quand il existe. Charger toutes les images dès la construction de la carte. Rejeté : le coût déjà évité est le travail sur le fil de l'interface.

## 6. Recherche, filtres, recherches récentes

- **Decision**: Après confirmation, masquer le clavier et donner la grille à `FeedGrid` sur la hauteur libérée. Un contrôle « Modifier » rappelle le clavier. Les filtres sont un encodage protobuf des paramètres de recherche WEB (tri, date, type, durée, direct, HD, sous-titres), produit dans le cœur, pas une table de toutes les combinaisons. Les recherches confirmées (dix, plus récente d'abord) sont un fichier local `recent-searches.json`, effaçables une à une ou en bloc. Ce fichier n'est pas envoyé comme historique de compte.
- **Rationale**: FR-018 exige des combinaisons. Une table statique explose. L'encodage des filtres de recherche YouTube est stable (champ tri + message filtres). `ytcheck` devra montrer qu'une requête filtrée « durée > 20 min » ne renvoie pas un court.
- **Alternatives considered**: Un seul filtre à la fois. Rejeté : la spec permet de cumuler. Réutiliser l'historique YouTube comme recherches récentes. Rejeté : la spec les garde sur le téléviseur.

## 7. Guide, Explorer, recherche et compte

- **Decision**: Pas de barre du haut. Le rail actuel reste le guide : Accueil, Recherche, Abonnements, Vous, Explorer, sections personnelles, chaînes suivies, Compte, Réglages. Recherche et Compte satisfont FR-028. Explorer ouvre un écran liste vers les browse ids de `remote-config.json` (`explore`). Les chaînes suivies viennent de `youtubei/v1/guide` avec le client TV et le jeton. L'avatar du compte déjà renvoyé par `omega_auth_account` est affiché sur l'entrée Compte.
- **Rationale**: Un bandeau permanent réduit la grille 1920×1080 et duplique le rail. La spec demande que la recherche et le compte soient atteignables, pas qu'ils soient en haut de l'écran.
- **Alternatives considered**: Copier le bandeau du site (logo, champ, cloche, avatar). Rejeté : la cloche d'inbox est hors spec ; le champ duplique l'écran Recherche. Lister les chaînes seulement depuis les vidéos récentes d'abonnements. Rejeté : une chaîne sans vidéo récente disparaît.

## 8. Abonnements, bibliothèque, historique

- **Decision**: Abonnements : la liste chronologique déjà renvoyée par `FEsubscriptions` (les `items`, pas `highlights`), plus une bande d'avatars prise du guide. Filtres Non vues et En direct appliqués sur la liste chargée (`watched_percent == 0` et non présent dans les positions locales ; `live == true`). Vous : Historique (`FEhistory`), À regarder plus tard (playlist `WL`), Vidéos aimées (playlist `LL`), Playlists (browse TV `FElibrary`). `WL` et `LL` passent par `playlist_page`, déjà capable de `VL` + id.
- **Rationale**: Le cœur sait déjà paginer une playlist WEB. Les filtres d'abonnements de la spec n'exigent pas un second appel.
- **Alternatives considered**: Refaire `FEsubscriptions` en client WEB. Rejeté : on perdrait l'état de lecture du compte déjà parsé. Créer quatre entrées de rail au lieu d'un écran Vous. Rejeté : le rail dépasse déjà douze lignes visibles.

## 9. Actions de compte et « Pas intéressé »

- **Decision**: Ajouter, enregistrer dans une playlist et créer une playlist passent par l'édition de playlist du compte (`WL` pour À regarder plus tard, id choisi ou créé pour les autres). Historique : retirer une entrée, pause et effacement utilisent la commande portée par le bouton de la réponse `FEhistory`, renvoyée telle quelle. « Pas intéressé » et « Ne plus recommander cette chaîne » font pareil quand un jeton de menu est présent, et retirent toujours la carte d'un ensemble en mémoire tant que l'application reste ouverte. Sans compte, le menu d'actions de compte explique qu'il faut se connecter et n'affiche pas de succès.
- **Rationale**: FR-023 n'exige pas que le masquage survive à la fermeture. FR-021 et FR-024 exigent, eux, l'effet sur le compte. Les jetons de menu bougent moins si on les recopie que si on reconstruit l'URL.
- **Alternatives considered**: Masquer seulement en local, y compris À regarder plus tard. Rejeté : FR-021 exige le même compte que le site. Hardcoder les URL d'historique. Rejeté : elles changent sans que le JSON de config puisse les décrire entièrement.

## 10. Notifications d'abonnement

- **Decision**: Lire les trois états (Toutes, Personnalisées, Aucune) sur le bouton d'abonnement de la page chaîne et de la réponse « next » TV déjà utilisée pour `omega_yt_details`. L'interface envoie l'état choisi avec ce jeton. Le contrôle est visible pour un compte connecté dès que les trois états sont présents. S'ils manquent, l'écran le dit au lieu d'inventer un état.
- **Rationale**: Ces jetons sont par chaîne et par réponse. Les inventer enverrait une préférence à la mauvaise chaîne.
- **Alternatives considered**: Une inbox de notifications. Rejeté : hors spec. Cacher le contrôle sans message quand les jetons manquent. Rejeté : FR-031 exige le réglage.

## 11. Chaîne, playlist, aléatoire

- **Decision**: Étendre l'en-tête de chaîne : bannière, identifiant public, description courte, onglets. Les onglets réutilisent le browse WEB déjà en place, avec un `params` par onglet dans `remote-config.json`. Vidéos récentes = params actuel. Vidéos populaires = second params. En direct, Playlists et À propos sont des params ou le bloc « about » de la même réponse. La page playlist lit l'en-tête (illustration, propriétaire, nombre) en plus du titre. Aléatoire charge toutes les pages déjà paginées, mélange localement (Fisher-Yates), puis joue cette file. Une playlist d'une vidéo se comporte comme Lire tout. La vidéo en cours est l'id du lecteur si sa playlist est celle affichée.
- **Rationale**: Le mélange local respecte « chaque vidéo une fois » sans dépendre d'un paramètre shuffle instable. Le chargement complet est visible (attente), y compris sur une longue playlist.
- **Alternatives considered**: Mélanger seulement la première page. Rejeté : saute le reste, contredit FR-033. Demander à YouTube un ordre mélangé dont le paramètre n'est pas versionné ici. Rejeté : pas de preuve dans le client actuel ; le mélange local est vérifiable.

## 12. Lecteur : infos, fin, chapitres, description, file, partage

- **Decision**: Le bloc d'informations est un état du `PlayerScreen` déjà ouvert, pas un second lecteur. Il réutilise détails, suggestions (`omega_yt_related`) et description déjà renvoyée dans le JSON du lecteur. L'interrupteur de lecture automatique écrit `PlayerOptions.Autoplay`. La carte de fin remplace le compte à rebours actuel, avec annulation. Les chapitres viennent des marqueurs du player déjà téléchargé (`macroMarkers` / barre décorée), ajoutés à ce JSON. Les temps du texte de description sont reconnus dans l'interface (`h:mm:ss` ou `m:ss`) et ignorés s'ils dépassent la durée. La file est une liste en mémoire, vidée à la fermeture, prioritaire sur les suggestions. Partager réutilise `omega_qr_encode` sur `https://www.youtube.com/watch?v=<id>`. Les compteurs je n'aime pas restent ceux de Return YouTube Dislike.
- **Rationale**: Presque tout est déjà sur le lecteur. Un mini-lecteur est hors spec : Retour depuis le lecteur masque la vidéo et garde la progression sur la carte.
- **Alternatives considered**: Ouvrir les infos dans un écran qui détruit le lecteur. Rejeté : FR-011 interdit d'interrompre la lecture. Parser les chapitres seulement dans la description. Rejeté : les vidéos avec chapitres officiels mais sans temps dans le texte échoueraient SC-007.

## 13. Commentaires

- **Decision**: Nouvel appel cœur : première page via le endpoint `next` WEB (panneau commentaires), avec les jetons de tri « pertinents » et « récents » renvoyés par cette première page. Les réponses utilisent le jeton de continuation du commentaire. Aimer envoie la commande du bouton, pas un id reconstruit. Commentaires fermés = signal explicite de la réponse, distinct d'une erreur réseau. Pas de rédaction.
- **Rationale**: Le tri doit changer l'ordre réel (FR-037). Les deux jetons évitent de deviner un paramètre de tri.
- **Alternatives considered**: Ouvrir les commentaires dans le navigateur du téléviseur. Rejeté : sort du lecteur et de la télécommande. Réutiliser le endpoint TV `next` des suggestions. Rejeté : il ne porte pas le fil de commentaires du site.

## 14. Preuve sans nouveau framework de test

- **Decision**: Étendre `core/tools/ytcheck` pour imprimer les nouveaux JSON (accueil anonyme, filtres, guide, WL, LL, commentaires, chapitres). Les fonctions pures (encodage des filtres, lecture d'un temps dans une description, seuil Reprendre) ont une vérification appelable par `ytcheck --selfcheck`, qui échoue avec un code non nul. L'interface se valide sur la télévision selon `quickstart.md`. Pas de projet de test C#.
- **Rationale**: Le dépôt n'a pas de tests C#. Ajouter un framework pour cette feature seule contredit le principe du plus petit changement. Le cœur a déjà `ytcheck` pour le même code que la télévision.
- **Alternatives considered**: Projet NUnit pour l'UI. Rejeté : rien ne lance de test C# aujourd'hui, et l'UI NUI ne s'exécute pas sur l'hôte. Se fier uniquement à un essai manuel. Rejeté : l'encodage des filtres et les temps de description sont des règles faciles à casser en silence.

## 15. Ce qui ne sera pas fait

Lecture verticale des courts, aperçu dans la miniature, mini-lecteur, chat de direct, posts de communauté, pourboires, clips, halo ambiant, rédaction d'un commentaire, inbox de notifications. Le rail ne devient pas un bandeau de site.

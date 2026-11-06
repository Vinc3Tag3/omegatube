# Data model: Parité de navigation et de lecture avec YouTube

Modèle vu de l'interface. Les noms techniques entre parenthèses indiquent où l'état vit. Les règles reprennent la spec. Détail des JSON : [contracts/shell-json.md](contracts/shell-json.md).

## ContentCard

Une vidéo, une playlist ou un mix dans une liste.

| Champ | Règle |
| --- | --- |
| id | Obligatoire pour ouvrir la lecture. Vide = entrée ignorée. |
| kind | `video`, `playlist` ou `mix`. |
| playlistId | Vide pour une vidéo hors liste. Une playlist ou un mix en a un. |
| title | Deux lignes maximum à l'écran. |
| channel, views, published | Affichés seulement s'ils sont non vides. |
| channelAvatar | Affiché seulement s'il est non vide. Pas d'image de remplacement. |
| verified | Marque à côté du nom seulement si vrai. |
| badge | `none`, `live`, `new`, `premiere`. `live` remplace la durée. |
| duration | Masquée si `badge` est `live`. |
| itemCount | Playlists : affiché quand il est connu (> 0). |
| watchedPercent | 0 à 100. Barre si > 0. |
| feedbackTokens | Optionnels. Servent à « Pas intéressé », « Ne plus recommander », retirer de l'historique. Absents = l'action compte n'est pas proposée, le masquage local de session reste possible pour les deux premières. |

Relation : une carte appartient à une étagère, une grille, une file ou un résultat de recherche.

## ResumeEntry

Vidéo de l'étagère « Reprendre ».

- Incluse si la position est > 10 s et qu'il reste > 30 s. Sinon exclue.
- Source : `positions.json` (local) et `watchedPercent` des cartes du compte. Même id = une seule carte.
- Ordre : `at` local décroissant, puis l'ordre d'apparition dans le flux.
- Si la liste est vide, l'étagère n'existe pas.
- Les champs d'affichage manquants (titre, miniature) empêchent d'inventer une carte : l'entrée locale sans titre n'est pas montrée tant qu'un flux ne l'a pas complétée. Dès la prochaine sauvegarde de position, le titre connu est écrit avec la position.

Transition : regarder jusqu'à la fin (moins de 30 s restantes) retire la vidéo de Reprendre et laisse la barre pleine sur sa carte.

## HomeChip

Pastille d'accueil.

- `id` stable, `label` affiché, `target` = browse id ou « tout ».
- « Tout » n'a pas de cible réseau : il réaffiche le flux principal.
- Une pastille dont le chargement échoue montre l'erreur et Réessayer, pas le flux d'une autre pastille.
- Une pastille vide montre l'état vide.

## Search

- Requête courante (texte).
- Filtres cumulables : date (`any`, `hour`, `today`, `week`, `month`, `year`), durée (`any`, `short`, `medium`, `long`), type (`any`, `video`, `channel`, `playlist`), tri (`relevance`, `date`, `views`), booléens direct, HD, sous-titres. Défaut : tout à `any` / faux / pertinence.
- Résultats : liste de ContentCard, plus des cartes chaîne (id chaîne, nom, avatar, abonnés) qui ne sont pas des vidéos.
- Recherches récentes : au plus 10 chaînes, ordre antéchronologique, fichier local. Confirmer une requête déjà présente la remonte en tête. Retirer une entrée ou vider la liste n'appelle pas le compte.

Transition d'écran : `keyboard` (saisie) → `results` (clavier caché, grille pleine) → retour à `keyboard` sans perdre la requête ni les filtres.

## GuideEntry

Entrée du rail.

- `kind` : `home`, `search`, `subscriptions`, `you`, `explore`, `custom`, `channel`, `account`, `settings`.
- `custom` reprend `HomeRow` existant (titre, requête, icône, visible). Rien n'est effacé.
- `channel` : id, nom, avatar. Ouvre la page chaîne.
- Ordre fixe : Accueil, Recherche, Abonnements, Vous, Explorer, sections `custom` visibles, chaînes suivies, Compte, Réglages.

## Library

Pour un compte connecté.

| Liste | Source | Persistance |
| --- | --- | --- |
| Historique | `FEhistory`, groupes = titres de section de la réponse | Compte. Pause = plus aucun ajout. Effacement après confirmation. |
| À regarder plus tard | Playlist `WL` | Compte |
| Vidéos aimées | Playlist `LL` | Compte |
| Playlists | `FElibrary` | Compte. Créer donne un nom non vide. |

Sans compte, ouvrir une action de ces listes ne change aucun état et affiche la demande de connexion.

Groupe d'historique : titre (Aujourd'hui, Hier, ou le libellé renvoyé) + cartes. Une recherche interne filtre les titres de vidéos dans ces groupes. Un groupe devenu vide disparaît.

## Subscription

- Chaîne suivie : id, nom, avatar, abonné oui/non.
- Notifications : `all`, `personalized`, `none`, ou `unknown` si les jetons manquent.
- Changer de niveau exige les trois jetons de la réponse. Succès = le niveau affiché devient celui demandé. Échec = l'ancien niveau reste, avec le message d'erreur.

## ChannelPage

- En-tête : nom, identifiant public, avatar, bannière optionnelle, abonnés, description courte, état d'abonnement, notifications.
- Onglet actif : `home`, `videos`, `live`, `playlists`, `about`.
- Tri des vidéos : `newest` (défaut) ou `popular`. Les autres onglets n'ont pas de tri.
- Onglet sans contenu : message vide, l'en-tête reste.

## PlaylistPage

- Id, titre, propriétaire, illustration, nombre, cartes numérotées dans l'ordre renvoyé.
- `playingVideoId` optionnel : mis en évidence si la lecture en cours a le même playlist id.
- Mode lecture : `ordered` ou `shuffled`. Shuffled = toutes les pages chargées, puis permutation. Une seule vidéo : identique à `ordered`.
- Enregistrer : même action que le menu de carte, vers une playlist du compte (pas vers `WL`, sauf si l'utilisateur choisit À regarder plus tard).

## Playback

État du lecteur, une seule vidéo affichée.

- `autoplay` : réglage mémorisé. L'interrupteur à côté de « À suivre » lit et écrit ce même booléen.
- Suggestions : liste verticale, les trois premières visibles quand il en existe au moins trois.
- File : liste ordonnée d'ids en mémoire. Vide à chaque ouverture de l'application. « Suivante » et fin de vidéo consomment la tête de file avant les suggestions.
- Fin : `playing` → `ended`. Si lecture automatique et (file ou suggestion) : carte « À suivre » + compte à rebours. Annuler ou couper la lecture automatique → rester sur `ended`. Direct : pas de compte à rebours.
- Chapitre : titre + début en millisecondes, 0 ≤ début ≤ durée. Liste triée. Aucun chapitre inventé.
- Lien de description : destination `video`, `channel`, `playlist`, `search` (mot-dièse), ou `external` (partage par code).
- Partage : URL `https://www.youtube.com/watch?v=` + id, rendue en code.

Transition Retour : depuis une page ouverte au-dessus du lecteur, retour au lecteur à ± 1 s. Depuis le lecteur, la vidéo disparaît (pas de lecture réduite) et la carte garde la progression.

## Comment

- id, auteur, texte, âge, nombre de j'aime, aimé par le compte, présence de réponses, jeton de réponse, jeton de j'aime.
- Tri de la page : `top` ou `newest`, chacun avec le jeton renvoyé à l'ouverture.
- État de la zone : `ready`, `disabled`, `failed`. `disabled` n'a pas de bouton Réessayer. `failed` en a un.
- Aimer exige un compte. Sans compte : pas de changement de compteur.

## SessionHide

Ensemble en mémoire, vidé à la fermeture.

- Vidéos masquées par « Pas intéressé ».
- Chaînes masquées par « Ne plus recommander ».
- Appliqué à la liste affichée et à son prochain affichage tant que l'application reste ouverte.
- Ne remplace pas un succès de playlist ou d'historique : ces actions doivent aussi réussir côté compte.

## Réglages inchangés

Qualité, codecs, sous-titres, piste audio, SponsorBlock, langue, région, recherche de mise à jour, sections personnelles. Cette feature ne change pas leur signification. `Autoplay` existant est la valeur montrée à côté de « À suivre ».

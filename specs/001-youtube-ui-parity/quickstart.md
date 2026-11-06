# Quickstart: valider la parité YouTube

Preuve manuelle et appels hôte. Pas un plan d'implémentation. Contrats : [shell-json.md](contracts/shell-json.md), [screens.md](contracts/screens.md). Règles d'état : [data-model.md](data-model.md).

## Prérequis

- Dépôt construit comme dans le README, section Development.
- Outils hôte du cœur (même code YouTube que la télévision) :

```sh
cmake -S core -B build/host -DOMEGA_HOST_TOOLS=ON && cmake --build build/host -j
```

- Pour l'interface : télévision Tizen 6 ou plus récente, en mode développeur, puis `scripts/build-app.sh` et `TV_IP=<ip> scripts/deploy.sh`.
- Un compte YouTube de test, avec au moins une chaîne suivie, une vidéo dans À regarder plus tard, une vidéo aimée, une playlist, et une vidéo commencée non finie.

`ytcheck --selfcheck` doit sortir 0 avant le reste. Il couvre l'encodage des filtres de recherche, la lecture des temps dans une description, et le seuil de l'étagère Reprendre.

## Hôte, sans télévision

| Commande | Résultat attendu |
| --- | --- |
| `build/host/ytcheck --selfcheck` | Code 0. |
| `build/host/ytcheck --home-anon` | Au moins une vidéo, sans jeton. Aucun `signed out`. |
| `build/host/ytcheck --search "musique" --duration long` | Résultats dont la durée affichée n'est pas un court (< 4 min) quand la durée est connue. |
| `build/host/ytcheck --signin` puis guide | Liste de chaînes avec id et nom. |
| `build/host/ytcheck --playlist-page WL` | Vidéos, ou liste vide explicite si le compte n'en a pas. |
| `build/host/ytcheck --playlist-page LL` | Même chose pour les vidéos aimées. |
| `build/host/ytcheck --comments <videoId>` | `state` `ready` avec au moins un commentaire, ou `disabled` si le fil est fermé. Une vidéo connue ouverte et une vidéo à commentaires fermés couvrent les deux. |
| `build/host/ytcheck <videoId>` | Le JSON lecteur contient `description`. `chapters` est un tableau, vide seulement si la vidéo n'a pas de chapitres. |

Un browse id d'Explorer qui échoue se corrige dans `remote-config.json`, puis la commande est relancée. Ne pas valider un écran Explorer tant que son id ne renvoie pas un flux ou un vide métier (zéro élément sans `error`).

## Télévision

Faire les parcours dans l'ordre. Chaque ligne est indépendante une fois le build installé.

1. **Sans compte** : ouvrir l'accueil. Au moins une rangée de vraies cartes. Pas un écran qui ne propose que de se connecter.
2. **Cartes** : sur une vidéo classique, lire sans l'ouvrir le titre (deux lignes maximum), la chaîne, les vues, l'âge, la durée. Sélection : contour sur la miniature, texte toujours clair sur fond sombre. Playlist et direct reconnaissables sans lecture.
3. **Attente** : couper le réseau, ouvrir une liste. Des emplacements de cartes, puis un message et Réessayer. Rétablir le réseau et réessayer réussit.
4. **Reprendre** : avec une vidéo commencée (plus de 10 s, pas dans les 30 dernières secondes), elle est dans la première rangée. La terminer la retire de cette rangée.
5. **Pastilles** : choisir une pastille autre que Tout change les rangées. Tout les restaure. Une pastille vide ne montre pas un autre thème.
6. **Recherche** : confirmer une requête. Le clavier disparaît, la grille occupe l'écran, une confirmation ouvre la première vidéo. Appliquer « plus de 20 minutes » et vérifier le résultat. Retirer le filtre restaure la requête. Une recherche récente réapparaît ; la retirer suffit à ce qu'elle ne revienne pas.
7. **Guide** : Accueil, Recherche, Abonnements, Vous, Explorer, une section personnelle déjà créée, une chaîne suivie, Compte, Réglages. La section personnelle est encore là après redémarrage.
8. **Vous** : la vidéo ajoutée à À regarder plus tard depuis le menu maintenu sur une carte est dans cette liste, et la même liste sur le site web du même compte la montre aussi.
9. **Historique** : groupes datés, retrait d'une seule entrée, pause (une nouvelle lecture ne s'ajoute pas), effacement seulement après confirmation.
10. **Abonnements** : bande d'avatars, ordre du plus récent au plus ancien, filtre En direct vide avec son propre message s'il n'y a pas de direct.
11. **Chaîne** : bannière si la chaîne en a une, identifiant, onglets, tri populaire différent du tri récent sur une chaîne qui a les deux. Notifications : les trois niveaux, puis le même niveau vu sur une vidéo de cette chaîne.
12. **Playlist** : Lire tout suit l'ordre numéroté. Aléatoire, sur une playlist d'au moins trois vidéos, ne suit pas cet ordre et ne répète pas une vidéo avant d'avoir proposé les autres déjà chargées. Revenir à la page pendant la lecture marque la vidéo en cours.
13. **Lecteur** : ouvrir les informations sans pause. Titre, chaîne, abonnement, nombre de je n'aime pas si connu, au moins trois suggestions s'il en existe trois, interrupteur de lecture automatique. Le couper, finir la vidéo : rien ne démarre. Le rallumer : carte de fin, annulation empêche la suite.
14. **Chapitres** : sur une vidéo qui en a, un chapitre démarre en au plus trois actions, au bon moment (± 1 s).
15. **Commentaires** : premier commentaire lu, tri changé, réponse dépliée, j'aime refusé sans compte et compté avec compte. Vidéo à commentaires fermés : message dédié, pas une liste vide.
16. **Retour** : ouvrir la chaîne depuis le lecteur, Retour, la lecture reprend au même instant (± 1 s). Retour encore : plus d'image vidéo, la carte garde la barre de progression.
17. **File** : deux vidéos ajoutées, réordonnées, lues dans l'ordre affiché. Relancer l'application : la file est vide.

## Hors parcours

Ne pas exiger : courts métrages, aperçu dans la miniature, mini-lecteur, chat, posts, pourboires, clips, halo, rédaction d'un commentaire.

## Échec

Si une commande hôte échoue sur un browse id de config, corriger le JSON et recommencer cette commande. Si elle échoue sur un champ de contrat (chapitre, groupe d'historique, jeton de notification), ne pas valider l'écran correspondant : le parseur est incomplet par rapport à [shell-json.md](contracts/shell-json.md).

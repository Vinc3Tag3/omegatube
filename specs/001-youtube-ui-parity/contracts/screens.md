# Contrat: écrans et télécommande

Une seule vidéo est visible à la fois. Pas de miniature animée, pas de lecture réduite au-dessus des listes. Confirmation courte = action principale. Maintenir la confirmation sur une carte (répétition de touche) ouvre le menu d'actions. Retour ferme le menu, puis l'écran du dessus, puis demande de quitter depuis l'accueil, comme aujourd'hui.

Les libellés passent par les trois langues déjà supportées (anglais source, français, espagnol).

## Accueil

- Première rangée : « Reprendre » si au moins une vidéo éligible, sinon la première étagère du flux.
- Pastilles au-dessus des rangées. Une seule sélectionnée. « Tout » est la première et le défaut.
- Attente : emplacements de cartes, pas seulement une phrase. Erreur : phrase + réessayer, emplacements retirés.
- Carte sélectionnée : contour clair sur la miniature, texte inchangé (clair sur fond sombre).
- Confirmation : lecture, page playlist, ou page chaîne selon `kind`.
- Menu (maintien) : À regarder plus tard, Enregistrer dans une playlist, Pas intéressé, Ne plus recommander cette chaîne, Ajouter à la file. Sans compte, les trois premières actions de compte affichent « il faut se connecter » et ne ferment pas sur un succès.

## Recherche

- Ouverture : recherches récentes, puis suggestions, clavier visible.
- Confirmation de la requête : clavier masqué, grille de résultats, contrôle pour réafficher le clavier sans effacer le texte.
- Filtres accessibles depuis les résultats, cumulables, avec un état « aucun filtre » qui restaure la même requête.
- Carte chaîne → page chaîne. Carte playlist → page playlist. Carte vidéo → lecteur.
- Zéro résultat : message, filtres et clavier rappelables.

## Guide

Ordre des entrées : Accueil, Recherche, Abonnements, Vous, Explorer, sections personnelles visibles, chaînes suivies (avatar + nom), Compte (avatar du compte), Réglages.

- Vous ouvre Historique, À regarder plus tard, Vidéos aimées, Playlists.
- Explorer ouvre Tendances, Musique, Jeux, Actualités, Sport, Savoir, Podcasts.
- Une chaîne du guide ouvre sa page.
- Compte et Recherche restent des entrées du guide sur tous les écrans de consultation. Pas de second bandeau.

## Abonnements

Bande d'avatars (chaînes du guide), puis vidéos de la plus récente à la plus ancienne. Filtres : Tout, Non vues, En direct. Un filtre sans vidéo a son propre message vide.

## Historique

Groupes avec titre. Recherche interne. Retirer une entrée ne retire pas les autres. Pause visible. Effacer n'agit qu'après une confirmation explicite ; avant ça, Retour annule.

## Chaîne

En-tête (bannière si présente, avatar, nom, identifiant, abonnés, description courte, s'abonner, notifications). Onglets : Accueil, Vidéos, En direct, Playlists, À propos. Vidéos : tri plus récentes / plus populaires. Notifications : Toutes, Personnalisées, Aucune, aussi depuis le bloc d'informations du lecteur de cette chaîne. Si les jetons manquent, le contrôle le dit.

## Playlist

Illustration, propriétaire, nombre, Lire tout, Aléatoire, Enregistrer, liste numérotée. La vidéo en cours de cette playlist est marquée au retour sur la page. Aléatoire attend la fin du chargement avant de démarrer, et le dit.

## Lecteur

États du bas vers le haut, sans détruire la lecture :

1. Contrôles actuels (défilement, actions, sous-titres, qualité).
2. Bloc d'informations : titre, chaîne, abonnement, notifications, j'aime, je n'aime pas avec le nombre quand il est connu, aperçu de description, interrupteur Lecture automatique, liste verticale À suivre.
3. Description dépliée : texte complet, temps et mots-dièse sélectionnables, liens internes ouverts dans l'application, autres liens proposés en code à scanner.
4. Chapitres : marques sur la barre + liste titre et temps. Confirmation cherche à ce temps (± 1 s).
5. File : ordre visible, ajouter, retirer, réordonner. Tête de file prioritaire sur À suivre.
6. Commentaires : liste, tri pertinents / récents, réponses dépliables, j'aime si connecté. Fermés = message dédié. Échec = réessayer.
7. Partage : code de l'URL de la vidéo.
8. Fin de vidéo à la demande : grande carte À suivre, compte à rebours, annulation. Lecture automatique éteinte = pas de démarrage. Un direct qui se termine ne lance pas ce compte à rebours.

Ouvrir une chaîne ou une playlist depuis le lecteur empile la page. Retour revient au lecteur au même instant (± 1 s). Retour depuis le lecteur lui-même ferme la vidéo.

## Échecs communs

- Réseau : message + réessayer, pas une liste d'un autre écran.
- Action de compte refusée : message, état précédent conservé (abonnement, j'aime d'un commentaire, contenu de playlist).
- Suggestion suivante illisible : message et possibilité de choisir une autre entrée ou de revenir. Pas d'écran bloqué sans Retour.

# Feature Specification: Parité de navigation et de lecture avec YouTube

**Feature Branch**: `001-youtube-ui-parity`

**Created**: 2026-11-04

**Status**: Draft

**Input**: User description: "reprend les élément énoncé et spécifie les mieux" — spécifier les améliorations d'interface les plus utiles pour rapprocher OmegaTube de l'expérience du site YouTube, en gardant une application télévisée pilotée à la télécommande.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Reconnaître l'accueil YouTube (Priority: P1)

Un téléspectateur ouvre l'accueil et y retrouve la lecture visuelle du site : chaque vidéo se comprend sans l'ouvrir, l'accueil n'est pas vide sans compte, et les vidéos commencées reviennent en premier.

**Why this priority**: C'est le premier écran. Sans cartes et sans accueil lisibles, le reste de l'application ne ressemble pas au site, même si la lecture fonctionne déjà.

**Independent Test**: Ouvrir l'accueil, connecté puis non connecté, et vérifier les cartes, la sélection, le chargement, l'étagère « Reprendre » et les pastilles de filtre, sans passer par la recherche ni le lecteur.

**Acceptance Scenarios**:

1. **Given** une vidéo classique avec miniature, titre, chaîne, vues, âge, durée et avatar de chaîne, **When** elle apparaît dans une liste, **Then** le téléspectateur voit la miniature, la durée, au plus deux lignes de titre, le nom de la chaîne, le nombre de vues, l'âge et l'avatar, sans ouvrir la vidéo.
2. **Given** une chaîne indiquée comme vérifiée, **When** sa vidéo est affichée, **Then** une marque « vérifiée » est visible à côté du nom.
3. **Given** une playlist ou un mix, **When** sa carte est affichée, **Then** elle se distingue d'une vidéo seule (marque de playlist ou de mix, et nombre d'éléments quand il est connu).
4. **Given** une vidéo marquée nouvelle, en première ou en direct, **When** sa carte est affichée, **Then** le badge correspondant remplace ou accompagne la durée, et une vidéo en direct ne montre pas une durée ordinaire.
5. **Given** une vidéo déjà commencée, **When** sa carte est affichée, **Then** une barre de progression montre la part déjà regardée.
6. **Given** le focus sur une carte, **When** le téléspectateur se déplace, **Then** une seule carte est sélectionnée, signalée par un contour clair autour de la miniature, et le texte de la carte reste lisible.
7. **Given** un accueil dont le contenu n'est pas encore arrivé, **When** l'écran s'ouvre, **Then** des emplacements de cartes vides sont visibles à la place d'un simple texte d'attente, puis laissés place aux vraies cartes.
8. **Given** un compte avec au moins une vidéo commencée et non terminée, **When** l'accueil s'ouvre, **Then** une étagère « Reprendre » est la première rangée et ne contient que ces vidéos.
9. **Given** l'accueil affiché, **When** le téléspectateur choisit une pastille autre que « Tout », **Then** les rangées se limitent à ce thème sans quitter l'accueil, et « Tout » restaure l'ensemble.
10. **Given** aucun compte connecté, **When** l'accueil s'ouvre, **Then** des recommandations publiques sont visibles, et l'écran n'est pas seulement une invitation à se connecter.
11. **Given** une carte sélectionnée, **When** le téléspectateur la confirme puis fait retour, **Then** la lecture démarre, et le retour ramène à la liste sur cette même carte. Quitter le lecteur masque la vidéo ; la progression déjà regardée reste sur la carte.

---

### User Story 2 - Lire une vidéo avec le contexte du site (Priority: P1)

Pendant la lecture, le téléspectateur retrouve le bloc d'informations du site : titre, chaîne, abonnement, suite proposée, lecture automatique visible, et une grande carte de fin qu'il peut annuler.

**Why this priority**: Le temps passé est sur la vidéo. Le site garde ces informations autour du lecteur ; aujourd'hui elles sont surtout dans une barre de contrôle.

**Independent Test**: Lancer une vidéo avec des suggestions, ouvrir le bloc d'informations, couper la lecture automatique, laisser la vidéo finir, et vérifier que l'image continue pendant la consultation.

**Acceptance Scenarios**:

1. **Given** une vidéo en lecture, **When** le téléspectateur ouvre les informations, **Then** il voit le titre, la chaîne (avatar, nom, abonnés quand ils sont connus), l'état d'abonnement, l'état j'aime / je n'aime pas, un aperçu de la description, et une liste verticale « À suivre », sans que la lecture s'arrête.
2. **Given** des suggestions disponibles, **When** « À suivre » est affichée, **Then** au moins trois entrées sont proposées quand il en existe au moins trois, chacune avec miniature, titre et chaîne.
3. **Given** le bloc « À suivre » visible, **When** le téléspectateur change « Lecture automatique », **Then** l'interrupteur affiché et le réglage mémorisé changent ensemble, et restent dans cet état à la prochaine vidéo.
4. **Given** la lecture automatique allumée et une suggestion disponible, **When** la vidéo se termine, **Then** une grande carte « À suivre » apparaît avec un compte à rebours, et la suggestion démarre seulement à la fin du compte à rebours.
5. **Given** cette carte de fin visible, **When** le téléspectateur annule, **Then** la suggestion ne démarre pas et il reste sur la vidéo terminée, avec la possibilité de la relancer.
6. **Given** la lecture automatique éteinte, **When** la vidéo se termine, **Then** aucune vidéo suivante ne démarre seule.
7. **Given** une vidéo en cours, **When** le téléspectateur demande de la partager, **Then** un code affichable au téléphone ouvre cette vidéo sur le site YouTube.
8. **Given** une vidéo dont le nombre de « je n'aime pas » est connu, **When** le bloc d'informations est ouvert, **Then** ce nombre reste affiché avec « je n'aime pas ».

---

### User Story 3 - Chercher comme sur le site (Priority: P2)

Le téléspectateur saisit une recherche, retrouve ses recherches récentes, puis obtient une grille de résultats pleine page, mélangeant vidéos, chaînes et playlists, qu'il peut filtrer.

**Why this priority**: La recherche est le second parcours quotidien. Une seule rangée sous le clavier ne permet pas de parcourir les résultats comme sur le site.

**Independent Test**: Depuis l'entrée Recherche, lancer une requête connue, appliquer un filtre, ouvrir un résultat de chaque type, sans modifier l'accueil ni le guide.

**Acceptance Scenarios**:

1. **Given** l'écran de recherche avant toute saisie, **When** il s'ouvre et que des recherches récentes existent sur ce téléviseur, **Then** elles sont proposées avant les suggestions de la saisie en cours.
2. **Given** une requête confirmée, **When** les résultats arrivent, **Then** le clavier se range et les résultats occupent l'écran en grille ; le téléspectateur peut rappeler le clavier pour modifier la requête.
3. **Given** des résultats de types différents, **When** la grille s'affiche, **Then** une vidéo, une chaîne et une playlist ont chacune une présentation distincte, et la confirmation ouvre la bonne destination (lecture, page chaîne ou page playlist).
4. **Given** une grille de résultats, **When** le téléspectateur filtre par date de mise en ligne, durée, type, tri, direct, haute définition ou sous-titres, **Then** la grille ne garde que les résultats compatibles, et retirer les filtres restaure la recherche d'origine.
5. **Given** une recherche sans résultat, **When** la page s'affiche, **Then** un message le dit clairement et le téléspectateur peut modifier la requête ou les filtres.
6. **Given** des recherches récentes, **When** le téléspectateur en retire une ou vide la liste, **Then** ces entrées ne réapparaissent pas à la prochaine ouverture de la recherche.

---

### User Story 4 - Retrouver abonnements, historique et bibliothèque (Priority: P2)

Un téléspectateur connecté parcourt ses abonnements comme sur le site, range une vidéo pour plus tard, retrouve ses playlists et son historique, et retire ce qui ne l'intéresse pas.

**Why this priority**: Le site organise le compte autour de « Vous » et des abonnements. Sans ces destinations, les actions de la carte n'ont nulle part où aboutir.

**Independent Test**: Compte connecté avec abonnements, historique et au moins une playlist. Ouvrir Abonnements et Vous, enregistrer une vidéo, la retrouver, masquer une recommandation.

**Acceptance Scenarios**:

1. **Given** un compte avec des chaînes suivies, **When** Abonnements s'ouvre, **Then** une bande d'avatars de chaînes est proposée, puis les vidéos de la plus récente à la plus ancienne, avec les filtres Tout, Non vues et En direct.
2. **Given** ce filtre « Non vues » ou « En direct », **When** il est choisi, **Then** seules les vidéos correspondantes restent, et une bande vide de résultat explique l'absence de vidéos plutôt que de réutiliser un autre filtre en silence.
3. **Given** le guide d'un compte connecté, **When** le téléspectateur ouvre Vous, **Then** il atteint Historique, À regarder plus tard, Vidéos aimées et Playlists.
4. **Given** une carte de vidéo et un compte connecté, **When** il ouvre le menu de la carte, **Then** il peut ajouter à À regarder plus tard, enregistrer dans une playlist existante ou nouvelle, marquer Pas intéressé, choisir Ne plus recommander cette chaîne, ou ajouter à la file de lecture.
5. **Given** une vidéo ajoutée à À regarder plus tard, **When** il ouvre cette liste sur ce téléviseur, **Then** la vidéo y figure, y compris si elle a été ajoutée depuis le site avec le même compte.
6. **Given** « Pas intéressé » ou « Ne plus recommander cette chaîne », **When** l'action réussit, **Then** la carte disparaît de la liste en cours et ne revient pas au prochain affichage de cette liste tant que l'application reste ouverte.
7. **Given** l'historique du compte, **When** il s'affiche, **Then** les vidéos sont groupées par jour (au minimum Aujourd'hui, Hier, puis les jours ou semaines plus anciens), une recherche interne filtre ces groupes, et une entrée peut être retirée sans effacer le reste.
8. **Given** l'historique, **When** le téléspectateur met l'historique en pause ou demande à l'effacer, **Then** l'effacement exige une confirmation explicite, et la pause empêche les nouveaux visionnages de s'y ajouter jusqu'à reprise.
9. **Given** aucun compte connecté, **When** une action de bibliothèque est demandée, **Then** l'application explique qu'il faut se connecter et ne présente pas l'action comme réussie.

---

### User Story 5 - Circuler avec le guide du site (Priority: P2)

Le téléspectateur se déplace par un guide du même découpage que le site : Accueil, Abonnements, Vous, Explorer, et les chaînes suivies, avec la recherche et le compte toujours atteignables.

**Why this priority**: Le découpage du site est le moyen de trouver Musique, Tendances ou une chaîne suivie sans créer une recherche à la main. Il vient après les écrans de contenu, qui restent utilisables avec le menu actuel.

**Independent Test**: Ouvrir le guide, entrer dans chaque entrée d'Explorer, ouvrir une chaîne suivie depuis le guide, et atteindre Recherche et le compte sans passer par un écran intermédiaire imprévu.

**Acceptance Scenarios**:

1. **Given** le guide, **When** il est affiché, **Then** il propose Accueil, Abonnements, Vous et Explorer, plus les sections personnelles déjà choisies par le téléspectateur.
2. **Given** un compte connecté avec des chaînes suivies, **When** le guide est ouvert, **Then** ces chaînes y apparaissent avec avatar et nom, et la confirmation ouvre la page de cette chaîne.
3. **Given** Explorer, **When** le téléspectateur en parcourt les entrées, **Then** Tendances, Musique, Jeux, Actualités, Sport, Savoir et Podcasts sont proposées et ouvrent chacune une liste de contenus de ce thème.
4. **Given** un écran de consultation (accueil, abonnements, explorer ou bibliothèque), **When** le téléspectateur cherche le champ de recherche ou son compte, **Then** les deux sont atteignables sans quitter l'application ni ouvrir les réglages.
5. **Given** une section personnelle déjà présente avant cette évolution, **When** le guide est reconstruit, **Then** cette section reste accessible et son contenu n'est pas effacé.

---

### User Story 6 - Ouvrir une chaîne ou une playlist (Priority: P3)

Le téléspectateur ouvre une chaîne ou une playlist et y trouve la même structure que sur le site : identité de la chaîne, onglets, tri, et pour une playlist la lecture, l'aléatoire et l'enregistrement.

**Why this priority**: Ces pages complètent la navigation. L'application a déjà une page chaîne simple et une page playlist avec lecture intégrale ; les rapprocher du site apporte du confort une fois les parcours principaux en place.

**Independent Test**: Ouvrir une chaîne connue et une playlist connue depuis une carte, parcourir les onglets et le tri, lancer la lecture aléatoire, sans dépendre des commentaires ni des chapitres.

**Acceptance Scenarios**:

1. **Given** une chaîne, **When** sa page s'ouvre, **Then** elle montre la bannière quand elle existe, l'avatar, le nom, l'identifiant public, le nombre d'abonnés, un bouton d'abonnement et une description courte.
2. **Given** cette page, **When** le téléspectateur parcourt les onglets, **Then** Accueil, Vidéos, En direct, Playlists et À propos sont disponibles ; Vidéos peut être triée par plus récentes ou plus populaires.
3. **Given** un compte connecté sur cette chaîne, **When** il règle les notifications d'abonnement, **Then** il peut choisir Toutes, Personnalisées ou Aucune, et le choix est visible aussi depuis la vidéo de cette chaîne.
4. **Given** une playlist, **When** sa page s'ouvre, **Then** elle montre une illustration, le propriétaire, le nombre de vidéos, Lire tout, Aléatoire et Enregistrer, puis la liste numérotée.
5. **Given** cette playlist lancée, **When** le téléspectateur revient à la page pendant la lecture, **Then** la vidéo en cours est mise en évidence dans la liste.
6. **Given** Aléatoire sur une playlist de plusieurs vidéos, **When** la lecture démarre, **Then** l'ordre n'est pas celui de la liste d'origine, et chaque vidéo de la playlist reste proposée une fois avant répétition.

---

### User Story 7 - Chapitres, description et file (Priority: P3)

Pendant la lecture, le téléspectateur utilise les chapitres et les temps cités dans la description, déplie la description, et décide de la suite via une file de lecture.

**Why this priority**: Ce sont les outils de lecture du site au-delà de la simple suggestion. Ils sont utiles, mais l'application reste compréhensible sans eux grâce au bloc « À suivre ».

**Independent Test**: Lire une vidéo qui possède des chapitres et des temps dans la description, chercher un chapitre, ajouter deux vidéos à la file, réordonner, et vérifier l'ordre de lecture.

**Acceptance Scenarios**:

1. **Given** une vidéo avec chapitres, **When** la barre de progression est affichée, **Then** chaque chapitre y est marqué, et une liste des chapitres indique le titre et le temps de départ.
2. **Given** cette liste ou un temps sélectionnable dans la description, **When** le téléspectateur le confirme, **Then** la lecture reprend à ce moment, à une seconde près.
3. **Given** une description plus longue que l'aperçu, **When** le téléspectateur la déplie, **Then** le texte complet tient à l'écran par défilement, et les liens et mots-dièse présents sont sélectionnables.
4. **Given** un mot-dièse confirmé, **When** l'action aboutit, **Then** une recherche de ce mot s'ouvre. **Given** un lien confirmé, **When** l'action aboutit, **Then** le téléspectateur voit la destination et peut l'ouvrir dans le parcours adapté (vidéo, chaîne, playlist ou recherche) quand la destination est une page YouTube connue ; sinon il peut la partager vers le téléphone.
5. **Given** une file contenant plusieurs vidéos, **When** la vidéo en cours se termine ou que le téléspectateur demande la suivante, **Then** la prochaine entrée de la file passe avant les suggestions.
6. **Given** la file affichée, **When** le téléspectateur ajoute, retire ou réordonne une entrée, **Then** l'ordre affiché est celui qui sera lu tant que l'application reste ouverte, et la file est vide à la prochaine ouverture.

---

### User Story 8 - Lire les commentaires (Priority: P3)

Le téléspectateur ouvre les commentaires de la vidéo en cours, change le tri, lit les réponses et aime un commentaire avec son compte.

**Why this priority**: C'est le plus grand manque par rapport au site, mais ce n'est pas nécessaire pour trouver et regarder une vidéo. Il vient en dernier parmi les éléments retenus.

**Independent Test**: Ouvrir les commentaires d'une vidéo qui en a, changer le tri, déplier une réponse, aimer un commentaire connecté, puis ouvrir une vidéo dont les commentaires sont fermés.

**Acceptance Scenarios**:

1. **Given** une vidéo dont les commentaires sont ouverts, **When** le téléspectateur ouvre les commentaires, **Then** il voit pour chaque commentaire affiché l'auteur, le texte, l'âge, le nombre de mentions j'aime, et l'indication qu'il y a des réponses quand c'est le cas.
2. **Given** la liste des commentaires, **When** il change le tri, **Then** il peut choisir au moins les plus pertinents et les plus récents, et l'ordre change en conséquence.
3. **Given** un commentaire avec des réponses, **When** il les ouvre, **Then** les réponses apparaissent sous ce commentaire et peuvent être refermées.
4. **Given** un compte connecté, **When** il aime un commentaire, **Then** l'état aimé est visible et le compteur augmente d'un ; aimer de nouveau retire cet état.
5. **Given** les commentaires désactivés ou absents, **When** il ouvre les commentaires, **Then** un message l'explique, sans liste vide ambiguë.
6. **Given** aucun compte connecté, **When** il tente d'aimer un commentaire, **Then** l'application demande de se connecter et ne compte pas l'action comme réussie.

---

### Edge Cases

- Titre très long : il est limité à deux lignes sur la carte, sans recouvrir la durée ni la barre de progression.
- Avatar, nombre de vues, âge ou bannière absent : la carte ou l'en-tête reste aligné, sans emplacement cassé ni texte « null ».
- Vidéo terminée ou à peine commencée : elle n'entre pas dans « Reprendre ». S'il n'y a aucune vidéo à reprendre, l'étagère n'est pas affichée.
- Échec de chargement d'une liste : les emplacements d'attente disparaissent, un message explique l'échec et une action réessaye.
- Pastille, filtre ou onglet sans contenu : message dédié, pas une liste d'un autre thème.
- Dernière carte d'une rangée masquée par « Pas intéressé » : la rangée disparaît si elle devient vide.
- Action de compte en double (déjà dans À regarder plus tard, déjà abonné) : l'état réel est montré, sans doublon dans la liste.
- Effacer l'historique : annulable tant que la confirmation n'est pas donnée ; après confirmation, la liste affichée est vide ou le dit.
- Historique en pause : regarder une vidéo ne l'ajoute pas ; reprendre l'historique rétablit l'ajout.
- File vide et lecture automatique allumée : la carte de fin utilise la première suggestion, comme lorsqu'il n'y a pas de file.
- Suggestion suivante illisible (privée, retirée, limite d'âge) : elle est sautée ou signalée, et le téléspectateur n'est pas bloqué sur un écran d'erreur sans retour.
- Chapitre au-delà de la durée, ou description sans temps exploitable : pas de marqueur fictif ; les temps illisibles restent du texte.
- Direct en cours : pas de chapitres ni de barre de recherche temporelle ; la fin de vidéo ne lance pas un compte à rebours de replay.
- Direct annoncé mais non commencé : l'écran le dit et n'affiche pas une lecture vide comme si la vidéo avait échoué.
- Commentaires qui n'arrivent pas : message d'échec et nouvel essai, distinct de « commentaires désactivés ».
- Playlist d'une seule vidéo : Aléatoire se comporte comme Lire tout.
- Déconnexion pendant la consultation : les listes du compte se ferment ou se vident avec un message, sans mélanger l'ancien compte au suivant.
- Chaîne suivie retirée pendant qu'elle est ouverte : le bouton repasse à « S'abonner » et la chaîne quitte le guide au prochain affichage du guide.
- Région sans contenu pour une entrée d'Explorer : message vide, les autres entrées restent ouvrables.
- Préférence de lecture automatique, de langue et de sections personnelles : elles survivent à la fermeture de l'application.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Chaque carte de vidéo classique MUST montrer la miniature, la durée, au plus deux lignes de titre, le nom de la chaîne, le nombre de vues et l'âge, dès que chacune de ces informations existe.
- **FR-002**: La carte MUST montrer l'avatar de la chaîne quand il existe, et une marque « vérifiée » quand la chaîne est indiquée comme vérifiée.
- **FR-003**: Une carte de playlist MUST être distinguable d'une vidéo seule, avec un repère de playlist et le nombre d'éléments quand il est connu. Une carte de mix MUST porter un repère de mix.
- **FR-004**: Une vidéo en direct, nouvelle ou en première MUST porter le badge correspondant. Une vidéo en direct MUST NOT être présentée avec une durée ordinaire.
- **FR-005**: Une vidéo commencée MUST garder une barre de progression visible sur la miniature.
- **FR-006**: Une seule carte MUST être sélectionnée à la fois. La sélection MUST être un contour clair sur la miniature, et le texte de la carte MUST rester lisible.
- **FR-007**: Pendant l'attente d'une liste, l'écran MUST montrer des emplacements de cartes. En cas d'échec, ces emplacements MUST laisser place à un message et à une action pour réessayer.
- **FR-008**: L'accueil d'un compte qui a des vidéos commencées et non terminées MUST commencer par une étagère « Reprendre » limitée à ces vidéos. Sans aucune vidéo dans ce cas, l'étagère MUST être absente.
- **FR-009**: L'accueil MUST proposer des pastilles de filtre, dont « Tout ». Choisir une autre pastille MUST limiter l'accueil à ce thème sans changer d'écran. « Tout » MUST restaurer l'accueil complet.
- **FR-010**: Sans compte connecté, l'accueil MUST afficher des recommandations publiques et MUST NOT se limiter à un message demandant de se connecter.
- **FR-011**: Depuis une vidéo en lecture, le téléspectateur MUST pouvoir ouvrir un bloc d'informations sans interrompre la lecture, contenant le titre, la chaîne, l'état d'abonnement, l'état j'aime / je n'aime pas, un aperçu de description et une liste verticale « À suivre ».
- **FR-012**: « À suivre » MUST lister les suggestions disponibles, jusqu'à au moins trois quand il en existe au moins trois, avec miniature, titre et chaîne.
- **FR-013**: Un interrupteur « Lecture automatique » MUST être visible à côté de « À suivre ». Son état MUST être celui du réglage mémorisé, et le modifier MUST mettre à jour ce réglage.
- **FR-014**: À la fin d'une vidéo à la demande, si la lecture automatique est allumée et qu'une suite existe, l'application MUST montrer une grande carte « À suivre » avec un compte à rebours annulable. La suite MUST démarrer seulement si le compte à rebours va au bout. S'il est annulé, ou si la lecture automatique est éteinte, aucune suite MUST démarrer seule.
- **FR-015**: Le téléspectateur MUST pouvoir afficher un code qui ouvre la vidéo en cours sur le site YouTube depuis un téléphone.
- **FR-016**: Confirmer une recherche MUST afficher les résultats en grille sur l'espace principal et MUST ranger le clavier. Le téléspectateur MUST pouvoir rappeler le clavier pour modifier la requête.
- **FR-017**: Les résultats MUST pouvoir contenir des vidéos, des chaînes et des playlists, avec une présentation distincte par type et une ouverture vers la lecture, la page chaîne ou la page playlist.
- **FR-018**: Le téléspectateur MUST pouvoir filtrer les résultats par date de mise en ligne (dernière heure, aujourd'hui, cette semaine, ce mois, cette année), par durée (moins de 4 minutes, de 4 à 20 minutes, plus de 20 minutes), par type (vidéo, chaîne, playlist), par tri (pertinence, date, nombre de vues), et par les critères direct, haute définition et sous-titres. Retirer les filtres MUST restaurer la recherche d'origine.
- **FR-019**: Les recherches confirmées sur ce téléviseur MUST être reproposées à l'ouverture de la recherche, de la plus récente à la plus ancienne, jusqu'à dix entrées. Le téléspectateur MUST pouvoir en retirer une ou vider la liste.
- **FR-020**: Pour un compte connecté, Abonnements MUST montrer une bande de chaînes suivies puis les vidéos de la plus récente à la plus ancienne, avec les filtres Tout, Non vues et En direct.
- **FR-021**: Vous MUST donner accès à Historique, À regarder plus tard, Vidéos aimées et Playlists du compte connecté. Un enregistrement fait sur le site avec le même compte MUST apparaître dans la liste correspondante, et l'inverse MUST être vrai.
- **FR-022**: Le menu d'une carte de vidéo MUST proposer, pour un compte connecté, À regarder plus tard, Enregistrer dans une playlist, Pas intéressé, Ne plus recommander cette chaîne et Ajouter à la file. Une playlist nouvelle MUST pouvoir être nommée depuis cet enregistrement.
- **FR-023**: « Pas intéressé » MUST retirer la vidéo de la liste affichée. « Ne plus recommander cette chaîne » MUST retirer les vidéos de cette chaîne de cette liste. Ces retraits MUST encore être vrais au prochain affichage de la même liste tant que l'application reste ouverte.
- **FR-024**: L'historique MUST être groupé par jour, filtrable par une recherche, et une entrée MUST pouvoir être retirée seule. Mettre l'historique en pause MUST empêcher les nouveaux ajouts jusqu'à reprise. Effacer l'historique MUST exiger une confirmation.
- **FR-025**: Sans compte connecté, toute action qui modifie le compte MUST expliquer qu'il faut se connecter et MUST NOT être présentée comme réussie.
- **FR-026**: Le guide MUST proposer Accueil, Abonnements, Vous et Explorer. Explorer MUST proposer Tendances, Musique, Jeux, Actualités, Sport, Savoir et Podcasts.
- **FR-027**: Pour un compte connecté, le guide MUST lister les chaînes suivies avec avatar et nom, et les ouvrir sur leur page.
- **FR-028**: Depuis les écrans de consultation, la recherche et le compte MUST rester atteignables. Les sections personnelles déjà enregistrées MUST rester présentes et leur contenu MUST NOT être effacé.
- **FR-029**: La page chaîne MUST montrer la bannière si elle existe, l'avatar, le nom, l'identifiant public, le nombre d'abonnés, l'abonnement et une description courte, avec les onglets Accueil, Vidéos, En direct, Playlists et À propos.
- **FR-030**: L'onglet Vidéos MUST pouvoir être trié par plus récentes ou plus populaires.
- **FR-031**: Un compte connecté MUST pouvoir régler les notifications d'une chaîne suivie sur Toutes, Personnalisées ou Aucune, depuis la page chaîne et depuis la vidéo de cette chaîne.
- **FR-032**: La page playlist MUST montrer une illustration, le propriétaire, le nombre de vidéos, Lire tout, Aléatoire, Enregistrer et une liste numérotée. La vidéo en cours de cette playlist MUST être mise en évidence au retour sur la page.
- **FR-033**: Aléatoire MUST parcourir les vidéos de la playlist dans un ordre différent de la liste quand elle en contient plusieurs, sans répétition avant d'avoir proposé chaque vidéo une fois.
- **FR-034**: Quand une vidéo a des chapitres, la barre de progression MUST les marquer et une liste MUST indiquer le titre et le temps de départ de chacun. Confirmer un chapitre, ou un temps cité dans la description, MUST reprendre la lecture à ce moment, à une seconde près.
- **FR-035**: La description MUST pouvoir être dépliée. Les liens et mots-dièse qu'elle contient MUST être sélectionnables. Un mot-dièse MUST lancer sa recherche. Un lien vers une vidéo, une chaîne ou une playlist connue MUST ouvrir cette destination ; un autre lien MUST pouvoir être partagé vers le téléphone.
- **FR-036**: La file de lecture MUST pouvoir être consultée, complétée, réduite et réordonnée tant que l'application reste ouverte. À la fin de la vidéo ou sur « suivante », la file MUST passer avant les suggestions tant qu'elle n'est pas vide. Elle MUST être vide à la prochaine ouverture de l'application.
- **FR-037**: Pour une vidéo dont les commentaires sont ouverts, le téléspectateur MUST pouvoir les lire avec auteur, texte, âge, nombre de mentions j'aime et présence de réponses, les trier au moins par pertinence et par date, et ouvrir ou fermer les réponses.
- **FR-038**: Un compte connecté MUST pouvoir aimer ou retirer son j'aime sur un commentaire, et l'état affiché MUST suivre cette action. Sans compte, l'action MUST demander une connexion et MUST NOT compter comme réussie.
- **FR-039**: Si les commentaires sont désactivés, l'application MUST le dire explicitement. Si leur chargement échoue, elle MUST proposer un nouvel essai, distinct du cas désactivé.
- **FR-040**: Les compteurs « je n'aime pas » déjà montrés par l'application MUST rester visibles. Cette évolution MUST NOT les masquer pour imiter le site sur ce point.
- **FR-041**: Confirmer une vidéo, une playlist ou une chaîne MUST mener à la lecture ou à la page correspondante. Retour MUST ramener à l'écran précédent. Retour depuis le lecteur MUST masquer la vidéo (pas de lecture réduite au-dessus des listes) et MUST conserver la progression sur la carte. Retour depuis une page ouverte par-dessus le lecteur MUST revenir au lecteur à la même position, à une seconde près.

### Key Entities

- **Carte de contenu**: Vidéo, playlist ou mix affiché dans une liste. Porte miniature, titre, chaîne, vues, âge, durée ou badge, avatar, marque vérifiée, progression regardée.
- **Étagère d'accueil**: Rangée titrée de cartes. « Reprendre » est la première quand elle a des éléments. Une pastille de filtre limite les étagères visibles.
- **Recherche**: Requête, jusqu'à dix recherches récentes locales, suggestions, filtres actifs, résultats mêlés.
- **Guide**: Entrées Accueil, Abonnements, Vous, Explorer, sections personnelles et chaînes suivies.
- **Bibliothèque du compte**: Historique groupé par jour, À regarder plus tard, vidéos aimées, playlists, état de pause de l'historique. Partagée avec le même compte sur le site.
- **Abonnement**: Chaîne suivie, avec niveau de notifications Toutes, Personnalisées ou Aucune.
- **Page chaîne**: Identité publique, onglets et tri des vidéos.
- **Page playlist**: Illustration, propriétaire, nombre, ordre des vidéos, vidéo en cours, mode aléatoire.
- **Lecture en cours**: Vidéo en cours, interrupteur de lecture automatique, liste « À suivre », file ordonnée tant que l'application reste ouverte, chapitres, description dépliée.
- **Commentaire**: Auteur, texte, âge, mentions j'aime, état aimé par le compte, réponses.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Sans compte, un téléspectateur voit au moins une rangée de vidéos sur l'accueil dès la première ouverture, sans passer par l'écran Compte.
- **SC-002**: Sur 20 cartes de vidéos classiques qui possèdent les données, 20 montrent à la fois le titre, la chaîne, les vues et l'âge sans ouvrir la vidéo.
- **SC-003**: Depuis l'accueil, le téléspectateur ouvre une vidéo de « Reprendre » en au plus trois actions de télécommande quand cette étagère est présente.
- **SC-004**: Après confirmation d'une recherche, la première vidéo des résultats s'ouvre en une confirmation, le clavier n'occupant plus l'écran.
- **SC-005**: Un téléspectateur connecté ajoute une vidéo à À regarder plus tard depuis son menu de carte et la retrouve dans Vous en au plus deux déplacements.
- **SC-006**: Pendant la lecture, le téléspectateur affiche le titre, la chaîne, l'état d'abonnement et au moins trois suggestions verticales sans mettre la vidéo en pause, en au plus deux actions.
- **SC-007**: Sur une vidéo à chapitres, il démarre au chapitre choisi en au plus trois actions à partir de la barre de progression ou de la liste des chapitres.
- **SC-008**: Sur une vidéo à commentaires ouverts, il lit le premier commentaire et change le tri en au plus trois actions depuis le bloc d'informations.
- **SC-009**: Les sections personnelles déjà présentes avant l'évolution restent toutes accessibles après l'ouverture du nouveau guide.
- **SC-010**: Sur un parcours de dix vidéos terminées avec lecture automatique allumée, chaque suite ne démarre qu'après le compte à rebours, et une annulation empêche la suite dans les dix cas.

## Assumptions

- L'application reste une application de télévision en français, anglais ou espagnol, pilotée à la télécommande. Le contour de sélection remplace le survol à la souris du site. Une seule vidéo est lue à la fois : pas d'extrait qui joue dans la miniature.
- « Les mieux » retient ce qui change la ressemblance avec le site au quotidien. Hors périmètre : lecture des courts métrages verticaux, aperçu vidéo au survol ou au focus, mini-lecteur au-dessus du guide, chat des directs, messages de communauté, pourboires, création de clips, et halo coloré autour du lecteur.
- Le compte déjà proposé par l'application sert pour l'accueil personnalisé, les abonnements, la bibliothèque, les mentions j'aime et les commentaires. Rien de nouveau n'est demandé pour créer un compte.
- Les pastilles d'un compte connecté reprennent les thèmes fournis pour ce compte. S'il n'y en a pas, les pastilles sont Tout, Musique, En direct, Jeux, Actualités et Sport. Sans compte, ce même jeu fixe est utilisé.
- « Reprendre » concerne les vidéos commencées et non terminées, qu'elles aient été regardées sur ce téléviseur ou sur le compte. Une vidéo à peine lancée ou déjà finie n'y entre pas.
- À regarder plus tard, les playlists, les vidéos aimées, l'historique, les abonnements et les préférences de notification suivent le compte. La file de lecture et les recherches récentes restent sur ce téléviseur. La file est oubliée à la fermeture de l'application.
- Les recherches récentes restent sur ce téléviseur, dix au plus. Elles ne constituent pas un historique de compte.
- Les commentaires couvrent la lecture, le tri, les réponses et la mention j'aime. Rédiger un nouveau commentaire n'est pas inclus : la saisie longue à la télécommande n'est pas le parcours retenu.
- Les compteurs « je n'aime pas » déjà affichés restent. Les rapprocher du site ne signifie pas les cacher.
- Les réglages déjà présents (qualité, sous-titres, vitesse, saut de segments, langue, mise à jour) ne changent pas de rôle. Le partage par code réutilise le geste déjà connu pour ouvrir une page sur le téléphone.
- Une entrée d'Explorer ou une pastille sans contenu dans la région du téléspectateur affiche un état vide plutôt qu'un autre thème.

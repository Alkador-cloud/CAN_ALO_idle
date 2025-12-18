Jeu inspiré de Schedule One et Breaking bad.

Avis actuel : J'arrive bien à réaliser les différentes méchaniques que je veut intégrer dans mon jeu, et grâce à ma planification je sais quoi faire et dans quel ordre.

Priorités de développement - Jeu Idle

Phase 1 : Boucle de gameplay brute

Objectif 1 : Système de base de clic
•	[x] T1.1 - Créer la scène principale du jeu avec un GameObject Plant
•	[x] T1.2 - Implémenter un script PlantClickHandler détectant les clics sur la plante
•	[x] T1.3 - Ajouter un feedback visuel au clic (animation simple, changement de couleur)

Objectif 2 : Cycle de production (6 étapes)
•	[x] T2.1 - Créer un enum ProductionStage avec les 6 étapes : Planter, Arroser, Pousser, Cueillir, Emballer, Vendre
•	[x] T2.2 - Créer une classe CropState gère la progression dans le cycle

Objectif 3 : Système monétaire
•	[x] T3.1 - Créer une classe Currency pour gérer le solde du joueur
•	[x] T3.2 - Implémenter le calcul de revenu à l'étape Vendre
•	[x] T3.3 - Ajouter un multiplicateur de prix basé sur le type de drogue (future extensibilité)

Objectif 4 : Interface utilisateur basique
•	[x] T4.1 - Créer un Canvas UI avec affichage du solde (TextMeshPro)
• [x] T4.2 - Ajouter un indicateur de progression du cycle (barre/texte étape actuelle)
• [x] T4.3 - Afficher l'argent possédé, ainsi que l'étape actuelle et le pourcentage de progression

Objectif 5 : Système de sauvegarde/chargement
•	[x] T5.1 - Créer une classe GameData sérialisant l'état du jeu
•	[x] T5.2 - Implémenter un SaveManager (données sauvegardées dans le dossier Appdata de l'utilisateur)
•	[x] T5.3 - Charger les données au démarrage du jeu
•	[x] T5.4 - Sauvegarder automatiquement à chaque changement d'étape

Objectif 6 : Un seul type de drogue
•	[x] T6.1 - Créer un ScriptableObject CropType (Cannabis initial)
•	[x] T6.2 - Définir les propriétés : Duration, SellPrice, Name
•	[x] T6.3 - Lier la récolte actuelle à ce type unique
---
Phase 2 : Progression et Engagement - Division en Tâches

Objectif 7 : Système d'expérience
•	[x] T7.1 - Créer une classe ExperienceManager gerant les types d'XP par type de drogue, et gerant les type d'XP par niveau de compétence dans les différentes étapes de productions.
•	[x] T7.2 - Implémenter le gain d'1 XP à chaque complétion d'un cycle complet de production d'une drogue, et d'1XP à chaque complétion d'une étape de production.
•	[x] T7.3 - Créer un système de niveaux (seuils d'XP par niveau)
•	[x] T7.4 - Afficher XP/niveau dans l'UI (TextMeshPro)

Objectif 8 : Système d'améliorations
•	[x] T8.1 - Créer une classe Upgrade et un enum UpgradeType (amélioration du pourcentage des cliques, prix de vente, automatisation, vitesse et puissance d'automatisation,...)
•	[x] T8.2 - Implémenter un UpgradeManager débloquant les bonus avec l'XP ou les niveaux,
avec options dans unity inspector pour choisir la condition entre niveau et level et
les différents types d'XP et de levels.
•	[x] T8.3 - Créer un script configurable dans l'inspecteur unity qui aura pour but de configurer et paramètrer le débloquage des bonus,
avec options dans unity inspector pour choisir la ou les conditions entre niveau et level et les différents types d'XP et de levels avec possiblitée de mettre plusieurs conditions. 
•	[x] T8.4 - Créer une UI d'améliorations avec boutons d'achat (coût en argent, bouton qui passent de gris à coloré quand conditions remplies)

Objectif 9 : Système de notifications
•	[x] T11.1 - Créer un NotificationManager
•	[~] T11.2 - Implémenter les événements déclenchant des notifications (niveau gagné, upgrade débloqué, vente)

Objectif 10 : Système d'automatisation
•	[ ] T9.1 - Créer un AutomationManager accélérant progressivement les étapes
•	[ ] T9.2 - Implémenter l'automatisation par étape (Planter, Arroser, etc.)
•	[ ] T9.3 - Ajouter des indicateurs visuels du niveau d'automatisation
•	[ ] T9.4 - Intégrer la sauvegarde de l'état d'automatisation

Objectif 11 : Variété de cultures (2-3 types supplémentaires)
•	[ ] T10.1 - Créer 2 nouveaux CropType ScriptableObjects (ex: Pavot, Champignon)
•	[ ] T10.2 - Implémenter un système de sélection de culture
•	[ ] T10.3 - Adapter les durées/prix selon le type choisi
•	[ ] T10.4 - Ajouter une UI de sélection avec indicateurs de rentabilité

Objectif 12 : Graphismes simples mais attrayants
•	[ ] T12.1 - Importer des sprites pour chaque étape de production
•	[ ] T12.2 - Ajouter des animations de transition entre étapes
•	[ ] T12.3 - Styliser l'UI (couleurs, polices, icônes)
•	[ ] T12.4 - Ajouter des effets visuels simples (particules, glow sur actions importantes)

---
Phase 3 : Contenu additionnel
Pour maintenir l'intérêt long terme
13.	Système de quêtes/objectifs - Missions guidées
14.	Système de succès - Badges et accomplissements
15.	Système de défis quotidiens/hebdomadaires - Engagement régulier
16.	Zone de stockage - Limite de ressources
17.	Système de prix/fluctuation - Prix variables selon la demande
---
Phase 4 : Profondeur de gameplay
Ajouter de la complexité stratégique
18.	Système d'employés - Déléguer le travail
19.	Améliorations indirectes - Marketing, sécurité, etc.
20.	Améliorations de setup - Serre, éclairage, irrigation
21.	Système de réputation - Impact sur les ventes
22.	Système de statistiques détaillées - Tracking avancé
23.	Tutoriel interactif - Apprentissage progressif
24.	Aides contextuelles - Help en jeu
---
Phase 5 : Endgame et réplayabilité
Pour les joueurs hardcore
25.	Système de prestige/reset - Redémarrer avec des bonus permanents
26.	Système de bonus aléatoires - Variabilité et surprise
27.	Feedback visuel et sonore - Animations et effets
28.	Musique et effets sonores immersifs - Ambiance audio

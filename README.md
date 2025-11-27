# CAN_ALO_idle
Priorités de développement - Jeu Idle
---
Objectif 1 : Système de base de clic

•	[ ] T1.1 - Créer la scène principale du jeu avec un GameObject Plant
•	[ ] T1.2 - Implémenter un script PlantClickHandler détectant les clics sur la plante
•	[ ] T1.3 - Ajouter un feedback visuel au clic (animation simple, changement de couleur)

Objectif 2 : Cycle de production (6 étapes)

•	[ ] T2.1 - Créer un enum ProductionStage avec les 6 étapes : Planter, Arroser, Pousser, Cueillir, Emballer, Vendre
•	[ ] T2.2 - Créer une classe CropState gère la progression dans le cycle
•	[ ] T2.3 - Ajouter les timers pour chaque étape (durée de transition)
•	[ ] T2.4 - Implémenter les transitions visuelles entre étapes (modèles/sprites)

Objectif 3 : Système monétaire

•	[ ] T3.1 - Créer une classe Currency pour gérer le solde du joueur
•	[ ] T3.2 - Implémenter le calcul de revenu à l'étape Vendre
•	[ ] T3.3 - Ajouter un multiplicateur de prix basé sur le type de drogue (future extensibilité)

Objectif 4 : Interface utilisateur basique

•	[ ] T4.1 - Créer un Canvas UI avec affichage du solde (TextMeshPro)
•	[ ] T4.2 - Ajouter un indicateur de progression du cycle (barre/texte étape actuelle)
•	[ ] T4.3 - Afficher le gain prévisuel à la vente

Objectif 5 : Système de sauvegarde/chargement

•	[ ] T5.1 - Créer une classe GameData sérialisant l'état du jeu
•	[ ] T5.2 - Implémenter un SaveManager utilisant JSON (ou PlayerPrefs)
•	[ ] T5.3 - Charger les données au démarrage du jeu
•	[ ] T5.4 - Sauvegarder automatiquement à chaque changement d'étape

Objectif 6 : Un seul type de drogue

•	[ ] T6.1 - Créer un ScriptableObject CropType (Cannabis initial)
•	[ ] T6.2 - Définir les propriétés : Duration, SellPrice, Name
•	[ ] T6.3 - Lier la récolte actuelle à ce type unique

---

Phase 2 : Progression et engagement

Pour rendre le jeu addictif
7.	Système d'expérience - Points XP par type de drogue
8.	Système d'améliorations - Débloquer des bonus avec l'expérience
9.	Système d'automatisation - Accélérer les étapes progressivement
10.	Ajouter 2-3 types de drogues supplémentaires - Variété de gameplay
11.	Système de notifications - Alerter le joueur des événements
12.	Graphismes simples mais attrayants - Assets visuels de base

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

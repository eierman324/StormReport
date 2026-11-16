<div align="center">

[English](README.md) · **Français** · [Español](README.es.md)

<img src="docs/icon.png" alt="" width="88">

# Storm Report

[![version](https://img.shields.io/github/v/release/eierman324/ChestAddictionCounter?style=flat-square&label=version&color=7d55ff)](../../releases/latest) [![téléchargements](https://img.shields.io/github/downloads/eierman324/ChestAddictionCounter/total?style=flat-square&label=t%C3%A9l%C3%A9chargements&color=7d55ff)](../../releases) ![Windows 10 | 11](https://img.shields.io/badge/Windows-10%20%7C%2011-3a7bd5?style=flat-square) [![licence](https://img.shields.io/github/license/eierman324/ChestAddictionCounter?style=flat-square&label=licence&color=555)](LICENSE)

**Ton rapport Fortnite : victoires, éliminations, dégâts, coffres et trésors, ton casier, avec un overlay pour le jeu et pour le stream.**

[**⬇ Télécharger StormReport.exe**](../../releases/latest) · Windows 10 / 11 · gratuit · rien à installer

</div>

---

> Storm Report est né d'un fork de [Fortnite Chest Counter](https://github.com/LED-N/Fortnite-ChestCounter) de LED-N. Il en garde le comptage des coffres et les données de la chasse au trésor, et en fait un rapport Fortnite complet. Tu utilisais Fortnite Chest Counter ou Chest Addiction Counter ? Ton historique et tes réglages sont repris tels quels.

## Ce qu'il montre

Storm Report réunit deux sources, sans jamais montrer le même chiffre des deux :

| De ton **compte Epic** (connexion obligatoire) | De tes **replays** (les parties enregistrées sur ton PC) |
|---|---|
| Parties, victoires, % de victoires, éliminations, K/D et temps de jeu, au total et par mode, pour toutes tes parties sur tous tes appareils | Chaque partie : place, éliminations et assistances, dégâts infligés et reçus, précision, matériaux, distance |
| Tout ton casier (recherche, catégories, favoris), V-Bucks, achats, saisons passées | Place moyenne, dégâts par partie, la session en cours, les chiffres en direct pendant une partie |
| Les trésors que tu possèdes (la collection se remplit toute seule) | Chaque coffre ouvert ou détruit, coffres par jour et par heure, trésors trouvés et leur origine |

- **Rapport** : vue d'ensemble, un tableau par mode avec les deux sources côte à côte, les dernières parties, puis coffres et trésors (tes chances pour la tenue en or, la collection, les trésors obtenus par d'autres joueurs près de toi).
- **Compte / Casier** : ton compte Epic, en lecture seule, et un bouton **Transférer tout mon casier vers fortnite.gg**.
- **Overlay** : en jeu (par-dessus Fortnite en plein écran fenêtré), pour OBS (une page locale) et une **fenêtre de stream** pour les applis sans source navigateur comme TikTok LIVE Studio (capture de fenêtre, fond sombre ou vert pour le chroma key). Le grand chiffre peut être les coffres, les éliminations ou les victoires.

## Démarrer

1. Télécharge **[`StormReport.exe`](../../releases/latest)** et lance-le. Rien à installer.
   Windows peut afficher *« Windows a protégé votre ordinateur »* (exe non signé) : *Informations complémentaires → Exécuter quand même*. La version contient aussi `StormReport.exe.sha256` pour vérifier le téléchargement (`Get-FileHash .\StormReport.exe`).
2. Active les rediffusions dans Fortnite : *Paramètres → Jeu → Rediffusions → Enregistrer les rediffusions : **Activé***.
3. Connecte-toi à Epic dans l'onglet **Compte** : le navigateur ouvre la connexion Epic ; quand une page affiche un texte avec `authorizationCode`, fais Ctrl+A, Ctrl+C (détecté tout seul) ou colle-le dans la fenêtre.

## Vie privée et sécurité

- **Compte Epic, en lecture seule** : Storm Report utilise les mêmes adresses Epic et clients du jeu que les outils de la communauté (par exemple l'import de casier de fortnite.gg). Rien n'est jamais modifié sur ton compte. Ce n'est pas une API publique officielle : Epic peut la changer ou la bloquer.
- **Rester connecté** garde un accès Epic chiffré avec Windows DPAPI (lisible seulement par ton utilisateur Windows). **Se déconnecter** le supprime sur ton PC et chez Epic.
- **Les replays sont seulement lus** : `%LOCALAPPDATA%\FortniteGame\Saved\Demos`, ouverts en lecture seule. Aucune modification du jeu, aucun accès à sa mémoire.
- **Tes données restent sur ton PC** (`%APPDATA%\ChestCounter`). La fenêtre est une page servie à ton PC seulement (`127.0.0.1`, clé aléatoire).
- **Réseau** : Epic (ton compte), fortnite-api.com (noms et icônes des objets), fortnite.gg (seulement quand tu transfères ton casier), GitHub (données de la chasse au trésor du projet d'origine et mises à jour de Storm Report).

## Les chiffres sont-ils fiables ?

- **D'Epic** : les totaux officiels de ton compte.
- **Des replays** : place, éliminations, dégâts, précision et les autres stats de fin de partie sont écrits par Fortnite lui-même dans le replay. Les coffres sont reconnus à partir des événements du replay : les coffres détruits par un véhicule ne sont parfois pas enregistrés (ces parties sont signalées), et les coéquipiers loin de toi manquent dans les chiffres d'équipe. L'origine d'un trésor est estimée et signalée en cas de doute.
- Le lecteur de replays n'est pas fait par Epic : après une grosse mise à jour de Fortnite, il peut falloir une mise à jour avant de relire les replays.

## Remerciements

- [Fortnite Chest Counter](https://github.com/LED-N/Fortnite-ChestCounter) de LED-N : comptage des coffres, données et images de la chasse au trésor.
- Corrections et idées des forks de cevival (origine des trésors), koiseka (chances hebdo), tbpg58 (empreintes des versions, journal d'activité), voiddog30 (coffres et trésors par heure) et cherjp (vérification de la clé des replays).
- Interface [Obsidian](https://docs.mspaint.cc/obsidian) de mspaint, [Tailwind CSS](https://tailwindcss.com) (MIT), [IBM Plex Mono](https://github.com/IBM/plex) (SIL OFL).
- [fortnite-api.com](https://fortnite-api.com) pour les noms et icônes ; le lien de casier fortnite.gg est construit comme [FNGG-LockerGenerator](https://github.com/Liqutch/FNGG-LockerGenerator) (MIT).

Storm Report n'est ni affilié à Epic Games ni approuvé par Epic Games. Fortnite est une marque d'Epic Games, Inc.

## Le compiler soi-même

Installe le [SDK .NET 10](https://dotnet.microsoft.com/download/dotnet/10.0) et lance `build.bat` : l'exe arrive dans `dist\`.

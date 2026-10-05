# PPE 05 – Projet 1 : application console de gestion d'une bibliothèque (C#)

| | |
|---|---|
| **Cours** | 05 – Algorithmique orientée C# : concevoir et implémenter des applications (CFC 319) |
| **Classe** | E1B – Geneva Institute of Technology |
| **Étudiant** | Paul Giocanti |
| **Semaine PPE** | 5 – 9 octobre 2026 |
| **Niveau du projet** | Débutant |
| **Code source complet** | [github.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque](https://github.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque) |

[TOC]

---

## 1. Le projet en bref

**Contexte (énoncé)** : une petite bibliothèque associative veut une application console simple pour gérer ses livres et ses emprunts.

> Une **application console** est un programme sans fenêtre graphique : il s'utilise au clavier, dans une fenêtre de texte (le terminal). On tape un choix, le programme répond par du texte.

**Ce que fait l'application réalisée :**

- gérer le **catalogue** : ajouter un livre, afficher la liste avec la disponibilité de chaque livre ;
- gérer les **membres** : inscrire un membre, afficher la liste avec le nombre de livres que chacun a chez lui ;
- enregistrer un **emprunt** (avec une date de retour prévue à 14 jours) et un **retour** ;
- **refuser proprement** ce qui est impossible : livre déjà emprunté, livre ou membre inexistant, plus de 3 emprunts en même temps, saisie qui n'est pas un nombre… Dans tous ces cas, un message rouge explique le problème et le programme continue : il ne plante jamais.

**Correspondance entre l'énoncé et ce rendu :**

| Étape demandée dans l'énoncé | Ce qui a été fait | Où le voir |
|---|---|---|
| 1. Modéliser les classes nécessaires | 5 classes : `Livre`, `Membre`, `Emprunt`, `Bibliotheque`, `BibliothequeException` (+ `Program` pour l'interface) | §3 et §4 |
| 2. Implémenter l'ajout et la liste des livres | Menus 1 et 2 | §4.4, tests T02 à T05 |
| 3. Implémenter l'emprunt et le retour | Menus 5 et 6 | §4.4, tests T08 et T14 |
| 4. Gérer les cas d'erreur (livre déjà emprunté) | Exception métier + `try` / `catch` | §4.5, tests T09 à T12 et T15 |
| 5. Tester avec plusieurs scénarios | 19 tests manuels enchaînés, une capture par test | §5 et §6 |

**Livrables attendus :**

| Livrable | Où le trouver |
|---|---|
| Code source C# commenté | §4 (extraits expliqués) et dossier `GestionBibliotheque/` du dépôt GitHub |
| Jeu de tests manuels documenté | §6 (tableau récapitulatif + une capture par test) |
| Diagramme de classes simple | §3 |

---

## 2. Environnement de travail

| Outil | À quoi il sert | Pourquoi ce choix |
|---|---|---|
| **Visual Studio 2026** | IDE (*Integrated Development Environment*, environnement de développement intégré) : éditeur de code, compilation et débogage réunis dans un seul logiciel | C'est l'outil utilisé en cours de C#. |
| **SDK .NET 10** | SDK (*Software Development Kit*, kit de développement) : le compilateur C#, les bibliothèques de base de .NET et la commande `dotnet` | Indispensable pour compiler et exécuter du C#. Visual Studio l'utilise lui-même en arrière-plan. |
| **CLI `dotnet`** dans le Terminal Windows | CLI (*Command Line Interface*, interface en ligne de commande) : on tape des commandes au lieu de cliquer dans des menus | Les commandes donnent exactement le même projet qu'avec les menus de Visual Studio, mais elles sont **reproductibles** et visibles sur les captures. |
| **Git + GitHub** | Git garde l'historique des versions du code ; GitHub l'héberge en ligne | Publication du code source et des captures. |

Avant de commencer, on vérifie que les bons outils sont installés et dans quelle version : c'est la première chose à faire sur un poste de développement.

![Capture 01 – Versions du SDK .NET et de Visual Studio](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/01-environnement-dotnet-vs.png)

**Ce que montre la capture :** `dotnet --list-sdks` liste les SDK .NET installés (un seul : **10.0.401**) et `dotnet --version` confirme que c'est lui qui sera utilisé. `vswhere` (un petit outil installé avec Visual Studio) indique **Visual Studio Community 2026**, version **18.10.3**. L'environnement est donc prêt pour un projet .NET 10.

:::info
**À retenir – SDK ou runtime ?**
Le **SDK** sert à *fabriquer* des programmes .NET (il contient le compilateur). Le **runtime** sert seulement à les *exécuter*. Sur un poste de développement, il faut le SDK ; sur l'ordinateur d'un simple utilisateur, le runtime suffit.
:::

---

## 3. Modélisation : le diagramme de classes

### 3.1 Classe et objet

- Une **classe** est un *plan de construction* : elle décrit ce qu'un élément contient (ses **propriétés**, par exemple `Titre`) et ce qu'il sait faire (ses **méthodes**, par exemple `MarquerCommeEmprunte()`).
- Un **objet** est un *exemplaire concret* fabriqué à partir de ce plan avec le mot-clé `new`. La classe `Livre` est le plan ; « Le Petit Prince » et « Heidi » sont deux objets `Livre`.

### 3.2 Les classes du projet : une classe = un rôle

| Classe | Son rôle | Ce qu'elle contient |
|---|---|---|
| `Livre` | Représente un livre du catalogue | Numéro, titre, auteur, année, disponibilité |
| `Membre` | Représente une personne inscrite | Numéro, prénom, nom |
| `Emprunt` | Relie **un** livre à **un** membre, avec les dates | Livre, membre, date d'emprunt, date de retour prévue, date de retour réelle |
| `Bibliotheque` | Le « cerveau » : contient les listes et fait respecter les règles | Listes de livres, membres, emprunts ; méthodes `AjouterLivre`, `Emprunter`, `Retourner`… |
| `BibliothequeException` | Un type d'erreur propre à l'application | Le message qui explique l'erreur |
| `Program` | L'interface : menu, saisies au clavier, affichage | `Main` et les méthodes d'affichage et de saisie |

Séparer ainsi les rôles évite d'avoir un seul gros fichier où tout est mélangé : si on remplace un jour la console par une fenêtre graphique, seule la classe `Program` change, les règles de `Bibliotheque` restent les mêmes.

### 3.3 Le diagramme

Le diagramme ci-dessous a été écrit en **Mermaid** (un langage qui décrit un schéma avec du texte ; la source est dans `docs/diagramme-classes.mmd`), puis exporté en image.

![Diagramme de classes UML du projet](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/docs/diagramme-classes.png)

**Comment le lire :**

| Symbole | Signification | Exemple dans le diagramme |
|---|---|---|
| `+` / `-` devant un nom | `+` = **public** (utilisable depuis les autres classes), `-` = **privé** (réservé à la classe elle-même) | `-_livres` : la liste des livres est privée |
| Nom souligné | Membre **partagé** par tous les objets (`const` ou `static` en C#) | `MaxEmpruntsParMembre = 3` |
| Losange plein ◆ | **Composition** : la bibliothèque crée et contient ses livres, ses membres et ses emprunts | `Bibliotheque` ◆— `Livre` |
| Flèche simple → | **Association** : un objet connaît un autre objet | un `Emprunt` connaît son `Livre` et son `Membre` |
| `1`, `0..*` | **Multiplicités** : combien d'objets de chaque côté (`0..*` = zéro ou plusieurs) | une bibliothèque a `0..*` livres ; un emprunt concerne `1` livre |
| Triangle vide ▷ | **Héritage** : la classe du bas « est une » sorte de la classe du haut | `BibliothequeException` est une `Exception` |
| Pointillés | **Dépendance** : une classe en utilise une autre sans la contenir | `Program` utilise `Bibliotheque` |

### 3.4 Les 4 piliers de la programmation orientée objet dans ce projet

La page Microsoft Learn « Programmation orientée objet (C#) », citée dans l'énoncé, présente quatre principes. Voici où on les retrouve dans le code :

| Principe | En une phrase | Dans le projet |
|---|---|---|
| **Abstraction** | Ne garder du monde réel que ce qui est utile au programme | Un `Livre` n'a que titre, auteur, année et disponibilité (pas de nombre de pages ni de couverture) |
| **Encapsulation** | Cacher l'intérieur d'un objet et n'autoriser l'accès que par des méthodes publiques | Les listes de `Bibliotheque` sont `private` ; `EstDisponible` se lit partout mais ne se modifie que par `MarquerCommeEmprunte()` / `MarquerCommeDisponible()` |
| **Héritage** | Créer une classe à partir d'une classe existante | `BibliothequeException : Exception` |
| **Polymorphisme** | Une même méthode, un comportement différent selon la classe | `Livre` et `Membre` redéfinissent `ToString()` avec `override` pour s'afficher chacun à leur façon |

---

## 4. Réalisation pas à pas

### 4.1 Créer la solution et le projet

Dans Visual Studio, tout projet est rangé dans une **solution** :

- la **solution** (`GestionBibliotheque.slnx`) est le « classeur » qui regroupe un ou plusieurs projets. C'est le fichier qu'on ouvre dans Visual Studio ;
- le **projet** (`GestionBibliotheque.csproj`) correspond à **un** programme : ses fichiers de code et ses réglages.

| Commande | Ce qu'elle fait |
|---|---|
| `dotnet new sln -n GestionBibliotheque` | Crée une solution vide nommée `GestionBibliotheque` |
| `dotnet new console -n GestionBibliotheque --use-program-main` | Crée un projet console à partir du modèle (*template*) « console ». L'option `--use-program-main` génère une vraie classe `Program` avec sa méthode `Main`, plus claire pour un projet orienté objet que le style abrégé proposé par défaut |
| `dotnet sln add GestionBibliotheque\GestionBibliotheque.csproj` | Inscrit le projet dans la solution, pour que Visual Studio l'ouvre avec elle |

On commence par créer la structure vide, avant d'écrire la moindre ligne de code :

![Capture 02 – Création de la solution et du projet console](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/02-creation-solution-projet.png)

**Ce que montre la capture :** les trois commandes réussissent : « Le modèle « Fichier solution » a bien été créé », puis « Le modèle « Application console » a bien été créé », suivi de la *restauration* (la préparation des dépendances du projet, terminée par « Restauration réussie »), et enfin « Projet … ajouté à la solution ».

Avant de modifier quoi que ce soit, on regarde ce que le modèle a généré, pour comprendre le point de départ :

![Capture 03 – Fichiers générés par le modèle console](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/03-projet-genere.png)

**Ce que montre la capture :** le modèle a créé le fichier projet `.csproj`, un `Program.cs` qui affiche « Hello, World! » et un dossier `obj` (fichiers techniques de compilation). Dans le `.csproj` :

| Ligne | Signification |
|---|---|
| `<OutputType>Exe</OutputType>` | Le projet produit un programme exécutable |
| `<TargetFramework>net10.0</TargetFramework>` | Le programme cible la version 10 de .NET |
| `<ImplicitUsings>enable</ImplicitUsings>` | Les `using` les plus courants (`System`, `System.Collections.Generic`…) sont ajoutés automatiquement |
| `<Nullable>enable</Nullable>` | Le compilateur prévient quand une variable risque de valoir `null` (vide) sans qu'on l'ait prévu |

:::info
**À retenir – `.slnx` ou `.sln` ?**
Avec .NET 10 et Visual Studio 2026, une nouvelle solution est créée au format **`.slnx`** (un fichier XML court et lisible). L'ancien format **`.sln`** fonctionne toujours. Les deux s'ouvrent par un double-clic dans Visual Studio.
:::

### 4.2 Un fichier par classe, ouvert dans Visual Studio

Le `Program.cs` « Hello World » est ensuite remplacé par le vrai programme, et chaque classe est écrite dans son propre fichier (`Livre.cs`, `Membre.cs`…). C'est la convention en C# : on retrouve une classe immédiatement grâce au nom de son fichier. La solution est ouverte dans Visual Studio pour écrire et relire le code. Comme pour les tests (voir §5), les captures de Visual Studio ont été prises par un script, qui ouvre chaque fichier à la bonne ligne puis capture la fenêtre.

![Capture 04 – La solution dans Visual Studio et le fichier Program.cs](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/04-vs-solution-program.png)

**Ce que montre la capture :** à droite, l'**Explorateur de solutions** (le panneau qui liste les fichiers) affiche les six fichiers du projet. Au centre, le début de `Program.cs` : la méthode `Main`, point de départ du programme. En bas, la **Liste d'erreurs** indique **0 erreur et 0 avertissement** ; les 25 « messages » (icône « i » bleue) sont de simples suggestions de style (voir l'encadré ci-dessous). Au-dessus de chaque classe et de chaque méthode, Visual Studio affiche « N références » : c'est le nombre d'endroits du code qui l'utilisent (fonction *CodeLens*).

:::info
**À retenir – erreurs, avertissements et suggestions dans Visual Studio**
- **Erreur** (croix rouge) : le programme ne peut pas être compilé.
- **Avertissement** (triangle jaune) : le programme compile, mais il y a un risque de bug.
- **Message / suggestion** (« i » bleu) : proposition d'écriture plus moderne ou plus courte, sans aucun risque. Par exemple `IDE0090 – L'expression 'new' peut être simplifiée` propose d'écrire `new("...")` au lieu de `new Bibliotheque("...")`, et `IDE0028 – L'initialisation des collections peut être simplifiée` propose `[]` au lieu de `new List<Livre>()`. Ces suggestions ont été laissées volontairement : l'écriture longue est plus explicite quand on débute.
:::

### 4.3 Les classes Livre, Membre et Emprunt

**La classe `Livre`**

```csharp
public class Livre
{
    public int Id { get; }                          // lecture seule : fixé à la création
    public string Titre { get; }
    public string Auteur { get; }
    public int AnneePublication { get; }
    public bool EstDisponible { get; private set; } // lisible partout, modifiable seulement ici

    public Livre(int id, string titre, string auteur, int anneePublication)
    {
        Id = id;
        Titre = titre;
        Auteur = auteur;
        AnneePublication = anneePublication;
        EstDisponible = true;   // un nouveau livre arrive toujours en rayon
    }

    public void MarquerCommeEmprunte()   { EstDisponible = false; }
    public void MarquerCommeDisponible() { EstDisponible = true; }
}
```

- Une **propriété** (`{ get; }`) est une donnée de l'objet. Sans `set`, elle ne peut plus changer après la création : un livre ne change pas de titre.
- `{ get; private set; }` : tout le monde peut **lire** `EstDisponible`, mais seule la classe `Livre` peut le **modifier**. C'est l'**encapsulation** : impossible de rendre un livre disponible « par erreur » depuis un autre endroit du code.
- Le **constructeur** (la méthode qui porte le nom de la classe) est exécuté au moment du `new Livre(...)` : il remplit les propriétés.

Dans Visual Studio, le fichier tient en trois captures (à partir d'ici, les captures sont recadrées sur l'éditeur : la Liste d'erreurs, identique à la capture 04, n'est plus affichée). La première montre les données du livre :

![Capture 05 – La classe Livre : les propriétés](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/05-vs-classe-livre.png)

**Ce que montre la capture :** l'en-tête `public class Livre` (utilisée à 19 endroits du code d'après CodeLens), les quatre propriétés en lecture seule `Id`, `Titre`, `Auteur` et `AnneePublication`, puis le commentaire qui annonce `EstDisponible` et son `private set` (sa ligne de déclaration, coupée en bas de l'écran, est reprise dans l'extrait de code ci-dessus).

La deuxième montre le constructeur :

![Capture 05b – La classe Livre : constructeur et MarquerCommeEmprunte](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/05b-vs-classe-livre-suite.png)

**Ce que montre la capture :** le constructeur `Livre(...)`, qui recopie ses quatre paramètres dans les propriétés et met `EstDisponible` à `true`, puis la méthode `MarquerCommeEmprunte()`, qui passe la disponibilité à `false`.

La troisième montre la fin de la classe :

![Capture 05c – La classe Livre : MarquerCommeDisponible et ToString](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/05c-vs-classe-livre-fin.png)

**Ce que montre la capture :** `MarquerCommeDisponible()` remet le livre en rayon, et `ToString()` décrit le livre en texte, par exemple « Heidi » (Johanna Spyri, 1880). La petite flèche dans la marge, à côté de `ToString()`, est l'indicateur de Visual Studio pour une méthode qui **redéfinit** (`override`) une méthode héritée : c'est le polymorphisme vu au §3.4.

**La classe `Membre`** suit le même modèle, avec une **propriété calculée** : `NomComplet` n'est pas stockée, elle est recalculée à partir du prénom et du nom à chaque lecture.

![Capture 06 – La classe Membre : propriétés](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/06-vs-classe-membre.png)

**Ce que montre la capture :** l'en-tête `public class Membre` et ses trois propriétés en lecture seule (`Id`, `Prenom`, `Nom`), puis le début de `NomComplet`.

![Capture 06b – La classe Membre : NomComplet, constructeur et ToString](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/06b-vs-classe-membre-suite.png)

**Ce que montre la capture :** la propriété calculée `NomComplet` (bloc `get { return $"{Prenom} {Nom}"; }`), le constructeur et `ToString()`, qui renvoie simplement le nom complet.

**La classe `Emprunt`** relie un livre à un membre :

```csharp
public const int DureeEnJours = 14;               // valeur fixe, la même pour tous les emprunts
public DateTime DateRetourPrevue { get; }
public DateTime? DateRetour { get; private set; } // null tant que le livre n'est pas rendu

public bool EstEnCours
{
    get { return DateRetour == null; }
}
```

- `const` : une constante, fixée une fois pour toutes dans le code.
- `DateTime` : le type .NET pour les dates. `dateEmprunt.AddDays(14)` calcule la date de retour prévue.
- `DateTime?` (avec un `?`) est un type **nullable** : il peut valoir `null`, ce qui veut dire ici « pas encore rendu ». Un emprunt est donc **en cours** tant que sa date de retour est `null`.

![Capture 07 – La classe Emprunt : constante et propriétés](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/07-vs-classe-emprunt.png)

**Ce que montre la capture :** la constante `DureeEnJours = 14`, les liens vers le `Livre` et le `Membre` concernés, puis `DateEmprunt` et `DateRetourPrevue`. La troisième date, `DateRetour` de type `DateTime?`, est juste en dessous (voir l'extrait de code ci-dessus).

![Capture 07b – La classe Emprunt : EstEnCours et constructeur](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/07b-vs-classe-emprunt-suite.png)

**Ce que montre la capture :** la propriété calculée `EstEnCours` (vraie tant que `DateRetour` vaut `null`) et le constructeur, qui calcule la date de retour prévue avec `dateEmprunt.AddDays(DureeEnJours)` et met `DateRetour` à `null`.

![Capture 07c – La classe Emprunt : la méthode Cloturer](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/07c-vs-classe-emprunt-fin.png)

**Ce que montre la capture :** `Cloturer(DateTime dateRetour)` enregistre la date de retour : à partir de là, `EstEnCours` devient faux et l'emprunt est terminé.

### 4.4 La classe Bibliotheque : les règles de gestion

`Bibliotheque` garde trois listes **privées** (`List<Livre>`, `List<Membre>`, `List<Emprunt>`) et deux compteurs qui donnent automatiquement les numéros 1, 2, 3…

> `List<Livre>` est une **liste** : une collection d'objets `Livre` qui s'agrandit quand on ajoute des éléments (`_livres.Add(livre)`). Les chevrons `< >` indiquent le type des éléments rangés dedans.

```csharp
public Livre AjouterLivre(string titre, string auteur, int anneePublication)
{
    if (string.IsNullOrWhiteSpace(titre))
        throw new BibliothequeException("Le titre du livre est obligatoire.");
    ...
    int anneeMax = DateTime.Today.Year;
    if (anneePublication < 1450 || anneePublication > anneeMax)
        throw new BibliothequeException($"L'année {anneePublication} n'est pas valable : ...");

    Livre livre = new Livre(_prochainIdLivre, titre.Trim(), auteur.Trim(), anneePublication);
    _prochainIdLivre++;          // le prochain livre aura le numéro suivant
    _livres.Add(livre);
    return livre;
}
```

Les données sont **vérifiées avant** de créer le livre : un livre sans titre, ou publié en l'an 3000, n'entre jamais dans le catalogue (1450 : à peu près l'invention de l'imprimerie par Gutenberg).

![Capture 08 – Début de la classe Bibliotheque](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/08-vs-classe-bibliotheque.png)

**Ce que montre la capture :** le haut de `Bibliotheque.cs` : le commentaire de la classe (« Cette classe ne fait AUCUN affichage »), la constante `MaxEmpruntsParMembre = 3`, puis les listes privées `_livres` et `_membres` (la troisième, `_emprunts`, suit juste en dessous). `readonly` signifie que la liste elle-même ne peut pas être remplacée par une autre ; on peut seulement y ajouter des éléments.

![Capture 08a – La méthode AjouterLivre et ses vérifications](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/08a-vs-bibliotheque-ajouterlivre.png)

**Ce que montre la capture :** `AjouterLivre` vérifie d'abord le titre et l'auteur (`string.IsNullOrWhiteSpace` : vide ou composé seulement d'espaces), puis l'année, comparée à `DateTime.Today.Year` (l'année en cours). Chaque problème déclenche une `BibliothequeException` avec un message clair.

**Retrouver un livre : la recherche séquentielle.** Pour trouver le livre n°5, `TrouverLivre` parcourt la liste élément par élément avec une boucle `foreach` et s'arrête dès qu'il le trouve (`return`). Si la boucle arrive au bout sans l'avoir trouvé, une erreur est déclenchée. C'est l'algorithme de recherche le plus simple ; il est largement suffisant pour une petite bibliothèque.

![Capture 08b – TrouverLivre : la recherche séquentielle](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/08b-vs-bibliotheque-recherche.png)

**Ce que montre la capture :** la méthode `TrouverLivre` : la boucle `foreach` qui compare le numéro de chaque livre, le `return` qui arrête la recherche dès que le livre est trouvé, puis le `throw` exécuté seulement si la boucle s'est terminée sans résultat.

**Emprunter : l'algorithme.** Avant de coder, on écrit les étapes en français (pseudo-code) :

```text
EMPRUNTER(numéro du livre, numéro du membre)
  1. trouver le livre et le membre       -> si l'un n'existe pas : ERREUR « aucun … ne porte le numéro … »
  2. si le livre n'est pas disponible     -> ERREUR « déjà emprunté par … »
  3. si le membre a déjà 3 emprunts en cours -> ERREUR « maximum autorisé »
  4. sinon : créer l'emprunt (date du jour, retour prévu dans 14 jours)
             et marquer le livre comme emprunté
```

Le code C# suit exactement ces quatre étapes, numérotées de la même façon dans les commentaires :

![Capture 09 – La méthode Emprunter dans Visual Studio](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/09-vs-bibliotheque-emprunter.png)

**Ce que montre la capture :** le début de `Emprunter` : l'étape `// 1)` cherche le livre et le membre (une erreur part automatiquement si l'un n'existe pas), puis l'étape `// 2)` refuse un livre déjà emprunté ; le message est enrichi avec le nom de l'emprunteur et la date de retour prévue.

![Capture 09a – La méthode Emprunter : étapes 3 et 4](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/09a-vs-bibliotheque-emprunter-suite.png)

**Ce que montre la capture :** l'étape `// 3)` compare le nombre d'emprunts en cours du membre à `MaxEmpruntsParMembre` ; l'étape `// 4)` crée l'`Emprunt` daté du jour, l'ajoute à la liste et marque le livre comme emprunté, puis le renvoie à `Program` pour l'affichage.

Le retour fait le chemin inverse :

![Capture 09b – La méthode Retourner dans Visual Studio](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/09b-vs-bibliotheque-retourner.png)

**Ce que montre la capture :** `Retourner` cherche l'emprunt **en cours** du livre ; s'il n'y en a pas (`null`), le retour est refusé. Sinon, l'emprunt est clôturé à la date du jour et le livre redevient disponible.

### 4.5 La gestion des erreurs : `throw`, `try` et `catch`

Le programme distingue deux niveaux d'erreurs :

| Niveau | Exemple | Traitement |
|---|---|---|
| **Erreur de saisie** | On tape « abc » au lieu d'un nombre, ou on laisse un champ vide | `Program` affiche un message et **redemande** la saisie (boucle) |
| **Règle métier non respectée** | Livre déjà emprunté, numéro inexistant, 3 emprunts atteints, année impossible | `Bibliotheque` **déclenche** une `BibliothequeException` (`throw`), `Program` l'**attrape** (`catch`) et affiche le message en rouge |

Une **exception** est un signal d'erreur. Quand le code exécute `throw`, il s'arrête net et le signal « remonte » jusqu'au bloc `catch` le plus proche :

```text
Program (menu)  ── appelle ──▶  bibliotheque.Emprunter(1, 2)
      ▲                                  │  le livre n°1 est déjà emprunté
      │                                  ▼
   catch  ◀──── l'erreur remonte ──── throw new BibliothequeException("Le livre … est déjà emprunté …")
   affiche le message en rouge, puis le menu reprend normalement
```

Plutôt que d'utiliser une exception générique, le projet définit **sa propre exception**, qui hérite de la classe `Exception` de .NET :

![Capture 10 – La classe BibliothequeException](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/10-vs-classe-exception.png)

**Ce que montre la capture :** `BibliothequeException : Exception` (les deux-points signifient « hérite de »). Son constructeur transmet le message à la classe parente avec `: base(message)`. Grâce à ce type dédié, le `catch` de `Program` n'attrape **que** les erreurs prévues par les règles de la bibliothèque ; un vrai bug, lui, ne serait pas masqué.

:::info
**À retenir – exceptions**
- `throw new MonException("message")` : déclencher une erreur.
- `try { ... } catch (MonException e) { ... }` : surveiller un bloc de code et réagir si l'erreur arrive ; `e.Message` contient le texte.
- On attrape le type d'exception le plus précis possible, pour ne pas cacher les autres problèmes.
:::

### 4.6 L'interface console (classe Program)

Le cœur de `Program` est une **boucle** qui affiche le menu tant que l'utilisateur ne choisit pas 0 :

```csharp
bool continuer = true;
while (continuer)                       // on recommence tant que continuer vaut true
{
    AfficherMenu(bibliotheque);
    string choix = LireLigne("Votre choix : ");
    try
    {
        switch (choix)                  // on exécute le cas qui correspond au choix
        {
            case "1": AjouterUnLivre(bibliotheque); break;
            ...
            case "5": EmprunterUnLivre(bibliotheque); break;
            case "0": continuer = false; break;     // fait sortir de la boucle
            default: AfficherErreur($"Choix « {choix} » inconnu : tapez un chiffre de 0 à 7."); break;
        }
    }
    catch (BibliothequeException erreur)
    {
        AfficherErreur(erreur.Message);  // message rouge, puis retour au menu
    }
}
```

![Capture 04a – La boucle du menu dans Program.cs](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/04a-vs-program-boucle-menu.png)

**Ce que montre la capture :** dans Visual Studio, la variable `continuer`, la boucle `while (continuer)`, l'affichage du menu, la lecture du choix, puis le bloc `try` et le `switch` avec un `case` par choix du menu (de « 1 » à « 7 », et « 0 » qui met `continuer` à `false`). Le `catch` qui affiche les erreurs se trouve juste en dessous (voir l'extrait ci-dessus).

Pour lire un nombre sans faire planter le programme, on utilise `int.TryParse` :

```csharp
static int LireEntier(string question)
{
    while (true)                                  // on redemande autant de fois que nécessaire
    {
        string saisie = LireLigne(question);
        if (int.TryParse(saisie, out int nombre)) // true si le texte est bien un nombre entier
        {
            return nombre;                        // return fait sortir de la boucle et de la méthode
        }
        AfficherErreur($"« {saisie} » n'est pas un nombre entier, réessayez.");
    }
}
```

![Capture 04b – Les méthodes de saisie dans Program.cs](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/04b-vs-program-saisie-tryparse.png)

**Ce que montre la capture :** la méthode `LireEntier` complète dans Visual Studio : la boucle `while (true)`, `int.TryParse` qui range le nombre dans `nombre` si la conversion réussit, le `return` qui sort de la boucle, et sinon le message d'erreur avant de redemander. En bas commence `AttendreEntree`, la pause « Appuyez sur Entrée pour revenir au menu ».

:::info
**À retenir – `int.Parse` ou `int.TryParse` ?**
`int.Parse("abc")` **plante** le programme (exception `FormatException`). `int.TryParse("abc", out int n)` renvoie simplement `false` : on peut alors afficher un message et redemander. Pour une saisie au clavier, on utilise toujours `TryParse`.
:::

Point d'attention : `Console.ReadLine()` renvoie un `string?` (un texte **ou** `null`), par exemple si le flux d'entrée est fermé avec Ctrl+Z. Comme `<Nullable>enable</Nullable>` est activé, le compilateur signalerait un avertissement si ce cas était ignoré. Dans `LireLigne`, le cas `null` est donc traité : le programme s'arrête proprement avec un message.

Autres détails de l'interface : les couleurs (`Console.ForegroundColor`) affichent les succès en vert et les erreurs en rouge, les tableaux sont alignés en colonnes grâce à la syntaxe `{texte,-34}` (texte aligné à gauche sur 34 caractères), et `Console.OutputEncoding = Encoding.UTF8` permet d'afficher correctement les accents et les guillemets « ».

### 4.7 Compilation

**Compiler**, c'est traduire le code C# (lisible par un humain) en un programme que l'ordinateur peut exécuter. Le compilateur vérifie au passage qu'il n'y a pas d'erreur de syntaxe ni d'incohérence de types.

![Capture 11 – Compilation de la solution](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/11-compilation-dotnet-build.png)

**Ce que montre la capture :** `dotnet build` compile la solution et produit `GestionBibliotheque.dll` dans `bin\Debug\net10.0`. Le résumé final indique **0 avertissement et 0 erreur** : le code est propre, y compris pour les vérifications `null` activées par `<Nullable>enable</Nullable>`. L'option `-tl:off` sert seulement à afficher ce résumé classique plutôt que l'affichage animé par défaut.

---

## 5. Démarche de test

**Un test manuel**, c'est jouer le rôle de l'utilisateur : on fait des saisies précises et on compare le **résultat obtenu** au **résultat attendu**, décidé *avant* de lancer le test. Un **jeu de tests** est la liste de ces scénarios. Il doit couvrir les cas normaux **et** les cas d'erreur, sinon on ne prouve pas que le programme résiste aux mauvaises manipulations.

**Données de départ.** Au lancement, le programme charge un petit jeu de données de démonstration, pour pouvoir tester tout de suite :

| Livres | Membres | Emprunt déjà en cours |
|---|---|---|
| 1 Le Petit Prince · 2 L'Étranger · 3 Les Misérables · 4 Vingt mille lieues sous les mers · 5 Heidi · 6 Le Comte de Monte-Cristo | 1 Alice Martin · 2 Lucas Rossier · 3 Sofia Bianchi | « Les Misérables » chez Lucas Rossier |

**Déroulement.** Les 19 tests s'enchaînent dans **une seule session** du programme, dans l'ordre du tableau : l'état de la bibliothèque évolue d'un test à l'autre (par exemple, T09 tente d'emprunter le livre qu'Alice a pris en T08). Les tests portent sur la date du 5 octobre 2026 : les retours prévus tombent donc le 19 octobre 2026.

**Exécution et captures.** Pour que les tests soient rejouables à l'identique, les saisies de chaque scénario ont été envoyées au programme par un script PowerShell, ligne par ligne, exactement comme si elles étaient tapées au clavier : c'est la **redirection de l'entrée standard** (l'entrée standard, *stdin*, est le canal par lequel un programme console reçoit ce qu'on tape). Le script prend une capture de la fenêtre à la fin de chaque test. Seule différence avec une vraie frappe : la console n'affiche pas un texte qui arrive par redirection. Le programme réaffiche donc lui-même chaque saisie lorsque `Console.IsInputRedirected` vaut `true`, pour que les captures restent lisibles.

---

## 6. Jeu de tests manuels

### 6.1 Tableau récapitulatif

| N° | Fonction testée | Saisies | Résultat attendu | Résultat obtenu | Statut |
|---|---|---|---|---|---|
| T01 | Démarrage | *(lancement)* | Menu affiché, résumé « 6 livres (5 disponibles), 3 membres, 1 emprunt » | Conforme | ✅ |
| T02 | Liste des livres | `2` | 6 livres ; « Les Misérables » est « Emprunté » | Conforme | ✅ |
| T03 | Ajout d'un livre valide | `1` · Le Seigneur des anneaux · J. R. R. Tolkien · 1954 | Message vert « Livre n°7 ajouté » | Conforme | ✅ |
| T04 | Liste après ajout | `2` | 7 livres, le n°7 est « Disponible » | Conforme | ✅ |
| T05 | Saisies invalides à l'ajout | `1` · *(vide)* · Dune · Frank Herbert · abc · 3000 | Champ vide redemandé ; « abc » refusé ; année 3000 refusée ; livre **non** ajouté | Conforme | ✅ |
| T06 | Inscription d'un membre | `3` · Noah · Dubois | Message vert « Membre n°4 inscrit » | Conforme | ✅ |
| T07 | Liste des membres | `4` | 4 membres ; Lucas a 1 emprunt sur 3 | Conforme | ✅ |
| T08 | Emprunt valide | `5` · 1 · 1 | Alice emprunte « Le Petit Prince », retour prévu le 19.10.2026 | Conforme | ✅ |
| T09 | **Livre déjà emprunté** | `5` · 1 · 2 | Erreur « déjà emprunté par Alice Martin » ; aucun emprunt créé | Conforme | ✅ |
| T10 | Livre inexistant | `5` · 99 | Erreur « Aucun livre ne porte le numéro 99 » | Conforme | ✅ |
| T11 | Membre inexistant | `5` · 2 · 99 | Erreur « Aucun membre ne porte le numéro 99 » | Conforme | ✅ |
| T12 | Limite de 3 emprunts | *(Alice emprunte aussi les livres 2 et 4)* puis `5` · 5 · 1 | Erreur « Alice Martin a déjà 3 emprunts en cours » | Conforme | ✅ |
| T13 | Liste des emprunts en cours | `7` | 4 emprunts : 1 de Lucas, 3 d'Alice | Conforme | ✅ |
| T14 | Retour valide | `6` · 1 | « Le Petit Prince » de nouveau disponible | Conforme | ✅ |
| T15 | Retour d'un livre non emprunté | `6` · 1 | Erreur « n'est pas emprunté : il est déjà en rayon » | Conforme | ✅ |
| T16 | Emprunt après un retour | `5` · 5 · 1 | Alice (2/3) peut à nouveau emprunter : elle prend « Heidi » | Conforme | ✅ |
| T17 | Catalogue final | `2` | État cohérent avec les tests précédents ; « Dune » absent | Conforme | ✅ |
| T18 | Choix de menu invalide | `9` | Erreur « Choix « 9 » inconnu » | Conforme | ✅ |
| T19 | Quitter | `0` | Message d'au revoir, fin du programme | Conforme | ✅ |

**Bilan : 19 tests sur 19 conformes**, dont 7 consacrés aux cas d'erreur (T05, T09, T10, T11, T12, T15 et T18).

### 6.2 Détail des tests avec captures

#### T01 – Démarrage du programme

Premier contrôle : le programme se lance, charge les données de démonstration et affiche le menu.

![Capture 12 – T01 menu au démarrage](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/12-T01-menu-demarrage.png)

**Ce que montre la capture :** le titre de la bibliothèque, la ligne de résumé « 6 livres (5 disponibles) | 3 membres | 1 emprunt(s) en cours » (les 5 disponibles s'expliquent par l'emprunt de départ) et les 8 choix du menu. Les accents et les guillemets « » s'affichent correctement grâce à l'UTF-8.

#### T02 – Liste des livres (état initial)

On vérifie que le catalogue de départ est complet et que la disponibilité de chaque livre est juste.

![Capture 13 – T02 catalogue initial](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/13-T02-catalogue-initial.png)

**Ce que montre la capture :** les 6 livres, alignés en colonnes. « Les Misérables » est marqué « Emprunté » (en jaune), les 5 autres « Disponible » (en vert) : l'emprunt de départ est bien pris en compte.

#### T03 – Ajout d'un livre valide

Étape 2 de l'énoncé : ajouter un livre avec des données correctes.

![Capture 14 – T03 ajout d'un livre](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/14-T03-ajout-livre.png)

**Ce que montre la capture :** après la saisie du titre, de l'auteur et de l'année, le message vert « [OK] Livre n°7 ajouté au catalogue » confirme l'ajout. Le numéro 7 a été attribué automatiquement par le compteur de `Bibliotheque`.

#### T04 – Liste après l'ajout

Un ajout ne se vérifie pas seulement par le message : il faut contrôler que le livre apparaît vraiment dans la liste.

![Capture 15 – T04 catalogue après ajout](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/15-T04-catalogue-apres-ajout.png)

**Ce que montre la capture :** le catalogue compte maintenant 7 livres ; « Le Seigneur des anneaux » apparaît en dernière ligne, « Disponible ».

#### T05 – Saisies invalides lors d'un ajout

On teste les mauvaises manipulations : champ vide, texte à la place d'un nombre, année impossible.

![Capture 16 – T05 saisies invalides](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/16-T05-saisies-invalides.png)

**Ce que montre la capture :** les trois protections fonctionnent : le titre vide est refusé et redemandé (« Ce champ est obligatoire »), « abc » est refusé et l'année est redemandée (`TryParse`), puis l'année 3000 est rejetée par la règle de `Bibliotheque` (« doit être comprise entre 1450 et 2026 »). Le livre « Dune » n'est pas ajouté, ce que confirme le catalogue final (T17).

#### T06 – Inscription d'un membre

![Capture 17 – T06 inscription d'un membre](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/17-T06-inscription-membre.png)

**Ce que montre la capture :** Noah Dubois est inscrit avec le numéro 4, attribué automatiquement.

#### T07 – Liste des membres

On contrôle l'inscription et le compteur d'emprunts de chaque membre, qui servira pour la limite de 3.

![Capture 18 – T07 liste des membres](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/18-T07-liste-membres.png)

**Ce que montre la capture :** les 4 membres, dont Noah Dubois. Lucas Rossier a « 1 / 3 » emprunt (« Les Misérables »), les autres « 0 / 3 ».

#### T08 – Emprunt valide

Étape 3 de l'énoncé : Alice Martin (membre n°1) emprunte « Le Petit Prince » (livre n°1).

![Capture 19 – T08 emprunt valide](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/19-T08-emprunt-ok.png)

**Ce que montre la capture :** l'écran d'emprunt affiche d'abord les livres et les membres pour choisir les numéros. Après la saisie, le programme confirme le livre et le membre choisis (lignes « -> »), puis affiche « [OK] Emprunt enregistré » avec la date de retour prévue : 19.10.2026, soit 14 jours plus tard.

#### T09 – Emprunt d'un livre déjà emprunté (cas d'erreur de l'énoncé)

C'est le cas d'erreur demandé explicitement par l'énoncé : Lucas Rossier essaie d'emprunter « Le Petit Prince », qu'Alice vient de prendre.

![Capture 20 – T09 livre déjà emprunté](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/20-T09-livre-deja-emprunte.png)

**Ce que montre la capture :** dans la liste, « Le Petit Prince » est maintenant « Emprunté » et Alice a « 1 / 3 ». L'emprunt est refusé avec un message précis : « Le livre « Le Petit Prince » est déjà emprunté par Alice Martin (retour prévu le 19.10.2026) ». Le programme ne plante pas et revient au menu.

#### T10 – Livre inexistant

![Capture 21 – T10 livre introuvable](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/21-T10-livre-introuvable.png)

**Ce que montre la capture :** le numéro 99 n'existe pas : l'erreur « Aucun livre ne porte le numéro 99 » s'affiche **tout de suite**, sans demander le membre, car `TrouverLivre` est appelée dès la saisie du numéro du livre.

#### T11 – Membre inexistant

![Capture 22 – T11 membre introuvable](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/22-T11-membre-introuvable.png)

**Ce que montre la capture :** le livre n°2 (« L'Étranger ») est bien trouvé, mais le membre n°99 n'existe pas : « Aucun membre ne porte le numéro 99 ». Aucun emprunt n'est créé.

#### T12 – Limite de 3 emprunts par membre

Préparation (non capturée, mêmes écrans que T08) : Alice emprunte aussi « L'Étranger » (n°2) et « Vingt mille lieues sous les mers » (n°4). Elle a donc 3 emprunts en cours, et tente d'en faire un quatrième.

![Capture 23 – T12 limite de 3 emprunts](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/23-T12-limite-3-emprunts.png)

**Ce que montre la capture :** la liste des membres affiche « 3 / 3 » pour Alice Martin, et la demande d'emprunt de « Heidi » est refusée : « Alice Martin a déjà 3 emprunts en cours : c'est le maximum autorisé ».

#### T13 – Liste des emprunts en cours

![Capture 24 – T13 emprunts en cours](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/24-T13-emprunts-en-cours.png)

**Ce que montre la capture :** 4 emprunts en cours : « Les Misérables » chez Lucas Rossier et 3 livres chez Alice Martin, chacun avec sa date d'emprunt et sa date de retour prévue. Les emprunts refusés (T09 à T12) n'apparaissent pas.

#### T14 – Retour valide

Étape 3 de l'énoncé, deuxième partie : Alice rend « Le Petit Prince ».

![Capture 25 – T14 retour valide](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/25-T14-retour-ok.png)

**Ce que montre la capture :** l'écran de retour liste d'abord les emprunts en cours. Après la saisie du n°1 : « [OK] Retour enregistré : « Le Petit Prince » … est de nouveau disponible », avec la période d'emprunt.

#### T15 – Retour d'un livre qui n'est pas emprunté

![Capture 26 – T15 retour d'un livre non emprunté](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/26-T15-retour-livre-non-emprunte.png)

**Ce que montre la capture :** « Le Petit Prince » n'est plus dans la liste des emprunts en cours ; le rendre une deuxième fois est refusé : « n'est pas emprunté : il est déjà en rayon ».

#### T16 – Emprunt possible après un retour

On vérifie que la limite de 3 se libère bien après un retour.

![Capture 27 – T16 emprunt après retour](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/27-T16-emprunt-apres-retour.png)

**Ce que montre la capture :** Alice est revenue à « 2 / 3 » ; l'emprunt de « Heidi », refusé en T12, est maintenant accepté.

#### T17 – Catalogue final

Contrôle global : après tous ces tests, le catalogue doit être cohérent avec ce qui s'est passé.

![Capture 28 – T17 catalogue final](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/28-T17-catalogue-final.png)

**Ce que montre la capture :** « Le Petit Prince » est « Disponible » (rendu en T14) ; « L'Étranger », « Les Misérables », « Vingt mille lieues sous les mers » et « Heidi » sont « Emprunté » ; « Dune », refusé en T05, n'existe pas. Les 7 livres sont cohérents avec l'historique des tests.

#### T18 – Choix de menu invalide

![Capture 29 – T18 choix invalide](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/29-T18-choix-invalide.png)

**Ce que montre la capture :** le choix « 9 » n'existe pas : le cas `default` du `switch` affiche « Choix « 9 » inconnu : tapez un chiffre de 0 à 7 », puis le programme attend Entrée pour réafficher le menu. Au passage, la ligne de résumé (« 7 livres (3 disponibles) | 4 membres | 4 emprunt(s) en cours ») est cohérente avec le catalogue final de T17.

#### T19 – Quitter le programme

![Capture 30 – T19 quitter](https://raw.githubusercontent.com/L4ami/ppe-csharp-projet1-gestion-bibliotheque/main/captures/30-T19-quitter.png)

**Ce que montre la capture :** le choix 0 fait sortir de la boucle du menu (`continuer = false`) ; le programme affiche « Au revoir et à bientôt à la bibliothèque ! » et se termine normalement.

---

## 7. Problèmes rencontrés et solutions

| Problème | Cause | Solution |
|---|---|---|
| Les captures du programme en marche devaient être faites sans taper au clavier dans le Terminal | L'outil d'assistance utilisé pour piloter le PC ne peut que cliquer dans un terminal, pas y taper de texte | Un script PowerShell lance le programme et lui envoie les saisies par **redirection de l'entrée standard**, puis capture la fenêtre après chaque test. Bonus : les tests deviennent rejouables à l'identique. |
| Avec l'entrée redirigée, les saisies n'apparaissaient pas à l'écran (« Votre choix : » restait vide) | La console n'affiche que ce qui est tapé au clavier, pas ce qui arrive par un autre canal | Dans `LireLigne`, le programme réécrit la saisie quand `Console.IsInputRedirected` vaut `true`. |
| `dotnet new sln` a créé un fichier `.slnx` au lieu du `.sln` habituel | Depuis .NET 10, le format XML `.slnx` est le format par défaut | Format conservé : Visual Studio 2026 l'ouvre directement (voir §4.1). |
| Au premier passage, certaines captures étaient peu lisibles : messages longs coupés en plein milieu (« 19. » / « 10.2026 »), et dans Visual Studio on ne voyait qu'un bout de chaque classe | Fenêtre de test trop étroite pour les lignes de plus de 100 caractères ; dans Visual Studio, la Liste d'erreurs occupe le bas de l'écran et l'éditeur n'affiche qu'une vingtaine de lignes | Fenêtre de test élargie, puis tests rejoués (le script le permet en un double-clic). Pour Visual Studio, chaque fichier est capturé en plusieurs parties qui se suivent, et les captures sont recadrées sur l'éditeur ; la Liste d'erreurs reste visible sur la capture 04 comme preuve (0 erreur, 0 avertissement). |

---

## 8. Bilan

L'application répond aux cinq étapes de l'énoncé : les classes sont modélisées (diagramme §3), l'ajout et la liste des livres, l'emprunt et le retour fonctionnent, les cas d'erreur sont gérés sans plantage, et le jeu de **19 tests manuels** est entièrement conforme. Le code compile sans avertissement ni erreur.

**Pistes d'amélioration :**

- **sauvegarder les données dans un fichier** : aujourd'hui, tout est perdu à la fermeture du programme (c'est justement le sujet du projet 3 de ce cours) ;
- rechercher un livre par titre ou par auteur, et pas seulement par numéro ;
- signaler les emprunts **en retard** (date du jour après la date de retour prévue) ;
- ajouter des **tests unitaires automatiques** (par exemple avec xUnit) pour vérifier les règles de `Bibliotheque` à chaque modification du code.

---

## 9. Glossaire

| Terme | Explication simple |
|---|---|
| **Algorithme** | Suite d'étapes précises pour résoudre un problème (ex. : les 5 étapes de l'emprunt). |
| **Application console** | Programme sans fenêtre graphique, utilisé au clavier dans un terminal. |
| **Classe / objet** | La classe est le plan (ex. `Livre`), l'objet est un exemplaire créé avec `new` (ex. « Heidi »). |
| **Propriété** | Donnée d'un objet (ex. `Titre`). `{ get; }` = lecture ; `set` = modification. |
| **Méthode** | Action qu'un objet sait faire (ex. `Emprunter()`). |
| **Constructeur** | Méthode spéciale exécutée au `new`, qui remplit les propriétés de l'objet. |
| **public / private** | `public` : utilisable partout ; `private` : réservé à l'intérieur de la classe. |
| **Encapsulation** | Protéger les données d'un objet en n'y donnant accès que par des méthodes contrôlées. |
| **Héritage** | Créer une classe à partir d'une autre (`BibliothequeException : Exception`). |
| **Polymorphisme / override** | Redéfinir une méthode héritée pour qu'elle se comporte différemment (`override ToString()`). |
| **Exception** | Signal d'erreur ; `throw` le déclenche, `try` / `catch` l'attrape. |
| **const** | Valeur fixe, écrite une fois dans le code (ex. 14 jours de prêt). |
| **Nullable (`?`)** | Type qui peut valoir `null`, c'est-à-dire « rien » (ex. `DateTime?`). |
| **List\<T>** | Liste d'éléments de type T qui s'agrandit avec `Add()`. |
| **foreach / while** | Boucles : `foreach` parcourt chaque élément d'une liste ; `while` répète tant qu'une condition est vraie. |
| **switch** | Choisit le bloc de code à exécuter selon une valeur (le choix du menu). |
| **int.TryParse** | Essaie de convertir un texte en nombre entier sans planter (renvoie `true` ou `false`). |
| **Recherche séquentielle** | Parcourir une liste élément par élément jusqu'à trouver celui qu'on cherche. |
| **IDE** | Logiciel qui réunit éditeur de code, compilation et débogage (Visual Studio). |
| **SDK .NET** | Kit de développement : compilateur C#, bibliothèques et commande `dotnet`. |
| **CLI** | Interface en ligne de commande : on tape des commandes au lieu de cliquer. |
| **Compiler** | Traduire le code C# en programme exécutable, en vérifiant les erreurs. |
| **Solution / projet** | Le projet (`.csproj`) est un programme ; la solution (`.slnx`) regroupe un ou plusieurs projets. |
| **namespace** | « Nom de famille » qui regroupe les classes du projet (`GestionBibliotheque`). |
| **Entrée standard (stdin) / redirection** | Canal par lequel un programme console reçoit les saisies ; on peut le remplacer par un fichier ou un script. |
| **UTF-8** | Façon de coder les caractères qui gère les accents et les symboles. |
| **Git / GitHub** | Git enregistre les versions du code ; GitHub héberge ces versions en ligne. |

---

## 10. Sources

- Énoncé du projet : `05-algo-csharp-319/projet-1-gestion-bibliotheque.md` et `README.md` du cours 05 – dépôt de classe Satom-IT-Learning-Solutions/E1B.
- Microsoft Learn – [Visite guidée de C#](https://learn.microsoft.com/fr-fr/dotnet/csharp/tour-of-csharp/) (ressource citée dans l'énoncé).
- Microsoft Learn – [Programmation orientée objet (C#)](https://learn.microsoft.com/fr-fr/dotnet/csharp/fundamentals/tutorials/oop) (ressource citée dans l'énoncé) : les 4 piliers de la POO (§3.4).
- Documentation .NET – [commande `dotnet new`](https://learn.microsoft.com/fr-fr/dotnet/core/tools/dotnet-new).
- Accompagnement : Claude (assistant IA d'Anthropic), utilisé comme support d'apprentissage, pour la relecture et pour automatiser les captures d'écran.

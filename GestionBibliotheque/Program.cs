using System.Text;

namespace GestionBibliotheque;

/// <summary>
/// Interface de l'application (ce que voit l'utilisateur) : menu, saisies au
/// clavier et affichage des résultats. Les règles sont dans la classe Bibliotheque.
/// </summary>
class Program
{
    /// <summary>
    /// Point d'entrée : c'est la première méthode exécutée au lancement.
    /// </summary>
    static void Main(string[] args)
    {
        // UTF-8 : pour que les accents (é, è, à) et les guillemets « » s'affichent bien.
        Console.OutputEncoding = Encoding.UTF8;
        Console.Title = "Gestion de bibliothèque";

        Bibliotheque bibliotheque = new Bibliotheque("Bibliothèque associative Les Pages Libres");
        ChargerDonneesDeDemonstration(bibliotheque);

        // Boucle principale : on réaffiche le menu tant que l'utilisateur ne choisit pas 0.
        bool continuer = true;
        while (continuer)
        {
            AfficherMenu(bibliotheque);
            string choix = LireLigne("Votre choix : ");

            // try / catch : si une règle n'est pas respectée, la classe Bibliotheque
            // déclenche une BibliothequeException. On l'« attrape » ici pour afficher
            // le message en rouge, au lieu de laisser le programme planter.
            try
            {
                switch (choix)
                {
                    case "1": AjouterUnLivre(bibliotheque); break;
                    case "2": AfficherLivres(bibliotheque); break;
                    case "3": InscrireUnMembre(bibliotheque); break;
                    case "4": AfficherMembres(bibliotheque); break;
                    case "5": EmprunterUnLivre(bibliotheque); break;
                    case "6": RetournerUnLivre(bibliotheque); break;
                    case "7": AfficherEmpruntsEnCours(bibliotheque); break;
                    case "0": continuer = false; break;
                    default:
                        AfficherErreur($"Choix « {choix} » inconnu : tapez un chiffre de 0 à 7.");
                        break;
                }
            }
            catch (BibliothequeException erreur)
            {
                Console.WriteLine();
                AfficherErreur(erreur.Message);
            }

            if (continuer)
            {
                AttendreEntree();
            }
        }

        Console.WriteLine();
        Console.WriteLine("Au revoir et à bientôt à la bibliothèque !");
    }

    // ======================================================================
    //  MENU
    // ======================================================================

    static void AfficherMenu(Bibliotheque bibliotheque)
    {
        AfficherTitre(bibliotheque.Nom.ToUpper());

        // Petit résumé de la situation, recalculé à chaque affichage du menu.
        int nbLivres = bibliotheque.ObtenirLivres().Count;
        int nbMembres = bibliotheque.ObtenirMembres().Count;
        int nbEmprunts = bibliotheque.ObtenirEmpruntsEnCours().Count;
        Console.WriteLine($"  {nbLivres} livres ({nbLivres - nbEmprunts} disponibles)  |  {nbMembres} membres  |  {nbEmprunts} emprunt(s) en cours");
        Console.WriteLine();

        Console.WriteLine("  1. Ajouter un livre");
        Console.WriteLine("  2. Afficher le catalogue des livres");
        Console.WriteLine("  3. Inscrire un membre");
        Console.WriteLine("  4. Afficher les membres");
        Console.WriteLine("  5. Emprunter un livre");
        Console.WriteLine("  6. Retourner un livre");
        Console.WriteLine("  7. Afficher les emprunts en cours");
        Console.WriteLine("  0. Quitter");
        Console.WriteLine();
    }

    // ======================================================================
    //  LES 7 ACTIONS DU MENU
    // ======================================================================

    static void AjouterUnLivre(Bibliotheque bibliotheque)
    {
        AfficherTitre("AJOUTER UN LIVRE");

        string titre = LireTexteObligatoire("Titre                : ");
        string auteur = LireTexteObligatoire("Auteur               : ");
        int annee = LireEntier("Année de publication : ");

        // Si l'année n'est pas valable, AjouterLivre déclenche une erreur :
        // on saute directement au "catch" de Main et le livre n'est PAS ajouté.
        Livre livre = bibliotheque.AjouterLivre(titre, auteur, annee);

        Console.WriteLine();
        AfficherSucces($"Livre n°{livre.Id} ajouté au catalogue : {livre}.");
    }

    static void AfficherLivres(Bibliotheque bibliotheque)
    {
        AfficherTitre("CATALOGUE DES LIVRES");
        AfficherTableauLivres(bibliotheque.ObtenirLivres());
    }

    static void InscrireUnMembre(Bibliotheque bibliotheque)
    {
        AfficherTitre("INSCRIRE UN MEMBRE");

        string prenom = LireTexteObligatoire("Prénom : ");
        string nom = LireTexteObligatoire("Nom    : ");

        Membre membre = bibliotheque.AjouterMembre(prenom, nom);

        Console.WriteLine();
        AfficherSucces($"Membre n°{membre.Id} inscrit : {membre.NomComplet}.");
    }

    static void AfficherMembres(Bibliotheque bibliotheque)
    {
        AfficherTitre("MEMBRES INSCRITS");
        AfficherTableauMembres(bibliotheque);
    }

    static void EmprunterUnLivre(Bibliotheque bibliotheque)
    {
        AfficherTitre("EMPRUNTER UN LIVRE");
        AfficherTableauLivres(bibliotheque.ObtenirLivres());
        Console.WriteLine();
        AfficherTableauMembres(bibliotheque);
        Console.WriteLine();

        int idLivre = LireEntier("N° du livre à emprunter : ");
        // On vérifie tout de suite que le livre existe (erreur immédiate sinon).
        Livre livre = bibliotheque.TrouverLivre(idLivre);
        Console.WriteLine($"   -> {livre}");

        int idMembre = LireEntier("N° du membre emprunteur  : ");
        Membre membre = bibliotheque.TrouverMembre(idMembre);
        Console.WriteLine($"   -> {membre.NomComplet}");

        // C'est ici que les règles sont vérifiées (livre libre, limite de 3...).
        Emprunt emprunt = bibliotheque.Emprunter(livre.Id, membre.Id);

        Console.WriteLine();
        AfficherSucces($"Emprunt enregistré : {emprunt.Membre.NomComplet} emprunte {emprunt.Livre}.");
        Console.WriteLine($"  À rendre au plus tard le {emprunt.DateRetourPrevue:dd.MM.yyyy} (durée du prêt : {Emprunt.DureeEnJours} jours).");
    }

    static void RetournerUnLivre(Bibliotheque bibliotheque)
    {
        AfficherTitre("RETOURNER UN LIVRE");
        Console.WriteLine("  Emprunts en cours :");
        Console.WriteLine();
        AfficherTableauEmprunts(bibliotheque.ObtenirEmpruntsEnCours());
        Console.WriteLine();

        int idLivre = LireEntier("N° du livre rendu : ");
        Emprunt emprunt = bibliotheque.Retourner(idLivre);

        Console.WriteLine();
        AfficherSucces($"Retour enregistré : {emprunt.Livre} est de nouveau disponible.");
        Console.WriteLine($"  Emprunté par {emprunt.Membre.NomComplet} du {emprunt.DateEmprunt:dd.MM.yyyy} au {emprunt.DateRetour:dd.MM.yyyy}.");
    }

    static void AfficherEmpruntsEnCours(Bibliotheque bibliotheque)
    {
        AfficherTitre("EMPRUNTS EN COURS");
        AfficherTableauEmprunts(bibliotheque.ObtenirEmpruntsEnCours());
    }

    // ======================================================================
    //  AFFICHAGE (tableaux, couleurs)
    // ======================================================================

    static void AfficherTableauLivres(List<Livre> livres)
    {
        // {texte,-N} : écrit le texte aligné à gauche sur N caractères,
        // ce qui donne des colonnes bien alignées.
        Console.WriteLine($"  {"N°",-4}{"Titre",-34}{"Auteur",-27}{"Année",-7}Statut");
        Console.WriteLine("  " + new string('-', 82));

        foreach (Livre livre in livres)
        {
            Console.Write($"  {livre.Id,-4}{Raccourcir(livre.Titre, 33),-34}{Raccourcir(livre.Auteur, 26),-27}{livre.AnneePublication,-7}");
            if (livre.EstDisponible)
            {
                EcrireEnCouleur("Disponible", ConsoleColor.Green);
            }
            else
            {
                EcrireEnCouleur("Emprunté", ConsoleColor.Yellow);
            }
        }
    }

    static void AfficherTableauMembres(Bibliotheque bibliotheque)
    {
        Console.WriteLine($"  {"N°",-4}{"Membre",-30}Emprunts en cours");
        Console.WriteLine("  " + new string('-', 51));

        foreach (Membre membre in bibliotheque.ObtenirMembres())
        {
            int nbEmprunts = bibliotheque.CompterEmpruntsEnCours(membre);
            Console.WriteLine($"  {membre.Id,-4}{Raccourcir(membre.NomComplet, 29),-30}{nbEmprunts} / {Bibliotheque.MaxEmpruntsParMembre}");
        }
    }

    static void AfficherTableauEmprunts(List<Emprunt> emprunts)
    {
        if (emprunts.Count == 0)
        {
            Console.WriteLine("  Aucun emprunt en cours.");
            return;
        }

        Console.WriteLine($"  {"N°",-4}{"Livre",-34}{"Membre",-20}{"Emprunté le",-13}À rendre le");
        Console.WriteLine("  " + new string('-', 82));

        foreach (Emprunt emprunt in emprunts)
        {
            Console.WriteLine($"  {emprunt.Livre.Id,-4}{Raccourcir(emprunt.Livre.Titre, 33),-34}{Raccourcir(emprunt.Membre.NomComplet, 19),-20}{emprunt.DateEmprunt,-13:dd.MM.yyyy}{emprunt.DateRetourPrevue:dd.MM.yyyy}");
        }
    }

    static void AfficherTitre(string titre)
    {
        EffacerEcran();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  " + new string('=', 60));
        Console.WriteLine("    " + titre);
        Console.WriteLine("  " + new string('=', 60));
        Console.ResetColor();
        Console.WriteLine();
    }

    static void AfficherSucces(string message)
    {
        EcrireEnCouleur("  [OK] " + message, ConsoleColor.Green);
    }

    static void AfficherErreur(string message)
    {
        EcrireEnCouleur("  [ERREUR] " + message, ConsoleColor.Red);
    }

    static void EcrireEnCouleur(string texte, ConsoleColor couleur)
    {
        Console.ForegroundColor = couleur;
        Console.WriteLine(texte);
        Console.ResetColor(); // on remet la couleur normale pour la suite
    }

    static void EffacerEcran()
    {
        // Console.Clear() plante si l'affichage est redirigé vers un fichier
        // (ex. : dotnet run > resultat.txt) : on ne l'appelle que dans une vraie console.
        if (!Console.IsOutputRedirected)
        {
            Console.Clear();
        }
        else
        {
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Coupe un texte trop long pour qu'il ne déborde pas de sa colonne.
    /// </summary>
    static string Raccourcir(string texte, int longueurMax)
    {
        if (texte.Length <= longueurMax)
        {
            return texte;
        }
        return texte.Substring(0, longueurMax - 3) + "...";
    }

    // ======================================================================
    //  SAISIES AU CLAVIER (avec contrôle des erreurs de frappe)
    // ======================================================================

    /// <summary>
    /// Affiche une question et renvoie la réponse tapée (sans espaces autour).
    /// </summary>
    static string LireLigne(string question)
    {
        Console.Write(question);
        string? saisie = Console.ReadLine();

        // ReadLine() renvoie null quand il n'y a plus rien à lire
        // (par exemple Ctrl+Z puis Entrée) : on arrête le programme proprement.
        if (saisie == null)
        {
            Console.WriteLine();
            Console.WriteLine("[Fin des saisies : le programme s'arrête.]");
            Environment.Exit(0);
        }

        // Quand les saisies viennent d'un fichier ou d'un script (entrée redirigée),
        // la console ne les affiche pas : on les réécrit pour garder une trace lisible.
        if (Console.IsInputRedirected)
        {
            Console.WriteLine(saisie);
        }

        return saisie.Trim();
    }

    /// <summary>
    /// Redemande tant que la réponse est vide.
    /// </summary>
    static string LireTexteObligatoire(string question)
    {
        while (true)
        {
            string saisie = LireLigne(question);
            if (saisie != "")
            {
                return saisie;
            }
            AfficherErreur("Ce champ est obligatoire, merci de le remplir.");
        }
    }

    /// <summary>
    /// Redemande tant que la réponse n'est pas un nombre entier.
    /// </summary>
    static int LireEntier(string question)
    {
        while (true)
        {
            string saisie = LireLigne(question);

            // int.TryParse essaie de convertir le texte en nombre entier :
            // il renvoie true si ça marche (le nombre est rangé dans "nombre"),
            // false sinon, SANS planter (contrairement à int.Parse).
            if (int.TryParse(saisie, out int nombre))
            {
                return nombre;
            }
            AfficherErreur($"« {saisie} » n'est pas un nombre entier, réessayez.");
        }
    }

    static void AttendreEntree()
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        LireLigne("Appuyez sur Entrée pour revenir au menu...");
        Console.ResetColor();
    }

    // ======================================================================
    //  DONNÉES DE DÉMONSTRATION
    // ======================================================================

    /// <summary>
    /// Remplit la bibliothèque au démarrage, pour pouvoir tester tout de suite.
    /// On passe par les mêmes méthodes que l'utilisateur : les règles s'appliquent aussi.
    /// </summary>
    static void ChargerDonneesDeDemonstration(Bibliotheque bibliotheque)
    {
        bibliotheque.AjouterLivre("Le Petit Prince", "Antoine de Saint-Exupéry", 1943);
        bibliotheque.AjouterLivre("L'Étranger", "Albert Camus", 1942);
        bibliotheque.AjouterLivre("Les Misérables", "Victor Hugo", 1862);
        bibliotheque.AjouterLivre("Vingt mille lieues sous les mers", "Jules Verne", 1870);
        bibliotheque.AjouterLivre("Heidi", "Johanna Spyri", 1880);
        bibliotheque.AjouterLivre("Le Comte de Monte-Cristo", "Alexandre Dumas", 1844);

        bibliotheque.AjouterMembre("Alice", "Martin");
        bibliotheque.AjouterMembre("Lucas", "Rossier");
        bibliotheque.AjouterMembre("Sofia", "Bianchi");

        // Un emprunt déjà en cours au démarrage : Les Misérables (n°3) chez Lucas (n°2).
        bibliotheque.Emprunter(3, 2);
    }
}

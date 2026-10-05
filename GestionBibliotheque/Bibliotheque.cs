namespace GestionBibliotheque;

/// <summary>
/// Le « cerveau » de l'application : elle contient les livres, les membres et
/// les emprunts, et fait respecter les règles de la bibliothèque.
/// Cette classe ne fait AUCUN affichage : l'affichage est le travail de Program.
/// </summary>
public class Bibliotheque
{
    /// <summary>
    /// Nombre maximum d'emprunts en cours pour un même membre.
    /// </summary>
    public const int MaxEmpruntsParMembre = 3;

    // Les trois listes sont privées : on ne peut les modifier qu'en passant
    // par les méthodes publiques ci-dessous, qui vérifient les règles.
    private readonly List<Livre> _livres = new List<Livre>();
    private readonly List<Membre> _membres = new List<Membre>();
    private readonly List<Emprunt> _emprunts = new List<Emprunt>();

    // Compteurs qui attribuent automatiquement les numéros : 1, 2, 3...
    private int _prochainIdLivre = 1;
    private int _prochainIdMembre = 1;

    public string Nom { get; }

    public Bibliotheque(string nom)
    {
        Nom = nom;
    }

    // ======================================================================
    //  LIVRES
    // ======================================================================

    /// <summary>
    /// Ajoute un livre au catalogue et le renvoie (avec son numéro).
    /// </summary>
    public Livre AjouterLivre(string titre, string auteur, int anneePublication)
    {
        // On vérifie les données AVANT de créer le livre.
        if (string.IsNullOrWhiteSpace(titre))
        {
            throw new BibliothequeException("Le titre du livre est obligatoire.");
        }
        if (string.IsNullOrWhiteSpace(auteur))
        {
            throw new BibliothequeException("L'auteur du livre est obligatoire.");
        }

        int anneeMax = DateTime.Today.Year;
        if (anneePublication < 1450 || anneePublication > anneeMax)
        {
            throw new BibliothequeException(
                $"L'année {anneePublication} n'est pas valable : elle doit être comprise entre 1450 et {anneeMax}.");
        }

        Livre livre = new Livre(_prochainIdLivre, titre.Trim(), auteur.Trim(), anneePublication);
        _prochainIdLivre++;
        _livres.Add(livre);
        return livre;
    }

    /// <summary>
    /// Renvoie une COPIE de la liste des livres : le programme peut la lire et
    /// l'afficher, mais pas ajouter ou supprimer des livres sans passer par les règles.
    /// </summary>
    public List<Livre> ObtenirLivres()
    {
        return new List<Livre>(_livres);
    }

    /// <summary>
    /// Cherche un livre par son numéro. Déclenche une erreur s'il n'existe pas.
    /// </summary>
    public Livre TrouverLivre(int id)
    {
        // Recherche séquentielle : on regarde les livres un par un.
        foreach (Livre livre in _livres)
        {
            if (livre.Id == id)
            {
                return livre; // trouvé : on s'arrête là
            }
        }

        // Si on arrive ici, la boucle a tout parcouru sans trouver.
        throw new BibliothequeException($"Aucun livre ne porte le numéro {id}.");
    }

    // ======================================================================
    //  MEMBRES
    // ======================================================================

    /// <summary>
    /// Inscrit un nouveau membre et le renvoie (avec son numéro).
    /// </summary>
    public Membre AjouterMembre(string prenom, string nom)
    {
        if (string.IsNullOrWhiteSpace(prenom) || string.IsNullOrWhiteSpace(nom))
        {
            throw new BibliothequeException("Le prénom et le nom du membre sont obligatoires.");
        }

        Membre membre = new Membre(_prochainIdMembre, prenom.Trim(), nom.Trim());
        _prochainIdMembre++;
        _membres.Add(membre);
        return membre;
    }

    public List<Membre> ObtenirMembres()
    {
        return new List<Membre>(_membres);
    }

    /// <summary>
    /// Cherche un membre par son numéro. Déclenche une erreur s'il n'existe pas.
    /// </summary>
    public Membre TrouverMembre(int id)
    {
        foreach (Membre membre in _membres)
        {
            if (membre.Id == id)
            {
                return membre;
            }
        }

        throw new BibliothequeException($"Aucun membre ne porte le numéro {id}.");
    }

    /// <summary>
    /// Compte les livres qu'un membre a actuellement chez lui.
    /// </summary>
    public int CompterEmpruntsEnCours(Membre membre)
    {
        int compteur = 0;
        foreach (Emprunt emprunt in _emprunts)
        {
            if (emprunt.Membre == membre && emprunt.EstEnCours)
            {
                compteur++;
            }
        }
        return compteur;
    }

    // ======================================================================
    //  EMPRUNTS ET RETOURS
    // ======================================================================

    /// <summary>
    /// Enregistre l'emprunt d'un livre par un membre, si les règles le permettent.
    /// </summary>
    public Emprunt Emprunter(int idLivre, int idMembre)
    {
        // 1) Le livre et le membre doivent exister (sinon erreur).
        Livre livre = TrouverLivre(idLivre);
        Membre membre = TrouverMembre(idMembre);

        // 2) Règle : un livre déjà emprunté ne peut pas partir une deuxième fois.
        if (!livre.EstDisponible)
        {
            Emprunt? empruntActuel = TrouverEmpruntEnCours(livre);
            string detail = "";
            if (empruntActuel != null)
            {
                detail = $" par {empruntActuel.Membre.NomComplet} (retour prévu le {empruntActuel.DateRetourPrevue:dd.MM.yyyy})";
            }
            throw new BibliothequeException($"Le livre « {livre.Titre} » est déjà emprunté{detail}.");
        }

        // 3) Règle : pas plus de 3 emprunts en cours par membre.
        if (CompterEmpruntsEnCours(membre) >= MaxEmpruntsParMembre)
        {
            throw new BibliothequeException(
                $"{membre.NomComplet} a déjà {MaxEmpruntsParMembre} emprunts en cours : c'est le maximum autorisé.");
        }

        // 4) Tout est en ordre : on crée l'emprunt et le livre n'est plus disponible.
        Emprunt emprunt = new Emprunt(livre, membre, DateTime.Today);
        _emprunts.Add(emprunt);
        livre.MarquerCommeEmprunte();
        return emprunt;
    }

    /// <summary>
    /// Enregistre le retour d'un livre emprunté.
    /// </summary>
    public Emprunt Retourner(int idLivre)
    {
        Livre livre = TrouverLivre(idLivre);

        // Règle : on ne peut rendre qu'un livre qui est vraiment emprunté.
        Emprunt? emprunt = TrouverEmpruntEnCours(livre);
        if (emprunt == null)
        {
            throw new BibliothequeException(
                $"Le livre « {livre.Titre} » n'est pas emprunté : il est déjà en rayon.");
        }

        emprunt.Cloturer(DateTime.Today);
        livre.MarquerCommeDisponible();
        return emprunt;
    }

    /// <summary>
    /// Renvoie la liste des emprunts pas encore rendus.
    /// </summary>
    public List<Emprunt> ObtenirEmpruntsEnCours()
    {
        List<Emprunt> enCours = new List<Emprunt>();
        foreach (Emprunt emprunt in _emprunts)
        {
            if (emprunt.EstEnCours)
            {
                enCours.Add(emprunt);
            }
        }
        return enCours;
    }

    /// <summary>
    /// Cherche l'emprunt en cours d'un livre. Renvoie null s'il n'y en a pas.
    /// "private" : cette méthode ne sert qu'à l'intérieur de la classe.
    /// </summary>
    private Emprunt? TrouverEmpruntEnCours(Livre livre)
    {
        foreach (Emprunt emprunt in _emprunts)
        {
            if (emprunt.Livre == livre && emprunt.EstEnCours)
            {
                return emprunt;
            }
        }
        return null;
    }
}

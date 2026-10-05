namespace GestionBibliotheque;

/// <summary>
/// Un emprunt relie UN livre à UN membre, avec ses dates.
/// L'objet est créé au moment de l'emprunt et "clôturé" au moment du retour.
/// </summary>
public class Emprunt
{
    /// <summary>
    /// Durée du prêt en jours. "const" = valeur fixe, identique pour tous les emprunts.
    /// </summary>
    public const int DureeEnJours = 14;

    public Livre Livre { get; }
    public Membre Membre { get; }
    public DateTime DateEmprunt { get; }
    public DateTime DateRetourPrevue { get; }

    // "DateTime?" (avec un point d'interrogation) = type "nullable" :
    // la valeur peut être null. Ici, null veut dire « pas encore rendu ».
    public DateTime? DateRetour { get; private set; }

    /// <summary>
    /// Vrai tant que le livre n'a pas été rendu.
    /// </summary>
    public bool EstEnCours
    {
        get { return DateRetour == null; }
    }

    public Emprunt(Livre livre, Membre membre, DateTime dateEmprunt)
    {
        Livre = livre;
        Membre = membre;
        DateEmprunt = dateEmprunt;
        DateRetourPrevue = dateEmprunt.AddDays(DureeEnJours);
        DateRetour = null; // le livre vient de partir : pas encore de retour
    }

    /// <summary>
    /// Enregistre la date de retour : l'emprunt est terminé.
    /// </summary>
    public void Cloturer(DateTime dateRetour)
    {
        DateRetour = dateRetour;
    }
}

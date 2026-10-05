namespace GestionBibliotheque;

/// <summary>
/// Un membre inscrit à la bibliothèque (une personne qui peut emprunter des livres).
/// </summary>
public class Membre
{
    public int Id { get; }
    public string Prenom { get; }
    public string Nom { get; }

    /// <summary>
    /// Propriété calculée : elle n'est pas stockée, elle est recalculée
    /// à partir du prénom et du nom à chaque fois qu'on la lit.
    /// </summary>
    public string NomComplet
    {
        get { return $"{Prenom} {Nom}"; }
    }

    public Membre(int id, string prenom, string nom)
    {
        Id = id;
        Prenom = prenom;
        Nom = nom;
    }

    public override string ToString()
    {
        return NomComplet;
    }
}

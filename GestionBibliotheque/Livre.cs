namespace GestionBibliotheque;

/// <summary>
/// Un livre du catalogue de la bibliothèque.
/// </summary>
public class Livre
{
    // Propriétés en lecture seule ({ get; } sans "set") : elles sont fixées
    // une fois pour toutes dans le constructeur, plus personne ne peut les changer.
    public int Id { get; }
    public string Titre { get; }
    public string Auteur { get; }
    public int AnneePublication { get; }

    // Disponibilité du livre : true = en rayon, false = emprunté.
    // "private set" : tout le monde peut LIRE la valeur, mais seule la classe
    // Livre peut la MODIFIER, via les deux méthodes plus bas (encapsulation).
    public bool EstDisponible { get; private set; }

    /// <summary>
    /// Constructeur : il est appelé avec "new Livre(...)" et crée le livre.
    /// </summary>
    public Livre(int id, string titre, string auteur, int anneePublication)
    {
        Id = id;
        Titre = titre;
        Auteur = auteur;
        AnneePublication = anneePublication;
        EstDisponible = true; // un nouveau livre arrive toujours en rayon
    }

    /// <summary>
    /// Le livre part chez un membre.
    /// </summary>
    public void MarquerCommeEmprunte()
    {
        EstDisponible = false;
    }

    /// <summary>
    /// Le livre revient en rayon.
    /// </summary>
    public void MarquerCommeDisponible()
    {
        EstDisponible = true;
    }

    /// <summary>
    /// Texte qui représente le livre, par exemple : « Heidi » (Johanna Spyri, 1880).
    /// "override" remplace la version de base de ToString() héritée de "object".
    /// </summary>
    public override string ToString()
    {
        return $"« {Titre} » ({Auteur}, {AnneePublication})";
    }
}

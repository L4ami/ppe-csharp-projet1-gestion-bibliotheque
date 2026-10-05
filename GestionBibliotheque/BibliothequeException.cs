namespace GestionBibliotheque;

/// <summary>
/// Erreur « métier » : elle est déclenchée (avec "throw") quand une règle de la
/// bibliothèque n'est pas respectée : livre introuvable, livre déjà emprunté,
/// limite d'emprunts atteinte, etc.
/// Elle HÉRITE de la classe Exception de .NET (": Exception").
/// </summary>
public class BibliothequeException : Exception
{
    // ": base(message)" transmet le message au constructeur de la classe parente
    // (Exception), qui le range dans sa propriété Message.
    public BibliothequeException(string message) : base(message)
    {
    }
}

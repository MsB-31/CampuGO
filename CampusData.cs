namespace CampuGO;

/// <summary>
/// Toutes les données d'exemple liées au contexte camerounais (Université de Douala, ENSPD).
/// Pour adapter l'application, il suffit de modifier ce fichier.
/// </summary>
public static class CampusData
{
	public const string City = "Douala";
	public const string University = "Université de Douala";

	// Lieux affichés dans l'onglet « Explorer » : (catégorie, titre, détail, icône).
	public static readonly (string Category, string Title, string Detail, string Icon)[] Places =
	{
		("Restauration", "Restaurant universitaire de Bassa", "Cité SIC · Menu du jour", "☕"),
		("Services", "Bibliothèque de l’ENSPD", "Étude · Campus de Logbessou", "▤"),
		("Services", "Scolarité de l’ENSPD", "Dépôt et retrait de documents · Logbessou", "▦"),
		("Activités", "Club informatique de l’ENSPD", "Association · Rencontre chaque jeudi", "◉"),
		("Activités", "Tournoi de football inter-écoles", "Sport · Samedi matin", "⚽"),
		("Restauration", "Cafétéria du campus de Logbessou", "Repas et boissons", "✦")
	};

	// Programme du jeudi 22 octobre : (heure, titre, lieu, catégorie).
	public static readonly (string Time, string Title, string Place, string Category)[] Schedule =
	{
		("12:00", "Forum des associations étudiantes", "Esplanade de l’ENSPD · Logbessou", "ÉTUDIANT"),
		("14:30", "Atelier CV & entretien", "Espace orientation · Campus de Bassa", "ATELIER"),
		("16:00", "Conférence : l’IA au service de l’industrie", "Amphithéâtre · ENSPD", "CONFÉRENCE"),
		("18:30", "Soirée culturelle étudiante", "Cité SIC · Bassa", "CULTURE")
	};

	public static readonly string[] ExploreFilters = { "Tout", "Activités", "Services", "Restauration" };
}

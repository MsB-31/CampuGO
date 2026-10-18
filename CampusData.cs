namespace CampuGO;

/// <summary>Un établissement de l'Université de Douala, avec son campus par défaut et ses filières.</summary>
public sealed record Institution(string Label, string DefaultCampus, string[] Tracks);

/// <summary>
/// Toutes les données d'exemple liées au contexte camerounais (Université de Douala, ENSPD).
/// Pour adapter l'application, il suffit de modifier ce fichier.
/// </summary>
public static class CampusData
{
	public const string City = "Douala";
	public const string University = "Université de Douala";

	public const string CampusBassa = "Campus de Bassa (Cité SIC)";
	public const string CampusNdogbong = "Campus de Ndogbong";
	public const string CampusLogbessou = "Campus de Logbessou";
	public const string CampusAkwa = "Campus d’Akwa";
	public const string OtherCampus = "Autre campus";

	public static readonly string[] Campuses =
	{
		CampusBassa, CampusNdogbong, CampusLogbessou, CampusAkwa, OtherCampus
	};

	public static readonly string[] Levels =
	{
		"1re année", "2e année", "3e année", "4e année", "5e année", "Doctorat"
	};

	public static readonly Institution[] Institutions =
	{
		new("ENSPD (Polytechnique de Douala)", CampusLogbessou, new[]
		{
			"Génie Informatique et Télécommunications", "Génie Civil", "Génie Électrique",
			"Génie Industriel", "Génie Mécanique", "Autre filière"
		}),
		new("IUT de Douala", CampusNdogbong, new[]
		{
			"Génie Informatique", "Génie Électrique", "Gestion", "Autre filière"
		}),
		new("ENSET de Douala", CampusNdogbong, new[]
		{
			"Génie Informatique", "Génie Électrique", "Génie Mécanique", "Autre filière"
		}),
		new("ESSEC de Douala", CampusBassa, new[]
		{
			"Comptabilité", "Marketing", "Gestion", "Autre filière"
		}),
		new("FSEGA", CampusBassa, new[]
		{
			"Sciences économiques", "Gestion", "Comptabilité", "Autre filière"
		}),
		new("Faculté des Sciences (FS)", CampusLogbessou, new[]
		{
			"Mathématiques", "Informatique", "Physique", "Chimie", "Biologie", "Autre filière"
		}),
		new("FMSP (Médecine et Pharmacie)", CampusLogbessou, new[]
		{
			"Médecine", "Pharmacie", "Autre filière"
		}),
		new("FLSH (Lettres et Sciences Humaines)", CampusBassa, new[]
		{
			"Lettres", "Histoire", "Géographie", "Autre filière"
		}),
		new("Autre établissement", OtherCampus, new[] { "Autre filière" })
	};

	public static readonly string[] InstitutionLabels = Institutions.Select(i => i.Label).ToArray();

	public static Institution? FindInstitution(string label) =>
		Institutions.FirstOrDefault(i => i.Label == label);

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

	// Alertes : (catégorie, titre, message, horodatage, urgent).
	public static readonly (string Category, string Title, string Message, string Time, bool Urgent)[] Alerts =
	{
		("INFO CAMPUS", "Horaires de la bibliothèque", "La bibliothèque de l’ENSPD ferme à 18:00 aujourd’hui.", "Il y a 12 min", false),
		("VIE ÉTUDIANTE", "Forum des associations", "Rendez-vous à l’esplanade de l’ENSPD, à Logbessou, jeudi à partir de midi.", "Aujourd’hui · 09:42", false),
		("DÉPLACEMENTS", "Trajets vers Logbessou", "Prévois une marge supplémentaire aux heures de pointe : la circulation est dense autour de Douala.", "Hier · 18:10", true)
	};

	public static readonly string[] ExploreFilters = { "Tout", "Activités", "Services", "Restauration" };
}

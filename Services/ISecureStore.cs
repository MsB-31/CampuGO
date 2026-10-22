namespace CampuGO.Services;

/// <summary>Stockage de secrets (jetons). L'implémentation de production s'appuie sur le stockage sécurisé de l'appareil.</summary>
public interface ISecureStore
{
	Task<string?> GetAsync(string key);

	Task SetAsync(string key, string value);

	void Remove(string key);
}

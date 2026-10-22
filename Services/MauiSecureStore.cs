namespace CampuGO.Services;

/// <summary>Stockage sécurisé de .NET MAUI (Keystore sous Android). En cas d'indisponibilité, la session reste en mémoire.</summary>
public sealed class MauiSecureStore : ISecureStore
{
	public async Task<string?> GetAsync(string key)
	{
		try { return await SecureStorage.Default.GetAsync(key); }
		catch (Exception) { return null; }
	}

	public async Task SetAsync(string key, string value)
	{
		try { await SecureStorage.Default.SetAsync(key, value); }
		catch (Exception) { /* stockage indisponible : le jeton reste uniquement en mémoire */ }
	}

	public void Remove(string key)
	{
		try { SecureStorage.Default.Remove(key); }
		catch (Exception) { /* rien à supprimer */ }
	}
}

using ScavLib.i18n;

namespace TrapLib.Utilities;

/// <summary>
/// Centralised display-name / description resolver for trap <see cref="TrapConfig"/>.
///
/// <para>Lookup order:</para>
/// <list type="number">
///   <item><description>Game <c>Locale</c> dictionaries (populated by ScavLib's
///   <see cref="LocaleManager"/> from JSON or <c>RegisterItem</c>/<c>RegisterString</c>).</description></item>
///   <item><description>Legacy <c>FullNameCn</c>/<c>DescriptionCn</c> branch from the
///   trap's own config — kept as a fallback so existing trap mods that inline
///   their CN strings in <c>TrapConfig</c> keep working without code changes.</description></item>
///   <item><description>Final fallback to English / id so we never display empty text.</description></item>
/// </list>
///
/// <para>The Locale check rejects results that equal the lookup key — vanilla
/// <c>Locale.Get*</c> conventionally returns the key itself when a translation is
/// missing, which we must not treat as a hit. Likewise empty strings are rejected.</para>
/// </summary>
internal static class TrapLocale
{
    /// <summary>
    /// Resolve the display name + description for a trap config.
    /// Safe to call with a partially-populated config; never throws.
    /// </summary>
    internal static void Resolve(TrapConfig config, out string fullName, out string description)
    {
        fullName = null;
        description = null;
        if (config == null) return;

        string id = config.Id;
        bool cn = LocaleHelper.IsChinese();

        // ── Name ─────────────────────────────────────────────────────
        // Try Locale first so downstream mods can register translations
        // through ScavLib.i18n.LocaleManager without touching TrapConfig.
        string fromLocale = SafeGet(Locale.GetBuilding, id);
        if (!string.IsNullOrEmpty(fromLocale) && fromLocale != id)
        {
            fullName = fromLocale;
        }
        else
        {
            // Legacy path — backwards compat with TrapConfig.FullNameCn.
            fullName = (cn && !string.IsNullOrEmpty(config.FullNameCn))
                ? config.FullNameCn
                : (config.FullName ?? id);
        }

        // ── Description ──────────────────────────────────────────────
        // LocaleManager.RegisterItem stores descs under key "<id>dsc" in
        // Locale.currentLang.other; mirror that convention.
        string descKey = id + "dsc";
        string descFromLocale = SafeGet(Locale.GetOther, descKey);
        if (!string.IsNullOrEmpty(descFromLocale) && descFromLocale != descKey)
        {
            description = descFromLocale;
        }
        else
        {
            description = (cn && !string.IsNullOrEmpty(config.DescriptionCn))
                ? config.DescriptionCn
                : (config.Description ?? string.Empty);
        }
    }

    /// <summary>
    /// Defensive wrapper: Locale may be uninitialised on early callers
    /// (e.g. during boot before <c>Locale.LoadLanguage</c> has run).
    /// </summary>
    private static string SafeGet(System.Func<string, string> getter, string key)
    {
        if (getter == null || string.IsNullOrEmpty(key)) return null;
        try { return getter(key); }
        catch { return null; }
    }
}

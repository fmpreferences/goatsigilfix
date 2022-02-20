using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using DiskCardGame;
using HarmonyLib;

namespace GoatSigilFix;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public class GoatSigilFix : BaseUnityPlugin
{
    private const string PluginName = "GoatSigilFix";
    private const string PluginGuid = "fmpreferences.goatfix";
    private const string PluginVersion = "1.1.0";

    internal static ManualLogSource PluginLogger;
    internal static bool AllowTransfers;

    private void Awake()
    {
        PluginLogger = Logger;

        var h = new Harmony(PluginGuid);

        AllowTransfers = Config.Bind(PluginName, "allowTransferredSigils", true,
                new ConfigDescription("Will give bone lord boon even if the sigil is transferred via sacrifice"))
            .Value;

        PluginLogger.LogInfo("Config Options Loaded!");
        try
        {
            h.PatchAll();
            PluginLogger.LogInfo($"{PluginName} loaded!");
        }
        catch
        {
            PluginLogger.LogInfo("something went wrong!");
        }
    }
}

[HarmonyPatch(typeof(CardRemoveSequencer), "GetValidCards")]
public class CanIAddGoat
{
    /*
     * what this thing does is add the goat trait to anything with the triple blood sigil and makes it
     * have the goat trait which makes the sacrifice give the bone lord boon.
     */
    [HarmonyPrefix]
    public static void PatchValidCards()
    {
        var deckInfo = RunState.Run.playerDeck;
        List<CardInfo> defaultGoats =
            deckInfo.Cards.FindAll(card => card.DefaultAbilities.Contains(Ability.TripleBlood));

        foreach (var card in defaultGoats)
            if (!card.traits.Contains(Trait.Goat))
                card.traits.Add(Trait.Goat);

        if (!GoatSigilFix.AllowTransfers) return;

        List<CardInfo> transferredGoats =
            deckInfo.Cards.FindAll(card => card.ModAbilities.Contains(Ability.TripleBlood));
        foreach (var card in transferredGoats)
            if (!card.traits.Contains(Trait.Goat))
                card.traits.Add(Trait.Goat);
    }
}
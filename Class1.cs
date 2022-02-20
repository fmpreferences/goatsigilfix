using BepInEx;
using BepInEx.Logging;
using DiskCardGame;
using HarmonyLib;

namespace GoatSigilAlwaysBoneLord;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public class GoatSigilFix : BaseUnityPlugin
{
    private const string PluginName = "Goat Sigil Fix";
    private const string PluginGuid = "fmpreferences.goatfix";
    private const string PluginVersion = "0.0.1";

    internal static ManualLogSource PluginLogger;

    private void Awake()
    {
        PluginLogger = Logger;
        PluginLogger.LogInfo("Goat Sigil Fix loaded!");

        new Harmony(PluginGuid).PatchAll();
    }
}

[HarmonyPatch(typeof(CardRemoveSequencer), "GetValidCards")]
public class CanIAddGoat
{
    /*
     * what this thing does is add the goat trait to anything with the triple blood sigil and makes it
     * have the goat trait which makes the sacrifice give the bone lord boon. I was way overcomplicating
     * the code
     */
    [HarmonyPrefix]
    public static void PatchValidCards()
    {
        var deckInfo = RunState.Run.playerDeck;
        List<CardInfo> cards = deckInfo.Cards.FindAll(card => card.HasAbility(Ability.TripleBlood));
        foreach (var card in cards)
        {
            if (!card.traits.Contains(Trait.Goat))
            {
                card.traits.Add(Trait.Goat);
            }
        }
    }
}
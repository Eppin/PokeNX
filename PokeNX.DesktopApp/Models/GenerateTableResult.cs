namespace PokeNX.DesktopApp.Models;

using Avalonia.Media;
using Core.Models;

public class GenerateTableResult : GenerateResult
{
    public GenerateTableResult(GenerateResult generateResult)
    {
        Advances = generateResult.Advances;
        Seed = generateResult.Seed;
        PID = generateResult.PID;
        EC = generateResult.EC;
        EncounterSlot = generateResult.EncounterSlot;
        HeldItem = generateResult.HeldItem;
        Shiny = generateResult.Shiny;
        Nature = generateResult.Nature;
        Ability = generateResult.Ability;
        Gender = generateResult.Gender;
        HP = generateResult.HP;
        Atk = generateResult.Atk;
        Def = generateResult.Def;
        SpA = generateResult.SpA;
        SpD = generateResult.SpD;
        Speed = generateResult.Speed;
    }

    public IBrush RowColor => Shiny > 0 ? new SolidColorBrush(Color.FromRgb(144, 0, 0)) : null;
}

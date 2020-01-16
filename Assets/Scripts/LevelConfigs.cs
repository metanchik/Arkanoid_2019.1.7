using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelConfigs
{
    private static List<string> levels = new List<string>() {
        "-/." +
        "-/." +
        "-/." +
        "r/."+
        "g/."+
        "b/."+
        "y/."+
        "r/."+
        "c/."+
        "m/."+
        "g/."+
        "r/."+
        "-/." +
        "-/." +
        "-/.",
        //-----------
        "-/." +
        "-/." +
        "-/." +
        "r2/."+
        "r/."+
        "r/."+
        "r/."+
        "r/."+
        "r/."+
        "r/."+
        "r/."+
        "r/."+
        "-/." +
        "-/." +
        "-/.",
        //-----------
        "-/-/y/y/-/-/." +
        "-/ra/-/-/r/-/." +
        "-/r/-/-/rb/-/." +
        "g2/-/b/b/-/g2/." +
        "g2/-/b/b/-/g2/."+
        "g2/-/-/-/-/g2/."+
        "g2s/c3/c3/c3/c3/g2/."+
        "g2/c3/c3/c3/c3/g2/."+
        "g2/-/-/-/-/g2/."+
        "g2/-/g/gb/-/g2/."+
        "m/g/-/-/g/m/."+
        "y/m/-/-/ms/y/."+
        "y/yw/m/m/y/y/."+
        "-/y/y/y/y/-." +
        "-/-/y/y/-/-/.",
    };

    public static string GetLevelConfig(int index) {
        return levels[index];
    }

    public static int GetLevelsCount() {
        return levels.Count;
    }
    
    private static Dictionary<string, Bonus> bonuses = new Dictionary<string, Bonus>() {
        {"a", new Bonus("a", () =>  GameObject.FindObjectsOfType<BallController>().ToList().ForEach(b => b.EditSpeed(2f)))},
        {"s", new Bonus("s", () => GameObject.FindObjectsOfType<BallController>().ToList().ForEach(b => b.EditSpeed(0.5f)))},
        {"b", new Bonus("b", () => GameObject.FindObjectOfType<LevelController>().CloneBalls())},
        {"w", new Bonus("w", () => GameObject.FindObjectOfType<PlatformController>().EditLength(1.5f))}
    };

    public static Bonus GetBonus(string bonusKey) {
        Bonus bonus = null;
        bonuses.TryGetValue(bonusKey, out bonus);
        return bonus;
    }
}

using System;
using UnityEngine;

public class Bonus {
    public Sprite icon;
    public Action action;

    public Bonus(string iconName, Action action) {
        this.action = action;
        icon = Resources.Load<Sprite>("Sprites/" + iconName);
    }
}

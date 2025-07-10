public enum UpgradeCategory
{
    Speed,
    Firepower,
    Defense
}

[System.Serializable]
public class Upgrade
{
    public UpgradeCategory category;
    public string label;
    public System.Action applyEffect;

    public Upgrade(string label, UpgradeCategory category, System.Action effect)
    {
        this.label = label;
        this.category = category;
        this.applyEffect = effect;
    }
}

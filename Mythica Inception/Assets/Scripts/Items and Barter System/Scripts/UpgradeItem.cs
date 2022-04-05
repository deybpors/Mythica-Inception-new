using _Core.Others;
using Items_and_Barter_System.Scripts;

namespace Assets.Scripts.Items_and_Barter_System.Scripts
{
    [System.Serializable]
    public enum UpgradeType
    {
        BothPhysical,
        PhysicalAttack,
        PhysicalDefense,
        BothSpecial,
        SpecialAttack,
        SpecialDefense,
    }

    [System.Serializable]
    public struct UpgradeValues
    {
        public int attackToAdd;
        public int defenseToAdd;

        public UpgradeValues(int attack, int defense)
        {
            attackToAdd = attack;
            defenseToAdd = defense;
        }
    }

    public class UpgradeItem : ItemObject
    {
        public UpgradeType upgradeType;

        public void Awake()
        {
            itemType = ItemType.Upgrades;
            usable = false;
            stackable = false;
        }

        public override bool TryUse(IEntity entity)
        {
            return usable;
        }

        public virtual int GetValueToUpgrade() { return 0; }

        public virtual UpgradeValues GetUpgradeValues()
        {
            return new UpgradeValues(0, 0);
        }

        public UpgradeType GetUpgradeType()
        {
            return upgradeType;
        }
    }
}

using System;
using System.Linq;

namespace HyperElk.Core
{
    public abstract class Aura
    {
        internal static void Initialize()
        {
            Buff.Initialize();
            Debuff.Initialize();
        }

        private protected static (int low, int normal, int high) Priorities { get; } = (low: 0, normal: 1, high: 2);

        public DispelType? DType { get; private set; }

        public int Id { get; private set; }

        public string Name { get; private set; } = "";

        private int priority;

        public int Priority { get { return priority; } private set { priority = Math.Max(Math.Min(value, 2), 0); } }

        private protected double RefreshTime { get; private set; }

        private protected Aura(DispelType? dispelType, int id, string name, int priority, int refreshTime)
        {
            DType = dispelType;
            Id = id;
            Name = name;
            Priority = priority;
            RefreshTime = refreshTime;
        }

        public abstract double Elapsed(in Unit unit, bool playerIsSrc = false);

        public abstract bool NeedRefresh();

        public abstract bool NeedRefresh(in Unit unit);

        public abstract double Remaining(in Unit unit, bool playerIsSrc);

        public abstract int Stacks(in Unit unit);

        public abstract int Stacks();
    }

    public sealed class Buff : Aura
    {
        public static Buff[] Buffs { get; private set; } = Array.Empty<Buff>();

        internal static new void Initialize()
        {
            _ = new Buff("Agitation", 390938, true);
            _ = new Buff("Call of the Flock", 377389, true);
            _ = new Buff("Enrage Affix", 228318, true);
            _ = new Buff("Enrage Angerhoof Bull", 190225, true);
            _ = new Buff("Enraged Regeneration", 397410, true);
            _ = new Buff("Ferocity", 211477, true);
            _ = new Buff("Fit of Rage", 396018, true);
            _ = new Buff("Raging Kin", 383067, true);
        }

        public bool Enrage { get; private set; }

        public bool Immunity { get; private set; }

        private Buff(DispelType? dispelType, int id, string name, int refreshTime, bool enrage, bool immunity) : base(dispelType, id, name, 0, refreshTime)
        {
            Enrage = enrage;
            Immunity = immunity;
            Buffs = Buffs.Append(this).ToArray();
            CombatRoutine.AddBuff(name, id);
        }

        public Buff(string name, int id, int refreshTime = 0) : this(null, id, name, refreshTime, false, false) { }

        public Buff(string name, int id, bool enrage, bool immunity = false) : this(null, id, name, 0, enrage, immunity) { }

        public Buff(int id, in Spell spell, int refreshTime = 0) : this(null, id, spell.Name, refreshTime, false, false) { }

        public Buff(in Spell spell, int refreshTime = 0) : this(null, spell.Id, spell.Name, refreshTime, false, false) { }

        public override double Elapsed(in Unit unit, bool playerIsSrc = false) { return API.UnitBuffTimeElapsed(Name, unit.Id, playerIsSrc) / 100; }

        public override bool NeedRefresh() { return Remaining(Unit.target, true) < RefreshTime; }

        public override bool NeedRefresh(in Unit unit) { return Remaining(unit, true) < RefreshTime; }

        public override double Remaining(in Unit unit, bool playerIsSrc = false) { return API.UnitBuffTimeRemaining(Name, unit.Id, playerIsSrc) / 100; }

        public override int Stacks(in Unit unit) { return API.UnitBuffStacks(Name, unit.Id); }

        public override int Stacks() { return API.UnitBuffStacks(Name, Unit.player.Id); }
    }

    public sealed class Debuff : Aura
    {
        public static Debuff[] Debuffs { get; private set; } = Array.Empty<Debuff>();

        #region debuffs
#pragma warning disable CS8618
        public static Debuff Burst { get; private set; }
        public static Debuff FrostBombDebuff { get; private set; }
        public static Debuff QuakeDebuff { get; private set; }
#pragma warning restore CS8618
        #endregion

        internal static new void Initialize()
        {
            _ = new Debuff("Absolute Zero", 396722, false, true, dispelType: DispelType.Magic);
            _ = new Debuff("Bewitch", 211370, false, true, dispelType: DispelType.Magic, priority: Priorities.high);
            _ = new Debuff("Bloodcurdling Shout", 373395, false, true, dispelType: DispelType.Magic, priority: Priorities.high);
            _ = new Debuff("Burning Touch", 373869, false, true, dispelType: DispelType.Magic);
            Burst = new Debuff("Burst", 240443, false, true, minStacksDispel: 5, dispelType: DispelType.Magic, priority: Priorities.low);
            _ = new Debuff("Conductive Strike", 376827, false, true, dispelType: DispelType.Magic, priority: Priorities.high);
            _ = new Debuff("Death Venom", 156717, false, true, dispelType: DispelType.Poison);
            _ = new Debuff("Dragon Strike", 384978, false, true, dispelType: DispelType.Magic);
            _ = new Debuff("Energy Bomb", 374350, false, true, dispelType: DispelType.Magic);
            _ = new Debuff("Forbidden Knowledge", 371352, false, true, dispelType: DispelType.Magic, priority: Priorities.high);
            _ = new Debuff("Frightful Roar", 386063, false, true, dispelType: DispelType.Magic, priority: Priorities.high);
            FrostBombDebuff = new Debuff("Frost Bomb", 386881);
            _ = new Debuff("Icy Bindings", 377488, false, true, dispelType: DispelType.Magic);
            _ = new Debuff("Impending Doom", 397907, false, true, dispelType: DispelType.Magic);
            _ = new Debuff("Mana Fang", 209516, false, true, dispelType: DispelType.Magic);
            _ = new Debuff("Monotonous Lecture", 388392, false, true, dispelType: DispelType.Magic, priority: Priorities.high);
            _ = new Debuff("Mystic Vapors", 387564, false, true, minStacksDispel: 2, dispelType: DispelType.Magic);
            _ = new Debuff("Necrotic Burst", 156718, false, true, dispelType: DispelType.Disease);
            _ = new Debuff("Oppressive Miasma", 388777, false, true, minStacksDispel: 5, dispelType: DispelType.Magic, priority: Priorities.low);
            _ = new Debuff("Oversurge", 391977, false, true, dispelType: DispelType.Magic);
            _ = new Debuff("Plague Spit", 153524, false, true, dispelType: DispelType.Disease);
            _ = new Debuff("Primal Chill", 372682, false, true, minStacksDispel: 3, dispelType: DispelType.Magic);
            QuakeDebuff = new Debuff("Quake", 240447);
            _ = new Debuff("Resonant Slash", 207261, false, true, dispelType: DispelType.Magic);
            _ = new Debuff("Rolling Thunder", 392641, false, true, dispelType: DispelType.Magic);
            _ = new Debuff("Rotting Wind", 387629, false, true, dispelType: DispelType.Disease, priority: Priorities.high);
            _ = new Debuff("Seal Magic", 309404, false, true, dispelType: DispelType.Magic);
            _ = new Debuff("Shadow Word: Frailty", 152819, false, true, dispelType: DispelType.Magic);
            _ = new Debuff("Shock Blast", 392924, false, true, dispelType: DispelType.Magic);
            _ = new Debuff("Sleepy Soliloquy", 395872, false, true, dispelType: DispelType.Magic, priority: Priorities.high);
            _ = new Debuff("Storm Shock", 381530, false, true, dispelType: DispelType.Magic);
            _ = new Debuff("Stormslam", 381515, false, true, dispelType: DispelType.Magic);
            _ = new Debuff("Suppress", 209413, false, true, dispelType: DispelType.Magic, priority: Priorities.high);
            _ = new Debuff("Tainted Ripple", 397878, false, true, dispelType: DispelType.Magic);
            _ = new Debuff("Throw Torch", 114803, false, true, dispelType: DispelType.Magic);
            _ = new Debuff("Thunder Clap", 386028, false, true, dispelType: DispelType.Magic, priority: Priorities.low);
            _ = new Debuff("Thunderstrike", 215429, false, true, dispelType: DispelType.Magic);
            _ = new Debuff("Touch of Nothingness", 106113, false, true, dispelType: DispelType.Magic, priority: Priorities.high);
            _ = new Debuff("Touch of Ruin", 397911, false, true, dispelType: DispelType.Curse);
            _ = new Debuff("Unlucky Strike", 385313, false, true, dispelType: DispelType.Curse, priority: Priorities.high);
            _ = new Debuff("Waking Bane", 386549, false, true, dispelType: DispelType.Magic, priority: Priorities.high);
            _ = new Debuff("Withering Soul", 208165, false, true, dispelType: DispelType.Magic, priority: Priorities.high);
            _ = new Debuff("Serpent Strike", 106823, false, true, dispelType: DispelType.Magic, priority: Priorities.high);
            _ = new Debuff("Enveloping Winds", 224333, false, true, dispelType: DispelType.Magic, priority: Priorities.high);

        }

        public bool Dispel { get; private set; }

        public double DispelRemaining { get; private set; }

        public bool Ignore { get; private set; }

        public int MinStacksDispel { get; private set; }

        private Debuff(DispelType? dispelType, int id, string name, int priority, int refreshTime, bool dispel, int dispelRemaining, bool ignore, int minStacksDispel) : base(dispelType, id, name, priority, refreshTime)
        {
            if (dispel && dispelType == null)
            {
                throw new ArgumentException("When dispel is true a dispelType has to be given!");
            }

            Dispel = dispel;
            DispelRemaining = dispelRemaining;
            Ignore = ignore;
            MinStacksDispel = minStacksDispel;
            Debuffs = Debuffs.Append(this).ToArray();
            CombatRoutine.AddDebuff(name, id);
        }

        public Debuff(string name, int id, int refreshTime = 0) : this(null, id, name, 0, refreshTime, false, 0, false, 0) { }

        public Debuff(string name, int id, bool ignore, bool dispel = false, int dispelRemaining = 0, int minStacksDispel = 0, DispelType? dispelType = null, int priority = 1)
            : this(dispelType, id, name, priority, 0, dispel, dispelRemaining, ignore, minStacksDispel) { }

        public Debuff(int id, in Spell spell, int refreshTime = 0) : this(null, id, spell.Name, 0, refreshTime, false, 0, false, 0) { }

        public Debuff(in Spell spell, int refreshTime = 0) : this(null, spell.Id, spell.Name, 0, refreshTime, false, 0, false, 0) { }

        public override double Elapsed(in Unit unit, bool playerIsSrc = true) { return API.UnitDebuffElapsedTime(Name, unit.Id, playerIsSrc) / 100; }

        public override bool NeedRefresh() { return Remaining(Unit.target) < RefreshTime; }

        public override bool NeedRefresh(in Unit unit) { return Remaining(unit) < RefreshTime; }

        public override double Remaining(in Unit unit, bool playerIsSrc = true) { return API.UnitDebuffRemainingTime(Name, unit.Id, playerIsSrc) / 100; }

        public override int Stacks() { return API.UnitBuffStacks(Name, Unit.target.Id); }

        public override int Stacks(in Unit unit) { return API.UnitDebuffStacks(Name, unit.Id); }
    }
}

namespace HyperElk.Core
{
    public class Base
    {
        internal static bool HealingRotation { get; private set; }

        public static void Initialize(int priorityTargetRange, bool healingRotation = true, bool pvpRotation = false)
        {
            CombatRoutine.isHealingRotationFocus = healingRotation;

            HealingRotation = healingRotation;

            CombatRoutine.isAutoBindReady = true;

            CombatRoutine.SetPriorityTargetsRange(priorityTargetRange);

            CombatRoutine.AddSpell("Auto Attack", 6603);

            if (!pvpRotation)
            {
                SettingBool.Initialize();
                SettingStrings.Initialize();
                Aura.Initialize();
                MechanicSpell.Initialize();
                Mechanic.Initialize();
            }

            Cast.Initialize();
        }
    }

}

namespace HyperElk.Core
{
    public abstract class Cast
    {
        internal static void Initialize()
        {
            Item.Initialize();
        }

        public const double DELAY = 0.4;

        public static double MaxGCD { get { return Math.Max(1.5 / (1 + Unit.PlayerHaste), 0.75); } }

        public static double GCD { get { return Math.Max(API.SpellGCDDuration / 100 - DELAY, 0); } }

        public static bool RootBreak
        {
            get
            {
                foreach (Mechanic mechanic in Mechanic.Mechanics)
                {
                    if (!mechanic.RootBreak || mechanic.MDebuff == null) continue;

                    if (Unit.player.HasAura(mechanic.MDebuff, false)) return true;
                }

                return false;
            }
        }

        public static bool StopCast
        {
            get
            {
                if (Unit.player.CurrentSpellRemaining == 0) return false;

                foreach (Mechanic mechanic in Mechanic.Mechanics)
                {
                    if (!mechanic.IsInterrupt) continue;

                    if (mechanic.MSpell != null)
                    {
                        foreach (Unit unit in Unit.EnemyUnits)
                        {
                            if (unit == mechanic && unit <= Unit.player) return true;
                        }
                    }

                    if (mechanic.MDebuff == null) continue;

                    if (Unit.player.HasAura(mechanic.MDebuff, false) && mechanic <= Unit.player) return true;
                }

                return false;
            }
        }

        public static bool UseDeff
        {
            get
            {
                foreach (Mechanic mechanic in Mechanic.Mechanics)
                {
                    if (!mechanic.AutoDeff) continue;

                    if (mechanic.MSpell != null)
                    {
                        foreach (Unit unit in Unit.EnemyUnits)
                        {
                            if (unit.CurrentSpell == mechanic.MSpell.Id && unit.CurrentSpellRemaining <= MaxGCD + DELAY && (!mechanic.MSpell.Targeted
                                || unit.TargetingPlayer) && (!mechanic.MSpell.OnTank || Unit.Tank.Range <= mechanic.Range)) return true;
                        }
                    }

                    if (mechanic.MDebuff == null) continue;

                    if (mechanic.OnPlayer)
                    {
                        if (Unit.player.HasAura(mechanic.MDebuff, false) && mechanic.MDebuff.Remaining(Unit.player, false) <= MaxGCD + DELAY) return true;
                    }
                    else
                    {
                        foreach (Unit unit in Unit.FriendlyUnits)
                        {
                            if (unit.HasAura(mechanic.MDebuff, false) && mechanic.MDebuff.Remaining(unit, false) <= MaxGCD + DELAY && unit.Range <= mechanic.Range)
                                return true;
                        }
                    }
                }

                return false;
            }
        }

        public string Name { get; private set; } = "";

        public int Range { get; private set; }

        private protected Cast(string name, int range)
        {
            Name = name;
            Range = range;
        }

        private protected bool InRange(in Unit unit) { return unit.Range <= Range; }

        public abstract bool Usable(in Unit? unit = null);

        public bool SpellIsIgnored { get { return API.SpellIsIgnored(Name); } }

        public void Use() { API.CastSpell(Name); }

        public static string operator +(in Cast a, string b) => a.Name + b;

        public static string operator +(string a, in Cast b) => a + b.Name;

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }

    public abstract class NativeCast : Cast
    {
        public int Id { get; private set; }

        private protected NativeCast(string name, int range, int id) : base(name, range)
        {
            Id = id;
        }
    }

    public sealed class Macro : Cast
    {
        #region macro functions
        public static readonly string castFunction = "/cast ";
        public static readonly string useFunction = "/use ";
        #endregion

        #region macro modifiers
        public static readonly string cursorModifier = "[@cursor] ";
        public static readonly string focusModifier = "[@focus] ";
        public static readonly string mouseoverModifier = "[@mouseover] ";
        public static readonly string noChannelModifier = "[nochanneling] ";
        public static readonly string playerModifier = "[@player] ";
        #endregion

        #region macro types
        public static readonly string cursorMacro = " Cursor";
        public static readonly string focusMacro = " Focus";
        public static readonly string mouseoverMacro = " Mouseover";
        public static readonly string playerMacro = " Player";
        #endregion

        #region macros
        public static readonly Macro stopCastMacro = new Macro("Stop Cast Macro", "/stopcasting", 100);
        #endregion

        public string Text { get; private set; }

        public Macro(string name, string text, int range) : base(name, range)
        {
            Text = text;
            CombatRoutine.AddMacro(name, MacroText: text);
        }

        public Macro(string name, int range) : this(name, name, range) { }

        public override bool Usable(in Unit? unit = null) { return InRange(unit ?? Unit.target) && !SpellIsIgnored; }
    }

    public sealed class Spell : NativeCast
    {
        public bool CanCast { get { return API.CanCast(Name, false); } }

        private double ChargeCooldown { get { return Math.Max(API.SpellChargeCD(Name) / 100 - DELAY, 0); } }

        public int Charges
        {
            get
            {
                int maxCharges = API.SpellMaxCharges(Name);
                int charges = API.SpellCharges(Name);

                if (charges < maxCharges) { if (ChargeCooldown == 0) charges += 1; }

                return charges;
            }
        }

        public double Cooldown { get { return Math.Max(API.SpellCDDuration(Name) / 100 - DELAY, 0); } }

        private bool NoCooldown { get { return Cooldown == 0; } }

        public Spell(string name, int id, int range) : base(name, range, id)
        {
            CombatRoutine.AddSpell(name, id);
            if (Base.HealingRotation)
                CombatRoutine.AddMacro(name + Macro.focusMacro, MacroText: Macro.castFunction + Macro.focusModifier + "#" + id + "#");
        }

        private void CastAtFocus() { API.CastSpell(Name + " Focus"); }

        public override bool Usable(in Unit? unit = null) { return CanCast && NoCooldown && InRange(unit ?? Unit.target) && !SpellIsIgnored; }

        public bool Usable(in Unit unit, bool castableCheck, bool rangeCheck = true) { return (CanCast || !castableCheck) && (NoCooldown || Charges > 0) && (InRange(unit) || !rangeCheck) && !SpellIsIgnored; }

        public bool Usable(bool castableCheck, bool rangeCheck = true) { return (CanCast || !castableCheck) && (NoCooldown || Charges > 0) && (InRange(Unit.target) || !rangeCheck) && !SpellIsIgnored; }

        public void Use(in Unit unit) { unit.Focus(); CastAtFocus(); }

        public void UseOnPlayer() { API.CastSpell(Name + " Player"); }

        public static bool operator ==(int a, in Spell b) => a == b.Id;

        public static bool operator !=(int a, in Spell b) => a != b.Id;

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }

    public abstract class Item : NativeCast
    {
        internal static new void Initialize()
        {
            Consumable.Initialize();
            Potion.Initialize();
            Trinket.Initialize();
        }

        public virtual bool CanCast { get { return API.PlayerItemCanUse(Name); } }

        private protected Item(string name, int range, int id, bool isTrinket) : base(name, range, id)
        {
            if (!isTrinket)
            {
                CombatRoutine.AddItem(name, id);
            }
            else
            {
                CombatRoutine.AddMacro(name, MacroText: Macro.useFunction + (id + 12));
            }
        }

        private void CastAtFocus() { API.CastSpell(Name + " Focus"); }

        public override bool Usable(in Unit? unit = null) { return CanCast && InRange(unit ?? Unit.target); }

        public void Use(in Unit unit) { unit.Focus(); CastAtFocus(); }
    }

    public sealed class Consumable : Item
    {
        #region consumables
#pragma warning disable CS8618
        public static Consumable HealthstoneConsumable { get; private set; }
#pragma warning restore CS8618
        #endregion

        internal static new void Initialize()
        {
            HealthstoneConsumable = new Consumable("Healthstone Healing", 5512, 100);
        }

        public Consumable(string name, int id, int range) : base(name, range, id, false) { }
    }

    public sealed class Potion : Item
    {
        #region potions
#pragma warning disable CS8618
        public static Potion RefreshingHealingPotion { get; private set; }
#pragma warning restore CS8618
        #endregion

        internal static new void Initialize()
        {
            RefreshingHealingPotion = new Potion("Refreshing Healing Potion", 191380, 100);
        }

        public Potion(string name, int id, int range) : base(name, range, id, false) { }
    }

    public sealed class Trinket : Item
    {
        #region trinkets
#pragma warning disable CS8618
        public static Trinket Trinket1 { get; private set; }
        public static Trinket Trinket2 { get; private set; }
#pragma warning restore CS8618
        #endregion

        internal static new void Initialize()
        {
            Trinket1 = new Trinket(1);
            Trinket2 = new Trinket(2);
        }

        public static readonly string[] targets = { "No Target", "Player", "Lowest Ally", "Target" };

        public override bool CanCast { get { return API.PlayerTrinketRemainingCD(Id) == 0; } }

        public Trinket(int id) : base("trinket" + id, 40, id, true) { }

        public override bool Usable(in Unit? unit = null) { return API.PlayerTrinketIsUsable(Id) && CanCast && InRange(unit ?? Unit.target); }

        public static int operator +(in Trinket a, int b) => a.Id + b;
    }

    public sealed class Weapon : Item
    {
        public static Weapon[] Weapons { get; private set; } = Array.Empty<Weapon>();

        public Weapon(string name, int id, int range) : base(name, range, id, false) { }
    }
}

namespace HyperElk.Core
{
    public abstract class Class
    {
        public string ClassName { get; private set; }

        protected Class(string className)
        {
            ClassName = className;
        }
    }

    public sealed class Specc : Class
    {
        public DispelType[] DispelTypes { get; private set; }

        public string SpeccName { get; private set; }

        public Specc(string baseClass, DispelType[] dispelTypes, string speccName) : base(baseClass)
        {
            DispelTypes = dispelTypes;
            SpeccName = speccName;
        }
    }
}

namespace HyperElk.Core
{
    public class DispelType
    {
        public static DispelType Curse { get; private set; } = new DispelType("curse");

        public static DispelType Disease { get; private set; } = new DispelType("disease");

        public static DispelType Magic { get; private set; } = new DispelType("magic");

        public static DispelType Poison { get; private set; } = new DispelType("poison");

        public string Name { get; private set; }

        private DispelType(string name)
        {
            Name = name;
        }

        public DispelType()
        {
            throw new NotSupportedException("DispelTypes can only be globally created!");
        }
    }
}

namespace HyperElk.Core
{
    public sealed class Mechanic
    {
        public static Mechanic[] Mechanics { get; private set; } = Array.Empty<Mechanic>();

        internal static void Initialize()
        {
            _ = new Mechanic(Debuff.Burst, minStacks: 5, heavyDamage: true);
            _ = new Mechanic(MechanicSpell.DeafeningScreechMS, autoDeff: true, isInterrupt: true);
            _ = new Mechanic(MechanicSpell.DisruptiveShoutMS, isInterrupt: true);
            _ = new Mechanic(MechanicSpell.EyeOfTheStorm, heavyDamage: true, multiplier: 0.7);
            _ = new Mechanic(Debuff.FrostBombDebuff, autoDeff: true, heavyDamage: true);
            _ = new Mechanic(MechanicSpell.InterruptingCloudburstMS, isInterrupt: true);
            _ = new Mechanic(Debuff.QuakeDebuff, isInterrupt: true);
            _ = new Mechanic(MechanicSpell.ScreamOfPainMS, isInterrupt: true);
            _ = new Mechanic(MechanicSpell.SlicingMaelstrom, heavyDamage: true, multiplier: 0.7);
            _ = new Mechanic(MechanicSpell.UnnervingHowlMS, isInterrupt: true);
            _ = new Mechanic(MechanicSpell.UnrulyYellMS, isInterrupt: true);
        }

        public static bool InterruptIncoming
        {
            get
            {
                foreach (Mechanic mechanic in Mechanics)
                {
                    if (!mechanic.IsInterrupt) continue;

                    if (mechanic.MSpell != null)
                    {
                        foreach (Unit unit in Unit.EnemyUnits)
                        {
                            if (unit.Range > mechanic.Range) continue;
                            if (unit.CurrentSpell == mechanic.MSpell.Id) return true;
                        }
                    }

                    if (mechanic.MDebuff == null) continue;

                    if (Unit.player.HasAura(mechanic.MDebuff, false)) return true;
                }

                return false;
            }
        }

        public bool AutoDeff { get; private set; }

        public bool HeavyDamage { get; private set; }

        public bool IsInterrupt { get; private set; }

        public Buff? MBuff { get; private set; }

        public Debuff? MDebuff { get; private set; }

        public int MinStacks { get; private set; }

        public MechanicSpell? MSpell { get; private set; }

        public double Multiplier { get; private set; }

        public bool OnEnemy { get; private set; }

        public bool OnPlayer { get; private set; }

        public int Range { get; private set; }

        public bool RootBreak { get; private set; }

        private Mechanic(bool autoDeff, bool heavyDamage, bool isInterrupt, Buff? mBuff, Debuff? mDebuff, int minStacks, MechanicSpell? mSpell, double multiplier, bool onEnemy, bool onPlayer, int range, bool rootBreak)
        {
            AutoDeff = autoDeff;
            RootBreak = rootBreak;
            HeavyDamage = heavyDamage;
            IsInterrupt = isInterrupt;
            MBuff = mBuff;
            MDebuff = mDebuff;
            MinStacks = minStacks;
            MSpell = mSpell;
            Multiplier = multiplier;
            OnEnemy = onEnemy;
            OnPlayer = onPlayer;
            Range = range;
            Mechanics = Mechanics.Append(this).ToArray();
        }

        public Mechanic(in MechanicSpell mSpell, bool autoDeff = false, bool isInterrupt = false, bool heavyDamage = false, double multiplier = 1, int range = 100)
            : this(autoDeff, heavyDamage, isInterrupt, null, null, 0, mSpell, multiplier, false, false, range, false) { }

        public Mechanic(in Debuff mDebuff, int minStacks = 0, bool autoDeff = false, bool isInterrupt = false, bool heavyDamage = false, bool rootBreak = false, bool onPlayer = false, double multiplier = 1, int range = 100)
            : this(autoDeff, heavyDamage, isInterrupt, null, mDebuff, minStacks, null, multiplier, onPlayer, false, range, rootBreak) { }

        public Mechanic(in Buff mBuff, double multiplier = 1, int range = 100, bool onEnemy = false) : this(false, false, false, mBuff, null, 0, null, multiplier, false, onEnemy, range, false) { }

        public static bool operator <=(in Mechanic a, in Unit b) => (a.MDebuff?.Remaining(Unit.player, false) ?? 0) <= b.CurrentSpellRemaining;

        public static bool operator >=(in Mechanic a, in Unit b) => (a.MDebuff?.Remaining(Unit.player, false) ?? 0) >= b.CurrentSpellRemaining;

        public static double operator *(double a, in Mechanic b) => a * b.Multiplier;
    }
}

namespace HyperElk.Core
{
    public sealed class MechanicSpell
    {
        #region mechanic spells
#pragma warning disable CS8618
        public static MechanicSpell DeafeningScreechMS { get; private set; }
        public static MechanicSpell DisruptiveShoutMS { get; private set; }
        public static MechanicSpell EyeOfTheStorm { get; private set; }
        public static MechanicSpell InterruptingCloudburstMS { get; private set; }
        public static MechanicSpell ScreamOfPainMS { get; private set; }
        public static MechanicSpell SlicingMaelstrom { get; private set; }
        public static MechanicSpell UnnervingHowlMS { get; private set; }
        public static MechanicSpell UnrulyYellMS { get; private set; }
#pragma warning restore CS8618
        #endregion

        internal static void Initialize()
        {
            DeafeningScreechMS = new MechanicSpell(377004);
            DisruptiveShoutMS = new MechanicSpell(384365);
            EyeOfTheStorm = new MechanicSpell(200901);
            InterruptingCloudburstMS = new MechanicSpell(381516);
            ScreamOfPainMS = new MechanicSpell(397892);
            SlicingMaelstrom = new MechanicSpell(209676);
            UnnervingHowlMS = new MechanicSpell(196543);
            UnrulyYellMS = new MechanicSpell(199726);
        }

        public int Id { get; private set; }

        public bool OnTank { get; private set; }

        public bool Targeted { get; private set; }

        public MechanicSpell(int id, bool targeted = false, bool onTank = false)
        {
            Id = id;
            Targeted = targeted;
            OnTank = onTank;
        }
    }
}

namespace HyperElk.Core
{
    public sealed class NPC
    {
        public static NPC[] NPCs { get; private set; } = {
            new NPC(190174, ignore: true, add: false), // hypnosis bat
            new NPC(89011,ignore: true, add: false), // rylak skyterror
            new NPC(151579, ignore: true, add: false),
        };

        #region npcs
        public static readonly NPC explosivesNPC = new NPC(120651, true);
        #endregion

        public int Id { get; private set; }

        public bool Ignore { get; private set; }

        public bool Priority { get; private set; }

        public NPC(int id, bool priority = false, bool ignore = false, bool add = true)
        {
            Id = id;
            Priority = priority;
            Ignore = ignore;
            if (add) NPCs = NPCs.Append(this).ToArray();
        }
    }
}

namespace HyperElk.Core
{
    public sealed class SettingCategory
    {
        #region categories
        public static readonly SettingCategory itemsCategory = new SettingCategory("Items");
        #endregion

        public SettingCategory(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }

    public abstract class Setting<T>
    {
        private static readonly SettingCategory noCategory = new SettingCategory("General");

        public SettingCategory Category { get; private set; }

        public string Description { get; private set; } = "";

        public string DisplayName { get; private set; } = "";

        public string Id { get; private set; } = "";

        private protected Setting(SettingCategory? category, string description, string displayName)
        {
            Category = category ?? noCategory;
            Description = description;
            DisplayName = displayName;
            Id = displayName.ToLower().Trim();
        }

        public abstract T Value();
    }

    public sealed class SettingBool : Setting<bool>
    {
        #region Settings
#pragma warning disable CS8618
        public static SettingBool Trinket1Setting { get; private set; }
        public static SettingBool Trinket2Setting { get; private set; }
#pragma warning restore CS8618
        #endregion

        internal static void Initialize()
        {
            Trinket1Setting = new SettingBool("Trinket 1", false, "Use Trinket 1 for healing?", SettingCategory.itemsCategory);
            Trinket2Setting = new SettingBool("Trinket 2", false, "Use Trinket 2 for healing?", SettingCategory.itemsCategory);
        }

        public bool DefaultValue { get; private set; }

        public SettingBool(string displayName, bool defaultValue, string description, SettingCategory category) : base(category, description, displayName)
        {
            DefaultValue = defaultValue;
            CombatRoutine.AddProp(Id, displayName, defaultValue, description, category.Name);
        }

        public override bool Value() { return (bool)CombatRoutine.GetProperty(Id); }
    }

    public sealed class SettingStrings : Setting<string>
    {
        #region Settings
#pragma warning disable CS8618
        public static SettingStrings Trinket1Target { get; private set; }
        public static SettingStrings Trinket2Target { get; private set; }
#pragma warning restore CS8618
        #endregion

        internal static void Initialize()
        {
            string description = "On which target should be the trinket cast?";
            Trinket1Target = new SettingStrings("Trinket 1 target", Trinket.targets, description, SettingCategory.itemsCategory);
            Trinket2Target = new SettingStrings("Trinket 2 target", Trinket.targets, description, SettingCategory.itemsCategory);
        }

        public string[] Values { get; private set; }

        public SettingStrings(string displayName, string[] values, string description, in SettingCategory category) : base(category, description, displayName)
        {
            Values = values;
            CombatRoutine.AddProp(Id, displayName, values, description, category.Name);
        }

        public override string Value() { return Values[(int)CombatRoutine.GetProperty(Id)]; }

        public static bool operator ==(in SettingStrings a, string b) => a.Value().Equals(b);

        public static bool operator !=(in SettingStrings a, string b) => a.Value().Equals(b);

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}

namespace HyperElk.Core
{
    public sealed class Talent
    {
        private int ID { get; set; }

        public bool IsSelected
        {
            get
            {
                return API.PlayerIsTalentSelected(ID);
            }
        }

        public Talent(int id)
        {
            ID = id;
            CombatRoutine.AddTalents(id);
        }
    }
}

namespace HyperElk.Core
{
    public sealed class Toggle
    {
        public string Name { get; private set; }

        public bool Toggled { get { return API.ToggleIsEnabled(Name); } }

        public Toggle(string name)
        {
            Name = name;
            CombatRoutine.AddToggle(name);
        }
    }
}

namespace HyperElk.Core
{
    public sealed class Unit
    {
#pragma warning disable IDE0051
        private static int DPSSpecc { get { return API.DamagerRole; } }
#pragma warning restore IDE0051

        public static Unit[] EnemyUnits { get; private set; } = {
            new Unit("mouseover"),
            new Unit("boss1"),
            new Unit("boss2"),
            new Unit("boss3"),
            new Unit("boss4"),
        };

        public static Unit[] FriendlyUnits { get; private set; } = {
            new Unit("party1"),
            new Unit("party2"),
            new Unit("party3"),
            new Unit("party4"),
            new Unit("raid1"),
            new Unit("raid2"),
            new Unit("raid3"),
            new Unit("raid4"),
            new Unit("raid5"),
            new Unit("raid6"),
            new Unit("raid7"),
            new Unit("raid8"),
            new Unit("raid9"),
            new Unit("raid10"),
            new Unit("raid11"),
            new Unit("raid12"),
            new Unit("raid13"),
            new Unit("raid14"),
            new Unit("raid15"),
            new Unit("raid16"),
            new Unit("raid17"),
            new Unit("raid18"),
            new Unit("raid19"),
            new Unit("raid20"),
            new Unit("raid21"),
            new Unit("raid22"),
            new Unit("raid23"),
            new Unit("raid24"),
            new Unit("raid25"),
            new Unit("raid26"),
            new Unit("raid27"),
            new Unit("raid28"),
            new Unit("raid29"),
            new Unit("raid30"),
            new Unit("raid31"),
            new Unit("raid32"),
            new Unit("raid33"),
            new Unit("raid34"),
            new Unit("raid35"),
            new Unit("raid36"),
            new Unit("raid37"),
            new Unit("raid38"),
            new Unit("raid39"),
            new Unit("raid40"),
        };

        public static int GroupTTD { get { return API.LowestTTD(); } }

#pragma warning disable IDE0051
        private static int HealerSpecc { get { return API.HealerRole; } }
#pragma warning restore IDE0051

        public static bool IgnoreTarget
        {
            get
            {
                foreach (NPC npc in NPC.NPCs)
                {
                    if (!npc.Ignore) continue;

                    if (TargetId == npc.Id) return true;
                }

                foreach (Debuff debuff in Debuff.Debuffs)
                {
                    if (!debuff.Ignore) continue;

                    if (Unit.target.HasAura(debuff)) return true;
                }

                return false;
            }
        }

        public static int MouseoverId { get { return API.MouseoverGUIDNPCID; } }

        private static readonly Unit noUnit = new Unit("none");

        public static readonly Unit player = new Unit("player", true);

        public static int PlayerArcaneCharges { get { return API.PlayerCurrentArcaneCharges; } }

        public static int PlayerComboPoints { get { return API.PlayerComboPoints; } }

        public static int PlayerEssence { get { return PlayerComboPoints; } }

        public static int PlayerFocus { get { return API.PlayerFocus; } }

        public static double PlayerHaste { get { return API.PlayerGetHaste; } }

        public static bool PlayerInCombat { get { return API.PlayerIsInCombat; } }

        public static bool PlayerInRaid { get { return API.PlayerIsInRaid; } }

        public static int PlayerMana { get { return API.PlayerMana; } }

        public static bool PlayerMoving { get { return API.PlayerIsMoving; } }

        public static bool PlayerRooted { get { return API.PlayerIsCC(CCList.ROOT); } }

        private static readonly Func<Unit, bool> standartCondition = unit => true;

        public static Unit Tank
        {
            get
            {
                foreach (Unit unit in FriendlyUnits)
                {
                    if (unit.Id.Equals(player.Id)) continue;

                    if (unit.HP > 0)
                    {
                        if (unit.Specc == TankSpecc) { return unit; }
                    }
                }

                return player;
            }
        }

        private static int TankSpecc { get { return API.TankRole; } }

        public static readonly Unit target = new Unit("target", true, true);

        public static int TargetId { get { return API.TargetGUIDNPCID; } }

        public int CurrentSpell { get { return API.CurrentCastSpellID(Id); } }

        public double CurrentSpellRemaining { get { return API.UnitCurrentCastTimeRemaining(Id) / 100; } }

        public bool Enraged
        {
            get
            {
                foreach (Buff buff in Buff.Buffs)
                {
                    if (!buff.Enrage) continue;

                    if (HasAura(buff)) return true;
                }

                return false;
            }
        }

        public int HP
        {
            get
            {
                if (this == noUnit) return 999;

                return (int)Math.Min(Math.Ceiling(API.UnitHealthPercent(Id) * Multiplier), 100);
            }
        }

        public string Id { get; private set; }

        public bool Immune
        {
            get
            {
                foreach (Buff buff in Buff.Buffs)
                {
                    if (!buff.Immunity) continue;

                    if (HasAura(buff)) return true;
                }

                return false;
            }
        }

        public double Multiplier
        {
            get
            {
                double multiplier = 1;

                if (Specc == TankSpecc) multiplier *= 1.1;

                foreach (Mechanic mechanic in Mechanic.Mechanics)
                {
                    if (mechanic.Multiplier == 1) continue;

                    if (mechanic.MSpell != null)
                    {
                        foreach (Unit unit in EnemyUnits)
                        {
                            if (unit.CurrentSpell == mechanic.MSpell.Id)
                            {
                                if (Id == player && unit.Range > mechanic.Range) continue;
                                if (mechanic.MSpell.Targeted)
                                {
                                    if (Id != player) continue;
                                    if (!unit.TargetingPlayer) continue;
                                }

                                multiplier *= mechanic;
                                break;
                            }
                        }
                    }
                    else if (mechanic.MDebuff != null && HasAura(mechanic.MDebuff)) multiplier *= mechanic;
                    else if (mechanic.MBuff != null)
                    {
                        if (mechanic.OnEnemy)
                        {
                            foreach (Unit unit in EnemyUnits)
                            {
                                if (Id == player && unit.Range > mechanic.Range) continue;
                                if (unit.HasAura(mechanic.MBuff))
                                {
                                    multiplier *= mechanic;
                                    break;
                                }
                            }
                        }
                        else if (HasAura(mechanic.MBuff)) multiplier *= mechanic;
                    }
                }

                return multiplier;
            }
        }

        public int Range { get { return API.UnitRange(Id); } }

        private int Specc { get { return API.UnitRoleSpec(Id); } }

        public bool TargetingPlayer { get { return API.PlayerIsUnitTarget(Id); } }

        public Unit()
        {
            throw new NotSupportedException("Units can only be globally created!");
        }

        private Unit(string id, bool add = false, bool hostile = false)
        {
            Id = id;
            if (add)
            {
                if (!hostile) FriendlyUnits = FriendlyUnits.Append(this).ToArray();
                else EnemyUnits = EnemyUnits.Append(this).ToArray();
            }
        }

        private int Dispel(Specc specc)
        {
            int priority = -1;

            foreach (Debuff debuff in Debuff.Debuffs)
            {
                if (!(debuff.Dispel && Array.Exists(specc.DispelTypes, type => type == debuff.DType))) continue;

                if (HasAura(debuff, false))
                    if (debuff.Elapsed(this, false) > 0.2 && (debuff.DispelRemaining == 0 || debuff.DispelRemaining > debuff.Remaining(this, false)))
                        if (debuff.Stacks(this) >= debuff.MinStacksDispel)
                            if (debuff.Priority > priority) priority = debuff.Priority;
            }

            return priority;
        }

        public static Unit DispelUnit(Specc specc, int range)
        {
            int highestPriority = 0;
            Unit lowestUnit = noUnit;
            int i = 0;

            foreach (Unit unit in FriendlyUnits)
            {
                i++;

                if (unit.HP > 0 && unit.Range <= range)
                {
                    int dispelPrio = unit.Dispel(specc);
                    if (dispelPrio > highestPriority) { highestPriority = dispelPrio; lowestUnit = unit; }
                    else if (dispelPrio == highestPriority && unit.HP < lowestUnit.HP) lowestUnit = unit;
                }
            }

            return lowestUnit;
        }

        public void Focus() { API.CastSpell(Id); }

        public bool HasAura(in Buff aura, bool playerIsSrc = false) { return API.UnitHasBuff(aura.Name, Id, playerIsSrc); }

        public bool HasAura(in Debuff aura, bool playerIsSrc = true) { return API.UnitHasDebuff(aura.Name, Id, playerIsSrc, false); }

        public static int InRange(bool melee = false)
        {
            if (melee) return API.PlayerUnitInMeleeRangeCount;
            else return API.TargetUnitInRangeCount;
        }

        public static Unit LowestUnit(int range, Func<Unit, bool>? condition = null)
        {
            condition ??= standartCondition;

            Unit lowestUnit = noUnit;
            int i = 0;

            foreach (Unit unit in FriendlyUnits)
            {
                i++;

                if (unit.HP > 0 && unit.Range <= range && condition(unit))
                {
                    if (unit.HP < lowestUnit.HP) lowestUnit = unit;
                }
            }

            return lowestUnit;
        }

        public static int NumUnitsGettingHeavyDamage(int range)
        {
            int num = 0;

            foreach (Unit unit in FriendlyUnits)
            {
                if (unit.HP == 0 || unit.Range > range) continue;

                foreach (Mechanic mechanic in Mechanic.Mechanics)
                {
                    if (!mechanic.HeavyDamage) continue;

                    if (mechanic.MDebuff != null)
                    {
                        if (unit.HasAura(mechanic.MDebuff, false) && mechanic.MDebuff.Stacks(unit) >= mechanic.MinStacks) num++;
                    }
                    else if (mechanic.MSpell != null)
                    {
                        foreach (Unit eunit in Unit.EnemyUnits)
                        {
                            if (eunit == mechanic.MSpell) num++;
                        }
                    }
                }
            }

            return num;
        }

        public static int UnitsFittingCondition(int range, in Func<Unit, bool> condition)
        {
            int num = 0;
            int i = 0;

            foreach (Unit unit in Unit.FriendlyUnits)
            {
                i++;

                if (unit.HP > 0 && unit.Range <= range && condition(unit))
                {
                    num++;
                }
            }

            return num;
        }

        public static bool operator <=(in Unit a, in Unit b) => a.CurrentSpellRemaining <= b.CurrentSpellRemaining;

        public static bool operator >=(in Unit a, in Unit b) => a.CurrentSpellRemaining >= b.CurrentSpellRemaining;

        public static bool operator ==(in Unit a, in Mechanic b) => a.CurrentSpell == (b.MSpell?.Id ?? -1);

        public static bool operator !=(in Unit a, in Mechanic b) => a.CurrentSpell != (b.MSpell?.Id ?? -1);

        public static bool operator ==(in Unit a, string b) => a.Id == b;

        public static bool operator !=(in Unit a, string b) => a.Id != b;

        public static bool operator ==(string a, in Unit b) => a == b.Id;

        public static bool operator !=(string a, in Unit b) => a != b.Id;

        public static bool operator ==(Unit a, Spell b) => a.CurrentSpell == b.Id;

        public static bool operator !=(Unit a, Spell b) => a.CurrentSpell != b.Id;

        public static bool operator ==(Unit a, MechanicSpell b) => a.CurrentSpell == b.Id;

        public static bool operator !=(Unit a, MechanicSpell b) => a.CurrentSpell != b.Id;

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}


namespace HyperElk.Core
{
    internal sealed class Restauration : CombatRoutine
    {
        private const int DEFAULTRANGE = 40;

        private readonly Specc specc = new Specc("Druid", new DispelType[] { DispelType.Magic, DispelType.Curse, DispelType.Poison }, "Restoration");

        private Unit? lowestUnit;
        private bool hasTarget;
        private bool ignoreTarget;
        private int lowest;

        #region Initilization
#pragma warning disable CS8618
        #region settings
        #region settings categories
        private SettingCategory autoFormCategory;
        private SettingCategory priorityCategory;
        private SettingCategory utilityCategory;
        #endregion

        #region settings
        #region auto form
        private SettingBool bearFormSetting;
        private SettingBool catFormSetting;
        #endregion

        #region items
        private SettingBool neuralSynapseEnhancerSetting;
        #endregion

        #region priority
        private SettingBool explosiveSetting;
        #endregion

        #region utility
        private SettingBool sootheSetting;
        private SettingBool wildChargeSetting;
        #endregion
        #endregion
        #endregion

        #region toggles
        private Toggle dispelToggle;
        private Toggle burstToggle;
        #endregion

        #region casts
        #region spells
        private Spell adaptiveSwarm;
        private Spell barkskin;
        private Spell bearForm;
        private Spell catForm;
        private Spell moonkinForm;
        private Spell cenarionWard;
        private Spell convokeTheSpirits;
        private Spell flourish;
        private Spell frenziedRegeneration;
        private Spell innervate;
        private Spell ironbark;
        private Spell lifebloom;
        private Spell markOfTheWild;
        private Spell moonfire;
        private Spell naturesCure;
        private Spell naturesSwiftness;
        private Spell naturesVigil;
        private Spell regrowth;
        private Spell rejuvenation;
        private Spell soothe;
        private Spell starfire;
        private Spell starsurge;
        private Spell sunfire;
        private Spell tranquility;
        private Spell swiftmend;
        private Spell wildGrowth;
        private Spell wrath;
        private Spell wildCharge;
        private Spell renewal;
        #endregion

        #region items
        private SettingBool healthstoneSetting;
        private Item neuralSynapseEnhancerItem;
        private SettingBool refreshingHealingPotionSetting;
        #endregion
        #endregion

        #region auras
        #region buffs
        private Buff adaptiveSwarmBuff;
        private Buff barkskinBuff;
        private Buff bearFormBuff;
        private Buff catFormBuff;
        private Buff moonkinFormBuff;
        private Buff clearcastingBuff;
        private Buff incarnationTreeOfLifeBuff;
        private Buff ironbarkBuff;
        private Buff lifebloomBuff;
        private Buff lifebloomUndergrowthBuff;
        private Buff markOfTheWildBuff;
        private Buff naturesSwiftnessBuff;
        private Buff regrowthBuff;
        private Buff rejuvenationBuff;
        private Buff soulOfTheForestBuff;
        private Buff travelFormBuff;
        private Buff wildGrowthBuff;
        #endregion

        #region debuffs
        private Debuff adaptiveSwarmDebuff;
        private Debuff moonfireDebuff;
        private Debuff sunfireDebuff;
        #endregion
        #endregion
#pragma warning restore CS8618

        public override void Initialize()
        {
            Base.Initialize(DEFAULTRANGE);

            #region settings
            #region settings categories
            autoFormCategory = new SettingCategory("Auto Form");
            priorityCategory = new SettingCategory("Priority");
            utilityCategory = new SettingCategory("Utility");
            #endregion

            #region bool settings
            #region auto form
            bearFormSetting = new SettingBool("Bear Form Mechanics", true, "Switch to Bear Form when a heavy mechanic is comming in?", autoFormCategory);
            catFormSetting = new SettingBool("Cat Form break Root", true, "Switch to Cat Form when rooted?", autoFormCategory);
            #endregion

            #region items
            neuralSynapseEnhancerSetting = new SettingBool("Neural Synapse Enhancer", false, "Do you have Neural Synapse Enhancer equipped?", SettingCategory.itemsCategory);
            #endregion

            #region priority
            explosiveSetting = new SettingBool("Explosive", true, "Do you want to focus Explosives when targeted?", priorityCategory);
            #endregion

            #region utility
            sootheSetting = new SettingBool("Soothe", true, "Do you want to Soothe enraged enemies?", utilityCategory);
            wildChargeSetting = new SettingBool("Wild Charge", false, "Do you want to use Wild Charge to dispell season affix ?", utilityCategory);

            #endregion
            #endregion
            #endregion

            #region toggles
            dispelToggle = new Toggle("Dispel");
            burstToggle = new Toggle("IncomingBurst");
            #endregion

            #region casts
            #region spells
            adaptiveSwarm = new Spell("Adaptive Swarm", 391888, DEFAULTRANGE);
            barkskin = new Spell("Barkskin", 22812, DEFAULTRANGE);
            bearForm = new Spell("Bear Form", 5487, DEFAULTRANGE);
            catForm = new Spell("Cat Form", 768, DEFAULTRANGE);
            moonkinForm = new Spell("Moonkin Form", 197625, DEFAULTRANGE);
            cenarionWard = new Spell("Cenarion Ward", 102351, DEFAULTRANGE);
            convokeTheSpirits = new Spell("Convoke the Spirits", 391528, DEFAULTRANGE);
            flourish = new Spell("Flourish", 197721, DEFAULTRANGE);
            frenziedRegeneration = new Spell("Frenzied Regeneration", 22842, DEFAULTRANGE);
            innervate = new Spell("Innervate", 29166, DEFAULTRANGE);
            ironbark = new Spell("Ironbark", 102342, DEFAULTRANGE);
            lifebloom = new Spell("Lifebloom", 33763, DEFAULTRANGE);
            markOfTheWild = new Spell("Mark of the Wild", 1126, DEFAULTRANGE);
            moonfire = new Spell("Moonfire", 8921, DEFAULTRANGE);
            naturesCure = new Spell("Nature's Cure", 88423, DEFAULTRANGE);
            naturesSwiftness = new Spell("Nature's Swiftness", 132158, DEFAULTRANGE);
            naturesVigil = new Spell("Nature's Vigil", 124974, DEFAULTRANGE);
            regrowth = new Spell("Regrowth", 8936, DEFAULTRANGE);
            rejuvenation = new Spell("Rejuvenation", 774, DEFAULTRANGE);
            soothe = new Spell("Soothe", 2908, DEFAULTRANGE);
            starfire = new Spell("Starfire", 197628, DEFAULTRANGE);
            starsurge = new Spell("Starsurge", 197626, DEFAULTRANGE);
            sunfire = new Spell("Sunfire", 93402, DEFAULTRANGE);
            tranquility = new Spell("Tranquility", 740, DEFAULTRANGE);
            swiftmend = new Spell("Swiftmend", 18562, DEFAULTRANGE);
            wildGrowth = new Spell("Wild Growth", 48438, DEFAULTRANGE);
            wrath = new Spell("Wrath", 5176, DEFAULTRANGE);
            wildCharge = new Spell("Wild Charge", 102401, DEFAULTRANGE);
            renewal = new Spell("Renewal", 108238, DEFAULTRANGE);
            #endregion

            #region items
            healthstoneSetting = new SettingBool("Healthstone Healing", true, "Do you want to use Healthstone?", SettingCategory.itemsCategory);
            neuralSynapseEnhancerItem = new Weapon("Neural Synapse Enhancer", 168973, DEFAULTRANGE);
            refreshingHealingPotionSetting = new SettingBool("Refreshing Healing Potion", false, "Do you want to use Refreshing Healing Potion?", SettingCategory.itemsCategory);
            #endregion
            #endregion

            #region auras
            #region buffs
            adaptiveSwarmBuff = new Buff(391891, adaptiveSwarm);
            barkskinBuff = new Buff(barkskin);
            bearFormBuff = new Buff(bearForm);
            catFormBuff = new Buff(catForm);
            moonkinFormBuff = new Buff(moonkinForm);
            clearcastingBuff = new Buff("Clearcasting", 16870);
            incarnationTreeOfLifeBuff = new Buff("Incarnation: Tree of Life", 33819);
            ironbarkBuff = new Buff(ironbark);
            lifebloomBuff = new Buff(lifebloom, 3);
            lifebloomUndergrowthBuff = new Buff("Lifebloom (Undergrowth)", 188550, 3);
            markOfTheWildBuff = new Buff(markOfTheWild, 300);
            naturesSwiftnessBuff = new Buff(naturesSwiftness);
            regrowthBuff = new Buff(regrowth);
            rejuvenationBuff = new Buff(rejuvenation, 3);
            soulOfTheForestBuff = new Buff("Soul of the Forest", 114108);
            travelFormBuff = new Buff("Travel Form", 783);
            wildGrowthBuff = new Buff(wildGrowth);
            #endregion

            #region debuffs
            adaptiveSwarmDebuff = new Debuff(325733, adaptiveSwarm);
            moonfireDebuff = new Debuff(164812, moonfire, 3);
            sunfireDebuff = new Debuff(164815, sunfire, 2);
            #endregion
            #endregion

            #region mechanics
            _ = new Mechanic(ironbarkBuff, 1.1);
            _ = new Mechanic(barkskinBuff, 1.1);
            #endregion
        }
        #endregion

        private bool CooldownRotation()
        {
            // innervate
            lowestUnit = Unit.LowestUnit(DEFAULTRANGE, unit => unit.HP < 60);
            if (lowestUnit != "none" && innervate.Usable(Unit.player) && Unit.PlayerMana < 80) innervate.Use(Unit.player);

            // barkskin
            if (barkskin.Usable(Unit.player) && Unit.player.HP < 45) barkskin.Use();

            // ironbark
            lowestUnit = Unit.LowestUnit(DEFAULTRANGE, unit => unit.HP < 50);
            if (lowestUnit != "none" && ironbark.Usable(lowestUnit) && (!lowestUnit.Equals(Unit.player) || !barkskin.Usable(Unit.player) && !Unit.player.HasAura(barkskinBuff, true))) ironbark.Use(lowestUnit);

            // neural synapse enhancer
            if (neuralSynapseEnhancerSetting.Value())
            {
                lowestUnit = Unit.LowestUnit(DEFAULTRANGE, unit => unit.HP < 50);
                if (lowestUnit != "none" && neuralSynapseEnhancerItem.Usable(Unit.player)) neuralSynapseEnhancerItem.Use();
            }

            // trinket 1
            if (SettingBool.Trinket1Setting.Value())
            {
                lowestUnit = Unit.LowestUnit(DEFAULTRANGE, unit => unit.HP < 50);
                if (lowestUnit != "none" && Trinket.Trinket1.Usable(Unit.player) && (!neuralSynapseEnhancerItem.Usable(Unit.player) || !neuralSynapseEnhancerSetting.Value()))
                {
                    if (SettingStrings.Trinket1Target == Trinket.targets[0]) Trinket.Trinket1.Use();
                    else if (SettingStrings.Trinket1Target == Trinket.targets[1]) Trinket.Trinket1.Use(Unit.player);
                    else if (SettingStrings.Trinket1Target == Trinket.targets[2]) Trinket.Trinket1.Use(lowestUnit);
                    else if (hasTarget && !ignoreTarget) Trinket.Trinket1.Use();
                }
            }

            // trinket 2
            if (SettingBool.Trinket2Setting.Value())
            {
                lowestUnit = Unit.LowestUnit(DEFAULTRANGE, unit => unit.HP < 50);
                if (lowestUnit != "none" && Trinket.Trinket2.Usable(Unit.player) && (!Trinket.Trinket1.Usable(Unit.player) || !SettingBool.Trinket1Setting.Value()) && (!neuralSynapseEnhancerItem.Usable(Unit.player)
                    || !neuralSynapseEnhancerSetting.Value()))
                {
                    if (SettingStrings.Trinket2Target == Trinket.targets[0]) Trinket.Trinket2.Use();
                    else if (SettingStrings.Trinket2Target == Trinket.targets[1]) Trinket.Trinket2.Use(Unit.player);
                    else if (SettingStrings.Trinket2Target == Trinket.targets[2]) Trinket.Trinket2.Use(lowestUnit);
                    else if (hasTarget && !ignoreTarget) Trinket.Trinket2.Use();
                }
            }

            // renewal
            if (renewal.Usable(Unit.player) && Unit.PlayerInCombat && Unit.player.HP < 50) { renewal.Use(); return true; }

            // Refreshing Healing Potion
            if (refreshingHealingPotionSetting.Value()) if (Potion.RefreshingHealingPotion.Usable(Unit.player) && Unit.player.HP < 30) { Potion.RefreshingHealingPotion.Use(); return true; }

            // Healthstone
            if (healthstoneSetting.Value()) if (Consumable.HealthstoneConsumable.Usable(Unit.player) && Unit.player.HP < 40) { Consumable.HealthstoneConsumable.Use(); return true; }

            // flourish
            int numLowUnitsWildGrowth = Unit.UnitsFittingCondition(DEFAULTRANGE, unit => unit.HP < 70 && unit.HasAura(wildGrowthBuff));
            int numLowUnits = Unit.UnitsFittingCondition(DEFAULTRANGE, unit => unit.HP < 70);
            bool enoughLowUnits = flourish.Usable(Unit.player) && (numLowUnitsWildGrowth > 1 || numLowUnits > 1 && Unit.player == wildGrowth);
            if (enoughLowUnits) { flourish.Use(); return true; }

            // swiftmend
            lowestUnit = Unit.LowestUnit(DEFAULTRANGE, unit => unit.HP < 60);
            if (lowestUnit != "none" && swiftmend.Usable(lowestUnit, false) && !naturesSwiftness.Usable(lowestUnit) && !Unit.player.HasAura(naturesSwiftnessBuff) && (lowestUnit.HasAura(rejuvenationBuff, true)
                || lowestUnit.HasAura(regrowthBuff, true)) && Unit.PlayerInCombat) { swiftmend.Use(lowestUnit); return true; }

            // nature's swiftness
            lowestUnit = Unit.LowestUnit(DEFAULTRANGE, unit => unit.HP < 70);
            if (lowestUnit != "none" && naturesSwiftness.Usable(Unit.player) && regrowth.Usable(lowestUnit) && flourish.Usable() && !enoughLowUnits && !Unit.player.HasAura(naturesSwiftnessBuff) && Unit.PlayerInCombat)
            { naturesSwiftness.Use(); regrowth.Use(lowestUnit); return true; }

            // regrowth nature's swiftness
            lowestUnit = Unit.LowestUnit(DEFAULTRANGE, unit => unit.HP < 70);
            if (lowestUnit != "none" && regrowth.Usable(lowestUnit) && Unit.player.HasAura(naturesSwiftnessBuff)) { regrowth.Use(lowestUnit); return true; }

            // convoke the spirits
            lowestUnit = Unit.LowestUnit(DEFAULTRANGE, unit => unit.HP < 60);
            if (lowestUnit != "none" && convokeTheSpirits.Usable(lowestUnit) && Unit.PlayerInCombat && !Unit.player.HasAura(moonkinFormBuff) && !enoughLowUnits && !Mechanic.InterruptIncoming) { convokeTheSpirits.Use(); return true; }

            return false;
        }

        private bool UtilityRotation()
        {
            // nature's cure
            Unit dispelUnit = Unit.DispelUnit(specc, DEFAULTRANGE);
            if (dispelToggle.Toggled) if (dispelUnit != "none" && naturesCure.Usable(dispelUnit, false)) { naturesCure.Use(dispelUnit); return true; }

            // soothe
            if (sootheSetting.Value()) if (hasTarget && !ignoreTarget && soothe.Usable(false) && Unit.target.Enraged && lowest > 50) { soothe.Use(); return true; }

            // moonfire explosive
            if (explosiveSetting.Value()) if (hasTarget && moonfire.Usable(false) && Unit.TargetId == NPC.explosivesNPC.Id && lowest > 50) { moonfire.Use(); return true; }

            // cat form
            if (catFormSetting.Value()) if ((Unit.PlayerRooted || Cast.RootBreak) && catForm.Usable(Unit.player) && !Unit.player.HasAura(catFormBuff) && lowest > 50) { catForm.Use(); return true; }

            return false;
        }

        private bool AdaptiveSwarmRotation()
        {
            // adaptive swarm healing
            Unit lowestUnit = Unit.LowestUnit(DEFAULTRANGE, unit => adaptiveSwarmBuff.Stacks(unit) < 3 && unit.HP < 95);
            if (lowestUnit != "none" && adaptiveSwarm.Usable(lowestUnit)) { adaptiveSwarm.Use(lowestUnit); return true; }

            // adaptive swarm enemy
            if (hasTarget && !ignoreTarget && adaptiveSwarm.Usable() && adaptiveSwarmDebuff.Stacks() < 3) { adaptiveSwarm.Use(); return true; }

            // adaptive swarm ally
            lowestUnit = Unit.LowestUnit(DEFAULTRANGE, unit => adaptiveSwarmBuff.Stacks(unit) < 3);
            if (lowestUnit != "none" && adaptiveSwarm.Usable(lowestUnit)) { adaptiveSwarm.Use(lowestUnit); return true; }

            return false;
        }

        private void DamageRotation()
        {
            if (hasTarget && !ignoreTarget && lowest > 40)
            {
                // transform to moonkin form
                if (moonkinForm.Usable() && !Unit.player.HasAura(moonkinFormBuff) && !Unit.PlayerMoving) { moonkinForm.Use(); return; }

                // moonfire
                if (moonfire.Usable() && moonfireDebuff.NeedRefresh() && Unit.InRange() < 3) { moonfire.Use(); return; }

                // sunfire
                if (sunfire.Usable() && sunfireDebuff.NeedRefresh()) { sunfire.Use(); return; }

                // moonfire
                if (moonfire.Usable() && moonfireDebuff.NeedRefresh()) { moonfire.Use(); return; }

                // stasurge
                if (starsurge.Usable()) { starsurge.Use(); return; }

                // wrath
                if (!Mechanic.InterruptIncoming && starfire.Usable() && !Unit.PlayerMoving && Unit.InRange() > 4) { starfire.Use(); return; }

                // wrath
                if (!Mechanic.InterruptIncoming && wrath.Usable() && !Unit.PlayerMoving) { wrath.Use(); return; }
            }
        }

        private void Rotation()
        {
            // bear form
            if (bearFormSetting.Value() && Cast.UseDeff)
            {
                if (Unit.player.CurrentSpellRemaining > 0) Macro.stopCastMacro.Use();
                if (!Unit.player.HasAura(bearFormBuff)) bearForm.Use();
                if (frenziedRegeneration.Usable(Unit.player)) frenziedRegeneration.Use();
                return;
            }

            int gcd = API.SpellGCDDuration;
            if (gcd > Cast.DELAY || Unit.player.CurrentSpellRemaining > Cast.DELAY) return;
            if (Unit.player == tranquility || Unit.player == convokeTheSpirits) return;

            // mark of the wild
            foreach (Unit unit in Unit.FriendlyUnits)
            {
                if (!Unit.PlayerInCombat && markOfTheWild.Usable(unit) && markOfTheWildBuff.NeedRefresh(unit) && unit.HP > 0) { markOfTheWild.Use(Unit.player); return; }
            }

            if (CooldownRotation()) return;

            // utility rotation
            ignoreTarget = Unit.target.Immune || Unit.IgnoreTarget;
            hasTarget = Unit.target.HP > 0 && API.PlayerCanAttackTarget;
            if (UtilityRotation()) return;

            // prepare for heavy damage
            if (wildGrowth.Usable(Unit.player) && Unit.NumUnitsGettingHeavyDamage(DEFAULTRANGE) > 2)
            {
                // swiftmend for soul of the forest buff
                lowestUnit = Unit.LowestUnit(DEFAULTRANGE, unit => unit.HasAura(rejuvenationBuff, true) || unit.HasAura(regrowthBuff, true));
                if (lowestUnit != "none" && swiftmend.Usable(lowestUnit, false) && !Unit.player.HasAura(soulOfTheForestBuff)) { swiftmend.Use(); return; }

                // wild growth
                lowestUnit = Unit.LowestUnit(DEFAULTRANGE);
                if (lowestUnit != "none" && wildGrowth.Usable(lowestUnit) && !Unit.PlayerMoving) { wildGrowth.Use(); return; }
            }

            // cenarion ward
            lowestUnit = Unit.LowestUnit(DEFAULTRANGE, unit => unit.HP < 80);
            if (lowestUnit != "none" && cenarionWard.Usable(lowestUnit) && Unit.PlayerInCombat) { cenarionWard.Use(lowestUnit); return; }

            if (!burstToggle.Toggled && lowest > 50)
            {
                // nature's vigil
                if (hasTarget && !ignoreTarget && naturesVigil.Usable(Unit.target)) { naturesVigil.Use(); return; }

                // sunfire
                if (hasTarget && !ignoreTarget && sunfire.Usable(Unit.target) && !Unit.target.HasAura(sunfireDebuff) && Unit.InRange() >= 3) { sunfire.Use(); return; }

                // moonfire
                if (hasTarget && !ignoreTarget && moonfire.Usable(Unit.target) && !Unit.target.HasAura(moonfireDebuff) && Unit.InRange() < 3) { moonfire.Use(); return; }
            }

            // lifebloom
            Unit tank = Unit.Tank;
            int numberLowUnits = Unit.UnitsFittingCondition(70, unit => unit.HP < 85);
            int numberSub50 = Unit.UnitsFittingCondition(70, unit => unit.HP < 60);
            lowestUnit = Unit.LowestUnit(DEFAULTRANGE, unit => unit.HP < 85);
            bool useWildgrowth = !Mechanic.InterruptIncoming && lowestUnit != "none" && wildGrowth.Usable(lowestUnit) && (numberLowUnits >= 3 || numberLowUnits >= 2 && !Unit.PlayerInRaid
                && Unit.player.HasAura(soulOfTheForestBuff));
            if (lifebloom.Usable(tank) && lifebloomBuff.NeedRefresh(tank) && lifebloomUndergrowthBuff.NeedRefresh(tank) && !((lowest < 35 || useWildgrowth && !Unit.PlayerMoving && numberSub50 > 1) && tank.HP > 25))
            { lifebloom.Use(tank); return; }

            // second lifebloom
            if (tank.HasAura(lifebloomUndergrowthBuff))
            {
                if (lifebloom.Usable(Unit.player) && lifebloomUndergrowthBuff.NeedRefresh(Unit.player) && !((lowest < 40 || useWildgrowth && !Unit.PlayerMoving && numberSub50 > 1) && Unit.player.HP > 25))
                { lifebloom.Use(Unit.player); return; }
            }

            if (useWildgrowth)
            {
                // swiftmend for soul of the forest buff
                if (!Unit.player.HasAura(soulOfTheForestBuff))
                {
                    Unit lowestUnitSwiftmend = Unit.LowestUnit(DEFAULTRANGE, unit => unit.HasAura(rejuvenationBuff, true) || unit.HasAura(regrowthBuff, true));
                    if (lowestUnitSwiftmend != "none" && swiftmend.Usable(lowestUnitSwiftmend, false)) { swiftmend.Use(lowestUnitSwiftmend); return; }
                }

                // wildgrowth
                if (!Unit.PlayerMoving) { wildGrowth.Use(lowestUnit); return; }
            }

            // regrowth soul of the forest
            if (!Mechanic.InterruptIncoming || Unit.player.HasAura(incarnationTreeOfLifeBuff))
            {
                lowestUnit = Unit.LowestUnit(DEFAULTRANGE, unit => unit.HP < 75);
                if (lowestUnit != "none" && regrowth.Usable(lowestUnit) && Unit.player.HasAura(soulOfTheForestBuff) && (!Unit.PlayerMoving || Unit.player.HasAura(incarnationTreeOfLifeBuff)))
                { regrowth.Use(lowestUnit); return; }
            }

            // adaptive swarm healing
            lowestUnit = Unit.LowestUnit(DEFAULTRANGE, unit => adaptiveSwarmBuff.Stacks(unit) < 3 && unit.HP < 95);
            if (lowestUnit != "none" && adaptiveSwarm.Usable(lowestUnit) && !(lowest < 40 && lowestUnit.HP > 25))
            { adaptiveSwarm.Use(lowestUnit); return; }

            // regrowth incarnation
            lowestUnit = Unit.LowestUnit(DEFAULTRANGE, unit => unit.HP < 60);
            if (lowestUnit != "none" && regrowth.Usable(lowestUnit) && Unit.player.HasAura(incarnationTreeOfLifeBuff)) { regrowth.Use(lowestUnit); return; }

            // rejuvenation expired
            lowestUnit = (!Unit.PlayerInRaid) ? Unit.LowestUnit(DEFAULTRANGE, unit => !unit.HasAura(rejuvenationBuff, true) && unit.HP < 95) :
                ((Unit.PlayerMana > 30) ? Unit.LowestUnit(DEFAULTRANGE, unit => !unit.HasAura(rejuvenationBuff, true) && unit.HP < 85) : Unit.LowestUnit(DEFAULTRANGE, unit => !unit.HasAura(rejuvenationBuff, true)
                && unit.HP < 60));
            if (lowestUnit != "none" && rejuvenation.Usable(lowestUnit) && !(lowest < 40 && lowestUnit.HP > 25)) { rejuvenation.Use(lowestUnit); return; }

            // regrowth incarnation
            lowestUnit = Unit.LowestUnit(DEFAULTRANGE, unit => unit.HP < 80);
            if (lowestUnit != "none" && regrowth.Usable(lowestUnit) && Unit.player.HasAura(incarnationTreeOfLifeBuff)) { regrowth.Use(lowestUnit); return; }

            // regrowth clearcasting
            if (!Mechanic.InterruptIncoming)
            {
                lowestUnit = Unit.LowestUnit(DEFAULTRANGE, unit => unit.HP < 75);
                if (lowestUnit != "none" && regrowth.Usable(lowestUnit) && Unit.player.HasAura(clearcastingBuff) && Unit.player != regrowth && !Unit.PlayerMoving) { regrowth.Use(lowestUnit); return; }
            }

            // regrowth
            if (!Mechanic.InterruptIncoming)
            {
                lowestUnit = Unit.LowestUnit(DEFAULTRANGE, unit => unit.HP < 50);
                if (lowestUnit != "none" && regrowth.Usable(lowestUnit) && !Unit.PlayerMoving) { regrowth.Use(lowestUnit); return; }
            }

            // rejuvenation expiring
            lowestUnit = (!Unit.PlayerInRaid) ? Unit.LowestUnit(DEFAULTRANGE, unit => rejuvenationBuff.NeedRefresh(unit) && unit.HP < 95) :
                ((Unit.PlayerMana > 30) ? Unit.LowestUnit(DEFAULTRANGE, unit => rejuvenationBuff.NeedRefresh(unit) && unit.HP < 75) : Unit.LowestUnit(DEFAULTRANGE, unit => rejuvenationBuff.NeedRefresh(unit) && unit.HP < 60));
            if (lowestUnit != "none" && rejuvenation.Usable(lowestUnit)) { rejuvenation.Use(lowestUnit); return; }

            // rejuvenation pre burst
            if (burstToggle.Toggled)
            {
                lowestUnit = Unit.LowestUnit(DEFAULTRANGE, unit => rejuvenationBuff.NeedRefresh(unit));
                if (lowestUnit != "none" && rejuvenation.Usable(lowestUnit)) { rejuvenation.Use(lowestUnit); return; }
            }

            if (AdaptiveSwarmRotation()) return;

            DamageRotation();
        }

        public override void Pulse()
        {
            if (API.PlayerIsMounted || Unit.player.HasAura(travelFormBuff)) { return; }

            // cancel cast
            if (Cast.StopCast) { Macro.stopCastMacro.Use(); return; }

            Unit lowestUnit = Unit.LowestUnit(DEFAULTRANGE);
            lowest = lowestUnit.HP;

            // cancel cast
            if (lowest < 40 && (Unit.player.CurrentSpell == wrath.Id || Unit.player.CurrentSpell == starfire.Id)) { Macro.stopCastMacro.Use(); return; }

            Rotation();
        }

        public override void CombatPulse() { }

        public override void OutOfCombatPulse() { }
    }
}

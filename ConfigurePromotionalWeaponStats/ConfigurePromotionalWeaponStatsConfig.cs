using PhoenixPoint.Modding;

namespace ConfigurePromotionalWeaponStats
{
    /// <summary>
    /// ModConfig is mod settings that players can change from within the game.
    /// Config is only editable from players in main menu.
    /// Only one config can exist per mod assembly.
    /// Config is serialized on disk as json.
    /// </summary>
    public class ConfigurePromotionalWeaponStatsConfig : ModConfig
    {
        /// these are the default game values as far as I can tell.
        /// we need to set them here so that they show up when enabling
        /// the mod, but not when starting the game with the mod already
        /// enabled.

        /// Gold Ares AR-1
        [ConfigField(text: "Ares Gold AR Damage")]
        public float AresGoldArDamage = 35f;
        [ConfigField(text: "Ares Gold AR Shred")]
        public float AresGoldArShred = 8f;
        [ConfigField(text: "Ares Gold AR Ammo Capacity")]
        public int AresGoldArAmmoCapacity = 36;
        [ConfigField(text: "Ares Gold AR Burst")]
        public int AresGoldArBurst = 6;
        [ConfigField(text: "Ares Gold AR Projectiles Per Shot")]
        public int AresGoldArProjectilesPerShot = 1;
        [ConfigField(text: "Ares Gold AR Effective Range")]
        public int AresGoldArEffectiveRange = 30;
        [ConfigField(text: "Ares Gold AR AP Cost")]
        public int AresGoldArApCost = 2;
        [ConfigField(text: "Ares Gold AR Hands To Use")]
        public int AresGoldArHandsToUse = 2;
        [ConfigField(text: "Ares Gold AR Weight")]
        public int AresGoldArWeight = 3;
        [ConfigField(text: "Ares Gold AR Stop On First Hit")]
        public bool AresGoldArStopOnFirstHit = false;

        /// Gold Firebird SR
        [ConfigField(text: "Firebird Gold SR Damage")]
        public float FirebirdGoldSrDamage = 130f;
        [ConfigField(text: "Firebird Gold SR Piercing")]
        public float FirebirdGoldSrPiercing = 30f;
        [ConfigField(text: "Firebird Gold SR Ammo Capacity")]
        public int FirebirdGoldSrAmmoCapacity = 8;
        [ConfigField(text: "Firebird Gold SR Burst")]
        public int FirebirdGoldSrBurst = 1;
        [ConfigField(text: "Firebird Gold SR Projectiles Per Shot")]
        public int FirebirdGoldSrProjectilesPerShot = 1;
        [ConfigField(text: "Firebird Gold SR Effective Range")]
        public int FirebirdGoldSrEffectiveRange = 45;
        [ConfigField(text: "Firebird Gold SR AP Cost")]
        public int FirebirdGoldSrApCost = 2;
        [ConfigField(text: "Firebird Gold SR Hands To Use")]
        public int FirebirdGoldSrHandsToUse = 2;
        [ConfigField(text: "Firebird Gold SR Weight")]
        public int FirebirdGoldSrWeight = 4;
        [ConfigField(text: "Firebird Gold SR Stop On First Hit")]
        public bool FirebirdGoldSrStopOnFirstHit = false;

        /// Gold Hel II Cannon
        [ConfigField(text: "Hel Gold II Cannon Damage")]
        public float HelGoldIiDamage = 240f;
        [ConfigField(text: "Hel Gold II Cannon Shred")]
        public float HelGoldIiShred = 20f;
        [ConfigField(text: "Hel Gold II Cannon Shock")]
        public float HelGoldIiShock = 280f;
        [ConfigField(text: "Hel Gold II Cannon Ammo Capacity")]
        public int HelGoldIiAmmoCapacity = 6;
        [ConfigField(text: "Hel Gold II Cannon Burst")]
        public int HelGoldIiBurst = 1;
        [ConfigField(text: "Hel Gold II Cannon Projectiles Per Shot")]
        public int HelGoldIiProjectilesPerShot = 1;
        [ConfigField(text: "Hel Gold II Cannon Effective Range")]
        public int HelGoldIiEffectiveRange = 20;
        [ConfigField(text: "Hel Gold II Cannon AP Cost")]
        public int HelGoldIiApCost = 2;
        [ConfigField(text: "Hel Gold II Cannon Hands To Use")]
        public int HelGoldIiHandsToUse = 2;
        [ConfigField(text: "Hel Gold II Cannon Weight")]
        public int HelGoldIiWeight = 5;
        [ConfigField(text: "Hel Gold II Cannon Stop On First Hit")]
        public bool HelGoldIiStopOnFirstHit = false;

        /// Phoenix Rising Firebird SR
        [ConfigField(text: "Firebird Phoenix Rising SR Damage")]
        public float FirebirdPRSrDamage = 150f;
        [ConfigField(text: "Firebird Phoenix Rising SR Piercing")]
        public float FirebirdPRSrPiercing = 50f;
        [ConfigField(text: "Firebird Phoenix Rising SR Ammo Capacity")]
        public int FirebirdPRSrAmmoCapacity = 8;
        [ConfigField(text: "Firebird Phoenix Rising SR Burst")]
        public int FirebirdPRSrBurst = 1;
        [ConfigField(text: "Firebird Phoenix Rising SR Projectiles Per Shot")]
        public int FirebirdPRSrProjectilesPerShot = 1;
        [ConfigField(text: "Firebird Phoenix Rising SR Effective Range")]
        public int FirebirdPRSrEffectiveRange = 60;
        [ConfigField(text: "Firebird Phoenix Rising SR AP Cost")]
        public int FirebirdPRSrApCost = 3;
        [ConfigField(text: "Firebird Phoenix Rising SR Hands To Use")]
        public int FirebirdPRSrHandsToUse = 2;
        [ConfigField(text: "Firebird Phoenix Rising SR Weight")]
        public int FirebirdPRSrWeight = 4;
        [ConfigField(text: "Firebird Phoenix Rising SR Stop On First Hit")]
        public bool FirebirdPRSrStopOnFirstHit = false;

        /// White Neon Deimos AR
        [ConfigField(text: "White Neon Deimos AR Damage")]
        public float WhiteNeonDeimosArDamage = 30;
        [ConfigField(text: "White Neon Deimos AR Piercing")]
        public float WhiteNeonDeimosArPiercing = 30f;
        [ConfigField(text: "White Neon Deimos AR Fire")]
        public float WhiteNeonDeimosArFire = 0f;
        [ConfigField(text: "White Neon Deimos AR Ammo Capacity")]
        public int WhiteNeonDeimosArAmmoCapacity = 60;
        [ConfigField(text: "White Neon Deimos AR Burst")]
        public int WhiteNeonDeimosArBurst = 6;
        [ConfigField(text: "White Neon Deimos AR Projectiles Per Shot")]
        public int WhiteNeonDeimosArProjectilesPerShot = 1;
        [ConfigField(text: "White Neon Deimos AR Effective Range")]
        public int WhiteNeonDeimosArEffectiveRange = 35;
        [ConfigField(text: "White Neon Deimos AR AP Cost")]
        public int WhiteNeonDeimosArApCost = 1;
        [ConfigField(text: "White Neon Deimos AR Hands To Use")]
        public int WhiteNeonDeimosArHandsToUse = 2;
        [ConfigField(text: "White Neon Deimos AR Weight")]
        public int WhiteNeonDeimosArWeight = 3;
        [ConfigField(text: "White Neon Deimos AR Stop On First Hit")]
        public bool WhiteNeonDeimosArStopOnFirstHit = false;

        /// Neon Deimos AR
        [ConfigField(text: "Neon Deimos AR Damage")]
        public float NeonDeimosArDamage = 30f;
        [ConfigField(text: "Neon Deimos AR Piercing")]
        public float NeonDeimosArPiercing = 30f;
        [ConfigField(text: "Neon Deimos AR Viral")]
        public float NeonDeimosArViral = 2f;
        [ConfigField(text: "Neon Deimos AR Poison")]
        public float NeonDeimosArPoison = 10f;
        [ConfigField(text: "Neon Deimos AR Ammo Capacity")]
        public int NeonDeimosArAmmoCapacity = 60;
        [ConfigField(text: "Neon Deimos AR Burst")]
        public int NeonDeimosArBurst = 6;
        [ConfigField(text: "Neon Deimos AR Projectiles Per Shot")]
        public int NeonDeimosArProjectilesPerShot = 1;
        [ConfigField(text: "Neon Deimos AR Effective Range")]
        public int NeonDeimosArEffectiveRange = 32;
        [ConfigField(text: "Neon Deimos AR AP Cost")]
        public int NeonDeimosArApCost = 2;
        [ConfigField(text: "Neon Deimos AR Hands To Use")]
        public int NeonDeimosArHandsToUse = 2;
        [ConfigField(text: "Neon Deimos AR Weight")]
        public int NeonDeimosArWeight = 3;
        [ConfigField(text: "Neon Deimos AR Stop On First Hit")]
        public bool NeonDeimosArStopOnFirstHit = false;

        /// Tobias West Handgun (NJ_TobiasWestGun_WeaponDef) - Always Active
        [ConfigField(text: "Tobias Handgun Damage")]
        public int TobiasHandgunDamage = 80;
        [ConfigField(text: "Tobias Handgun Piercing")]
        public float TobiasHandgunPiercing = 20f;
        [ConfigField(text: "Tobias Handgun Shred")]
        public float TobiasHandgunShred = 10f;
        [ConfigField(text: "Tobias Handgun Ammo Capacity")]
        public int TobiasHandgunAmmoCapacity = 12;
        [ConfigField(text: "Tobias Handgun Effective Range")]
        public int TobiasHandgunEffectiveRange = 20;
        [ConfigField(text: "Tobias Handgun AP Cost (1-4 points)")]
        public int TobiasHandgunAPCost = 1;

        /// Promotional Technician Arms (NJ_Technician_MechArms_ALN_WeaponDef) - VVA-2 Arms
        [ConfigField(text: "Tech Arms Base Damage")]
        public int TechArmsBaseDamage = 1;
        [ConfigField(text: "Tech Arms Piercing Damage")]
        public int TechArmsPiercingDamage = 40;
        [ConfigField(text: "Tech Arms Paralyzing Damage")]
        public int TechArmsParalyzingDamage = 30;
        [ConfigField(text: "Tech Arms Full Restore Ability")]
        public bool TechArmsFullRestoreAbility = true;

    }
}

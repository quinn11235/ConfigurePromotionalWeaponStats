using Base.Core;
using Base.Defs;
using Base.UI;
using Base.Levels;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities.Items;
using PhoenixPoint.Common.Entities.GameTags;
using PhoenixPoint.Common.Entities.GameTagsTypes;
using PhoenixPoint.Common.Game;
using PhoenixPoint.Modding;
using PhoenixPoint.Tactical.Entities.DamageKeywords;
using PhoenixPoint.Tactical.Entities.Weapons;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Common.Entities;
using Base.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HarmonyLib;

namespace ConfigurePromotionalWeaponStats
{
    /// <summary>
    /// This is the main mod class. Only one can exist per assembly.
    /// If no ModMain is detected in assembly, then no other classes/callbacks will be called.
    /// </summary>
    public class ConfigurePromotionalWeaponStats : ModMain
    {
        /// <summary>
        /// defines the modifiable values for any given weapon.
        /// </summary>
        private struct WeaponValues
        {
            public float[] Damage;
            public int AmmoCapacity;
            public int Burst;
            public int ProjectilesPerShot;
            public int EffectiveRange;
            public int ApCost;
            public int HandsToUse;
            public int Weight;
            public bool StopOnFirstHit;

            public WeaponValues(float[] damage, int ammoCapacity, int burst,
                int projectilesPerShot, int effectiveRange, int apCost,
                int handsToUse, int weight, bool stopOnFirstHit)
            {
                Damage = damage;
                AmmoCapacity = ammoCapacity;
                Burst = burst;
                ProjectilesPerShot = projectilesPerShot;
                EffectiveRange = effectiveRange;
                ApCost = apCost;
                HandsToUse = handsToUse;
                Weight = weight;
                StopOnFirstHit = stopOnFirstHit;
            }
        }

        /// Config is accessible at any time, if any is declared.
        public new ConfigurePromotionalWeaponStatsConfig Config => (ConfigurePromotionalWeaponStatsConfig)base.Config;

        /// This property indicates if mod can be Safely Disabled from the game.
        /// Safely disabled mods can be reenabled again. Unsafely disabled mods will need game restart to take effect.
        /// Unsafely disabled mods usually cannot revert their changes in OnModDisabled
        public override bool CanSafelyDisable => true;

        private WeaponDef AresGold, FirebirdGold, HelGold, FirebirdPR, WhiteNeonDeimos, NeonDeimos, TobiasHandgun, TechnicianArms;
        private ItemDef AresClip, FirebirdClip, HelClip, DeimosClip, TobiasHandgunClip, TechArmsClip;
        private WeaponValues DefaultAresGold, DefaultFirebirdGold, DefaultHelGold, DefaultFirebirdPR, DefaultWhiteNeonDeimos, DefaultNeonDeimos, DefaultTobiasHandgun;
        
        // Store original Technician Arms abilities and damage for restoration (since it has special handling)
        private AbilityDef[] DefaultTechArmsAbilities;
        private List<DamageKeywordPair> DefaultTechArmsDamageKeywords;
        private bool tobiasTagFixApplied = false;

        /// <summary>
        /// Callback for when mod is enabled. Called even on game startup.
        /// </summary>
        public override void OnModEnabled()
        {
            try
            {
                DefRepository Repo = GameUtl.GameComponent<DefRepository>();

                AresGold = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(a => a.name.Equals("PX_AssaultRifle_Gold_WeaponDef"));
                FirebirdGold = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(a => a.name.Equals("PX_SniperRifle_Gold_WeaponDef"));
                HelGold = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(a => a.name.Equals("PX_HeavyCannon_Gold_WeaponDef"));
                FirebirdPR = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(a => a.name.Equals("PX_SniperRifle_RisingSun_WeaponDef"));
                WhiteNeonDeimos = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(a => a.name.Equals("SY_LaserAssaultRifle_WhiteNeon_WeaponDef"));
                NeonDeimos = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(a => a.name.Equals("SY_LaserAssaultRifle_Neon_WeaponDef"));
                TobiasHandgun = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(a => a.name.Equals("NJ_TobiasWestGun_WeaponDef"));
                TechnicianArms = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(a => a.name.Equals("NJ_Technician_MechArms_ALN_WeaponDef"));

                AresClip = Repo.GetAllDefs<ItemDef>().FirstOrDefault(a => a.name.Equals("PX_AssaultRifle_AmmoClip_ItemDef"));
                FirebirdClip = Repo.GetAllDefs<ItemDef>().FirstOrDefault(a => a.name.Equals("PX_SniperRifle_AmmoClip_ItemDef"));
                HelClip = Repo.GetAllDefs<ItemDef>().FirstOrDefault(a => a.name.Equals("PX_HeavyCannon_AmmoClip_ItemDef"));
                DeimosClip = Repo.GetAllDefs<ItemDef>().FirstOrDefault(a => a.name.Equals("SY_LaserAssaultRifle_AmmoClip_ItemDef"));
                TobiasHandgunClip = Repo.GetAllDefs<ItemDef>().FirstOrDefault(a => a.name.Equals("NJ_Gauss_HandGun_AmmoClip_ItemDef"));
                TechArmsClip = Repo.GetAllDefs<ItemDef>().FirstOrDefault(a => a.name.Equals("MechArms_AmmoClip_ItemDef"));

                DefaultAresGold = getWeaponValuesFromWeaponDef(AresGold);
                DefaultFirebirdGold = getWeaponValuesFromWeaponDef(FirebirdGold);
                DefaultHelGold = getWeaponValuesFromWeaponDef(HelGold);
                DefaultFirebirdPR = getWeaponValuesFromWeaponDef(FirebirdPR);
                DefaultWhiteNeonDeimos = getWeaponValuesFromWeaponDef(WhiteNeonDeimos);
                DefaultNeonDeimos = getWeaponValuesFromWeaponDef(NeonDeimos);
                DefaultTobiasHandgun = getWeaponValuesFromWeaponDef(TobiasHandgun);
                
                // Store original Technician Arms abilities and damage for restoration (special handling)
                if (TechnicianArms != null)
                {
                    DefaultTechArmsAbilities = TechnicianArms.Abilities?.ToArray() ?? new AbilityDef[0];
                    DefaultTechArmsDamageKeywords = TechnicianArms.DamagePayload?.DamageKeywords?.ToList() ?? new List<DamageKeywordPair>();
                    Logger.LogInfo($"[ConfigurePromotionalWeaponStats] Stored original TechArms abilities count: {DefaultTechArmsAbilities.Length}, damage keywords: {DefaultTechArmsDamageKeywords.Count}");
                }

                OnConfigChanged();
                // Install UI damage row patch (handles piercing and fire)
                try
                {
                    var hUi = new Harmony("com.quinn11235.CPWS.UIRow");
                    CPWS_UIDamageRowPatch.Install(hUi, Logger, Config);

                    var hProbe = new Harmony("com.quinn11235.CPWS.Probe");
                    CPWS_TacticalProbe.Install(hProbe, Logger);
                }
                catch (Exception e)
                {
                    Logger.LogWarning("CPWS UI row patch failed: " + e);
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Callback for when mod is disabled. This will be called even if mod cannot be safely disabled.
        /// Guaranteed to have OnModEnabled before.
        /// </summary>
        public override void OnModDisabled()
        {
            setDefsFromWeaponValues(DefaultAresGold, AresGold, AresClip);
            setDefsFromWeaponValues(DefaultFirebirdGold, FirebirdGold, FirebirdClip);
            setDefsFromWeaponValues(DefaultHelGold, HelGold, HelClip);
            setDefsFromWeaponValues(DefaultFirebirdPR, FirebirdPR, FirebirdClip);
            setDefsFromWeaponValues(DefaultWhiteNeonDeimos, WhiteNeonDeimos, DeimosClip);
            setDefsFromWeaponValues(DefaultNeonDeimos, NeonDeimos, DeimosClip);
            // Always revert Tobias Handgun (no longer toggleable)
            setDefsFromWeaponValues(DefaultTobiasHandgun, TobiasHandgun, TobiasHandgunClip);
            // Always revert Technician Arms (special handling)
            RestoreTechnicianArms();
        }

        /// <summary>
        /// Callback for when level starts - safe time to apply tag fixes after all mods loaded
        /// </summary>
        public override void OnLevelStart(Base.Levels.Level level)
        {
            // Only apply tag fix once when first level loads (after all mods are initialized)
            if (!tobiasTagFixApplied)
            {
                ApplyTobiasHandgunTagFix();
                tobiasTagFixApplied = true;
            }
        }

        /// <summary>
        /// Callback for when any property from mod's config is changed.
        /// </summary>
        public override void OnConfigChanged()
        {
            float[] AresGoldDamage = { Config.AresGoldArDamage, Config.AresGoldArShred };
            WeaponValues AresGoldValues = new WeaponValues(
                AresGoldDamage,
                Config.AresGoldArAmmoCapacity,
                Config.AresGoldArBurst,
                Config.AresGoldArProjectilesPerShot,
                Config.AresGoldArEffectiveRange,
                Config.AresGoldArApCost,
                Config.AresGoldArHandsToUse,
                Config.AresGoldArWeight,
                Config.AresGoldArStopOnFirstHit
            );
            float[] FirebirdGoldDamage = { Config.FirebirdGoldSrDamage };
            WeaponValues FirebirdGoldValues = new WeaponValues(
                FirebirdGoldDamage,
                Config.FirebirdGoldSrAmmoCapacity,
                Config.FirebirdGoldSrBurst,
                Config.FirebirdGoldSrProjectilesPerShot,
                Config.FirebirdGoldSrEffectiveRange,
                Config.FirebirdGoldSrApCost,
                Config.FirebirdGoldSrHandsToUse,
                Config.FirebirdGoldSrWeight,
                Config.FirebirdGoldSrStopOnFirstHit
            );
            float[] HelGoldDamage = { Config.HelGoldIiDamage, Config.HelGoldIiShred, Config.HelGoldIiShock };
            WeaponValues HelGoldValues = new WeaponValues(
                HelGoldDamage,
                Config.HelGoldIiAmmoCapacity,
                Config.HelGoldIiBurst,
                Config.HelGoldIiProjectilesPerShot,
                Config.HelGoldIiEffectiveRange,
                Config.HelGoldIiApCost,
                Config.HelGoldIiHandsToUse,
                Config.HelGoldIiWeight,
                Config.HelGoldIiStopOnFirstHit
            );
            float[] FirebirdPRDamage = { Config.FirebirdPRSrDamage };
            WeaponValues FirebirdPRValues = new WeaponValues(
                FirebirdPRDamage,
                Config.FirebirdPRSrAmmoCapacity,
                Config.FirebirdPRSrBurst,
                Config.FirebirdPRSrProjectilesPerShot,
                Config.FirebirdPRSrEffectiveRange,
                Config.FirebirdPRSrApCost,
                Config.FirebirdPRSrHandsToUse,
                Config.FirebirdPRSrWeight,
                Config.FirebirdPRSrStopOnFirstHit
            );
            float[] WhiteNeonDeimosDamage = { Config.WhiteNeonDeimosArDamage };
            WeaponValues WhiteNeonDeimosValues = new WeaponValues(
                WhiteNeonDeimosDamage,
                Config.WhiteNeonDeimosArAmmoCapacity,
                Config.WhiteNeonDeimosArBurst,
                Config.WhiteNeonDeimosArProjectilesPerShot,
                Config.WhiteNeonDeimosArEffectiveRange,
                Config.WhiteNeonDeimosArApCost,
                Config.WhiteNeonDeimosArHandsToUse,
                Config.WhiteNeonDeimosArWeight,
                Config.WhiteNeonDeimosArStopOnFirstHit
            );
            setDefsFromWeaponValues(AresGoldValues, AresGold, AresClip);
            setDefsFromWeaponValues(FirebirdGoldValues, FirebirdGold, FirebirdClip);
            SetDamageKeywordValue(FirebirdGold, "pierc", Config.FirebirdGoldSrPiercing, Logger);
            setDefsFromWeaponValues(HelGoldValues, HelGold, HelClip);
            setDefsFromWeaponValues(FirebirdPRValues, FirebirdPR, FirebirdClip);
            SetDamageKeywordValue(FirebirdPR, "pierc", Config.FirebirdPRSrPiercing, Logger);
            // Setup WhiteNeonDeimos with custom damage keywords
            SetupWhiteNeonDeimosDamageKeywords(WhiteNeonDeimos, Config.WhiteNeonDeimosArDamage, Config.WhiteNeonDeimosArPiercing, Config.WhiteNeonDeimosArFire, Logger);
            SetWeaponPropertiesOnly(WhiteNeonDeimos, DeimosClip, Config.WhiteNeonDeimosArAmmoCapacity, Config.WhiteNeonDeimosArBurst, Config.WhiteNeonDeimosArProjectilesPerShot, Config.WhiteNeonDeimosArEffectiveRange, Config.WhiteNeonDeimosArApCost, Config.WhiteNeonDeimosArHandsToUse, Config.WhiteNeonDeimosArWeight, Config.WhiteNeonDeimosArStopOnFirstHit);
            
            // Setup NeonDeimos with custom damage keywords
            SetupNeonDeimosDamageKeywords(NeonDeimos, Config.NeonDeimosArDamage, Config.NeonDeimosArPiercing, Config.NeonDeimosArViral, Config.NeonDeimosArPoison, Logger);
            SetWeaponPropertiesOnly(NeonDeimos, DeimosClip, Config.NeonDeimosArAmmoCapacity, Config.NeonDeimosArBurst, Config.NeonDeimosArProjectilesPerShot, Config.NeonDeimosArEffectiveRange, Config.NeonDeimosArApCost, Config.NeonDeimosArHandsToUse, Config.NeonDeimosArWeight, Config.NeonDeimosArStopOnFirstHit);
            
            // Setup Tobias West Handgun modifications (always active)
            SetupTobiasHandgunDamageKeywords(TobiasHandgun, Config.TobiasHandgunDamage, Config.TobiasHandgunPiercing, Config.TobiasHandgunShred, Logger);
            SetWeaponPropertiesOnly(TobiasHandgun, TobiasHandgunClip, Config.TobiasHandgunAmmoCapacity, 1, 1, Config.TobiasHandgunEffectiveRange, Config.TobiasHandgunAPCost, 1, DefaultTobiasHandgun.Weight, true);
            
            // TEMPORARILY DISABLED: Test without Tobias handgun tag manipulation
            if (TobiasHandgun != null)
            {
                
                // DISABLED: Tag manipulation that might conflict with other mods
                /*DefRepository Repo = GameUtl.GameComponent<DefRepository>();
                var handgunTagDef = Repo.GetAllDefs<ItemTypeTagDef>().FirstOrDefault(tag => tag.name.Equals("HandgunItem_TagDef"));
                if (handgunTagDef != null)
                {
                    if (!TobiasHandgun.Tags.Contains(handgunTagDef))
                    {
                        TobiasHandgun.Tags.Add(handgunTagDef);
                        Logger.LogInfo($"Added HandgunItem_TagDef to Tobias West handgun for proper pistol proficiency recognition");
                    }
                    else
                    {
                        Logger.LogInfo($"Tobias West handgun already has HandgunItem_TagDef");
                    }
                }
                else
                {
                    Logger.LogWarning($"HandgunItem_TagDef not found - Tobias West handgun may not be recognized by pistol proficiency mods");
                }*/
                
                Logger.LogInfo($"Tobias Handgun weapon name: {TobiasHandgun.name}");
                Logger.LogInfo($"Tobias Handgun tags count: {TobiasHandgun.Tags?.Count ?? 0}");
                Logger.LogInfo($"CPWS: Tobias handgun tag manipulation disabled for testing mod compatibility");
                
                Logger.LogInfo($"Applied Tobias Handgun modifications: Damage={Config.TobiasHandgunDamage}, Shred={Config.TobiasHandgunShred}, Piercing={Config.TobiasHandgunPiercing}, AmmoCapacity={Config.TobiasHandgunAmmoCapacity}, EffectiveRange={Config.TobiasHandgunEffectiveRange}, APCost={Config.TobiasHandgunAPCost} points");
            }
            
            // Configure Promotional Technician Arms
            ConfigureTechnicianArms();
        }

        /// <summary>
        /// Configure Technician Arms with Paralyze damage and modified Full Restore ability
        /// </summary>
        private void ConfigureTechnicianArms()
        {
            if (TechnicianArms == null)
            {
                Logger.LogWarning("[ConfigurePromotionalWeaponStats] TechnicianArms not found");
                return;
            }

            Logger.LogInfo("[ConfigurePromotionalWeaponStats] Configuring Technician Arms...");
            
            // Replace Shock damage with Base + Piercing + Paralyze damage using CPWS existing methods
            SetupTechnicianArmsDamageKeywords(TechnicianArms, Config.TechArmsBaseDamage, Config.TechArmsPiercingDamage, Config.TechArmsParalyzingDamage, Logger);

            // Add modified Full Restore ability if enabled
            if (Config.TechArmsFullRestoreAbility)
            {
                AddModifiedFullRestoreAbility();
            }
        }

        /// <summary>
        /// Add modified Full Restore ability to Technician Arms (1 AP, 5 WP, 1 VVA-2 charge)
        /// </summary>
        private void AddModifiedFullRestoreAbility()
        {
            DefRepository repo = GameUtl.GameComponent<DefRepository>();
            
            // Find original Full Restore ability
            var originalFullRestore = repo.GetAllDefs<HealAbilityDef>().FirstOrDefault(a => a.name.Equals("SY_FullRestoration_AbilityDef"));
            if (originalFullRestore == null)
            {
                Logger.LogWarning("[ConfigurePromotionalWeaponStats] Could not find SY_FullRestoration_AbilityDef for Full Restore ability");
                return;
            }

            // Create modified Full Restore ability
            string modifiedAbilityName = "CPWS_ModifiedFullRestore_AbilityDef";
            
            // Check if already created
            var existingAbility = repo.GetAllDefs<HealAbilityDef>().FirstOrDefault(a => a.name.Equals(modifiedAbilityName));
            if (existingAbility == null)
            {
                try
                {
                    HealAbilityDef modifiedFullRestore = Helper.CreateDefFromClone(
                        originalFullRestore,
                        "CPWS-3F4E5D6A-7B8C-9D0E-AF11-2B3C4D5E6F70",
                        modifiedAbilityName);

                    // Modify the ability properties
                    modifiedFullRestore.ActionPointCost = 0.25f; // 1 AP
                    modifiedFullRestore.WillPointCost = 5; // 5 WP
                    modifiedFullRestore.ConsumedCharges = 1; // 1 VVA-2 charge
                    modifiedFullRestore.GeneralHealAmount = 400; // 400 HP healing
                    modifiedFullRestore.BodyPartHealAmount = 400; // 400 body part restoration

                    // Set display name and description
                    modifiedFullRestore.ViewElementDef.DisplayName1 = new LocalizedTextBind("ENHANCED RESTORATION", true);
                    modifiedFullRestore.ViewElementDef.Description = new LocalizedTextBind("Fully heal and repair target", true);

                    Logger.LogInfo("[ConfigurePromotionalWeaponStats] Created modified Full Restore ability for Technician Arms");
                    existingAbility = modifiedFullRestore;
                }
                catch (System.Exception e)
                {
                    Logger.LogError($"[ConfigurePromotionalWeaponStats] Error creating modified Full Restore ability: {e.Message}");
                    return;
                }
            }

            // Add the ability to Technician Arms
            var currentAbilities = TechnicianArms.Abilities?.ToList() ?? new List<AbilityDef>();
            if (!currentAbilities.Contains(existingAbility))
            {
                currentAbilities.Add(existingAbility);
                TechnicianArms.Abilities = currentAbilities.ToArray();
                Logger.LogInfo("[ConfigurePromotionalWeaponStats] Added modified Full Restore ability to Technician Arms");
            }

            // Register animations for the new ability so it doesn't crash on use
            try
            {
                // Add to actor interaction animations (e.g., E_TechnicianHeal [Soldier_Utka_AnimActionsDef])
                var actorInteractAnims = repo
                    .GetAllDefs<TacActorSimpleInteractionAnimActionDef>()
                    .Where(aad => aad != null && aad.name != null && aad.name.Contains("Soldier_Utka_AnimActionsDef"))
                    .ToList();

                // Reference abilities commonly present in the heal/restore interaction sets
                var techHeal = repo.GetAllDefs<HealAbilityDef>().FirstOrDefault(a => a.name == "TechnicianHeal_AbilityDef");
                var techRestore = repo.GetAllDefs<HealAbilityDef>().FirstOrDefault(a => a.name == "TechnicianRestoreBodyPart_AbilityDef");

                foreach (var anim in actorInteractAnims)
                {
                    try
                    {
                        if (anim.Abilities == null) continue;
                        bool isHealSet = anim.Abilities.Contains(techHeal) || anim.Abilities.Contains(techRestore) || anim.Abilities.Contains(originalFullRestore);
                        if (isHealSet && !anim.Abilities.Contains((TacticalAbilityDef)existingAbility))
                        {
                            anim.Abilities = anim.Abilities.Append((TacticalAbilityDef)existingAbility).ToArray();
                        }
                    }
                    catch { /* continue */ }
                }

                // Add to item interaction animations used by Technician Arms
                var itemInteractAnims = repo.GetAllDefs<TacItemSimpleInteractionAnimActionDef>()?.ToList();
                if (itemInteractAnims != null)
                {
                    foreach (var anim in itemInteractAnims)
                    {
                        try
                        {
                            if (anim.Abilities == null) continue;
                            bool relatesToArmsHeal = anim.Abilities.Contains(techHeal) || anim.Abilities.Contains(techRestore) || anim.Abilities.Contains(originalFullRestore);
                            if (relatesToArmsHeal && !anim.Abilities.Contains((TacticalAbilityDef)existingAbility))
                            {
                                anim.Abilities = anim.Abilities.Append((TacticalAbilityDef)existingAbility).ToArray();
                            }
                        }
                        catch { /* continue */ }
                    }
                }

                Logger.LogInfo("[ConfigurePromotionalWeaponStats] Registered modified Full Restore ability with Technician heal/restore animation sets");
            }
            catch (System.Exception e)
            {
                Logger.LogWarning($"[ConfigurePromotionalWeaponStats] Failed to register animations for modified Full Restore: {e.Message}");
            }
        }

        /// <summary>
        /// Restore original damage configuration and abilities for Technician Arms
        /// </summary>
        private void RestoreTechnicianArms()
        {
            if (TechnicianArms == null)
                return;
                
            // Restore original abilities
            if (DefaultTechArmsAbilities != null)
            {
                TechnicianArms.Abilities = DefaultTechArmsAbilities;
            }
            
            // Restore original damage configuration
            if (DefaultTechArmsDamageKeywords != null && DefaultTechArmsDamageKeywords.Count > 0)
            {
                TechnicianArms.DamagePayload.DamageKeywords = DefaultTechArmsDamageKeywords;
            }
            
            Logger.LogInfo("[ConfigurePromotionalWeaponStats] Restored original Technician Arms configuration");
        }

        /* CALCULATION FUNCTIONS */


        private static int CalculateAPToUsePerc(int ApCost)
        {
            return ApCost * 25;
        }
        private static int CalculateApCost(int APToUsePerc)
        {
            return APToUsePerc / 25;
        }

        /* WEAPON DATA FUNCTIONS */
        private WeaponValues getWeaponValuesFromWeaponDef(WeaponDef weaponDef)
        {
            float[] Damage = new float[weaponDef.DamagePayload.DamageKeywords.Count];
            for (int i = 0; i < weaponDef.DamagePayload.DamageKeywords.Count; i++)
            {
                Damage[i] = weaponDef.DamagePayload.DamageKeywords[i].Value;
            }
            return new WeaponValues(
                Damage,
                weaponDef.ChargesMax,
                weaponDef.DamagePayload.AutoFireShotCount,
                weaponDef.DamagePayload.ProjectilesPerShot,
                weaponDef.EffectiveRange,
                CalculateApCost(weaponDef.APToUsePerc),
                weaponDef.HandsToUse,
                weaponDef.Weight,
                weaponDef.DamagePayload.StopOnFirstHit
            );
        }
        private void setDefsFromWeaponValues(WeaponValues weaponValues, WeaponDef weaponDef, ItemDef itemDef)
        {
            if (weaponDef == null) return;

            weaponDef.APToUsePerc = CalculateAPToUsePerc(weaponValues.ApCost);
            weaponDef.DamagePayload.AutoFireShotCount = weaponValues.Burst;
            for (int i = 0; i < weaponValues.Damage.Length; i++)
            {
                weaponDef.DamagePayload.DamageKeywords[i].Value = weaponValues.Damage[i];
            }
            weaponDef.DamagePayload.ProjectilesPerShot = weaponValues.ProjectilesPerShot;
            weaponDef.DamagePayload.StopOnFirstHit = weaponValues.StopOnFirstHit;

            // Keep the ballistic calculation in sync
            weaponDef.SpreadDegrees = ConfigurePromotionalWeaponStatsHelper.CalculateSpreadDegreesFromRange(weaponValues.EffectiveRange);
            // Nudge all the UI-facing places that cache or read EffectiveRange
            ConfigurePromotionalWeaponStatsHelper.PushEffectiveRangeToUI(weaponDef, weaponValues.EffectiveRange);

            weaponDef.HandsToUse = weaponValues.HandsToUse;
            weaponDef.ChargesMax = weaponValues.AmmoCapacity;
            if (itemDef != null) itemDef.ChargesMax = weaponValues.AmmoCapacity;
            weaponDef.Weight = weaponValues.Weight;
        }

        private static void SetupWhiteNeonDeimosDamageKeywords(WeaponDef weaponDef, float damageValue, float piercingValue, float fireValue, ModLogger logger)
        {
            try
            {
                if (weaponDef == null || weaponDef.DamagePayload == null) return;

                // Set base damage first
                if (weaponDef.DamagePayload.DamageKeywords != null && weaponDef.DamagePayload.DamageKeywords.Count > 0)
                {
                    weaponDef.DamagePayload.DamageKeywords[0].Value = damageValue;
                }

                // Remove shred damage (replace with piercing)
                RemoveDamageKeyword(weaponDef, "shred", logger);
                
                // Set or add new damage types
                SetDamageKeywordValue(weaponDef, "pierc", piercingValue, logger);
                SetDamageKeywordValue(weaponDef, "fire", fireValue, logger);
                SetDamageKeywordValue(weaponDef, "burn", fireValue, logger);

                logger?.LogInfo($"Set up damage keywords for {weaponDef.name}: Damage={damageValue}, Piercing={piercingValue}, Fire={fireValue}");
            }
            catch (System.Exception e)
            {
                logger?.LogWarning("SetupWhiteNeonDeimosDamageKeywords failed: " + e);
            }
        }

        private static void SetupNeonDeimosDamageKeywords(WeaponDef weaponDef, float damageValue, float piercingValue, float viralValue, float poisonValue, ModLogger logger)
        {
            try
            {
                if (weaponDef == null || weaponDef.DamagePayload == null) return;

                // Set base damage first
                if (weaponDef.DamagePayload.DamageKeywords != null && weaponDef.DamagePayload.DamageKeywords.Count > 0)
                {
                    weaponDef.DamagePayload.DamageKeywords[0].Value = damageValue;
                }

                // Remove shred damage (replace with piercing/viral/poison)
                RemoveDamageKeyword(weaponDef, "shred", logger);
                
                // Set or add new damage types
                SetDamageKeywordValue(weaponDef, "pierc", piercingValue, logger);
                SetDamageKeywordValue(weaponDef, "viral", viralValue, logger);
                SetDamageKeywordValue(weaponDef, "poison", poisonValue, logger);
                SetDamageKeywordValue(weaponDef, "toxic", poisonValue, logger);

                logger?.LogInfo($"Set up damage keywords for {weaponDef.name}: Damage={damageValue}, Piercing={piercingValue}, Viral={viralValue}, Poison={poisonValue}");
            }
            catch (System.Exception e)
            {
                logger?.LogWarning("SetupNeonDeimosDamageKeywords failed: " + e);
            }
        }

        private static void SetupTechnicianArmsDamageKeywords(WeaponDef weaponDef, float baseDamage, float piercingDamage, float paralyzingDamage, ModLogger logger)
        {
            try
            {
                if (weaponDef == null || weaponDef.DamagePayload == null) return;

                // Set base damage first
                if (weaponDef.DamagePayload.DamageKeywords != null && weaponDef.DamagePayload.DamageKeywords.Count > 0)
                {
                    weaponDef.DamagePayload.DamageKeywords[0].Value = baseDamage;
                }

                // Remove shock damage (replace with piercing/paralyzing)
                RemoveDamageKeyword(weaponDef, "shock", logger);
                
                // Set or add new damage types
                SetDamageKeywordValue(weaponDef, "pierc", piercingDamage, logger);
                SetDamageKeywordValue(weaponDef, "paralysing", paralyzingDamage, logger);

                logger?.LogInfo($"Set up damage keywords for {weaponDef.name}: Base={baseDamage}, Piercing={piercingDamage}, Paralyzing={paralyzingDamage}");
            }
            catch (System.Exception e)
            {
                logger?.LogWarning("SetupTechnicianArmsDamageKeywords failed: " + e);
            }
        }

        private static void SetupTobiasHandgunDamageKeywords(WeaponDef weaponDef, float baseDamage, float piercingDamage, float shredDamage, ModLogger logger)
        {
            try
            {
                if (weaponDef == null || weaponDef.DamagePayload == null) return;

                // Set base damage first
                if (weaponDef.DamagePayload.DamageKeywords != null && weaponDef.DamagePayload.DamageKeywords.Count > 0)
                {
                    weaponDef.DamagePayload.DamageKeywords[0].Value = baseDamage;
                }

                // Set or add damage types
                SetDamageKeywordValue(weaponDef, "pierc", piercingDamage, logger);
                SetDamageKeywordValue(weaponDef, "shred", shredDamage, logger);

                logger?.LogInfo($"Set up damage keywords for {weaponDef.name}: Base={baseDamage}, Piercing={piercingDamage}, Shred={shredDamage}");
            }
            catch (System.Exception e)
            {
                logger?.LogWarning("SetupTobiasHandgunDamageKeywords failed: " + e);
            }
        }

        private static void SetWeaponPropertiesOnly(WeaponDef weaponDef, ItemDef itemDef, int ammoCapacity, int burst, int projectilesPerShot, int effectiveRange, int apCost, int handsToUse, int weight, bool stopOnFirstHit)
        {
            if (weaponDef == null) return;

            weaponDef.APToUsePerc = CalculateAPToUsePerc(apCost);
            weaponDef.DamagePayload.AutoFireShotCount = burst;
            weaponDef.DamagePayload.ProjectilesPerShot = projectilesPerShot;
            weaponDef.DamagePayload.StopOnFirstHit = stopOnFirstHit;

            // Keep the ballistic calculation in sync
            weaponDef.SpreadDegrees = ConfigurePromotionalWeaponStatsHelper.CalculateSpreadDegreesFromRange(effectiveRange);
            // Nudge all the UI-facing places that cache or read EffectiveRange
            ConfigurePromotionalWeaponStatsHelper.PushEffectiveRangeToUI(weaponDef, effectiveRange);

            weaponDef.HandsToUse = handsToUse;
            weaponDef.ChargesMax = ammoCapacity;
            if (itemDef != null) itemDef.ChargesMax = ammoCapacity;
            weaponDef.Weight = weight;
        }

        private static void RemoveDamageKeyword(WeaponDef weaponDef, string keywordName, ModLogger logger)
        {
            try
            {
                if (weaponDef == null || weaponDef.DamagePayload == null || weaponDef.DamagePayload.DamageKeywords == null) return;
                
                var keywordList = weaponDef.DamagePayload.DamageKeywords.ToList();
                bool removed = false;
                
                for (int i = keywordList.Count - 1; i >= 0; i--)
                {
                    var dk = keywordList[i];
                    try
                    {
                        var def = dk.DamageKeywordDef;
                        if (def == null) continue;
                        string n = def.name ?? def.ToString();
                        if (!string.IsNullOrEmpty(n) && n.IndexOf(keywordName, System.StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            keywordList.RemoveAt(i);
                            removed = true;
                            logger?.LogInfo($"Removed {keywordName} damage from {weaponDef.name}");
                        }
                    }
                    catch { /* continue */ }
                }
                
                if (removed)
                {
                    weaponDef.DamagePayload.DamageKeywords = keywordList;
                }
            }
            catch (System.Exception e)
            {
                logger?.LogWarning($"RemoveDamageKeyword failed for {keywordName}: " + e);
            }
        }

        private static void SetDamageKeywordValue(WeaponDef weaponDef, string keywordName, float value, ModLogger logger)
        {
            try
            {
                if (weaponDef == null || weaponDef.DamagePayload == null || weaponDef.DamagePayload.DamageKeywords == null) return;
                if (value <= 0f) return; // Don't add zero values
                
                foreach (var dk in weaponDef.DamagePayload.DamageKeywords)
                {
                    try
                    {
                        var def = dk.DamageKeywordDef;
                        if (def == null) continue;
                        string n = def.name ?? def.ToString();
                        if (!string.IsNullOrEmpty(n) && n.IndexOf(keywordName, System.StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            dk.Value = value;
                            logger?.LogInfo($"Set {keywordName} damage on {weaponDef.name} -> {value}");
                            return;
                        }
                    }
                    catch { /* continue */ }
                }
                
                // If we didn't find an existing keyword, try to add one from repository
                try
                {
                    var repo = GameUtl.GameComponent<DefRepository>();
                    var damageKeyword = repo.GetAllDefs<DamageKeywordDef>()
                        .FirstOrDefault(d => d.name != null && 
                            d.name.IndexOf(keywordName, System.StringComparison.OrdinalIgnoreCase) >= 0);
                    
                    if (damageKeyword != null)
                    {
                        var newKeywordPair = new DamageKeywordPair()
                        {
                            DamageKeywordDef = damageKeyword,
                            Value = value
                        };
                        
                        var keywordList = weaponDef.DamagePayload.DamageKeywords.ToList();
                        keywordList.Add(newKeywordPair);
                        weaponDef.DamagePayload.DamageKeywords = keywordList;
                        
                        logger?.LogInfo($"Added {keywordName} damage to {weaponDef.name} -> {value}");
                    }
                }
                catch (System.Exception e)
                {
                    logger?.LogWarning($"Failed to add {keywordName} damage keyword: " + e.Message);
                }
            }
            catch (System.Exception e)
            {
                logger?.LogWarning($"SetDamageKeywordValue failed for {keywordName}: " + e);
            }
        }

        private static void SetPiercing(WeaponDef weaponDef, float value, ModLogger logger)
        {
            try
            {
                if (weaponDef == null || weaponDef.DamagePayload == null || weaponDef.DamagePayload.DamageKeywords == null) return;
                foreach (var dk in weaponDef.DamagePayload.DamageKeywords)
                {
                    try
                    {
                        var def = dk.DamageKeywordDef;
                        if (def == null) continue;
                        // match by name contains "Pierc" to be robust across builds
                        string n = def.name ?? def.ToString();
                        if (!string.IsNullOrEmpty(n) && n.IndexOf("pierc", System.StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            dk.Value = value;
                            logger?.LogInfo($"Set Piercing on {weaponDef.name} -> {value}");
                            break;
                        }
                    }
                    catch { /* continue */ }
                }
            }
            catch (System.Exception e)
            {
                logger?.LogWarning("SetPiercing failed: " + e);
            }
        }

        private void ApplyTobiasHandgunTagFix()
        {
            try
            {
                if (TobiasHandgun != null)
                {
                    // Apply tag fix for pistol proficiency recognition - now safe after all mods loaded
                    DefRepository Repo = GameUtl.GameComponent<DefRepository>();
                    var handgunTagDef = Repo.GetAllDefs<ItemTypeTagDef>().FirstOrDefault(tag => tag.name.Equals("HandgunItem_TagDef"));
                    if (handgunTagDef != null)
                    {
                        if (!TobiasHandgun.Tags.Contains(handgunTagDef))
                        {
                            TobiasHandgun.Tags.Add(handgunTagDef);
                            Logger.LogInfo($"[CPWS] Added HandgunItem_TagDef to Tobias West handgun for proper pistol proficiency recognition");
                        }
                        else
                        {
                            Logger.LogInfo($"[CPWS] Tobias West handgun already has HandgunItem_TagDef");
                        }
                    }
                    else
                    {
                        Logger.LogWarning($"[CPWS] HandgunItem_TagDef not found - Tobias West handgun may not be recognized by pistol proficiency mods");
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"[CPWS] ApplyTobiasHandgunTagFix failed: {e.Message}");
            }
        }
    
    }

}

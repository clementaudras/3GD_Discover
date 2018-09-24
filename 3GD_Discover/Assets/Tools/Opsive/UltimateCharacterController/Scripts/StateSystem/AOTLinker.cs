/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

#if UNITY_WEBGL || UNITY_IOS || UNITY_ANDROID || UNITY_WII || UNITY_PS3 || UNITY_PS4 || UNITY_XBOXONE || UNITY_WSA
using UnityEngine;

namespace Opsive.UltimateCharacterController.StateSystem
{
    // This class is required in order for the preset system to work with AOT platforms. The preset system uses reflection to generate the delegates
    // and reflection doesn't play well with AOT because the classes need to be defined ahead of time. Define the classes here so the compiler will
    // add in the correct type. This code is not actually used anywhere, it is purely for the compiler.
    public class AOTLinker : MonoBehaviour
    {
        public void Linker()
        {
#pragma warning disable 0219
            var genericIntDelegate = new Preset.GenericDelegate<int>();
            var genericFloatDelegate = new Preset.GenericDelegate<float>();
            var genericUnsignedIntDelegate = new Preset.GenericDelegate<uint>();
            var genericUnsignedDoubleDelegate = new Preset.GenericDelegate<double>();
            var genericUnsignedLongDelegate = new Preset.GenericDelegate<long>();
            var genericBoolDelegate = new Preset.GenericDelegate<bool>();
            var genericStringDelegate = new Preset.GenericDelegate<string>();
            var genericByteDelegate = new Preset.GenericDelegate<byte>();
            var genericVector2Delegate = new Preset.GenericDelegate<Vector2>();
            var genericVector3Delegate = new Preset.GenericDelegate<Vector3>();
            var genericVector4Delegate = new Preset.GenericDelegate<Vector4>();
            var genericQuaternionDelegate = new Preset.GenericDelegate<Quaternion>();
            var genericColorDelegate = new Preset.GenericDelegate<Color>();
            var genericRectDelegate = new Preset.GenericDelegate<Rect>();
            var genericMatrix4x4Delegate = new Preset.GenericDelegate<Matrix4x4>();
            var genericAnimationCurveDelegate = new Preset.GenericDelegate<AnimationCurve>();
            var genericLayerMaskDelegate = new Preset.GenericDelegate<LayerMask>();
            var genericObjectDelegate = new Preset.GenericDelegate<Object>();
            var genericGameObjectDelegate = new Preset.GenericDelegate<GameObject>();
            var genericTransformDelegate = new Preset.GenericDelegate<Transform>();
            var genericMinMaxFloatDelegate = new Preset.GenericDelegate<Utility.MinMaxFloat>();
            var genericMinMaxVector3Delegate = new Preset.GenericDelegate<Utility.MinMaxVector3>();
            var genericLookVectorModeDelegate = new Preset.GenericDelegate<Input.PlayerInput.LookVectorMode>();
            var genericPreloadedPrefabDelegate = new Preset.GenericDelegate<Game.ObjectPool.PreloadedPrefab>();
            var genericAbilityStartTypeDelegate = new Preset.GenericDelegate<Character.Abilities.Ability.AbilityStartType>();
            var genericAbilityStopTypeDelegate = new Preset.GenericDelegate<Character.Abilities.Ability.AbilityStopType>();
            var genericAbilityBoolOverrideDelegate = new Preset.GenericDelegate<Character.Abilities.Ability.AbilityBoolOverride>();
            var genericRestrictionTypeDelegate = new Preset.GenericDelegate<Character.Abilities.RestrictPosition.RestrictionType>();
            var genericObjectDetectionModeDelegate = new Preset.GenericDelegate<Character.Abilities.DetectObjectAbilityBase.ObjectDetectionMode>();
            var genericAutoEquipTypeDelegate = new Preset.GenericDelegate<Character.Abilities.Items.EquipUnequip.AutoEquipType>();
            var genericAttributeAutoUpdateValueTypeDelegate = new Preset.GenericDelegate<Traits.Attribute.AutoUpdateValue>();
            var genericSurfaceImpactDelegate = new Preset.GenericDelegate<SurfaceSystem.SurfaceImpact>();
            var genericUVTextureDelegate = new Preset.GenericDelegate<SurfaceSystem.UVTexture>();
            var genericObjectSurfaceDelegate = new Preset.GenericDelegate<SurfaceSystem.ObjectSurface>();
            var genericObjectSpawnInfoDelegate = new Preset.GenericDelegate<Utility.ObjectSpawnInfo>();
            var genericAnimationEventTriggerDelegate = new Preset.GenericDelegate<Utility.AnimationEventTrigger>();
            var genericFootDelegate = new Preset.GenericDelegate<Character.CharacterFootEffects.Foot>();
            var genericFootstepPlacementModeDelegate = new Preset.GenericDelegate<Character.CharacterFootEffects.FootstepPlacementMode>();
            var genericSpawnPointSpawnShapeDelegate = new Preset.GenericDelegate<Game.SpawnPoint.SpawnShape>();
            var genericRespawnerSpawnPositioningModeDelegate = new Preset.GenericDelegate<Traits.Respawner.SpawnPositioningMode>();
            var genericMovingPlatformWaypointDelegate = new Preset.GenericDelegate<Objects.MovingPlatform.Waypoint>();
            var genericMovingPlatformPathMovementTypeDelegate = new Preset.GenericDelegate<Objects.MovingPlatform.PathMovementType>();
            var genericMovingPlatformPathDirectionDelegate = new Preset.GenericDelegate<Objects.MovingPlatform.PathDirection>();
            var genericMovingPlatformMovementInterpolationModeDelegate = new Preset.GenericDelegate<Objects.MovingPlatform.MovementInterpolationMode>();
            var genericMovingPlatformRotateInterpolationModeDelegate = new Preset.GenericDelegate<Objects.MovingPlatform.RotateInterpolationMode>();
            var genericAudioClipSetDelegate = new Preset.GenericDelegate<Audio.AudioClipSet>();
#if ULTIMATE_CHARACTER_CONTROLLER_SHOOTER
            var genericAutoReloadTypeDelegate = new Preset.GenericDelegate<Character.Abilities.Items.Reload.AutoReloadType>();
            var genericShootableWeaponFireModeDelegate = new Preset.GenericDelegate<Items.Actions.ShootableWeapon.FireMode>();
            var genericShootableWeaponFireTypeDelegate = new Preset.GenericDelegate<Items.Actions.ShootableWeapon.FireType>();
            var genericShootableWeaponProjectileVisibilityDelegate = new Preset.GenericDelegate<Items.Actions.ShootableWeapon.ProjectileVisiblityType>();
            var genericShootableWeaponReloadClipTypeDelegate = new Preset.GenericDelegate<Items.Actions.ShootableWeapon.ReloadClipType>();
#endif
#if ULTIMATE_CHARACTER_CONTROLLER_MELEE
            var genericMeleeWeaponTrailVisibilityDelegate = new Preset.GenericDelegate<Items.Actions.MeleeWeapon.TrailVisibilityType>();
#endif
            var genericHealthFlashMonitorFlashDelegate = new Preset.GenericDelegate<UI.HealthFlashMonitor.Flash>();
#pragma warning restore 0219
        }
    }
}
#endif
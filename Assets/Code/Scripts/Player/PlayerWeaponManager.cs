using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RuckusReloaded.Runtime.Player
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerWeaponManager : MonoBehaviour
    {
        public string[] equippedWeapons;
        public int maxEquippedWeapons = 10;

        private PlayerController player;
        private int currentWeaponIndex;
        private List<PlayerWeapon> registeredWeapons = new();

        public PlayerWeapon CurrentWeapon => currentWeaponIndex >= 0 && currentWeaponIndex < registeredWeapons.Count ? registeredWeapons[currentWeaponIndex] : null;

        private void Awake()
        {
            player = GetComponent<PlayerController>();

            var parent = transform.Find("View/Weapons");
            foreach (Transform e in parent)
            {
                var weapon = e.GetComponent<PlayerWeapon>();
                if (!weapon) continue;
                registeredWeapons.Add(weapon);
                weapon.gameObject.SetActive(false);
            }

            var inputAsset = player.inputAsset;
            for (var i = 0; i < maxEquippedWeapons; i++)
            {
                var action = inputAsset.FindAction($"Weapon.{i + 1}");
                if (action == null) continue;
                action.performed += SwitchWeaponInputCallback(i);
            }

            EquipWeapons(0);
        }

        private Action<InputAction.CallbackContext> SwitchWeaponInputCallback(int i) => _ => EquipWeapons(i);

        private int NameToIndex(string name)
        {
            for (var i = 0; i < registeredWeapons.Count; i++)
            {
                var other = registeredWeapons[i].name;
                if (other != name) continue;
                return i;
            }

            return -1;
        }

        private void EquipWeapons(int i)
        {
            if (CurrentWeapon) CurrentWeapon.gameObject.SetActive(false);
            currentWeaponIndex = i >= 0 && i < equippedWeapons.Length ? NameToIndex(equippedWeapons[i]) : -1;
            if (CurrentWeapon) CurrentWeapon.gameObject.SetActive(true);
        }
    }
}
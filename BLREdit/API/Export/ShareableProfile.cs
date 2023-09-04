﻿using BLREdit.Export;
using BLREdit.Import;
using BLREdit.UI.Views;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace BLREdit.API.Export;

public sealed class ShareableProfile : INotifyPropertyChanged, IBLRProfile
{
    #region Events
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion Events

    [JsonIgnore] private string name = "New Profile";
    public string Name { get { return name; } set { name = value; OnPropertyChanged(); } }
    public ObservableCollection<ShareableLoadout> Loadouts { get; set; } = new() { MagiCowsLoadout.DefaultLoadout1.ConvertToShareable(), MagiCowsLoadout.DefaultLoadout2.ConvertToShareable(), MagiCowsLoadout.DefaultLoadout3.ConvertToShareable() };

    public void RefreshInfo()
    {
        OnPropertyChanged(nameof(Name));
    }

    public BLRProfile ToBLRProfile()
    {
        var profile = new BLRProfile
        {
            Loadout1 = Loadouts[0].ToBLRLoadout(),
            Loadout2 = Loadouts[1].ToBLRLoadout(),
            Loadout3 = Loadouts[2].ToBLRLoadout()
        };
        return profile;
    }

    public ShareableProfile Clone()
    {
        var dup = new ShareableProfile()
        {
            Name = Name,
        };
        foreach (var loadout in Loadouts)
        {
            dup.Loadouts.Add(loadout.Clone());
        }
        return dup;
    }

    public ShareableProfile Duplicate()
    {
        var dup = this.Clone();
        ExportSystem.Profiles.Add(dup);
        return dup;
    }

    public IBLRLoadout GetLoadout(int index)
    {
        if (Loadouts.Count > index)
        { return Loadouts[index]; }
        else
        { return Loadouts[0]; }
    }

    public void Read(BLRProfile profile)
    {
        Loadouts[0].Read(profile.Loadout1);
        Loadouts[1].Read(profile.Loadout2);
        Loadouts[2].Read(profile.Loadout3);
    }

    public void Write(BLRProfile profile)
    {
        Loadouts[0].Write(profile.Loadout1);
        Loadouts[1].Write(profile.Loadout2);
        Loadouts[2].Write(profile.Loadout3);
    }
}

public sealed class Shareable3LoadoutSet : IBLRProfile
{
    [JsonPropertyName("L1")] public ShareableLoadout Loadout1 { get; set; } = new();
    [JsonPropertyName("L2")] public ShareableLoadout Loadout2 { get; set; } = new();
    [JsonPropertyName("L3")] public ShareableLoadout Loadout3 { get; set; } = new();

    public Shareable3LoadoutSet() { }
    public Shareable3LoadoutSet(BLRProfile profile)
    { 
        Loadout1 = new ShareableLoadout(profile.Loadout1);
        Loadout2 = new ShareableLoadout(profile.Loadout2);
        Loadout3 = new ShareableLoadout(profile.Loadout3);
    }

    public BLRProfile ToBLRProfile()
    {
        var profile = new BLRProfile
        {
            Loadout1 = Loadout1.ToBLRLoadout(),
            Loadout2 = Loadout2.ToBLRLoadout(),
            Loadout3 = Loadout3.ToBLRLoadout()
        };
        return profile;
    }

    public IBLRLoadout GetLoadout(int index)
    {
        return index switch
        {
            1 => Loadout2,
            2 => Loadout3,
            _ => Loadout1,
        };
    }

    public void Read(BLRProfile profile)
    {
        Loadout1.Read(profile.Loadout1);
        Loadout2.Read(profile.Loadout2);
        Loadout3.Read(profile.Loadout3);
    }

    public void Write(BLRProfile profile)
    {
        Loadout1.Write(profile.Loadout1);
        Loadout2.Write(profile.Loadout2);
        Loadout3.Write(profile.Loadout3);
    }
}

public sealed class ShareableLoadout : IBLRLoadout
{
    [JsonPropertyName("R1")] public ShareableWeapon Primary { get; set; } = new();
    [JsonPropertyName("R2")] public ShareableWeapon Secondary { get; set; } = new();
    [JsonPropertyName("F1")] public bool Female { get; set; } = false;
    [JsonPropertyName("B1")] public bool Bot { get; set; } = false;
    [JsonPropertyName("A1")] public int Avatar { get; set; } = 99;
    [JsonPropertyName("B2")] public int BodyCamo { get; set; } = 0;
    [JsonPropertyName("B3")] public int Badge { get; set; } = 0;
    [JsonPropertyName("B4")] public int ButtPack { get; set; } = 0;
    [JsonPropertyName("D1")] public int[] Depot { get; set; } = new int[5];
    [JsonPropertyName("U1")] public int UpperBody { get; set; } = 0;
    [JsonPropertyName("L1")] public int LowerBody { get; set; } = 0;
    [JsonPropertyName("H1")] public int Helmet { get; set; } = 0;
    [JsonPropertyName("H2")] public int Hanger { get; set; } = 0;

    [JsonPropertyName("G1")] public int Gear_R1 { get; set; } = 0;
    [JsonPropertyName("G2")] public int Gear_R2 { get; set; } = 0;
    [JsonPropertyName("G3")] public int Gear_L1 { get; set; } = 0;
    [JsonPropertyName("G4")] public int Gear_L2 { get; set; } = 0;

    [JsonPropertyName("P1")] public int PatchIcon { get; set; } = 0;
    [JsonPropertyName("P2")] public int PatchIconColor { get; set; } = 0;
    [JsonPropertyName("P3")] public int PatchShape { get; set; } = 0;
    [JsonPropertyName("P4")] public int PatchShapeColor { get; set; } = 0;
    [JsonPropertyName("T1")] public int Tactical { get; set; } = 0;
    [JsonPropertyName("T2")] public int[] Taunts { get; set; } = new int[8];

    public ShareableLoadout()
    { }

    public ShareableLoadout(BLRLoadout loadout)
    {
        Female = loadout.IsFemale;
        BodyCamo = BLRItem.GetMagicCowsID(loadout.BodyCamo);
        UpperBody = BLRItem.GetMagicCowsID(loadout.UpperBody);
        LowerBody = BLRItem.GetMagicCowsID(loadout.LowerBody);
        Helmet = BLRItem.GetMagicCowsID(loadout.Helmet);
        Tactical = BLRItem.GetMagicCowsID(loadout.Tactical);
        Badge = BLRItem.GetMagicCowsID(loadout.Trophy);

        Avatar = BLRItem.GetMagicCowsID(loadout.Avatar, 99);

        Gear_R1 = BLRItem.GetMagicCowsID(loadout.Gear1);
        Gear_R2 = BLRItem.GetMagicCowsID(loadout.Gear2);
        Gear_L1 = BLRItem.GetMagicCowsID(loadout.Gear3);
        Gear_L2 = BLRItem.GetMagicCowsID(loadout.Gear4);

        Taunts[0] = BLRItem.GetMagicCowsID(loadout.Taunt1);
        Taunts[1] = BLRItem.GetMagicCowsID(loadout.Taunt2);
        Taunts[2] = BLRItem.GetMagicCowsID(loadout.Taunt3);
        Taunts[3] = BLRItem.GetMagicCowsID(loadout.Taunt4);
        Taunts[4] = BLRItem.GetMagicCowsID(loadout.Taunt5);
        Taunts[5] = BLRItem.GetMagicCowsID(loadout.Taunt6);
        Taunts[6] = BLRItem.GetMagicCowsID(loadout.Taunt7);
        Taunts[7] = BLRItem.GetMagicCowsID(loadout.Taunt8);

        Depot[0] = BLRItem.GetMagicCowsID(loadout.Depot1);
        Depot[1] = BLRItem.GetMagicCowsID(loadout.Depot2);
        Depot[2] = BLRItem.GetMagicCowsID(loadout.Depot3);
        Depot[3] = BLRItem.GetMagicCowsID(loadout.Depot4);
        Depot[4] = BLRItem.GetMagicCowsID(loadout.Depot5);

        Primary = new(loadout.Primary);
        Secondary = new(loadout.Secondary);
    }

    public BLRLoadout ToBLRLoadout()
    {
        var loadout = new BLRLoadout
        {
            IsFemale = Female,
            IsBot = Bot,

            Avatar = ImportSystem.GetItemByIDAndType(ImportSystem.AVATARS_CATEGORY, Avatar),

            BodyCamo = ImportSystem.GetItemByIDAndType(ImportSystem.CAMOS_BODIES_CATEGORY, BodyCamo),

            Depot1 = ImportSystem.GetItemByIDAndType(ImportSystem.SHOP_CATEGORY, Depot[0]),
            Depot2 = ImportSystem.GetItemByIDAndType(ImportSystem.SHOP_CATEGORY, Depot[1]),
            Depot3 = ImportSystem.GetItemByIDAndType(ImportSystem.SHOP_CATEGORY, Depot[2]),
            Depot4 = ImportSystem.GetItemByIDAndType(ImportSystem.SHOP_CATEGORY, Depot[3]),
            Depot5 = ImportSystem.GetItemByIDAndType(ImportSystem.SHOP_CATEGORY, Depot[4]),

            Gear1 = ImportSystem.GetItemByIDAndType(ImportSystem.ATTACHMENTS_CATEGORY, Gear_R1),
            Gear2 = ImportSystem.GetItemByIDAndType(ImportSystem.ATTACHMENTS_CATEGORY, Gear_R2),
            Gear3 = ImportSystem.GetItemByIDAndType(ImportSystem.ATTACHMENTS_CATEGORY, Gear_L1),
            Gear4 = ImportSystem.GetItemByIDAndType(ImportSystem.ATTACHMENTS_CATEGORY, Gear_L2),

            Helmet = ImportSystem.GetItemByIDAndType(ImportSystem.HELMETS_CATEGORY, Helmet),
            LowerBody = ImportSystem.GetItemByIDAndType(ImportSystem.LOWER_BODIES_CATEGORY, LowerBody),

            Tactical = ImportSystem.GetItemByIDAndType(ImportSystem.TACTICAL_CATEGORY, Tactical),

            Taunt1 = ImportSystem.GetItemByIDAndType(ImportSystem.EMOTES_CATEGORY, Taunts[0]),
            Taunt2 = ImportSystem.GetItemByIDAndType(ImportSystem.EMOTES_CATEGORY, Taunts[1]),
            Taunt3 = ImportSystem.GetItemByIDAndType(ImportSystem.EMOTES_CATEGORY, Taunts[2]),
            Taunt4 = ImportSystem.GetItemByIDAndType(ImportSystem.EMOTES_CATEGORY, Taunts[3]),
            Taunt5 = ImportSystem.GetItemByIDAndType(ImportSystem.EMOTES_CATEGORY, Taunts[4]),
            Taunt6 = ImportSystem.GetItemByIDAndType(ImportSystem.EMOTES_CATEGORY, Taunts[5]),
            Taunt7 = ImportSystem.GetItemByIDAndType(ImportSystem.EMOTES_CATEGORY, Taunts[6]),
            Taunt8 = ImportSystem.GetItemByIDAndType(ImportSystem.EMOTES_CATEGORY, Taunts[7]),

            Trophy = ImportSystem.GetItemByIDAndType(ImportSystem.BADGES_CATEGORY, Badge),
            UpperBody = ImportSystem.GetItemByIDAndType(ImportSystem.UPPER_BODIES_CATEGORY, UpperBody)
        };

        loadout.Primary = Primary.ToBLRWeapon(true, loadout);
        loadout.Secondary = Secondary.ToBLRWeapon(false, loadout);

        return loadout;
    }

    public ShareableLoadout Clone()
    {
        var clone = new ShareableLoadout
        {
            Primary = Primary.Clone(),
            Secondary = Secondary.Clone(),
            Avatar = Avatar,
            Badge = Badge,
            BodyCamo = BodyCamo,
            Bot = Bot,
            ButtPack = ButtPack,
            Female = Female,
            Gear_L1 = Gear_L1,
            Gear_L2 = Gear_L2,
            Gear_R1 = Gear_R1,
            Gear_R2 = Gear_R2,
            Hanger = Hanger,
            Helmet = Helmet,
            LowerBody = LowerBody,
            PatchIcon = PatchIcon,
            PatchIconColor = PatchIconColor,
            PatchShape = PatchShape,
            PatchShapeColor = PatchShapeColor,
            Tactical = Tactical,
            UpperBody = UpperBody,
            Depot = new int[Depot.Length],
            Taunts = new int[Taunts.Length]
        };
        Array.Copy(Depot, clone.Depot, Depot.Length);
        Array.Copy(Taunts, clone.Taunts, Taunts.Length);
        return clone;
    }

    public IBLRWeapon GetPrimary()
    {
        return Primary;
    }

    public IBLRWeapon GetSecondary()
    {
        return Secondary;
    }

    public void Read(BLRLoadout loadout)
    {
        Primary.Read(loadout.Primary);
        Secondary.Read(loadout.Secondary);
        loadout.IsFemale = Female;
        loadout.IsBot = Bot;

        loadout.Avatar = ImportSystem.GetItemByIDAndType(ImportSystem.AVATARS_CATEGORY, Avatar);

        loadout.BodyCamo = ImportSystem.GetItemByIDAndType(ImportSystem.CAMOS_BODIES_CATEGORY, BodyCamo);

        loadout.Depot1 = ImportSystem.GetItemByIDAndType(ImportSystem.SHOP_CATEGORY, Depot[0]);
        loadout.Depot2 = ImportSystem.GetItemByIDAndType(ImportSystem.SHOP_CATEGORY, Depot[1]);
        loadout.Depot3 = ImportSystem.GetItemByIDAndType(ImportSystem.SHOP_CATEGORY, Depot[2]);
        loadout.Depot4 = ImportSystem.GetItemByIDAndType(ImportSystem.SHOP_CATEGORY, Depot[3]);
        loadout.Depot5 = ImportSystem.GetItemByIDAndType(ImportSystem.SHOP_CATEGORY, Depot[4]);

        loadout.Gear1 = ImportSystem.GetItemByIDAndType(ImportSystem.ATTACHMENTS_CATEGORY, Gear_R1);
        loadout.Gear2 = ImportSystem.GetItemByIDAndType(ImportSystem.ATTACHMENTS_CATEGORY, Gear_R2);
        loadout.Gear3 = ImportSystem.GetItemByIDAndType(ImportSystem.ATTACHMENTS_CATEGORY, Gear_L1);
        loadout.Gear4 = ImportSystem.GetItemByIDAndType(ImportSystem.ATTACHMENTS_CATEGORY, Gear_L2);

        loadout.Helmet = ImportSystem.GetItemByIDAndType(ImportSystem.HELMETS_CATEGORY, Helmet);
        loadout.UpperBody = ImportSystem.GetItemByIDAndType(ImportSystem.UPPER_BODIES_CATEGORY, UpperBody);
        loadout.LowerBody = ImportSystem.GetItemByIDAndType(ImportSystem.LOWER_BODIES_CATEGORY, LowerBody);

        loadout.Tactical = ImportSystem.GetItemByIDAndType(ImportSystem.TACTICAL_CATEGORY, Tactical);

        loadout.Taunt1 = ImportSystem.GetItemByIDAndType(ImportSystem.EMOTES_CATEGORY, Taunts[0]);
        loadout.Taunt2 = ImportSystem.GetItemByIDAndType(ImportSystem.EMOTES_CATEGORY, Taunts[1]);
        loadout.Taunt3 = ImportSystem.GetItemByIDAndType(ImportSystem.EMOTES_CATEGORY, Taunts[2]);
        loadout.Taunt4 = ImportSystem.GetItemByIDAndType(ImportSystem.EMOTES_CATEGORY, Taunts[3]);
        loadout.Taunt5 = ImportSystem.GetItemByIDAndType(ImportSystem.EMOTES_CATEGORY, Taunts[4]);
        loadout.Taunt6 = ImportSystem.GetItemByIDAndType(ImportSystem.EMOTES_CATEGORY, Taunts[5]);
        loadout.Taunt7 = ImportSystem.GetItemByIDAndType(ImportSystem.EMOTES_CATEGORY, Taunts[6]);
        loadout.Taunt8 = ImportSystem.GetItemByIDAndType(ImportSystem.EMOTES_CATEGORY, Taunts[7]);

        loadout.Trophy = ImportSystem.GetItemByIDAndType(ImportSystem.BADGES_CATEGORY, Badge);
    }

    public void Write(BLRLoadout loadout)
    {
        Primary.Write(loadout.Primary);
        Secondary.Write(loadout.Secondary);
        Female = loadout.IsFemale;
        Bot = loadout.IsBot;

        Avatar = BLRItem.GetMagicCowsID(loadout.Avatar, 99);

        BodyCamo = BLRItem.GetMagicCowsID(loadout.BodyCamo);

        Depot[0] = BLRItem.GetMagicCowsID(loadout.Depot1);
        Depot[1] = BLRItem.GetMagicCowsID(loadout.Depot2);
        Depot[2] = BLRItem.GetMagicCowsID(loadout.Depot3);
        Depot[3] = BLRItem.GetMagicCowsID(loadout.Depot4);
        Depot[4] = BLRItem.GetMagicCowsID(loadout.Depot5);

        Gear_L1 = BLRItem.GetMagicCowsID(loadout.Gear1);
        Gear_L2 = BLRItem.GetMagicCowsID(loadout.Gear2);
        Gear_R1 = BLRItem.GetMagicCowsID(loadout.Gear3);
        Gear_R2 = BLRItem.GetMagicCowsID(loadout.Gear4);

        Helmet = BLRItem.GetMagicCowsID(loadout.Helmet);
        UpperBody = BLRItem.GetMagicCowsID(loadout.UpperBody);
        LowerBody = BLRItem.GetMagicCowsID(loadout.LowerBody);

        Tactical = BLRItem.GetMagicCowsID(loadout.Tactical);

        Taunts[0] = BLRItem.GetMagicCowsID(loadout.Taunt1);
        Taunts[1] = BLRItem.GetMagicCowsID(loadout.Taunt2);
        Taunts[2] = BLRItem.GetMagicCowsID(loadout.Taunt3);
        Taunts[3] = BLRItem.GetMagicCowsID(loadout.Taunt4);
        Taunts[4] = BLRItem.GetMagicCowsID(loadout.Taunt5);
        Taunts[5] = BLRItem.GetMagicCowsID(loadout.Taunt6);
        Taunts[6] = BLRItem.GetMagicCowsID(loadout.Taunt7);
        Taunts[7] = BLRItem.GetMagicCowsID(loadout.Taunt8);

        Badge = BLRItem.GetMagicCowsID(loadout.Trophy);
    }
}

public sealed class ShareableWeapon : IBLRWeapon
{
    [JsonPropertyName("A1")] public int Ammo { get; set; } = 0;
    [JsonPropertyName("B1")] public int Barrel { get; set; } = 0;
    [JsonPropertyName("C1")] public int Camo { get; set; } = 0;
    [JsonPropertyName("G1")] public int Grip { get; set; } = 0;
    [JsonPropertyName("M1")] public int Muzzle { get; set; } = 0;
    [JsonPropertyName("M2")] public int Magazine { get; set; } = 0;
    [JsonPropertyName("R1")] public int Reciever { get; set; } = 1;
    [JsonPropertyName("S1")] public int Scope { get; set; } = 0;
    [JsonPropertyName("S2")] public int Stock { get; set; } = 0;
    [JsonPropertyName("S3")] public int Skin { get; set; } = -1;
    [JsonPropertyName("T1")] public int Tag { get; set; } = 0;

    private static Dictionary<string, PropertyInfo> Properties { get; } = GetAllProperties();
    private static Dictionary<string, PropertyInfo> GetAllProperties()
    {
        var props = new Dictionary<string, PropertyInfo>();
        var properties = typeof(ShareableWeapon).GetProperties().ToArray();
        foreach (var prop in properties)
        {
            props.Add(prop.Name, prop);
        }
        return props;
    }
    public ShareableWeapon() { }

    /// <summary>
    /// Creates a Loadout-Manager readable Weapon
    /// </summary>
    /// <param name="weapon"></param>
    public ShareableWeapon(BLRWeapon weapon)
    {
        foreach (var part in BLRWeapon.WeaponParts)
        {
            if (Properties.TryGetValue(part.Name, out PropertyInfo info))
            {
                info.SetValue(this, BLRItem.GetMagicCowsID((BLRItem)part.GetValue(weapon)));
            }
        }
    }

    public BLRWeapon ToBLRWeapon(bool isPrimary, BLRLoadout loadout) 
    {
        return new BLRWeapon(isPrimary, loadout, this);
    }

    public ShareableWeapon Clone()
    {
        return new()
        { 
            Ammo = Ammo,
            Barrel = Barrel,
            Camo = Camo,
            Grip = Grip,
            Magazine = Magazine,
            Muzzle = Muzzle,
            Reciever = Reciever,
            Scope = Scope,
            Skin = Skin,
            Stock = Stock,
            Tag = Tag
        };
    }

    public void Read(BLRWeapon weapon)
    {
        weapon.Reciever = ImportSystem.GetItemByIDAndType(weapon.IsPrimary ? ImportSystem.PRIMARY_CATEGORY : ImportSystem.SECONDARY_CATEGORY, Reciever);
        weapon.Barrel = ImportSystem.GetItemByIDAndType(ImportSystem.BARRELS_CATEGORY, Barrel);
        weapon.Muzzle = ImportSystem.GetItemByIDAndType(ImportSystem.MUZZELS_CATEGORY, Muzzle);
        weapon.Magazine = ImportSystem.GetItemByIDAndType(ImportSystem.MAGAZINES_CATEGORY, Magazine);
        weapon.Stock = ImportSystem.GetItemByIDAndType(ImportSystem.STOCKS_CATEGORY, Stock);
        weapon.Scope = ImportSystem.GetItemByIDAndType(ImportSystem.SCOPES_CATEGORY, Scope);
        weapon.Grip = ImportSystem.GetItemByIDAndType(ImportSystem.GRIPS_CATEGORY, Grip);
        weapon.Ammo = ImportSystem.GetItemByIDAndType(ImportSystem.AMMO_CATEGORY, Ammo);
        weapon.Tag = ImportSystem.GetItemByIDAndType(ImportSystem.HANGERS_CATEGORY, Tag);
        weapon.Camo = ImportSystem.GetItemByIDAndType(ImportSystem.CAMOS_WEAPONS_CATEGORY, Camo);
        weapon.Skin = ImportSystem.GetItemByIDAndType(ImportSystem.PRIMARY_SKIN_CATEGORY, Skin);
    }

    public void Write(BLRWeapon weapon)
    {
        Reciever = BLRItem.GetMagicCowsID(weapon.Reciever);
        Barrel = BLRItem.GetMagicCowsID(weapon.Barrel);
        Muzzle = BLRItem.GetMagicCowsID(weapon.Muzzle);
        Magazine = BLRItem.GetMagicCowsID(weapon.Magazine);
        Stock = BLRItem.GetMagicCowsID(weapon.Stock);
        Scope = BLRItem.GetMagicCowsID(weapon.Scope);
        Grip = BLRItem.GetMagicCowsID(weapon.Grip);
        Ammo = BLRItem.GetMagicCowsID(weapon.Ammo);
        Tag = BLRItem.GetMagicCowsID(weapon.Tag);
        Camo = BLRItem.GetMagicCowsID(weapon.Camo);
        Skin = BLRItem.GetMagicCowsID(weapon.Skin);
    }
}
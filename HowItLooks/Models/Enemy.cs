﻿using HowItLooks.Entities;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HowItLooks.Models;

public class Enemy : INotifyPropertyChanged
{
    private string _name;
    private int _hp;
    private int _hpLeft;
    private int _initiative;
    private bool _isActive;
    private int _armorClass;
    private int _tempHitPoints;
    private CreatureType _creatureType;

    public int Id { get; set; }
    public string Name { get => _name; set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged();
            }
        }
    }
    public int HitPoints { get => _hp; set
        {
            if (_hp != value)
            {
                _hp = value;
                OnPropertyChanged("HitPointsLabel");
            }
        }
    }
    public int ArmorClass
    {
        get => _armorClass;
        set
        {
            value = Math.Clamp(value, 0, 99);
            if (_armorClass != value)
            {
                _armorClass = value;
                OnPropertyChanged();
            }
        }
    }
    public CreatureType CreatureType
    {
        get => _creatureType;
        set
        {
            if (_creatureType != value)
            {
                _creatureType = value;
                OnPropertyChanged(); 
            }
        }
    }
    public int TempHitPoints
    {
        get => _tempHitPoints;
        set
        {
            _tempHitPoints = value;
            OnPropertyChanged(nameof(HitPointsLabel));
        }
    }
    public int HitPointsLeft { get => _hpLeft; set
        {
            if (_hpLeft != value)
            {
                _hpLeft = value;
                OnPropertyChanged("HitPointsLabel");
            }
        }
    }
    public int Initiative
    {
        get => _initiative;
        set
        {
            if (_initiative != value)
            {
                _initiative = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive != value)
            {
                _isActive = value;
                OnPropertyChanged();
            }
        }
    }

    public string HitPointsLabel { get => 
        TempHitPoints > 0
            ? $"{HitPointsLeft} / {HitPoints} +{TempHitPoints}"
            : $"{HitPointsLeft} / {HitPoints}";
    }

    public Enemy(string name, int hitPoints, int initiative = 0)
    {
        _name = string.Empty;
        Name = name;
        HitPoints = HitPointsLeft = hitPoints;
        Initiative = initiative;
    }

    public Enemy(EnemyEntity entity)
    {
        Id = entity.Id;
        Name = entity.Name;
        HitPoints = entity.HitPoints;
        Initiative = entity.Initiative;
        HitPointsLeft = entity.HitPointsLeft;
        IsActive = entity.IsActive;
        ArmorClass = entity.ArmorClass;
        TempHitPoints = entity.TempHitPoints;
        CreatureType = entity.CreatureType;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void UpdateHitPoints(int hitPoints)
    {
        HitPoints = HitPointsLeft = hitPoints;
        OnPropertyChanged("HitPointsLabel");
    }

    public void IncreaseHitPoints(int hitPoints)
    {
        if (hitPoints + HitPointsLeft > HitPoints)
            HitPointsLeft = HitPoints;
        else
            HitPointsLeft += hitPoints;
    }

    public void DecreaseHitPoints(int damage)
    {
        if (TempHitPoints > 0)
        {
            if (damage <= TempHitPoints)
            {
                TempHitPoints -= damage;
                damage = 0;
            }
            else
            {
                damage -= TempHitPoints;
                TempHitPoints = 0;
            }
        }

        HitPointsLeft -= damage;
        if (HitPointsLeft < 0) HitPointsLeft = 0;

        OnPropertyChanged(nameof(HitPointsLabel));
    }
}

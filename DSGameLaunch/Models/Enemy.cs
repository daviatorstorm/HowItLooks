using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DSGameLaunch.Models;

public class Enemy : INotifyPropertyChanged
{
    private string _name;
    private int _hp;
    private int _hpLeft;

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
    public int HitPointsLeft { get => _hpLeft; set
        {
            if (_hpLeft != value)
            {
                _hpLeft = value;
                OnPropertyChanged("HitPointsLabel");
            }
        }
    }
    public string HitPointsLabel { get => $"{HitPointsLeft} / {HitPoints} HP"; }

    public Enemy(string name, int hitPoints)
    {
        Name = name;
        HitPoints = HitPointsLeft = hitPoints;
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

    public void DecreaseHitPoints(int hitPoints)
    {
        if (HitPointsLeft - hitPoints < 0)
            HitPointsLeft = 0;
        else
            HitPointsLeft -= hitPoints;
    }
}

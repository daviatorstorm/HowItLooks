using HowItLooks.Entities;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HowItLooks.Models;

public class Campaign : INotifyPropertyChanged
{
    private int _id;
    private string _name = string.Empty;

    public int Id
    {
        get => _id;
        set
        {
            if (_id != value)
            {
                _id = value;
                OnPropertyChanged();
            }
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged();
            }
        }
    }

    public Campaign()
    {
    }

    public Campaign(CampaignEntity entity)
    {
        Id = entity.Id;
        Name = entity.Name;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}

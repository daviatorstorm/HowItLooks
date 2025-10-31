using HowItLooks.Models;
using HowItLooks.Services;
using HowItLooks.Extension;
using System.Collections.ObjectModel;
using HowItLooks.Entities;
using HowItLooks.Services;

namespace HowItLooks;

public partial class GroupDetailsPage : ContentPage
{
    public ObservableCollection<Enemy> Enemies { get; set; }
    private readonly DatabaseService _db;
    private readonly GroupEntity _group;
    private readonly EnemyActionsService _enemyActions = new();
    public GroupDetailsPage(GroupEntity group)
    {
        InitializeComponent();
        _db = new DatabaseService();
        _group = group;
        Title = $"{Translator.Instance["Groups"]}: {group.Name}";
        var enemyEntities = _db.GetEnemiesByGroupId(group.Id);
        Enemies = new ObservableCollection<Enemy>(enemyEntities.Select(e => new Enemy(e)));
        BindingContext = this;
    }

    private async void AddEnemy_Clicked(object sender, EventArgs e)
    {
        await _enemyActions.AddEnemy(
               page: this,
               enemies: Enemies,
               getActiveEnemy: null,
               setActiveEnemy: null,
               _group.Id
        );
    }

    private async void NameLabel_Clicked(object sender, TappedEventArgs e)
    {
        var label = sender as Label;
        var enemy = label?.BindingContext as Enemy;
        if (enemy == null) return;

        await _enemyActions.ChangeName(this, enemy);
    }

    private async void InitiativeLabel_Clicked(object sender, EventArgs e)
    {
        var label = sender as Label;
        var enemy = label?.BindingContext as Enemy;
        if (enemy == null) return;

        await _enemyActions.ChangeInitiative(this, enemy, SortEnemies);
    }

    private async void ArmorClassLabel_Clicked(object sender, EventArgs e)
    {
        var label = sender as Label;
        var enemy = label?.BindingContext as Enemy;
        if (enemy == null) return;

        await _enemyActions.ChangeArmor(this, enemy);
    }

    private async void HPLabel_Clicked(object sender, EventArgs e)
    {
        var label = sender as Label;
        var enemy = label?.BindingContext as Enemy;
        if (enemy == null) return;

        await _enemyActions.ChangeHP(this, enemy);
    }

    private async void CreatureTypeLabel_Clicked(object sender, EventArgs e)
    {
        var label = sender as Label;
        var enemy = label?.BindingContext as Enemy;
        if (enemy == null) return;

        await _enemyActions.ChangeCreatureType(this, enemy);
    }

    private async void RemoveEnemy_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var enemy = button?.BindingContext as Enemy;
        if (enemy == null) return;

        await _enemyActions.RemoveEnemy(
             page: this,
             enemy: enemy,
             enemies: Enemies
        );
    }

    private void SortEnemies()
    {
        var sorted = Enemies.OrderByDescending(e => e.Initiative).ToList();
        for (int i = 0; i < sorted.Count; i++)
        {
            if (Enemies[i] != sorted[i])
            {
                Enemies.Move(Enemies.IndexOf(sorted[i]), i);
            }
        }
    }
    public async void AddSingleEnemyToMainPage_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var enemy = button?.BindingContext as Enemy;

        if (enemy == null) return;

        bool confirm = await DisplayAlert($"{Translator.Instance["AddToBattle"]}?",
                                          $"{enemy.Name} {Translator.Instance["ToTheMainPage"]}?",
                                          Translator.Instance["Yes"],
                                          Translator.Instance["No"]);
        if (!confirm) return;

        var entity = _db.AddMonster(enemy.Name);
        entity.HitPoints = enemy.HitPoints;
        entity.HitPointsLeft = enemy.HitPointsLeft;
        entity.Initiative = enemy.Initiative;
        entity.ArmorClass = enemy.ArmorClass;
        entity.TempHitPoints = enemy.TempHitPoints;
        entity.CreatureType = enemy.CreatureType;
        entity.GroupId = null;

        _db.UpdateMonster(entity);

        var addedEnemy = new Enemy(entity);

        MessagingCenter.Send(this, "EnemiesUpdated");

        await DisplayAlert(Translator.Instance["Success"],
            $"{enemy.Name} {Translator.Instance["AddedInMainPage"]}",
            "OK");
    }
    private async void AddToMainPage_Clicked(object sender, EventArgs e)
    {

        if (Enemies.Count == 0)
        {
            await DisplayAlert(Translator.Instance["Info"],
                Translator.Instance["ThisGroupHasNoEnemies"],
                "OK");
            return;
        }

        List<Enemy> addedEnemies = new();

        foreach (var enemy in Enemies)
        {
            var entity = _db.AddMonster(enemy.Name);
            entity.HitPoints = enemy.HitPoints;
            entity.HitPointsLeft = enemy.HitPointsLeft;
            entity.Initiative = enemy.Initiative;
            entity.ArmorClass = enemy.ArmorClass;
            entity.TempHitPoints = enemy.TempHitPoints;
            entity.CreatureType = enemy.CreatureType;
            entity.GroupId = null; 

            _db.UpdateMonster(entity);
            addedEnemies.Add(new Enemy(entity));
        }

        MessagingCenter.Send(this, "EnemiesUpdated");

        await DisplayAlert(Translator.Instance["Success"],
            $"{Enemies.Count} {Translator.Instance["AddedInMainPage"]}",
            "OK");
        //Enemies.Clear();
    }
}

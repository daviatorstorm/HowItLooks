using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HowItLooks.Entities;
using HowItLooks.Extension;
using HowItLooks.Models;
using HowItLooks.Services;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace HowItLooks.ViewModels
{
    public partial class GroupsViewModel : ObservableObject
    {
        private readonly DatabaseService _db;
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty] private string _pageTitle = Translator.Instance["Groups"];
        [ObservableProperty] private string _searchText;

        [ObservableProperty] private ObservableCollection<Group> _groupList = new();

        public GroupsViewModel(DatabaseService db)
        {
            _db = db;
            LoadGroups();
        }

        private void LoadGroups()
        {
            GroupList.Clear();
            var groupsFromDb = _db.GetAllGroups();
            foreach (var g in groupsFromDb)
                GroupList.Add(new Group(g));
        }

        [RelayCommand]
        private async Task AddGroup()
        {
            string name = await Shell.Current.DisplayPromptAsync(
                Translator.Instance["GroupName"],
                Translator.Instance["EnterAGroupName"]);

            if (string.IsNullOrWhiteSpace(name)) return;

            if (_db.GetGroupByName(name) != null)
            {
                await Shell.Current.DisplayAlert(
                    Translator.Instance["Error"],
                    Translator.Instance["AGroupWithThisNameAlreadyExists"],
                    "OK");
                return;
            }

            name = char.ToUpper(name[0]) + name.Substring(1);
            var newGroupEntity = _db.AddGroup(name);
            GroupList.Add(new Group(newGroupEntity));
        }

        [RelayCommand]
        private async Task OpenGroup(Group group)
        {
            var groupEntity = new GroupEntity(group);
            //var viewModel = _serviceProvider.GetService<GroupDetailsViewModel>();
            var page = new GroupDetailsPage(groupEntity);
            await Shell.Current.Navigation.PushAsync(page);
        }

        [RelayCommand]
        private async Task ShowGroupOptions(Group group)
        {
            string addAllText = Translator.Instance["AddAllEnemiesToMainPage"];
            string changeNameText = Translator.Instance["ChangeName"];
            string deleteText = Translator.Instance["Delete"];

            string action = await Shell.Current.DisplayActionSheet(
                $"{Translator.Instance["OptionsFor"]} '{group.Name}'",
                Translator.Instance["Cancel"],
                null,
                addAllText,
                changeNameText,
                deleteText);

            if (action == addAllText)
                AddAllGroupEnemiesToMain(group);
            else if (action == changeNameText)
                await RenameGroup(group);
            else if (action == deleteText)
                await DeleteGroup(group);
        }

        private void AddAllGroupEnemiesToMain(Group group)
        {
            var enemies = _db.GetEnemiesByGroupId(group.Id);

            if (enemies.Count == 0)
            {
                Shell.Current.DisplayAlert(
                    Translator.Instance["Info"],
                    Translator.Instance["GroupHasNoEnemies"],
                    "OK");
                return;
            }

            foreach (var enemyEntity in enemies)
            {
                var newEntity = _db.AddMonster(enemyEntity.Name);
                newEntity.HitPoints = enemyEntity.HitPoints;
                newEntity.HitPointsLeft = enemyEntity.HitPointsLeft;
                newEntity.Initiative = enemyEntity.Initiative;
                newEntity.ArmorClass = enemyEntity.ArmorClass;
                newEntity.TempHitPoints = enemyEntity.TempHitPoints;
                newEntity.CreatureType = enemyEntity.CreatureType;
                newEntity.GroupId = null;

                _db.UpdateMonster(newEntity);
            }

            MessagingCenter.Send(this, "EnemiesUpdated");

            Shell.Current.DisplayAlert(
                Translator.Instance["Done"],
                $"{Translator.Instance["EnemiesFrom"]} '{group.Name}' {Translator.Instance["AddedToMainPage"]}!",
                "OK");
        }

        private async Task RenameGroup(Group group)
        {
            string newName = await Shell.Current.DisplayPromptAsync(
                Translator.Instance["ChangeName"],
                Translator.Instance["NewName"],
                initialValue: group.Name);

            if (string.IsNullOrWhiteSpace(newName)) return;

            newName = char.ToUpper(newName[0]) + newName.Substring(1);
            group.Name = newName;
            _db.UpdateGroup(new GroupEntity(group));
            await Shell.Current.DisplayAlert("✅", Translator.Instance["NameUpdated"], "OK");
        }

        private async Task DeleteGroup(Group group)
        {
            bool confirm = await Shell.Current.DisplayAlert(
                Translator.Instance["Delete"],
                $"{Translator.Instance["Delete"]} '{group.Name}' {Translator.Instance["AndAllEnemiesInIt"]}",
                Translator.Instance["Yes"],
                Translator.Instance["No"]);

            if (!confirm) return;

            _db.DeleteGroup(new GroupEntity(group));
            GroupList.Remove(group);

            await Shell.Current.DisplayAlert(
                Translator.Instance["Done"],
                $"{Translator.Instance["Group"]} '{group.Name}' {Translator.Instance["Deleted"]}.",
                "OK");
        }

        partial void OnSearchTextChanged(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                LoadGroups();
                return;
            }

            var filtered = _db.GetAllGroups()
                .Where(x => x.Name.Contains(value, StringComparison.OrdinalIgnoreCase))
                .Select(x => new Group(x))
                .ToList();

            GroupList.Clear();
            foreach (var g in filtered)
                GroupList.Add(g);
        }
    }
}

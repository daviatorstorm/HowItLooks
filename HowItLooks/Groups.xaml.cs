using HowItLooks.Entities;
using HowItLooks.Extension;
using HowItLooks.Models;
using HowItLooks.Services;
using System.Collections.ObjectModel;



namespace HowItLooks 
{ 
    public partial class Groups : ContentPage
    {
        public ObservableCollection<Group> GroupList { get; set; }
        private readonly DatabaseService _db;

        public Groups()
        {
            InitializeComponent();
            _db = new DatabaseService();

            var groupsFromDb = _db.GetAllGroups();

            GroupList = new ObservableCollection<Group>(groupsFromDb.Select(g => new Group(g)));

            BindingContext = this;
        }

        private async void AddGroup_Clicked(object sender, EventArgs e)
        {
            string name = await DisplayPromptAsync(Translator.Instance["GroupName"],
                                                   Translator.Instance["EnterAGroupName"]);
            if (string.IsNullOrWhiteSpace(name)) return;

            if (_db.GetGroupByName(name) != null)
            {
                await DisplayAlert(Translator.Instance["Error"],
                                    Translator.Instance["AGroupWithThisNameAlreadyExists"],
                                    "OK");
                return;
            }

            var newGroupEntity = _db.AddGroup(name);
            GroupList.Add(new Group(newGroupEntity));
        }

        private async void OpenGroup_Clicked(object sender, EventArgs e)
        {
            if (sender is Element element && element.BindingContext is Group group)
            {
                await Navigation.PushAsync(new GroupDetailsPage(new GroupEntity(group)));
            }
        }
        private async void ShowGroupOptions_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button?.CommandParameter is not Group group)
                return;

            string addAllText = Translator.Instance["AddAllEnemiesToMainPage"];
            string changeNameText = Translator.Instance["ChangeName"];
            string deleteText = Translator.Instance["Delete"];

            string action = await DisplayActionSheet(
                $"{Translator.Instance["OptionsFor"]} '{group.Name}'",
                Translator.Instance["Cancel"],
                null,
                Translator.Instance["AddAllEnemiesToMainPage"],
                Translator.Instance["ChangeName"],
                Translator.Instance["Delete"]
            );

            switch (action)
            {
                case var a when a == addAllText:
                    AddAllGroupEnemiesToMain(group);
                    break;

                case var a when a == changeNameText:
                    await RenameGroup(group);
                    break;

                case var a when a == deleteText:
                    await DeleteGroup(group);
                    break;
            }
        }
        private void AddAllGroupEnemiesToMain(Group group)
        {
            var enemies = _db.GetEnemiesByGroupId(group.Id);

            if (enemies.Count == 0)
            {
                DisplayAlert(
                    Translator.Instance["Info"],
                    Translator.Instance["GroupHasNoEnemies"],
                    "OK"
                );
                return;
            }

            List<Enemy> addedEnemies = new();

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
                addedEnemies.Add(new Enemy(newEntity));
            }
            //AddedToMainPage
            MessagingCenter.Send(this, "EnemiesUpdated");

            DisplayAlert(Translator.Instance["Done"],
                        $"{Translator.Instance["EnemiesFrom"]} '{group.Name}' {Translator.Instance["AddedToMainPage"]}!",
                        "OK");
        }

        private async Task RenameGroup(Group group)
        {
            string newName = await DisplayPromptAsync("Change name", "New name:", initialValue: group.Name);
            if (string.IsNullOrWhiteSpace(newName)) return;

            group.Name = newName;
            _db.UpdateGroup(new GroupEntity(group));
            await DisplayAlert("Change name", "Name updated!", "OK");
        }
        private async Task DeleteGroup(Group group)
        {
            bool confirm = await DisplayAlert(
                Translator.Instance["Delete"],
                $"{Translator.Instance["Delete"]}'{group.Name}' {Translator.Instance["AndAllEnemiesInIt"]}",
                Translator.Instance["Yes"],
                Translator.Instance["No"]);

            if (!confirm) return;

            _db.DeleteGroup(new GroupEntity(group));
            GroupList.Remove(group);

            await DisplayAlert(Translator.Instance["Done"],
                                $"{Translator.Instance["Group"]} '{group.Name}' {Translator.Instance["Deleted"]}.",
                                "OK");
        }

        private void SearchBarGroups_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = e.NewTextValue?.Trim() ?? "";

            GroupList.Clear();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                foreach (var g in _db.GetAllGroups().Select(x => new Group(x)))
                    GroupList.Add(g);
            }
            else
            {
                var filtered = _db.GetAllGroups()
                                  .Where(x => x.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                                  .Select(x => new Group(x))
                                  .ToList();

                foreach (var g in filtered)
                    GroupList.Add(g);
            }
        }

    }
}
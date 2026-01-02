using HowItLooks.Extension;
using HowItLooks.Services;
using HowItLooks.ViewModels;
using System.Threading.Tasks;

namespace HowItLooks;

[QueryProperty(nameof(Id), "id")]
public partial class GroupDetailsPage : ContentPage
{
    private int _id;
    private readonly GroupDetailsViewModel _viewModel;
    private readonly DatabaseService _db;

    public string Id { set => _id = int.Parse(value); }

    public GroupDetailsPage(DatabaseService db)
    {
        InitializeComponent();
        var viewModel = new GroupDetailsViewModel(db);
        BindingContext = viewModel;
        _viewModel = viewModel;
        _db = db;
    }

    protected override async void OnAppearing()
    {
        var group = _db.GetGroupById(_id);
        if (group == null)
        {
            await DisplayAlert(Translator.Instance["Error"], Translator.Instance["GroupNotFound"], "OK");
            await Shell.Current.GoToAsync("//Groups");
            return;
        }
        Title = $"{Translator.Instance["Groups"]}: {group.Name}";
        _viewModel.Init(group);

        base.OnAppearing();
    }
}

using HowItLooks.Models;
using HowItLooks.Services;
using HowItLooks.Extension;
using System.Collections.ObjectModel;
using HowItLooks.Entities;
using HowItLooks.ViewModels;

namespace HowItLooks;

public partial class GroupDetailsPage : ContentPage
{
    public GroupDetailsPage(GroupEntity group)
    {
        InitializeComponent();
        var db = new DatabaseService();
        var viewModel = new GroupDetailsViewModel(db);
        BindingContext =  viewModel;
        Title = $"{Translator.Instance["Groups"]}: {group.Name}";
        viewModel.Init(group);
    }
}
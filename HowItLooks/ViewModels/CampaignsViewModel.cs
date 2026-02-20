using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HowItLooks.Entities;
using HowItLooks.Extension;
using HowItLooks.Models;
using HowItLooks.Services;
using System.Collections.ObjectModel;

namespace HowItLooks.ViewModels;

public partial class CampaignsViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    [ObservableProperty]
    public ObservableCollection<Campaign> campaigns;

    public CampaignsViewModel(DatabaseService db)
    {
        _db = db;
        Campaigns = new ObservableCollection<Campaign>(_db.GetAllCampaigns().Select(e => new Campaign(e)));
    }

    [RelayCommand]
    private async Task AddCampaign()
    {
        var name = await Shell.Current.DisplayPromptAsync(
                         Translator.Instance["NewCampaign"],
                         Translator.Instance["CampaignName"]);

        if (string.IsNullOrWhiteSpace(name)) return;

        if (_db.GetCampaignByName(name) != null)
        {
            await Shell.Current.DisplayAlert(
                Translator.Instance["Error"],
                Translator.Instance["ACampaignWithThisNameAlreadyExists"],
                "OK");
            return;
        }

        var campaignEntity = _db.AddCampaign(name);
        Campaigns.Add(new Campaign(campaignEntity));
    }
    [RelayCommand]
    private async Task OpenCampaign(Campaign campaign)
    {
        await Shell.Current.GoToAsync($"//Campaigns/main?id={campaign.Id}");
    }

    [RelayCommand]
    private async Task ShowCampaignOptions(Campaign campaign)
    {
        if (campaign == null) return;

        string action = await Shell.Current.DisplayActionSheet(
                              campaign.Name,
                              Translator.Instance["Cancel"],
                              null,
                              Translator.Instance["Rename"],
                              Translator.Instance["Delete"]);

        if (action == Translator.Instance["Rename"])
            await RenameCampaign(campaign);
        else if (action == Translator.Instance["Delete"])
            await DeleteCampaign(campaign);
    }

    private async Task RenameCampaign(Campaign campaign)
    {
        string newName = await Shell.Current.DisplayPromptAsync(
                               Translator.Instance["RenameCampaign"],
                               Translator.Instance["NewName"],
                               initialValue: campaign.Name
        );

        if (string.IsNullOrWhiteSpace(newName)) return;

        if (_db.GetCampaignByName(newName) != null)
        {
            await Shell.Current.DisplayAlert(
                Translator.Instance["Error"],
                Translator.Instance["ACampaignWithThisNameAlreadyExists"],
                "OK");
            return;
        }

        campaign.Name = newName;
        _db.UpdateCampaign(new CampaignEntity(campaign));
    }
    private async Task DeleteCampaign(Campaign campaign)
    {
        bool confirm = await Shell.Current.DisplayAlert(
            Translator.Instance["Removal"],
            $"{Translator.Instance["Delete"]} {campaign.Name}?",
            Translator.Instance["Yes"],
            Translator.Instance["No"]
        );

        if (!confirm) return;

        _db.DeleteCampaign(new CampaignEntity(campaign));
        Campaigns.Remove(campaign);
    }

}

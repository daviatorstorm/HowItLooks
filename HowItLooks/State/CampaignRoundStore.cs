namespace HowItLooks.State
{
    public static class CampaignRoundStore
    {
        private static readonly Dictionary<int, RoundState> _states = new();

        public static RoundState Get(int campaignId)
        {
            if (!_states.TryGetValue(campaignId, out var state))
            {
                state = new RoundState();
                _states[campaignId] = state;
            }

            return state;
        }
    }
}

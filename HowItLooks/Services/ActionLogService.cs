using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HowItLooks.Services
{
    class ActionLogService
    {
        private readonly Frame _drawer;
        private readonly BoxView _overlay;
        private readonly VerticalStackLayout _container;
        private readonly List<string> _log = new();

        private bool _isOpen = false;
        private bool _isRoundStarted = false;

        public ActionLogService(Frame drawer, BoxView overlay, VerticalStackLayout container)
        {
            _drawer = drawer;
            _overlay = overlay;
            _container = container;

            var tap = new TapGestureRecognizer();
            tap.Tapped += async (s, e) => { if (_isOpen) await CloseAsync(); };
            _overlay.GestureRecognizers.Add(tap);

            _overlay.IsVisible = false;
            _drawer.IsVisible = false;
        }

        public void SetRoundState(bool started)
        {
            _isRoundStarted = started;
        }
        public bool GetIsRoundState()
        {
            return _isRoundStarted;
        }

        public void LogAction(string action, string creatureType = null)
        {
            if (!_isRoundStarted) return;

            string entry = $"{DateTime.Now:HH:mm:ss} → {action}";
            _log.Add(entry);

            Color textColor = Colors.White;

            if (creatureType != null)
            {
                try
                {
                    textColor = (Color)Application.Current.Resources[$"{creatureType}BaseLight"];
                }
                catch
                {
                    textColor = Colors.White;
                }
            }

            _container.Children.Add(new Label
            {
                Text = entry,
                TextColor = textColor,
                FontSize = 16
            });
        }

        public async Task ShowBattleSummary(Page page, int roundCounter, DateTime battleStartTime)
        {
            DateTime battleEndTime = DateTime.Now;
            var duration = battleEndTime - battleStartTime;

            string message =
                $"✅ Бій завершено!\n" +
                $"📌 Раундів: {roundCounter}\n" +
                $"⏱ Тривалість: {duration:mm\\:ss}";

            bool showLog = await page.DisplayAlert("Висновок", message, "Лог бою", "Закрити");

            if (showLog)
            {
                await OpenAsync();
            }
        }


        public async Task ToggleAsync()
        {
            if (_isOpen)
                await CloseAsync();
            else
                await OpenAsync();
        }

        public async Task OpenAsync()
        {
            if (_isOpen) return;

            _overlay.IsVisible = true;
            _drawer.IsVisible = true;

            await _drawer.TranslateTo(0, 0, 250, Easing.CubicOut);
            _isOpen = true;
        }

        public async Task CloseAsync()
        {
            if (!_isOpen) return;

            await _drawer.TranslateTo(300, 0, 200, Easing.CubicIn);
            _drawer.IsVisible = false;
            _overlay.IsVisible = false;
            _isOpen = false;
        }

        public void ClearLog()
        {
            _log.Clear();
            _container.Children.Clear();
        }

        public List<string> GetAllLogs()
        {
            return new List<string>(_log);
        }
    }
}

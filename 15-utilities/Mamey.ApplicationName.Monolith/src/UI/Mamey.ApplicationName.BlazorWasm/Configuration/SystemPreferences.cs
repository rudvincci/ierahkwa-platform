using ReactiveUI;

namespace Mamey.ApplicationName.BlazorWasm.Configuration
{
    public class SystemPreferences : ReactiveObject
    {
        private string _dateFormat = "MM/dd/yyyy";
        private string _timeZone = "UTC";
        private string _defaultPage = "/secure/dashboard";
        private string _fontSize = "medium";
        private string _language = "en";

        public string DateFormat
        {
            get => _dateFormat;
            set => this.RaiseAndSetIfChanged(ref _dateFormat, value);
        }

        public string TimeZone
        {
            get => _timeZone;
            set => this.RaiseAndSetIfChanged(ref _timeZone, value);
        }

        public string DefaultPage
        {
            get => _defaultPage;
            set => this.RaiseAndSetIfChanged(ref _defaultPage, value);
        }

        public string FontSize
        {
            get => _fontSize;
            set => this.RaiseAndSetIfChanged(ref _fontSize, value);
        }

        public string Language
        {
            get => _language;
            set => this.RaiseAndSetIfChanged(ref _language, value);
        }
    }
}
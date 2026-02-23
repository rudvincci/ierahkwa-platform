using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace Mamey.BlazorWasm.Components.FileUpload
{
    public partial class MameyFileUpload : ComponentBase
    {
        private const string DefaultDragClass = "relative rounded-lg border-2 border-dashed pa-4 mt-4 mud-width-full mud-height-full z-10";
        private string _dragClass = DefaultDragClass;
        private readonly List<string> _fileNames = new();

        [Inject] HttpClient _httpClient { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }

        [Parameter] public EventCallback<MouseEventArgs> OnUploadButtonClick { get; set; }
        [Parameter] public EventCallback<IReadOnlyList<IBrowserFile>?> FilesChanged { get; set; }

        [Parameter] public IReadOnlyList<IBrowserFile>? Files { get; set; }
        [Parameter] public EventCallback<IReadOnlyList<IBrowserFile>?> OnFilesChanged { get; set; }

        public async Task ClearAsync()
        {
            _fileNames.Clear();
            ClearDragClass();
            await Task.Delay(100);
        }

        private void OnInputFileChanged(InputFileChangeEventArgs e)
        {
            ClearDragClass();
            var files = e.GetMultipleFiles();
            foreach (var file in files)
            {
                _fileNames.Add(file.Name);
            }
            OnFilesChanged.InvokeAsync(files);
        }
        private void Upload()
        {
            // Upload the files here
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add("TODO: Upload your files!");
        }

        private void SetDragClass()
            => _dragClass = $"{DefaultDragClass} mud-border-primary";

        private void ClearDragClass()
            => _dragClass = DefaultDragClass;
        
    }
}


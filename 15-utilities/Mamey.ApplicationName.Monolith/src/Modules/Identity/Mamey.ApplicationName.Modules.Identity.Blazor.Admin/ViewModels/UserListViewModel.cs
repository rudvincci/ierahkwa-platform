using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mamey.ApplicationName.Modules.Identity.Blazor.Admin.Components.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Mamey.CQRS.Queries;
using Mamey.ApplicationName.Modules.Identity.Blazor.Admin.Dto;
using Mamey.ApplicationName.Modules.Identity.Blazor.Admin.Services;

namespace Mamey.ApplicationName.Modules.Identity.Blazor.Admin.ViewModels
{
    public class UserListViewModel
    {
        private readonly IUserService _userService;
        private readonly IDialogService _dialogService;
        private readonly NavigationManager _nav;

        public UserListViewModel(
            IUserService userService,
            IDialogService dialogService,
            NavigationManager nav)
        {
            _userService   = userService;
            _dialogService = dialogService;
            _nav           = nav;
        }

        public IEnumerable<UserDto> Users { get; private set; } = Enumerable.Empty<UserDto>();
        public bool         IsLoading { get; private set; }
        public long          TotalCount { get; private set; }

        public string SearchTerm { get; set; } = "";

        public Func<UserDto, bool> FilterFunc =>
            dto => string.IsNullOrWhiteSpace(SearchTerm)
                   || dto.FullName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)
                   || dto.Email.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase);

        public async Task InitializeAsync()
        {
            // initial load: first page, no sort
            await LoadPageAsync(0, 10, null, SortDirection.None, SearchTerm, CancellationToken.None);
        }

        public async Task<TableData<UserDto>> LoadPageAsync(
            TableState      state,
            int             page,
            string?         sortBy,
            SortDirection   sortDirection,
            string?         filter,
            CancellationToken cancellationToken)
        {
            return await LoadPageAsync(
                page,
                state.PageSize,
                sortBy,
                sortDirection,
                filter,
                cancellationToken
            );
        }

        public async Task<TableData<UserDto>> LoadPageAsync(
            int             page,
            int             pageSize,
            string?         sortBy,
            SortDirection   sortDirection,
            string?         filter,
            CancellationToken cancellationToken)
        {
            IsLoading = true;
            try
            {
                var result = await _userService.GetUsersAsync(
                    page,
                    pageSize,
                    sortBy,
                    sortDirection == SortDirection.Descending,
                    filter,
                    cancellationToken);

                Users      = result.Items;
                TotalCount = result.TotalResults;

                return new TableData<UserDto>
                {
                    Items      = Users,
                    TotalItems = (int)TotalCount
                };
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void EditUser(Guid id)
            => _nav.NavigateTo($"/admin/users/edit/{id}");

        public async Task ConfirmDelete(Guid id, string email)
        {
            var opts = new DialogOptions { MaxWidth = MaxWidth.Small };
            var parameters = new DialogParameters
            {
                ["ContentText"] = $"Are you sure you want to delete '{email}'?",
                ["ButtonText"]  = "Delete",
                ["Color"]       = Color.Error
            };

            var dialog = _dialogService.Show<ConfirmDialog>("Confirm delete", parameters, opts);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                await DeleteUser(id);
                // refresh current page
                await InitializeAsync();
            }
        }

        private async Task DeleteUser(Guid id)
        {
            // call your delete API here, e.g. IUserService.DeleteUserAsync(id);
            // then maybe show a toast/snackbar
        }

        public void ExportCsv()
        {
            // simply navigate the browser to your export endpoint, e.g.:
            _nav.NavigateTo("/admin/users/export?format=csv", forceLoad: true);
        }
    }
}

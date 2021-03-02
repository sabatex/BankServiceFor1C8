using BankServiceFor1C8.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BankServiceFor1C8.Shared
{
    public partial class MainLayout
    {
        private bool _drawerOpen = true;
        
        void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }

        protected override void OnInitialized()
        {
            _currentTheme=_defaultTheme;
        }

        protected override async Task OnParametersSetAsync()
        {
            // var currentCulture = await LocalStore.GetLocalValue(cultureSelector);
            // if (string.IsNullOrWhiteSpace(currentCulture) || !culturePresents.ContainsKey(currentCulture))
            // {
            //     currentCulture = defaultCulture;
            //     await LocalStore.SetLocalValue(cultureSelector, currentCulture);
            // }
            //setLocalCulture(currentCulture);

            //cs = await sabatexLocalizer.GetCulture();

            await base.OnParametersSetAsync();
        }

        #region Theme
        private MudTheme _currentTheme = new MudTheme();
        private readonly MudTheme _defaultTheme =
            new MudTheme()
            {
                Palette = new Palette()
                {
                    Black = "#272c34"
                }
            };
        private readonly MudTheme _darkTheme =
            new MudTheme()
            {
                Palette = new Palette()
                {
                    Black = "#27272f",
                    Background = "#32333d",
                    BackgroundGrey = "#27272f",
                    Surface = "#373740",
                    DrawerBackground = "#27272f",
                    DrawerText = "rgba(255,255,255, 0.50)",
                    AppbarBackground = "#27272f",
                    AppbarText = "rgba(255,255,255, 0.70)",
                    TextPrimary = "rgba(255,255,255, 0.70)",
                    TextSecondary = "rgba(255,255,255, 0.50)",
                    ActionDefault = "#adadb1",
                    ActionDisabled = "rgba(255,255,255, 0.26)",
                    ActionDisabledBackground = "rgba(255,255,255, 0.12)",
                    DrawerIcon = "rgba(255,255,255, 0.50)"
                }
            };
        private void SwithTheme()
        {
            if (_currentTheme == _defaultTheme)
            {
                _currentTheme = _darkTheme;
            }
            else
            {
                _currentTheme = _defaultTheme;
            }
        }
        #endregion
    }
}

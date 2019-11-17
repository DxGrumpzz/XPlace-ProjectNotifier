﻿namespace ProjectNotifier.XPlace.Client
{
    using ProjectNotifier.XPlace.Core;

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    ///
    /// </summary>
    public class SettingsListViewModel : BaseViewModel
    {

        public List<SettingIconViewModel> SettingIcons { get; set; }


        public SettingsListViewModel()
        {

            SettingIcons = new List<SettingIconViewModel>()
            {
                new SettingIconViewModel()
                {
                    Description = "הגדרות אפליקצייה",
                    Icon = SettingIcon.ApplicationSettings,

                    GotoSettingCommand = new RelayCommand(() =>
                    {
                        var s = DI.GetService<ProjectsPageViewModel>().SettingsViewModel.CurrentSettingsPage = new AppSettingsView()
                        {
                            ViewModel = new AppSettingsViewModel(),
                        };
                    }),
                },

                new SettingIconViewModel()
                {
                    Description = "הגדרות משתמש",
                    Icon = SettingIcon.UserSettings,

                    GotoSettingCommand = new RelayCommand(() =>
                    {
                        var ss = DI.GetService<IClientDataStore>().GetUserProfile();

                        var s = DI.GetService<ProjectsPageViewModel>().SettingsViewModel.CurrentSettingsPage = new UserSettingsView()
                        {
                            ViewModel = new UserSettingsViewModel()
                            {
                                ProjectPreferences = new ObservableCollection<UserProjectPreferenceItemViewModel>(
                                    ss.UserProjectPreferences
                                    .Select(projectType => new UserProjectPreferenceItemViewModel()
                                    {
                                        ProjectType = projectType.ProjectType,
                                    })),

                                
                            },
                        };
                    }),
                },
            };
        }
    };
};

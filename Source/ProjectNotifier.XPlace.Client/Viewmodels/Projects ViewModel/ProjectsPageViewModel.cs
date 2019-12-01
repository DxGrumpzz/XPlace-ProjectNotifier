﻿namespace ProjectNotifier.XPlace.Client
{
    using Microsoft.AspNetCore.SignalR.Client;

    using ProjectNotifier.XPlace.Core;

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    /// <summary>
    /// 
    /// </summary>
    public class ProjectsPageViewModel : BaseViewModel
    {

        #region Private fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ObservableCollection<ProjectItemViewModel> _projectList;

        #endregion


        #region Public properties

        public ObservableCollection<ProjectItemViewModel> ProjectList
        {
            get => _projectList;
            set
            {
                _projectList = value;
                OnPropertyChanged();
            }
        }

        public SettingsViewModel SettingsViewModel { get; set; }

        #endregion


        #region Commands

        public ICommand OpenSettingsCommand { get; }
        public ICommand ProjectListScrolledCommand { get; }

        #endregion


        public ProjectsPageViewModel()
        {
            SettingsViewModel = new SettingsViewModel();


            // Open settings page when user clicks the settings button
            OpenSettingsCommand = new RelayCommand(SettingsViewModel.OpenSettings);

            ProjectListScrolledCommand = new RelayCommand<ScrollChangedEventArgs>(ExecuteProjectListScrolled);


            // Bind hub events
            DI.GetService<IServerConnection>().ProjectsHub.On<IEnumerable<ProjectModel>>("ProjectListUpdated", ProjectListUpdated);
        }


        #region Command Callbacks


        /// <summary>
        /// A command that will be executed when the project list scrollviewer is scrolled
        /// </summary>
        /// <param name="eventArg"></param>
        private void ExecuteProjectListScrolled(ScrollChangedEventArgs eventArg)
        {
            // Get the event source/sender (the scrollviwer object)
            ScrollViewer sender = eventArg.Source as ScrollViewer;
            
            
            var appSettings = DI.ClientAppSettings();

            // Check if loading the projects is even neccessary 
            if (ProjectList.Count < appSettings.ProjectsToDisplay)
            {
                
                // Check if scrollbar is at the bottom
                if (eventArg.VerticalOffset + eventArg.ViewportHeight == eventArg.ExtentHeight)
                {
                    var userPrefferdProjects = DI.GetService<IClientCache>().UserPrefferedProjectsCache;

                    // The number of projects avaiable to load
                    int projectsCount = appSettings.ProjectsToDisplay - ProjectList.Count;

                    // How big of a chuck to take out of the cached project list
                    int chunccSize = 5 > projectsCount ?
                        projectsCount :
                        5;

                    // Check if in project loading chunk is within the bounds of the UserPrefferedProjectsCache 
                    if ((chunccSize + ProjectList.Count) >= userPrefferdProjects.Count())
                    {
                        // If not try to get last remaning projects 
                        chunccSize = userPrefferdProjects.Count() - ProjectList.Count;

                        // It there are none
                        if (chunccSize <= 0)
                            // Exit method
                            return;
                    };

                    // Get a chunk out of the cached projectlist
                    userPrefferdProjects
                    // Convert it to a List<> to get more functionality
                    .ToList()
                    // Get a "Chuck" that contians the missing projects
                    .GetRange(ProjectList.Count, chunccSize)
                    // For every project
                    .ForEach(project =>
                    {
                        // Add it to the project list
                        ProjectList.Add(new ProjectItemViewModel()
                        {
                            ProjectModel = project,
                        });
                    });
                };
            };
        }

        #endregion


        #region Public helpers

        /// <summary>
        /// Updates/refreshes the displayed project list depending on the the user's project preferences/> 
        /// </summary>
        public void UpdateProjectsList()
        {
            var preferredProjectsList = DI.GetService<IClientCache>().UserPrefferedProjectsCache;

            // Update current project list 
            ProjectList = new ObservableCollection<ProjectItemViewModel>(
            preferredProjectsList
            // Convert the ProjectModels to a ProjectItemViewModel
            .Select((p) =>
            new ProjectItemViewModel()
            {
                ProjectModel = p,
            })
            // Take the first 10
            .Take(10));
        }

        #endregion


        #region Private helpers
        private void ProjectListUpdated(IEnumerable<ProjectModel> projects)
        {
            // Get cache
            var clientCache = DI.GetService<IClientCache>();

            // Update project list cache 
            clientCache.ProjectListCache = projects;

            // Load new project list
            ProjectList = new ObservableCollection<ProjectItemViewModel>(
                clientCache.UserPrefferedProjectsCache.Select((project) => 
                new ProjectItemViewModel()
                {
                    ProjectModel = project,
                })
                .Take(DI.ClientAppSettings().ProjectsToDisplay));

            // Display new projects notification
            DI.UIManager().ShowProjectNotification(
                // Take first 8 projects that appeal to the user
                clientCache.UserPrefferedProjectsCache.Take(8));
        }

        #endregion

    };
};
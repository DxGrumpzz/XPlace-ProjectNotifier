﻿namespace ProjectNotifier.XPlace.Client
{
    using ProjectNotifier.XPlace.Core;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;


    /// <summary>
    ///
    /// </summary>
    public class ProjectPreferenceSelectionMenuViewModel : BaseViewModel
    {

        public static ProjectPreferenceSelectionMenuViewModel DesignInstance => new ProjectPreferenceSelectionMenuViewModel(null)
        {
            IsMenuOpen = true,
        };


        #region Private fields

        private bool _isMenuOpen;

        #endregion


        #region Public properties

        /// <summary>
        /// The list of project types that the user can select from 
        /// </summary>
        public ObservableCollection<ProjectTypes> AvailableProjectTypes { get; set; }

        /// <summary>
        /// A boolean flag that indicates if the menu is open
        /// </summary>
        public bool IsMenuOpen
        {
            get => _isMenuOpen;
            set
            {
                _isMenuOpen = value;
                OnPropertyChanged();
            }
        }

        #endregion


        #region Commands

        public ICommand CloseMenuCommand { get; }

        #endregion


        /// <summary>
        /// Default constructor, Takes a list of the user's already selected/prefered project types
        /// </summary>
        /// <param name="userProjects"> The user's prefered project types </param>
        public ProjectPreferenceSelectionMenuViewModel(IEnumerable<ProjectTypes> userProjects)
        {
            // Load available projects
            AvailableProjectTypes = new ObservableCollection<ProjectTypes>(GetNeccessaryProjects(userProjects));


            CloseMenuCommand = new RelayCommand(ExecuteCloseMenuCommand);
        }

        

        private void ExecuteCloseMenuCommand()
        {
            IsMenuOpen = false;
        }


        #region Private helpers

        /// <summary>
        /// Retrieves a list of preffered project types depending on the user's already existing list
        /// </summary>
        /// <param name="userProjects"> The list of the user's preffered projects </param>
        private IEnumerable<ProjectTypes> GetNeccessaryProjects(IEnumerable<ProjectTypes> userProjects)
        {
            // Get a array of integers representing each value in ProjectTypes
            var projectTypeNumericValues = ((int[])Enum.GetValues(typeof(ProjectTypes)))
                .Select(projectTypeValue => (ProjectTypes)projectTypeValue);

            // Get only the project types that aren't in the user's projects list
            var differences = projectTypeNumericValues.Except(userProjects);

            return differences;
        }

        #endregion
    };
};

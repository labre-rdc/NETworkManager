using System.Windows;
using NETworkManager.Controls;
using NETworkManager.Profiles;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using NETworkManager.Models;

namespace NETworkManager.ViewModels;

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using log4net;
using Models.Network;
using Settings;
using ConfigurationManager = System.Configuration.ConfigurationManager;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Localization.Resources;
using Models.Firewall;
using Utilities;

/// <summary>
/// Acts as the ViewModel responsible for controlling and managing firewall-related operations and data.
/// </summary>
/// <remarks>
/// This class bridges the gap between the user interface and the backend logic for firewall management,
/// offering mechanisms to handle firewall rules, exceptions, and other configuration options.
/// </remarks>
public class FirewallViewModel : ViewModelBase, IProfileManager
{
    #region Variables

    /// <summary>
    /// Represents a logging mechanism used to record runtime details, including informational messages,
    /// warnings, and errors, within the scope of the application.
    /// </summary>
    private static readonly ILog Log = LogManager.GetLogger(typeof(FirewallViewModel));

    /// <summary>
    /// Instance of a dialog coordinator used for managing and displaying dialogs
    /// associated with the current context or view model.
    /// </summary>
    private readonly IDialogCoordinator _dialogCoordinator;

    /// <summary>
    /// Dispatcher timer utilized to schedule and manage delayed execution of search operations,
    /// optimizing performance and responsiveness in the associated class.
    /// </summary>
    private readonly DispatcherTimer _searchDispatcherTimer = new();

    /// <summary>
    /// Represents an internal flag used to enable or disable the search functionality
    /// temporarily, preventing search operations while modifications or updates are performed.
    /// </summary>
    private bool _searchDisabled;


    /// <summary>
    /// Indicates whether the initial data or resources have been loaded for the current context.
    /// This variable is typically used to ensure that certain operations are only performed
    /// during the first load cycle, preventing redundant executions.
    /// </summary>
    private bool _firstLoad = true;

    /// <summary>
    /// Boolean flag indicating whether the specific process or feature is in a closed state.
    /// </summary>
    private bool _closed;

    /// <summary>
    /// Indicates whether a loading operation is currently in progress.
    /// </summary>
    private bool _isLoading;

    /// <summary>
    /// Indicates whether the current view is active and visible to the user.
    /// Used to manage state and behavior specific to the view's lifecycle.
    /// </summary>
    private bool _isViewActive = true;

    /// <summary>
    /// Represents the currently selected firewall rule in the context of the
    /// associated view model. Used for binding and operations specific to
    /// the selected rule.
    /// </summary>
    private FirewallRuleViewModel _selectedRule;

    /// <summary>
    /// Represents the currently selected rule within the context of the associated view model or system component.
    /// </summary>
    public FirewallRuleViewModel SelectedRule
    {
        get => _selectedRule;
        set
        {
            if (value == _selectedRule)
                return;

            _selectedRule = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Collection that holds the firewall rules associated with the <see cref="FirewallViewModel"/>.
    /// </summary>
    private ObservableCollection<FirewallRuleViewModel> _firewallRules = [];

    /// <summary>
    /// Represents a collection of rules used to configure and manage the behavior of a firewall.
    /// Each rule defines specific conditions and actions for network traffic filtering and security enforcement.
    /// </summary>
    public ObservableCollection<FirewallRuleViewModel> FirewallRules
    {
        get => _firewallRules;
        set
        {
            if (Equals(value, _firewallRules))
                return;

            _firewallRules = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Collection of rules currently selected by the user, utilized for processing or
    /// applying specific operations within the context of the associated functionality.
    /// </summary>
    private IList _selectedRules;

    /// <summary>
    /// Represents the collection of firewall rules currently selected by the user
    /// within the context of the <see cref="FirewallViewModel"/> class.
    /// </summary>
    public IList SelectedRules
    {
        get => _selectedRules;
        set
        {
            if (value ==  _selectedRules)
                return;

            _selectedRules = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Indicates whether the configuration process is currently active.
    /// Used to track the state of the system during configuration operations.
    /// </summary>
    private bool _isConfigurationRunning;

    /// <summary>
    /// Indicates whether the configuration process is currently active.
    /// This property helps monitor and manage the state of ongoing configuration tasks.
    /// </summary>
    public bool IsConfigurationRunning
    {
        get => _isConfigurationRunning;
        set
        {
            if (value == _isConfigurationRunning)
                return;

            _isConfigurationRunning = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Indicates whether the status message is currently displayed in the user interface.
    /// </summary>
    private bool _isStatusMessageDisplayed;

    /// <summary>
    /// Indicates whether the status message is currently being displayed in the user interface.
    /// </summary>
    public bool IsStatusMessageDisplayed
    {
        get => _isStatusMessageDisplayed;
        set
        {
            if (value == _isStatusMessageDisplayed)
                return;

            _isStatusMessageDisplayed = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Represents the current status message, typically used to convey feedback,
    /// alerts, or informational updates to the user within the application.
    /// </summary>
    private string _statusMessage;

    /// <summary>
    /// Represents a message that describes the current status or outcome of an operation
    /// within the context of the associated class or application.
    /// </summary>
    public string StatusMessage
    {
        get => _statusMessage;
        private set
        {
            if (value == _statusMessage)
                return;

            _statusMessage = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Indicates whether the current entity or component can be configured.
    /// This flag is typically used to enable or restrict configuration operations
    /// based on certain conditions or states within the application.
    /// </summary>
    private bool _canConfigure;

    /// <summary>
    /// Indicates whether the current user or context has the necessary permissions
    /// to configure settings or perform configuration-related actions.
    /// </summary>
    public bool CanConfigure
    {
        get => _canConfigure;
        set
        {
            if (value == _canConfigure)
                return;

            _canConfigure = value;
            OnPropertyChanged();
        }
    }

    #region Profiles

    /// <summary>
    /// Collection of profiles used to maintain user-specific or application-specific settings
    /// and configurations within the relevant context.
    /// </summary>
    private ICollectionView _profiles;

    /// <summary>
    /// Collection of user or system profiles associated with the current instance,
    /// providing access to profile-specific data and configurations.
    /// </summary>
    public ICollectionView Profiles
    {
        get => _profiles;
        private set
        {
            if (value == _profiles)
                return;

            _profiles = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Represents the currently selected user profile within the application,
    /// providing access to its associated data and functionality.
    /// </summary>
    private ProfileInfo _selectedProfile = new();

    /// <summary>
    /// Represents the currently selected user profile in the system or application,
    /// allowing access to profile-specific settings, preferences, or data.
    /// </summary>
    public ProfileInfo SelectedProfile
    {
        get => _selectedProfile;
        set
        {
            if (value == _selectedProfile)
                return;

            if (value != null)
            {
                // TODO: Create and set profile settings
            }

            _selectedProfile = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Represents the search query or filter criteria used to retrieve specific data
    /// or information within a dataset or collection.
    /// </summary>
    private string _search;

    /// <summary>
    /// Represents the search query or functionality used to filter or locate specific data
    /// within the context of the containing class.
    /// </summary>
    public string Search
    {
        get => _search;
        set
        {
            if (value == _search)
                return;

            _search = value;

            // Start searching...
            if (!_searchDisabled)
            {
                IsSearching = true;
                _searchDispatcherTimer.Start();
            }

            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Indicates whether a search operation is currently in progress.
    /// </summary>
    private bool _isSearching;

    /// <summary>
    /// Indicates whether a search operation is currently in progress within the associated context
    /// or functionality of the class.
    /// </summary>
    public bool IsSearching
    {
        get => _isSearching;
        set
        {
            if (value == _isSearching)
                return;

            _isSearching = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Indicates whether the profile filter panel is currently open or closed in the user interface.
    /// Used to manage the visibility state of the profile filter component.
    /// </summary>
    private bool _profileFilterIsOpen;

    /// <summary>
    /// Indicates whether the profile filter panel is currently open or closed.
    /// This property is used to control the visibility state of the profile filter UI element
    /// in the associated view or component.
    /// </summary>
    public bool ProfileFilterIsOpen
    {
        get => _profileFilterIsOpen;
        set
        {
            if (value == _profileFilterIsOpen)
                return;

            _profileFilterIsOpen = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Represents a view model that handles the display and management of profile filter tags
    /// within the application's user interface, enabling users to customize and refine data filtering
    /// based on specific tag selections.
    /// </summary>
    public ICollectionView ProfileFilterTagsView { get; set; }

    /// <summary>
    /// A collection of tags used to filter and categorize user profiles based on specific metadata.
    /// </summary>
    public ObservableCollection<ProfileFilterTagsInfo> ProfileFilterTags { get; set; } = [];

    /// <summary>
    /// A collection of tags used to filter user profiles based on whether any of the tags
    /// match the specified criteria. Profiles are included if they match at least one tag
    /// from this collection.
    /// </summary>
    private bool _profileFilterTagsMatchAny = GlobalStaticConfiguration.Profile_TagsMatchAny;

    /// <summary>
    /// Specifies whether the profile filter matches any of the tags provided.
    /// This property evaluates to true if any tag within the filter criteria aligns
    /// with the tags associated with the profile being evaluated.
    /// </summary>
    public bool ProfileFilterTagsMatchAny
    {
        get => _profileFilterTagsMatchAny;
        set
        {
            if (value == _profileFilterTagsMatchAny)
                return;

            _profileFilterTagsMatchAny = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Indicates whether the profile filter tags should be matched against all specified criteria.
    /// When set to true, all tags must match; otherwise, a partial match is sufficient.
    /// </summary>
    private bool _profileFilterTagsMatchAll;

    /// <summary>
    /// Represents the collection of filter tags that must all match in order to include a profile.
    /// This property enforces strict matching criteria by requiring every tag in the collection
    /// to be satisfied for the profile to be considered valid according to the applied filter.
    /// </summary>
    public bool ProfileFilterTagsMatchAll
    {
        get => _profileFilterTagsMatchAll;
        set
        {
            if (value == _profileFilterTagsMatchAll)
                return;

            _profileFilterTagsMatchAll = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Indicates whether the profile filter is currently set
    /// </summary>
    private bool _isProfileFilterSet;

    /// <summary>
    /// Indicates whether the profile filter has been configured or applied
    /// in the current context.
    /// </summary>
    public bool IsProfileFilterSet
    {
        get => _isProfileFilterSet;
        set
        {
            if (value == _isProfileFilterSet)
                return;

            _isProfileFilterSet = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Stores the state information of group expanders, allowing for tracking
    /// and persistence of their expanded or collapsed statuses throughout the application.
    /// </summary>
    private readonly GroupExpanderStateStore _groupExpanderStateStore = new();

    /// <summary>
    /// Represents a storage mechanism for maintaining the expanded or collapsed state
    /// of groups within a user interface that supports hierarchical or grouped views.
    /// </summary>
    public GroupExpanderStateStore GroupExpanderStateStore => _groupExpanderStateStore;

    /// <summary>
    /// Indicates whether the profile width can be modified dynamically during runtime.
    /// This variable typically reflects configuration settings or operational conditions
    /// that determine if adjustments to the profile width are allowed.
    /// </summary>
    private bool _canProfileWidthChange = true;

    /// <summary>
    /// Represents the temporary width value of a profile, used for intermediate calculations
    /// or adjustments before being finalized or applied.
    /// </summary>
    private double _tempProfileWidth;

    /// <summary>
    /// A boolean flag that determines whether the profile view in the UI
    /// should be expanded or collapsed.
    /// </summary>
    private bool _expandProfileView;

    /// <summary>
    /// Indicates whether the profile view should be expanded, providing a toggle mechanism
    /// for showing or hiding additional profile details in the UI.
    /// </summary>
    public bool ExpandProfileView
    {
        get => _expandProfileView;
        set
        {
            if (value == _expandProfileView)
                return;

            if (!_isLoading)
                SettingsManager.Current.Firewall_ExpandProfileView = value;

            _expandProfileView = value;

            if (_canProfileWidthChange)
                ResizeProfile(false);

            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Represents the width of the user profile area, used to define layout dimensions
    /// or UI element scaling related to profile display components.
    /// </summary>
    private GridLength _profileWidth;

    /// <summary>
    /// Represents the width of the profile, typically used to define or configure
    /// dimensions in a specific context where profile measurements are required.
    /// </summary>
    public GridLength ProfileWidth
    {
        get => _profileWidth;
        set
        {
            if (value == _profileWidth)
                return;

            if (!_isLoading && Math.Abs(value.Value - GlobalStaticConfiguration.Profile_WidthCollapsed) >
                GlobalStaticConfiguration.Profile_FloatPointFix) // Do not save the size when collapsed
                SettingsManager.Current.Firewall_ProfileWidth = value.Value;

            _profileWidth = value;

            if (_canProfileWidthChange)
                ResizeProfile(true);

            OnPropertyChanged();
        }
    }
    #endregion

    #endregion

    #region Constructor

    /// <summary>
    /// Encapsulates the logic and data binding required to handle
    /// firewall configuration and visualization in the user interface.
    /// </summary>
    public FirewallViewModel(IDialogCoordinator instance)
    {
        _dialogCoordinator = instance;

        // Profiles
        CreateTags();

        ProfileFilterTagsView = CollectionViewSource.GetDefaultView(ProfileFilterTags);
        ProfileFilterTagsView.SortDescriptions.Add(new SortDescription(nameof(ProfileFilterTagsInfo.Name),
            ListSortDirection.Ascending));

        SetProfilesView(new ProfileFilterInfo());

        ProfileManager.OnProfilesUpdated += ProfileManager_OnProfilesUpdated;

        _searchDispatcherTimer.Interval = GlobalStaticConfiguration.SearchDispatcherTimerTimeSpan;
        _searchDispatcherTimer.Tick += SearchDispatcherTimer_Tick;

        LoadSettings();

        _isLoading = false;
    }
    #endregion
    
    
    #region Methods
    
    #region Events

    /// <summary>
    /// Handles the logic to be executed when a close action is initiated,
    /// such as closing the current window or dialog, and performing any necessary cleanup or state updates.
    /// </summary>
    public void OnClose()
    {
        // Prevent multiple calls
        if (_closed)
            return;

        _closed = true;
    }

    /// <summary>
    /// Invoked when a view is hidden from the user interface.
    /// </summary>
    /// <remarks>
    /// This method can be overridden to handle cleanup tasks or update the
    /// application state when a view is no longer visible. It is typically
    /// triggered during the hiding phase of a view lifecycle.
    /// </remarks>
    /// <param name="viewId">
    /// The unique identifier of the view that is being hidden.
    /// </param>
    /// <param name="context">
    /// Additional contextual information associated with the view being hidden,
    /// which may include state data or metadata relevant to the operation.
    /// </param>
    public void OnViewHide()
    {
        
    }

    /// <summary>
    /// Method triggered when the associated view becomes visible.
    /// Typically used to initialize or update the state of the view
    /// or load necessary data for display.
    /// </summary>
    public void OnViewVisible()
    {
    }

    /// Handles the ProfilesUpdated event from the ProfileManager.
    /// This method is triggered when the profiles are updated and ensures the profile tags are refreshed and the profiles view is updated.
    /// <param name="sender">The source of the event, typically the ProfileManager instance.</param>
    /// <param name="e">An EventArgs object that contains no event data.</param>
    private void ProfileManager_OnProfilesUpdated(object sender, EventArgs e)
    {
        CreateTags();

        RefreshProfiles();
    }

    /// <summary>
    /// Handles the Tick event of the SearchDispatcherTimer.
    /// Responsible for executing periodic operations related to the search functionality,
    /// such as updating search results or triggering dependent actions at the timer's interval.
    /// </summary>
    /// <param name="sender">The source of the event, typically the timer object.</param>
    /// <param name="e">An instance of EventArgs containing event data.</param>
    private void SearchDispatcherTimer_Tick(object sender, EventArgs e)
    {
        _searchDispatcherTimer.Stop();

        RefreshProfiles();

        IsSearching = false;
    }

    /// <summary>
    /// Indicates whether the user has canceled an operation associated with the firewall settings.
    /// </summary>
    private void Firewall_UserHasCanceled(object sender, EventArgs e)
    {
        StatusMessage = Strings.CanceledByUserMessage;
        IsStatusMessageDisplayed = true;
    }

    #endregion Events

    #region ProfileMethods

    /// <summary>
    /// Provides functionality to manage and adjust the size of a user profile
    /// or related visual elements based on specified parameters or constraints.
    /// </summary>
    private void ResizeProfile(bool dueToChangedSize)
    {
        _canProfileWidthChange = false;

        if (dueToChangedSize)
        {
            ExpandProfileView = Math.Abs(ProfileWidth.Value - GlobalStaticConfiguration.Profile_WidthCollapsed) >
                                GlobalStaticConfiguration.Profile_FloatPointFix;
        }
        else
        {
            if (ExpandProfileView)
            {
                ProfileWidth =
                    Math.Abs(_tempProfileWidth - GlobalStaticConfiguration.Profile_WidthCollapsed) <
                    GlobalStaticConfiguration.Profile_FloatPointFix
                        ? new GridLength(GlobalStaticConfiguration.Profile_DefaultWidthExpanded)
                        : new GridLength(_tempProfileWidth);
            }
            else
            {
                _tempProfileWidth = ProfileWidth.Value;
                ProfileWidth = new GridLength(GlobalStaticConfiguration.Profile_WidthCollapsed);
            }
        }

        _canProfileWidthChange = true;
    }

    /// <summary>
    /// Populates and updates the list of profile filter tags based on the tags present
    /// in the profiles where the firewall is enabled.
    /// </summary>
    /// <remarks>
    /// This method retrieves all tags associated with profiles in all groups
    /// that have the firewall enabled. It ensures the `ProfileFilterTags` collection
    /// is synchronized with the current set of tags, removing any outdated tags and
    /// adding any new ones. Duplicate tags are not allowed, and the tags are maintained
    /// in a consistent state.
    /// </remarks>
    private void CreateTags()
    {
        var tags = ProfileManager.Groups.SelectMany(x => x.Profiles).Where(x => x.Firewall_Enabled)
            .SelectMany(x => x.TagsCollection).Distinct().ToList();

        var tagSet = new HashSet<string>(tags);

        for (var i = ProfileFilterTags.Count - 1; i >= 0; i--)
        {
            if (!tagSet.Contains(ProfileFilterTags[i].Name))
                ProfileFilterTags.RemoveAt(i);
        }

        var existingTagNames = new HashSet<string>(ProfileFilterTags.Select(ft => ft.Name));

        foreach (var tag in tags.Where(tag => !existingTagNames.Contains(tag)))
        {
            ProfileFilterTags.Add(new ProfileFilterTagsInfo(false, tag));
        }
    }

    /// <summary>
    /// Represents the View responsible for configuring and displaying
    /// profile settings within the application.
    /// </summary>
    private void SetProfilesView(ProfileFilterInfo filter, ProfileInfo profile = null)
    {
        Profiles = new CollectionViewSource
        {
            Source = ProfileManager.Groups.SelectMany(x => x.Profiles).Where(x => x.Firewall_Enabled && (
                    string.IsNullOrEmpty(filter.Search) ||
                    x.Name.IndexOf(filter.Search, StringComparison.OrdinalIgnoreCase) > -1) && (
                    // If no tags are selected, show all profiles
                    (!filter.Tags.Any()) ||
                    // Any tag can match
                    (filter.TagsFilterMatch == ProfileFilterTagsMatch.Any &&
                     filter.Tags.Any(tag => x.TagsCollection.Contains(tag))) ||
                    // All tags must match
                    (filter.TagsFilterMatch == ProfileFilterTagsMatch.All &&
                     filter.Tags.All(tag => x.TagsCollection.Contains(tag))))
            ).OrderBy(x => x.Group).ThenBy(x => x.Name)
        }.View;

        Profiles.GroupDescriptions.Add(new PropertyGroupDescription(nameof(ProfileInfo.Group)));

        // Set specific profile or first if null
        SelectedProfile = null;

        if (profile != null)
            SelectedProfile = Profiles.Cast<ProfileInfo>().FirstOrDefault(x => x.Equals(profile)) ??
                              Profiles.Cast<ProfileInfo>().FirstOrDefault();
        else
            SelectedProfile = Profiles.Cast<ProfileInfo>().FirstOrDefault();
    }

    /// <summary>
    /// Refreshes the list of user profiles by retrieving the latest data
    /// and updating the application state accordingly.
    /// </summary>
    private void RefreshProfiles()
    {
        if (!_isViewActive)
            return;

        var filter = new ProfileFilterInfo
        {
            Search = Search,
            Tags = [.. ProfileFilterTags.Where(x => x.IsSelected).Select(x => x.Name)],
            TagsFilterMatch = ProfileFilterTagsMatchAny ? ProfileFilterTagsMatch.Any : ProfileFilterTagsMatch.All
        };

        SetProfilesView(filter, SelectedProfile);

        IsProfileFilterSet = !string.IsNullOrEmpty(filter.Search) || filter.Tags.Any();
    }

    #endregion

    #region Action and commands

    /// <summary>
    /// Command responsible for applying the current configuration settings,
    /// ensuring the changes are validated and executed within the context of the application.
    /// </summary>
    public ICommand ApplyConfigurationCommand =>
        new RelayCommand(_ => ApplyConfigurationAction(), ApplyConfiguration_CanExecute);

    /// <summary>
    /// Determines whether the apply configuration command can execute
    /// based on the current state of the application.
    /// </summary>
    /// <param name="parameter">An optional parameter used for evaluating the command's execution status.</param>
    /// <returns>True if the command can execute; otherwise, false.</returns>
    private bool ApplyConfiguration_CanExecute(object parameter)
    {
        return Application.Current.MainWindow != null &&
               !((MetroWindow)Application.Current.MainWindow).IsAnyDialogOpen; //&&
//               !ConfigurationManager.Current.IsChildWindowOpen;
    }

    /// <summary>
    /// Represents an action responsible for applying configuration settings
    /// to a specified system or component. It encapsulates the logic required
    /// to validate, process, and enforce the provided configuration details.
    /// </summary>
    private void ApplyConfigurationAction()
    {
        ApplyConfiguration().ConfigureAwait(false);
    }

    /// <summary>
    /// Command responsible for applying the profile configuration settings.
    /// This command encapsulates the logic required to execute changes to
    /// user-defined or system-defined profile configurations.
    /// </summary>
    public ICommand ApplyProfileConfigCommand => new RelayCommand(_ => ApplyProfileProfileAction());

    /// <summary>
    /// Executes the action that applies the configuration settings stored in the selected profile.
    /// </summary>
    /// <remarks>
    /// This method is invoked as part of the ApplyProfileConfigCommand, which is bound to a user action
    /// in the view layer. It asynchronously applies configuration settings from the currently
    /// selected profile to the appropriate system components.
    /// </remarks>
    private void ApplyProfileProfileAction()
    {
        ApplyConfigurationFromProfile().ConfigureAwait(false);
    }

    /// <summary>
    /// Command used to initiate the process of adding a new profile,
    /// facilitating the binding between the user interface and the
    /// corresponding business logic.
    /// </summary>
    public ICommand AddProfileCommand => new RelayCommand(_ => AddProfileAction());

    /// <summary>
    /// Represents an action that handles the addition of a new profile,
    /// including the associated logic and state management necessary
    /// to persist and update profile-related information.
    /// </summary>
    private void AddProfileAction()
    {
        ProfileDialogManager
            .ShowAddProfileDialog(Application.Current.MainWindow, this, null, null, ApplicationName.Firewall)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Determines whether the ModifyProfile command can execute based on the current
    /// state or conditions of the application.
    /// </summary>
    /// <param name="parameter">An optional parameter provided to evaluate the command's executability.</param>
    /// <returns>
    /// True if the command can execute; otherwise, false.
    /// </returns>
    private bool ModifyProfile_CanExecute(object obj)
    {
        return SelectedProfile is { IsDynamic: false };
    }

    /// <summary>
    /// Command responsible for handling the execution of the profile editing functionality
    /// within the associated view model.
    /// </summary>
    public ICommand EditProfileCommand => new RelayCommand(_ => EditProfileAction(), ModifyProfile_CanExecute);

    /// <summary>
    /// Handles the action to edit the currently selected profile in the application.
    /// This method invokes the profile editing dialog through the <see cref="ProfileDialogManager.ShowEditProfileDialog"/>
    /// and passes the current application window, the view model, and the selected profile to it.
    /// The dialog allows users to modify the details of the selected profile.
    /// </summary>
    private void EditProfileAction()
    {
        ProfileDialogManager.ShowEditProfileDialog(Application.Current.MainWindow, this, SelectedProfile)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Command encapsulating the logic to copy the current configuration or selected
    /// settings as a new profile within the application.
    /// </summary>
    public ICommand CopyAsProfileCommand => new RelayCommand(_ => CopyAsProfileAction(), ModifyProfile_CanExecute);

    /// <summary>
    /// Defines an action responsible for creating a copy of an existing profile
    /// in a system or application, preserving all configurations and settings
    /// associated with the original profile.
    /// </summary>
    private void CopyAsProfileAction()
    {
        ProfileDialogManager.ShowCopyAsProfileDialog(Application.Current.MainWindow, this, SelectedProfile)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Command that facilitates the deletion of a user profile
    /// by encapsulating the associated logic and providing
    /// the necessary bindings for UI interaction.
    /// </summary>
    public ICommand DeleteProfileCommand => new RelayCommand(_ => DeleteProfileAction(), ModifyProfile_CanExecute);

    /// <summary>
    /// Represents an action responsible for handling the deletion of a user or application profile,
    /// including any associated resources or configurations tied to that profile.
    /// </summary>
    private void DeleteProfileAction()
    {
        ProfileDialogManager
            .ShowDeleteProfileDialog(Application.Current.MainWindow, this, new List<ProfileInfo> { SelectedProfile })
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Command responsible for handling the editing of a group, typically invoked to open
    /// or manage the group editing interface and apply updated changes.
    /// </summary>
    public ICommand EditGroupCommand => new RelayCommand(EditGroupAction);

    /// <summary>
    /// Represents an action that allows editing a group, including modifying its properties
    /// and managing its associated members or permissions.
    /// </summary>
    /// <param name="groupId">The unique identifier of the group to be edited.</param>
    /// <param name="newGroupName">The new name to assign to the group.</param>
    /// <param name="updatedProperties">An object containing the updated properties for the group.</param>
    /// <param name="userId">The identifier of the user performing the edit action.</param>
    private void EditGroupAction(object group)
    {
        ProfileDialogManager
            .ShowEditGroupDialog(Application.Current.MainWindow, this, ProfileManager.GetGroupByName($"{group}"))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Command that encapsulates the logic for opening and applying a profile filter
    /// within the associated view model, allowing dynamic adjustments to profile display.
    /// </summary>
    public ICommand OpenProfileFilterCommand => new RelayCommand(_ => OpenProfileFilterAction());

    /// <summary>
    /// Defines an action responsible for filtering profiles based on specific criteria
    /// within a system or application, enabling targeted operations on the resulting set of profiles.
    /// </summary>
    private void OpenProfileFilterAction()
    {
        ProfileFilterIsOpen = true;
    }

    /// <summary>
    /// Command responsible for applying a profile-based filter to the data
    /// or view model, enabling targeted operations based on the selected profile criteria.
    /// </summary>
    public ICommand ApplyProfileFilterCommand => new RelayCommand(_ => ApplyProfileFilterAction());

    /// <summary>
    /// Defines an action responsible for applying a specific profile-based filter
    /// to a data set or collection, modifying its state based on the selected profile criteria.
    /// </summary>
    private void ApplyProfileFilterAction()
    {
        RefreshProfiles();

        ProfileFilterIsOpen = false;
    }

    /// <summary>
    /// Command used to clear the applied profile filters, resetting the view or data
    /// to its unfiltered state within the context of the associated ViewModel.
    /// </summary>
    public ICommand ClearProfileFilterCommand => new RelayCommand(_ => ClearProfileFilterAction());

    /// <summary>
    /// Represents an action responsible for clearing the applied profile filters,
    /// resetting the state to display unfiltered results.
    /// </summary>
    private void ClearProfileFilterAction()
    {
        _searchDisabled = true;
        Search = string.Empty;
        _searchDisabled = false;

        foreach (var tag in ProfileFilterTags)
            tag.IsSelected = false;

        RefreshProfiles();

        IsProfileFilterSet = false;
        ProfileFilterIsOpen = false;
    }

    /// <summary>
    /// Command used to trigger the expansion of all profile groups
    /// in the associated view model, typically for improving visibility
    /// and accessibility of group details.
    /// </summary>
    public ICommand ExpandAllProfileGroupsCommand => new RelayCommand(_ => ExpandAllProfileGroupsAction());

    /// <summary>
    /// Represents the action responsible for expanding all profile groups
    /// within a given context, typically used to enhance visibility or access
    /// to grouped profile information in the UI.
    /// </summary>
    private void ExpandAllProfileGroupsAction()
    {
        SetIsExpandedForAllProfileGroups(true);
    }

    /// <summary>
    /// Command responsible for collapsing all profile groups in the associated view model,
    /// typically used to improve user navigation and focus by reducing the visible complexity.
    /// </summary>
    public ICommand CollapseAllProfileGroupsCommand => new RelayCommand(_ => CollapseAllProfileGroupsAction());

    /// Executes the action to collapse all profile groups in the firewall view.
    /// This method sets the expansion state of all profile groups to false, effectively collapsing them.
    /// Remarks:
    /// - It utilizes the `SetIsExpandedForAllProfileGroups` method to modify the expansion state.
    /// - This action is typically invoked by the related command bound to the UI.
    /// Dependencies:
    /// - Requires access to the profile group expansion state via `SetIsExpandedForAllProfileGroups
    private void CollapseAllProfileGroupsAction()
    {
        SetIsExpandedForAllProfileGroups(false);
    }

    /// <summary>
    /// Updates the expanded state for all profile groups in the data set.
    /// </summary>
    /// <param name="isExpanded">A boolean value indicating whether all profile groups should be expanded (true) or collapsed (false).</param>
    private void SetIsExpandedForAllProfileGroups(bool isExpanded)
    {
        foreach (var group in Profiles.Groups.Cast<CollectionViewGroup>())
            GroupExpanderStateStore[group.Name.ToString()] = isExpanded;
    }

    /// <summary>
    /// Command responsible for adding a new rule within the context of the associated
    /// view model, facilitating user-initiated actions to define and apply configuration rules.
    /// </summary>
    public ICommand AddRuleCommand => new RelayCommand(_ => AddRuleAction());

    /// <summary>
    /// Represents an action that defines the addition of a new rule to a collection
    /// of rules, typically used for enforcing or configuring specific behaviors in a system.
    /// </summary>
    private void AddRuleAction()
    {
        AddRule();
    }

    /// <summary>
    /// Adds a new rule to the system's configuration or policy set.
    /// This method is designed to enforce or specify logic controlling
    /// access, permissions, or restrictions within the application.
    /// </summary>
    private void AddRule()
    {
        FirewallRules ??= [];

        FirewallRules.Add(new FirewallRuleViewModel());
    }

    /// <summary>
    /// Command responsible for handling the deletion of rules within the context
    /// of the associated ViewModel or business logic. Encapsulates the logic
    /// required to execute the rule removal process.
    /// </summary>
    public ICommand DeleteRulesCommand => new RelayCommand(_ => DeleteRulesAction());

    /// <summary>
    /// Represents an action responsible for handling the deletion of rules
    /// within a specific system or application context.
    /// </summary>
    private void DeleteRulesAction()
    {
        DeleteRules();
    }

    /// <summary>
    /// Deletes the selected firewall rules from the collection of firewall rules.
    /// </summary>
    /// <remarks>
    /// This method removes firewall rules based on the current selection in the view model.
    /// If multiple rules are selected, all of them will be removed from the <see cref="FirewallRules"/> collection.
    /// If a single rule is selected, it will be removed from the <see cref="FirewallRules"/> collection.
    /// No action is taken if no rules are selected.
    /// </remarks>
    private void DeleteRules()
    {
        if (SelectedRules is null && SelectedRule is null)
            return;
        if (SelectedRules is not null)
        {
            var rulesToDelete = SelectedRules.Cast<FirewallRuleViewModel>().ToList();
            foreach (FirewallRuleViewModel rule in rulesToDelete)
                FirewallRules?.Remove(rule);
            return;
        }

        if (SelectedRule is not null)
            FirewallRules?.Remove(SelectedRule);
    }
    #endregion

    #region Configuration

    /// <summary>
    /// Loads the application settings from an appropriate data source,
    /// such as a configuration file or a database, and initializes the
    /// settings required for proper application behavior.
    /// </summary>
    private void LoadSettings()
    {
    }

    /// <summary>
    /// Applies the provided configuration to the system by updating the necessary settings
    /// and triggering any required processes to reflect the changes.
    /// </summary>
    /// <param name="configuration">The configuration object containing the settings to be applied.</param>
    /// <returns>Returns a boolean value indicating whether the configuration was successfully applied.</returns>
    private async Task ApplyConfiguration()
    {
        IsConfigurationRunning = true;
        IsStatusMessageDisplayed = false;

        try
        {
            var firewall = new Firewall(ProfileManager.LoadedProfileFile?.Name);

            firewall.UserHasCanceled += Firewall_UserHasCanceled;

            var firewallRules = FirewallRules
                .Select(ruleVm => ruleVm.ToRule()).ToList();

            await firewall.ApplyRulesAsync(firewallRules);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            IsStatusMessageDisplayed = true;
        }
        finally
        {
            IsConfigurationRunning = false;
        }
    }

    /// <summary>
    /// Applies the configuration settings from a specified profile
    /// to the current system or application environment.
    /// </summary>
    /// <param name="profile">The profile containing the configuration settings to be applied.</param>
    /// <returns>True if the configuration is successfully applied; otherwise, false.</returns>
    private async Task ApplyConfigurationFromProfile()
    {
        IsConfigurationRunning = true;
        IsStatusMessageDisplayed = false;

        /*
        var subnetmask = SelectedProfile.NetworkInterface_Subnetmask;

        // CIDR to subnetmask
        if (SelectedProfile.NetworkInterface_EnableStaticIPAddress && subnetmask.StartsWith("/"))
            subnetmask = Subnetmask.GetFromCidr(int.Parse(subnetmask.TrimStart('/'))).Subnetmask;

        var enableStaticDNS = SelectedProfile.NetworkInterface_EnableStaticDNS;

        var primaryDNSServer = SelectedProfile.NetworkInterface_PrimaryDNSServer;
        var secondaryDNSServer = SelectedProfile.NetworkInterface_SecondaryDNSServer;

        // If primary and secondary DNS are empty --> autoconfiguration
        if (enableStaticDNS && string.IsNullOrEmpty(primaryDNSServer) && string.IsNullOrEmpty(secondaryDNSServer))
            enableStaticDNS = false;

        // When primary DNS is empty, swap it with secondary (if not empty)
        if (SelectedProfile.NetworkInterface_EnableStaticDNS && string.IsNullOrEmpty(primaryDNSServer) &&
            !string.IsNullOrEmpty(secondaryDNSServer))
        {
            primaryDNSServer = secondaryDNSServer;
            secondaryDNSServer = string.Empty;
        }
        */

        /*
        var config = new NetworkInterfaceConfig
        {
            Name = SelectedNetworkInterface.Name,
            EnableStaticIPAddress = SelectedProfile.NetworkInterface_EnableStaticIPAddress,
            IPAddress = SelectedProfile.NetworkInterface_IPAddress,
            Subnetmask = subnetmask,
            Gateway = SelectedProfile.NetworkInterface_Gateway,
            EnableStaticDNS = enableStaticDNS,
            PrimaryDNSServer = primaryDNSServer,
            SecondaryDNSServer = secondaryDNSServer
        };
        */

        try
        {
            /*
            var networkInterface = new NetworkInterface();

            networkInterface.UserHasCanceled += NetworkInterface_UserHasCanceled;

            await networkInterface.ConfigureNetworkInterfaceAsync(config);

            ReloadNetworkInterfaces();
        */
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            IsStatusMessageDisplayed = true;
        }
        finally
        {
            IsConfigurationRunning = false;
        }
    }
    #endregion
    
    #endregion Methods
}
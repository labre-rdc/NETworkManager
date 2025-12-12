using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using NETworkManager.Localization.Resources;
using NETworkManager.Models.Firewall;
using NETworkManager.Profiles;

namespace NETworkManager.ViewModels;

/// <summary>
/// Represents the view model for a firewall rule, encapsulating the properties,
/// behavior, and validation logic required to manage and display the configuration
/// and state of a firewall rule within the application.
/// </summary>
/// <remarks>
/// This class serves as a bridge between the user interface and the underlying
/// data model, providing all necessary information to bind, display, and modify
/// firewall rule properties. It may include additional logic for validation,
/// transformation, or interaction with the firewall rule data.
/// </remarks>
public class FirewallRuleViewModel : ViewModelBase
{
        #region Variables

        /// <summary>
        /// Represents the underlying firewall rule associated with the configuration.
        /// </summary>
        /// <remarks>
        /// This private member serves as the data model for the firewall rule being configured.
        /// It is used to store and manipulate properties such as name, protocol, direction, action,
        /// local and remote ports, and network category, among others. Changes made to the rule data are reflected
        /// in the associated view model logic, enabling synchronization between the UI and the firewall rule.
        /// </remarks>
        [NotNull]
        private readonly FirewallRule _rule = new();

        /// <summary>
        /// Represents the name or identifier associated with an entity or object.
        /// </summary>
        /// <remarks>
        /// This property is used to store and retrieve the descriptive or unique name assigned
        /// to the corresponding entity within the system.
        /// </remarks>
        public string Name
        {
            get => _rule.Name;
            set
            {
                if (value == _rule.Name)
                    return;
                _rule.Name = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Represents the detailed information or metadata associated with an object or entity.
        /// </summary>
        /// <remarks>
        /// This property provides a textual explanation or summary, often used to describe the purpose,
        /// content, or functionality of an object. It is typically utilized in applications for displaying
        /// user-friendly descriptions or for debugging purposes.
        /// </remarks>
        public string Description { get; set; }

        /// <summary>
        /// Stores a user-defined name for a firewall rule.
        /// </summary>
        /// <remarks>
        /// This field holds a custom name that can be assigned to a firewall rule by the user.
        /// It serves as a basis for generating the overall rule name and can be updated dynamically
        /// within the application. The user-defined name takes priority over other naming components
        /// when specified.
        /// </remarks>
        private string _userDefinedName;

        /// <summary>
        /// Specifies a custom name defined by the user.
        /// </summary>
        /// <remarks>
        /// This property allows users to assign a meaningful name that aids in identifying or categorizing the associated entity.
        /// The value is defined and managed externally by the user.
        /// </remarks>
        public string UserDefinedName
        {
            get => _userDefinedName;
            set
            {
                if (value == _userDefinedName)
                    return;
                _userDefinedName = value;
                OnPropertyChanged();
                UpdateRuleName();
            }
        }

        /// <summary>
        /// Specifies the default name associated with the configuration or entity.
        /// </summary>
        /// <remarks>
        /// This property is intended to provide a predefined or fallback name when no custom name is supplied.
        /// It is typically used in scenarios where a default identifier is required.
        /// </remarks>
        public string DefaultName { get; private set; }

        /// <summary>
        /// Specifies the communication protocol to be used for data transmission.
        /// </summary>
        /// <remarks>
        /// This property defines the protocol standard such as HTTP, FTP, or custom protocols
        /// that governs the format and rules for exchanging data between systems.
        /// </remarks>
        public FirewallProtocol Protocol
        {
            get => _rule.Protocol;
            set
            {
                if (value == _rule.Protocol)
                    return;
                _rule.Protocol = value;
                UpdateRuleName();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Specifies the orientation or movement relative to a reference point or axis.
        /// </summary>
        /// <remarks>
        /// This property defines the direction to be used in various operations such as navigation,
        /// alignment, or data processing based on a specific framework or context.
        /// </remarks>
        public FirewallRuleDirection Direction
        {
            get => _rule.Direction;
            set
            {
                if (value == _rule.Direction)
                    return;
                _rule.Direction = value;
                UpdateRuleName();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Encapsulates the main entry point and core functionality of the application.
        /// </summary>
        /// <remarks>
        /// This class serves as the designated starting point for execution and may
        /// include high-level orchestration logic for the application's components.
        /// </remarks>
        public FirewallRuleProgram Program
        {
            get => _rule.Program;
            set
            {
                if (value == _rule.Program)
                    return;
                _rule.Program = value;
                UpdateRuleName();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Specifies the local ports that the rule applies to.
        /// </summary>
        /// <remarks>
        /// This property defines the port or range of ports on the local machine that the rule is targeting.
        /// It is used to control network traffic based on specific port configurations.
        /// </remarks>
        public List<FirewallPortSpecification> LocalPorts
        {
            get => _rule.LocalPorts;
            set
            {
                if (value == _rule.LocalPorts)
                    return;
                _rule.LocalPorts = value;
                UpdateRuleName();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Specifies the collection of remote ports associated with the configuration.
        /// </summary>
        /// <remarks>
        /// This property defines the set of ports on the remote system that are targeted or monitored
        /// as part of the operational settings. The ports provided are typically used to regulate or
        /// assess network communication with external entities.
        /// </remarks>
        public List<FirewallPortSpecification> RemotePorts
        {
            get => _rule.RemotePorts;
            set
            {
                if (value == _rule.RemotePorts)
                    return;
                _rule.RemotePorts = value;
                UpdateRuleName();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Specifies the category of a network in a given context.
        /// </summary>
        /// <remarks>
        /// This property provides a way to classify a network based on predefined categories,
        /// which could relate to its purpose, access level, or security requirements.
        /// </remarks>
        public bool[] NetworkCategory
        {
            get => _rule.NetworkCategory;
            set
            {
                if (value == _rule.NetworkCategory)
                    return;
                _rule.NetworkCategory = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Defines the specific operation or behavior to be executed.
        /// </summary>
        /// <remarks>
        /// This property encapsulates the action logic which determines the outcome or effect
        /// of the operation within the given context.
        /// </remarks>
        public FirewallRuleAction Action
        {
            get => _rule.Action;
            set
            {
                if (value == _rule.Action)
                    return;
                _rule.Action = value;
                OnPropertyChanged();
            }
        }

        #endregion
        
        #region Constructor

        /// <summary>
        /// Represents a view model for a firewall rule that provides details
        /// about the rule's configuration and state for user interface bindings.
        /// </summary>
        public FirewallRuleViewModel()
        {
            UpdateRuleName();
        }
        
        #endregion
        
        #region Methods

        /// <summary>
        /// Updates the rule name based on the current values of various properties such as
        /// user-defined name, direction, protocol, ports, and program. If a user-defined name
        /// is provided, it takes precedence. Otherwise, the rule name is generated dynamically
        /// by combining relevant properties with an optional profile name.
        /// </summary>
        /// <remarks>
        /// The method uses the following properties to construct the rule name:
        /// - UserDefinedName: If set, this name will be directly used.
        /// - Direction: Specifies if the rule is inbound or outbound.
        /// - Protocol: The network protocol associated with the rule.
        /// - LocalPorts: A list of local port specifications.
        /// - RemotePorts: A list of remote port specifications.
        /// - Program: The associated program's executable name.
        /// - ProfileManager.LoadedProfileFile: The name of the currently loaded network profile.
        /// The final name is prefixed with "NwM_" and suffixed with the profile name (or "Unknown"
        /// if no profile is loaded). The DefaultName property is also updated to reflect the generated
        /// rule name without the prefix and profile suffix.
        /// </remarks>
        public void UpdateRuleName()
        {
            StringBuilder resultBuilder = new();
            if (!string.IsNullOrWhiteSpace(_userDefinedName))
            {
                resultBuilder.Append(_userDefinedName);
            }
            else
            {
                var direction = Direction switch
                {
                    FirewallRuleDirection.Inbound => "in",
                    FirewallRuleDirection.Outbound => "out",
                    _ => null
                };
                if (direction is not null)
                    resultBuilder.Append(direction);
                resultBuilder.Append($"_{Protocol}");
                if (LocalPorts?.Count > 0)
                    resultBuilder.Append($"_{FirewallRule.PortsToString(LocalPorts)}");
                if (RemotePorts?.Count > 0)
                    resultBuilder.Append($"_{FirewallRule.PortsToString(RemotePorts)}");
                if (Program is not null && !string.IsNullOrEmpty(Program.Executable?.Name))
                    resultBuilder.Append($"_{Program.Executable.Name}");
            }
            string profile = ProfileManager.LoadedProfileFile?.Name;
            Name = $"NwM_{resultBuilder}_{profile ?? "Unknown"}";
            DefaultName = resultBuilder.ToString();
        }

        /// Converts the current instance of the FirewallRuleViewModel to a FirewallRule object.
        /// return A FirewallRule object representing the current FirewallRuleViewModel instance.
        public FirewallRule ToRule()
        {
            return _rule;
        }


        /// <summary>
        /// Retrieves the localized translation for a given enumeration value.
        /// </summary>
        /// <param name="enumValue">The enumeration value for which the translation is required.</param>
        /// <param name="culture">The culture information used to determine the appropriate translation.</param>
        /// <returns>The localized string corresponding to the provided enumeration value.</returns>
        public static string[] GetEnumTranslation(Type enumType)
        {
            if (!enumType.IsEnum)
                return null;

            var enumStrings = Enum.GetNames(enumType);
            var transStrings = new string[enumStrings.Length];
            for (int i = 0; i < enumStrings.Length; i++)
                transStrings[i] = Strings.ResourceManager.GetString(enumStrings[i]) ?? enumStrings[i];

            return transStrings;

        }
        #endregion
}
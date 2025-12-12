using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using NETworkManager.Localization.Resources;
using NETworkManager.Models.Firewall;
using NETworkManager.Profiles;

namespace NETworkManager.ViewModels;

public class FirewallRuleViewModel : ViewModelBase
{
        #region Variables

        [NotNull]
        private readonly FirewallRule _rule = new();

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
        
        public string Description { get; set; }

        private string _userDefinedName;

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
        
        public string DefaultName { get; private set; }

        /// <summary>
        /// Specifies the network protocol associated with the firewall rule.
        /// </summary>
        /// <remarks>
        /// This property defines the protocol type for traffic affected by the firewall rule.
        /// The protocol can be set to specific values such as TCP, UDP, ICMP, or Any, among others,
        /// based on the predefined options in the <c>FirewallProtocol</c> enumeration.
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
        /// Represents the direction of network traffic in a firewall rule.
        /// </summary>
        /// <remarks>
        /// This property determines whether the rule applies to inbound or outbound traffic,
        /// or if it is currently set to none.
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

        public FirewallRuleViewModel()
        {
            UpdateRuleName();
        }
        
        #endregion
        
        #region Methods

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

        public FirewallRule ToRule()
        {
            return _rule;
        }
        
        
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
﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PokemonGo.SimpleBot {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class UserSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static UserSettings defaultInstance = ((UserSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new UserSettings())));
        
        public static UserSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("37.808631")]
        public double Latitude {
            get {
                return ((double)(this["Latitude"]));
            }
            set {
                this["Latitude"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-122.409553")]
        public double Longitude {
            get {
                return ((double)(this["Longitude"]));
            }
            set {
                this["Longitude"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("20")]
        public double Altitude {
            get {
                return ((double)(this["Altitude"]));
            }
            set {
                this["Altitude"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool AllowCatching {
            get {
                return ((bool)(this["AllowCatching"]));
            }
            set {
                this["AllowCatching"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AllowEvolution {
            get {
                return ((bool)(this["AllowEvolution"]));
            }
            set {
                this["AllowEvolution"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool AllowRecycle {
            get {
                return ((bool)(this["AllowRecycle"]));
            }
            set {
                this["AllowRecycle"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2")]
        public int NumberOfSamePokemonsToKeep {
            get {
                return ((int)(this["NumberOfSamePokemonsToKeep"]));
            }
            set {
                this["NumberOfSamePokemonsToKeep"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool AllowTransfer {
            get {
                return ((bool)(this["AllowTransfer"]));
            }
            set {
                this["AllowTransfer"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("15")]
        public double WalkingSpeedKmh {
            get {
                return ((double)(this["WalkingSpeedKmh"]));
            }
            set {
                this["WalkingSpeedKmh"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool AllowFarming {
            get {
                return ((bool)(this["AllowFarming"]));
            }
            set {
                this["AllowFarming"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("100")]
        public int MaxPOkemonsPerPokestop {
            get {
                return ((int)(this["MaxPOkemonsPerPokestop"]));
            }
            set {
                this["MaxPOkemonsPerPokestop"] = value;
            }
        }
    }
}

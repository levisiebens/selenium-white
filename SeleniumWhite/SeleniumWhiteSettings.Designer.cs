﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SeleniumWhite {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class SeleniumWhiteSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static SeleniumWhiteSettings defaultInstance = ((SeleniumWhiteSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new SeleniumWhiteSettings())));
        
        public static SeleniumWhiteSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1234")]
        public ushort Port {
            get {
                return ((ushort)(this["Port"]));
            }
            set {
                this["Port"] = value;
            }
        }
    }
}
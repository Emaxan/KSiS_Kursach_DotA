﻿#pragma checksum "..\..\..\..\Properties\Pages\Game.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "6F5F19FFBDBF70706BA05485356A36EC"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using DotA.Properties.Pages;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace DotA.Properties.Pages {
    
    
    /// <summary>
    /// Game
    /// </summary>
    public partial class Game : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 13 "..\..\..\..\Properties\Pages\Game.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas CMain;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/DotA;component/properties/pages/game.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Properties\Pages\Game.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 9 "..\..\..\..\Properties\Pages\Game.xaml"
            ((DotA.Properties.Pages.Game)(target)).MouseMove += new System.Windows.Input.MouseEventHandler(this.Game_MouseMove);
            
            #line default
            #line hidden
            
            #line 9 "..\..\..\..\Properties\Pages\Game.xaml"
            ((DotA.Properties.Pages.Game)(target)).MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.Game_OnMouseLeftButtonUp);
            
            #line default
            #line hidden
            return;
            case 2:
            this.CMain = ((System.Windows.Controls.Canvas)(target));
            
            #line 13 "..\..\..\..\Properties\Pages\Game.xaml"
            this.CMain.Loaded += new System.Windows.RoutedEventHandler(this.CMain_OnLoaded);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}


﻿#pragma checksum "..\..\..\..\..\..\Views\MainPage.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "FC7135627BE4217FE6CAFAF4F4ABBB457E86ED3C"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using ArchivatorDb.Entities;
using Archivator_desktop_WPF_WTS.Strings;
using Archivator_desktop_WPF_WTS.ViewModels;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
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


namespace Archivator_desktop_WPF_WTS.Views {
    
    
    /// <summary>
    /// MainPage
    /// </summary>
    public partial class MainPage : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 33 "..\..\..\..\..\..\Views\MainPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button bt_submit;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\..\..\..\Views\MainPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tb_name;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\..\..\..\Views\MainPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tb_description;
        
        #line default
        #line hidden
        
        
        #line 50 "..\..\..\..\..\..\Views\MainPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid dg_files;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\..\..\..\..\Views\MainPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ProgressBar progress_bar;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.8.1.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Archivator_desktop_WPF_WTS;component/views/mainpage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\..\Views\MainPage.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.8.1.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.bt_submit = ((System.Windows.Controls.Button)(target));
            
            #line 33 "..\..\..\..\..\..\Views\MainPage.xaml"
            this.bt_submit.Click += new System.Windows.RoutedEventHandler(this.Bt_submit_OnClick);
            
            #line default
            #line hidden
            return;
            case 2:
            this.tb_name = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.tb_description = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            
            #line 46 "..\..\..\..\..\..\Views\MainPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.btn_add_file_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 47 "..\..\..\..\..\..\Views\MainPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Bt_navigateTest_OnClick);
            
            #line default
            #line hidden
            return;
            case 6:
            this.dg_files = ((System.Windows.Controls.DataGrid)(target));
            
            #line 50 "..\..\..\..\..\..\Views\MainPage.xaml"
            this.dg_files.Drop += new System.Windows.DragEventHandler(this.files_Drop_action);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 62 "..\..\..\..\..\..\Views\MainPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.bt_cancel_all_operations);
            
            #line default
            #line hidden
            return;
            case 8:
            this.progress_bar = ((System.Windows.Controls.ProgressBar)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}


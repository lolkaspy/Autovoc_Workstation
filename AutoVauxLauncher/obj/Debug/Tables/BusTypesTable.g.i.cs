﻿#pragma checksum "..\..\..\Tables\BusTypesTable.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "DACAEE3B64EDC1609CC8336F18AED45D149A85AD30643DA34835EE68ABA6ED04"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using AutoVauxLauncher;
using AutoVauxLauncher.HelpClasses;
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
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Chromes;
using Xceed.Wpf.Toolkit.Converters;
using Xceed.Wpf.Toolkit.Core;
using Xceed.Wpf.Toolkit.Core.Converters;
using Xceed.Wpf.Toolkit.Core.Input;
using Xceed.Wpf.Toolkit.Core.Media;
using Xceed.Wpf.Toolkit.Core.Utilities;
using Xceed.Wpf.Toolkit.Mag.Converters;
using Xceed.Wpf.Toolkit.Panels;
using Xceed.Wpf.Toolkit.Primitives;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Xceed.Wpf.Toolkit.PropertyGrid.Commands;
using Xceed.Wpf.Toolkit.PropertyGrid.Converters;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using Xceed.Wpf.Toolkit.Zoombox;


namespace AutoVauxLauncher {
    
    
    /// <summary>
    /// BusTypesTable
    /// </summary>
    public partial class BusTypesTable : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 22 "..\..\..\Tables\BusTypesTable.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid bustypes;
        
        #line default
        #line hidden
        
        
        #line 101 "..\..\..\Tables\BusTypesTable.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox bustype;
        
        #line default
        #line hidden
        
        
        #line 103 "..\..\..\Tables\BusTypesTable.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox hascard;
        
        #line default
        #line hidden
        
        
        #line 122 "..\..\..\Tables\BusTypesTable.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button add;
        
        #line default
        #line hidden
        
        
        #line 123 "..\..\..\Tables\BusTypesTable.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button del;
        
        #line default
        #line hidden
        
        
        #line 124 "..\..\..\Tables\BusTypesTable.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button upd;
        
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
            System.Uri resourceLocater = new System.Uri("/AutoVauxLauncher;component/tables/bustypestable.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Tables\BusTypesTable.xaml"
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
            this.bustypes = ((System.Windows.Controls.DataGrid)(target));
            
            #line 23 "..\..\..\Tables\BusTypesTable.xaml"
            this.bustypes.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.RowSelected);
            
            #line default
            #line hidden
            return;
            case 2:
            this.bustype = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.hascard = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 4:
            this.add = ((System.Windows.Controls.Button)(target));
            
            #line 122 "..\..\..\Tables\BusTypesTable.xaml"
            this.add.Click += new System.Windows.RoutedEventHandler(this.AddRow);
            
            #line default
            #line hidden
            return;
            case 5:
            this.del = ((System.Windows.Controls.Button)(target));
            
            #line 123 "..\..\..\Tables\BusTypesTable.xaml"
            this.del.Click += new System.Windows.RoutedEventHandler(this.DelRow);
            
            #line default
            #line hidden
            return;
            case 6:
            this.upd = ((System.Windows.Controls.Button)(target));
            
            #line 124 "..\..\..\Tables\BusTypesTable.xaml"
            this.upd.Click += new System.Windows.RoutedEventHandler(this.UpdRow);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}


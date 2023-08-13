using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Metadata;

namespace TreeDataGridEx;

public abstract class TreeDataGridColumn : AvaloniaObject
{
    [InheritDataTypeFromItems(nameof(TreeDataGridEx.ItemsSource), AncestorType = typeof(TreeDataGridEx))]
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] 
    public Type? DataType { get; set; }
}

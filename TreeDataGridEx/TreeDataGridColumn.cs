using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Metadata;

namespace TreeDataGridEx;

public abstract class TreeDataGridColumn : AvaloniaObject
{
    public static readonly DirectProperty<TreeDataGridColumn, Type?> DataTypeProperty =
        AvaloniaProperty.RegisterDirect<TreeDataGridColumn, Type?>(
            nameof(DataType),
            o => o.DataType,
            (o, v) => o.DataType = v);

    private Type? _dataType;

    [InheritDataTypeFromItems(nameof(TreeDataGridEx.ItemsSource), AncestorType = typeof(TreeDataGridEx))]
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] 
    public Type? DataType
    {
        get { return _dataType; }
        set { SetAndRaise(DataTypeProperty, ref _dataType, value); }
    }

    internal abstract IColumn? Create(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type modelType);
}

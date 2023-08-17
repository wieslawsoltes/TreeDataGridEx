using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Metadata;

namespace TreeDataGridEx;

/// <summary>
/// Represents an abstract base class for a column in a tree data grid.
/// </summary>
public abstract class TreeDataGridColumn : AvaloniaObject
{
    /// <summary>
    /// Identifies the <see cref="DataType"/> dependency property.
    /// </summary>
    public static readonly DirectProperty<TreeDataGridColumn, Type?> DataTypeProperty =
        AvaloniaProperty.RegisterDirect<TreeDataGridColumn, Type?>(
            nameof(DataType),
            o => o.DataType,
            (o, v) => o.DataType = v);

    private Type? _dataType;

    /// <summary>
    /// Gets or sets the data type associated with this column.
    /// </summary>
    /// <remarks>
    /// This property is marked with the <see cref="InheritDataTypeFromItemsAttribute"/> attribute to indicate
    /// that it is used to inherit the data type from the parent control's ItemsSource property.
    /// The <see cref="DynamicallyAccessedMembersAttribute"/> attribute is used to inform reflection analysis tools
    /// that this property references all members of the specified data type.
    /// </remarks>
    [InheritDataTypeFromItems(nameof(TreeDataGridEx.ItemsSource), AncestorType = typeof(TreeDataGridEx))]
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] 
    public Type? DataType
    {
        get { return _dataType; }
        set { SetAndRaise(DataTypeProperty, ref _dataType, value); }
    }
}

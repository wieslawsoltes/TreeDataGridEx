using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Templates;
using Avalonia.Media;

namespace TreeDataGridEx;

internal static class ColumnReflectionFactory
{
    public static IColumn? CreateTextColumn(
        Type modelType,
        PropertyInfo property,
        object? header,
        GridLength width,
        bool? canUserResizeColumn,
        bool? canUserSortColumn,
        GridLength minWidth,
        GridLength? maxWidth,
        BeginEditGestures beginEditGestures,
        bool isTextSearchEnabled,
        TextTrimming textTrimming,
        TextWrapping textWrapping)
    {
        var getter = CreateGetterLambda(modelType, property);
        var columnType = typeof(TextColumn<,>).MakeGenericType(modelType, property.PropertyType);

        var optionsType = typeof(TextColumnOptions<>).MakeGenericType(modelType);
        var options = Activator.CreateInstance(optionsType);

        optionsType.GetProperty("CanUserResizeColumn")?.SetValue(options, canUserResizeColumn);
        optionsType.GetProperty("CanUserSortColumn")?.SetValue(options, canUserSortColumn);
        optionsType.GetProperty("MinWidth")?.SetValue(options, minWidth);
        optionsType.GetProperty("MaxWidth")?.SetValue(options, maxWidth);
        optionsType.GetProperty("BeginEditGestures")?.SetValue(options, beginEditGestures);

        optionsType.GetProperty("IsTextSearchEnabled")?.SetValue(options, isTextSearchEnabled);
        optionsType.GetProperty("TextTrimming")?.SetValue(options, textTrimming);
        optionsType.GetProperty("TextWrapping")?.SetValue(options, textWrapping);

        if (!property.CanWrite || (property.SetMethod is not null && !property.SetMethod.IsPublic))
        {
            return (IColumn?)Activator.CreateInstance(columnType, header, getter, width, options);
        }

        var setter = CreateSetterLambda(modelType, property).Compile();
        return (IColumn?)Activator.CreateInstance(columnType, header, getter, setter, width, options);
    }

    public static IColumn? CreateCheckBoxColumn(
        Type modelType,
        PropertyInfo property,
        object? header,
        GridLength width,
        bool? canUserResizeColumn,
        bool? canUserSortColumn,
        GridLength minWidth,
        GridLength? maxWidth,
        BeginEditGestures beginEditGestures)
    {
        var getter = CreateGetterLambda(modelType, property);
        var columnType = typeof(CheckBoxColumn<>).MakeGenericType(modelType);

        var optionsType = typeof(CheckBoxColumnOptions<>).MakeGenericType(modelType);
        var options = Activator.CreateInstance(optionsType);

        optionsType.GetProperty("CanUserResizeColumn")?.SetValue(options, canUserResizeColumn);
        optionsType.GetProperty("CanUserSortColumn")?.SetValue(options, canUserSortColumn);
        optionsType.GetProperty("MinWidth")?.SetValue(options, minWidth);
        optionsType.GetProperty("MaxWidth")?.SetValue(options, maxWidth);
        optionsType.GetProperty("BeginEditGestures")?.SetValue(options, beginEditGestures);

        if (!property.CanWrite || (property.SetMethod is not null && !property.SetMethod.IsPublic))
        {
            return (IColumn?)Activator.CreateInstance(columnType, header, getter, width, options);
        }

        var setter = CreateSetterLambda(modelType, property).Compile();
        return (IColumn?)Activator.CreateInstance(columnType, header, getter, setter, width, options);
    }

    public static IColumn? CreateTemplateColumn(
        Type modelType,
        IDataTemplate cellTemplate,
        IDataTemplate? cellEditingTemplate,
        object? header,
        GridLength width,
        bool? canUserResizeColumn,
        bool? canUserSortColumn,
        GridLength minWidth,
        GridLength? maxWidth,
        BeginEditGestures beginEditGestures)
    {
        var columnType = typeof(TemplateColumn<>).MakeGenericType(modelType);

        var optionsType = typeof(TemplateColumnOptions<>).MakeGenericType(modelType);
        var options = Activator.CreateInstance(optionsType);

        optionsType.GetProperty("CanUserResizeColumn")?.SetValue(options, canUserResizeColumn);
        optionsType.GetProperty("CanUserSortColumn")?.SetValue(options, canUserSortColumn);
        optionsType.GetProperty("MinWidth")?.SetValue(options, minWidth);
        optionsType.GetProperty("MaxWidth")?.SetValue(options, maxWidth);
        optionsType.GetProperty("BeginEditGestures")?.SetValue(options, beginEditGestures);

        return (IColumn?)Activator.CreateInstance(columnType, header, cellTemplate, cellEditingTemplate, width, options);
    }

    public static IColumn? CreateHierarchicalExpanderColumn(
        Type modelType,
        TreeDataGridColumn? inner,
        string childrenName)
    {
        var innerColumn = inner?.Create(modelType);
        var property = modelType.GetProperty(childrenName);
        if (property is null)
        {
            return null;
        }

        var childSelector = CreateChildSelectorLambdaExpression(modelType, property).Compile();
        var columnType = typeof(HierarchicalExpanderColumn<>).MakeGenericType(modelType);

        return (IColumn?)Activator.CreateInstance(columnType, innerColumn, childSelector, null, null);
    }

    private static LambdaExpression CreateGetterLambda(Type modelType, PropertyInfo property)
    {
        var valueType = property.PropertyType;
        var modelParameter = Expression.Parameter(modelType, "model");
        var propertyAccess = Expression.Property(modelParameter, property);
        var convertedPropertyAccess = Expression.Convert(propertyAccess, valueType);
        var lambdaType = typeof(Func<,>).MakeGenericType(modelType, valueType);
        return Expression.Lambda(lambdaType, convertedPropertyAccess, modelParameter);
    }

    private static LambdaExpression CreateSetterLambda(Type modelType, PropertyInfo property)
    {
        var valueType = property.PropertyType;
        var modelParameter = Expression.Parameter(modelType, "model");
        var valueParameter = Expression.Parameter(valueType, "value");
        var propertyAccess = Expression.Property(modelParameter, property);
        var assign = Expression.Assign(propertyAccess, Expression.Convert(valueParameter, property.PropertyType));
        var lambdaType = typeof(Action<,>).MakeGenericType(modelType, valueType);
        return Expression.Lambda(lambdaType, assign, modelParameter, valueParameter);
    }

    private static LambdaExpression CreateChildSelectorLambdaExpression(Type modelType, PropertyInfo property)
    {
        var valueType = typeof(IEnumerable<>).MakeGenericType(modelType);
        var modelParameter = Expression.Parameter(modelType, "model");
        var propertyAccess = Expression.Property(modelParameter, property);
        var convertedPropertyAccess = Expression.Convert(propertyAccess, valueType);
        var lambdaType = typeof(Func<,>).MakeGenericType(modelType, valueType);
        return Expression.Lambda(lambdaType, convertedPropertyAccess, modelParameter);
    }

    public static ITreeDataGridSource? CreateFlatSource(Type modelType, System.Collections.IEnumerable items)
    {
        var type = typeof(FlatTreeDataGridSource<>).MakeGenericType(modelType);
        return (ITreeDataGridSource?)Activator.CreateInstance(type, items);
    }

    public static ITreeDataGridSource? CreateHierarchicalSource(Type modelType, System.Collections.IEnumerable items)
    {
        var type = typeof(HierarchicalTreeDataGridSource<>).MakeGenericType(modelType);
        return (ITreeDataGridSource?)Activator.CreateInstance(type, items);
    }

    public static Action<object?, object?>? GetColumnListAdd(Type modelType)
    {
        var columnsType = typeof(ColumnList<>).MakeGenericType(modelType);
        var add = columnsType.GetMethod("Add");
        if (add is null)
        {
            return null;
        }

        return (list, item) => add.Invoke(list, new[] { item });
    }
}


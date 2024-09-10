using System.Collections.ObjectModel;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Metadata;
using Avalonia.Styling;

namespace TreeDataGridEx;

/// <summary>
/// Represents a template for a collection of <see cref="TreeDataGridColumn"/> objects.
/// </summary>
public class TreeDataGridColumnsTemplate : ITemplate
{
    /// <summary>
    /// Gets or sets the content of the template.
    /// </summary>
    /// <remarks>
    /// This property is marked with the <see cref="ContentAttribute"/> and <see cref="TemplateContentAttribute"/> attributes
    /// to indicate that it is used as the template content and to specify the result type of the template.
    /// </remarks>
    [Content]
    [TemplateContent(TemplateResultType = typeof(ObservableCollection<TreeDataGridColumn?>))]
    public object? Content { get; set; }

    /// <summary>
    /// Builds the content of the template.
    /// </summary>
    /// <returns>An <see cref="ObservableCollection{T}"/> of <see cref="TreeDataGridColumn"/> objects, or <c>null</c>.</returns>
    object? ITemplate.Build() => TemplateContent.Load<ObservableCollection<TreeDataGridColumn?>>(Content)?.Result;
}

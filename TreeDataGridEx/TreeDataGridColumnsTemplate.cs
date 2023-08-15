using System.Collections.ObjectModel;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Metadata;
using Avalonia.Styling;

namespace TreeDataGridEx;

public class TreeDataGridColumnsTemplate : ITemplate
{
    [Content]
    [TemplateContent(TemplateResultType = typeof(ObservableCollection<TreeDataGridColumn?>))]
    public object? Content { get; set; }

    object? ITemplate.Build() => TemplateContent.Load<ObservableCollection<TreeDataGridColumn?>>(Content)?.Result;
}

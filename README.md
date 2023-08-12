# TreeDataGridEx

[![NuGet](https://img.shields.io/nuget/v/TreeDataGridEx.svg)](https://www.nuget.org/packages/TreeDataGridEx)

**TreeDataGridEx** is ex experimental version of [TreeDataGrid](https://github.com/AvaloniaUI/Avalonia.Controls.TreeDataGrid) for [Avalonia](https://github.com/AvaloniaUI/Avalonia) with added XAML syntax.

## Usage

_**Warning:** Please note that the TreeDataGridEx control uses reflection so it might cause issues when using AOT compilation or trimming._

### NuGet

TreeDataGridEx is delivered as a NuGet package.

You can find the packages here [NuGet](https://www.nuget.org/packages/TreeDataGridEx/) and install the package like this:

`Install-Package Avalonia.Xaml.Behaviors`

### Styles

Add styles to `App.axaml`:

```xaml
<Application.Styles>
  <FluentTheme />
  <StyleInclude Source="avares://Avalonia.Controls.TreeDataGrid/Themes/Fluent.axaml" />
  <StyleInclude Source="avares://TreeDataGridEx/TreeDataGridEx.axaml" />
</Application.Styles>
```

### Columns

Available column types:
- TreeDataGridTemplateColumn
- TreeDataGridTextColumn
- TreeDataGridCheckBoxColumn
- TreeDataGridHierarchicalExpanderColumn

### Example: Countries

```xaml
<TreeDataGridEx ItemsSource="{Binding Countries}">
  <TreeDataGridEx.Columns>
    <TreeDataGridCheckBoxColumn Header="*" Name="IsSelected" Width="Auto" />
    <TreeDataGridTextColumn Header="Country" Name="Name" Width="6*" />
    <TreeDataGridTemplateColumn Header="Region">
      <TreeDataGridTemplateColumn.CellTemplate>
        <DataTemplate>
          <TextBlock Text="{Binding Region}" VerticalAlignment="Center" />
        </DataTemplate>
      </TreeDataGridTemplateColumn.CellTemplate>
      <TreeDataGridTemplateColumn.CellEditingTemplate>
        <DataTemplate>
          <TextBox Text="{Binding Region}" />
        </DataTemplate>
      </TreeDataGridTemplateColumn.CellEditingTemplate>
    </TreeDataGridTemplateColumn>
    <TreeDataGridTextColumn Header="Population" Name="Population" Width="3*" />
    <TreeDataGridTextColumn Header="Area" Name="Area" Width="3*" />
    <TreeDataGridTextColumn Header="GDP" Name="GDP" Width="3*" />
  </TreeDataGridEx.Columns>
</TreeDataGridEx>
```

### Example: Drag/Drop

```xaml
<TreeDataGridEx ItemsSource="{Binding DragAndDropItems}">
  <TreeDataGridEx.Columns>
    <TreeDataGridHierarchicalExpanderColumn ChildrenName="Children">
      <TreeDataGridTextColumn Header="Name" Name="Name" Width="*" />
    </TreeDataGridHierarchicalExpanderColumn>
    <TreeDataGridCheckBoxColumn Header="Allow Drag" Name="AllowDrag" Width="Auto" />
    <TreeDataGridCheckBoxColumn Header="Allow Drop" Name="AllowDrop" Width="Auto" />
  </TreeDataGridEx.Columns>
</TreeDataGridEx>
```

## Resources

* [GitHub source code repository.](https://github.com/wieslawsoltes/TreeDataGridEx)

## License

TreeDataGridEx is licensed under the [MIT license](LICENSE.TXT).

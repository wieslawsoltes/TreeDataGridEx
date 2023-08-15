# TreeDataGridEx

[![CI](https://github.com/wieslawsoltes/TreeDataGridEx/actions/workflows/build.yml/badge.svg)](https://github.com/wieslawsoltes/TreeDataGridEx/actions/workflows/build.yml)

[![NuGet](https://img.shields.io/nuget/v/TreeDataGridEx.svg)](https://www.nuget.org/packages/TreeDataGridEx)

**TreeDataGridEx** is an experimental version of [TreeDataGrid](https://github.com/AvaloniaUI/Avalonia.Controls.TreeDataGrid) for [Avalonia](https://github.com/AvaloniaUI/Avalonia) with added XAML syntax.

![image](https://github.com/wieslawsoltes/TreeDataGridEx/assets/2297442/cd6d9484-3707-40a7-b012-e0d58966c406)

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
    <TreeDataGridCheckBoxColumn Header="*" Binding="{Binding IsSelected}" Width="Auto" />
    <TreeDataGridTextColumn Header="Country" Binding="{Binding Name}" Width="6*" />
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
    <TreeDataGridTextColumn Header="Population" Binding="{Binding Population}" Width="3*" />
    <TreeDataGridTextColumn Header="Area" Binding="{Binding Area}" Width="3*" />
    <TreeDataGridTextColumn Header="GDP" Binding="{Binding GDP}" Width="3*" />
  </TreeDataGridEx.Columns>
</TreeDataGridEx>
```

### Example: Drag/Drop

```xaml
<TreeDataGridEx ItemsSource="{Binding DragAndDropItems}">
  <TreeDataGridEx.Columns>
    <TreeDataGridHierarchicalExpanderColumn ChildrenName="Children">
      <TreeDataGridTextColumn Header="Name" Binding="{Binding Name}" Width="*" />
    </TreeDataGridHierarchicalExpanderColumn>
    <TreeDataGridCheckBoxColumn Header="Allow Drag" Binding="{Binding AllowDrag}" Width="Auto" />
    <TreeDataGridCheckBoxColumn Header="Allow Drop" Binding="{Binding AllowDrop}" Width="Auto" />
  </TreeDataGridEx.Columns>
</TreeDataGridEx>
```

### Example: Setting Columns from Style

To set `Columns` property from Style use `TreeDataGridColumnsTemplate` as `Setter.Value`.

```xaml
<TabItem Header="Columns from Style">

  <TabItem.Styles>
    <Style Selector="TreeDataGridEx.columns">
      <Setter Property="Columns">
        <TreeDataGridColumnsTemplate>
          <objectModel:ObservableCollection x:TypeArguments="TreeDataGridColumn">
            <TreeDataGridCheckBoxColumn Header="*" Binding="{Binding IsSelected}" Width="Auto" x:DataType="local:Country" />
            <TreeDataGridTextColumn Header="Country" Binding="{Binding Name}" Width="6*" x:DataType="local:Country" />
            <TreeDataGridTextColumn Header="Region" Binding="{Binding Region}" x:DataType="local:Country" />
            <TreeDataGridTextColumn Header="Population" Binding="{Binding Population}" Width="3*" x:DataType="local:Country" />
            <TreeDataGridTextColumn Header="Area" Binding="{Binding Area}" Width="3*" x:DataType="local:Country" />
            <TreeDataGridTextColumn Header="GDP" Binding="{Binding GDP}" Width="3*" x:DataType="local:Country" />
          </objectModel:ObservableCollection>
        </TreeDataGridColumnsTemplate>
      </Setter>
    </Style>
  </TabItem.Styles>

  <TreeDataGridEx Classes="columns" ItemsSource="{Binding Countries}" />

</TabItem>
```

## Resources

* [GitHub source code repository.](https://github.com/wieslawsoltes/TreeDataGridEx)

## License

TreeDataGridEx is licensed under the [MIT license](LICENSE.TXT).

using Avalonia;
using Avalonia.Data;
using Avalonia.Headless;
using TreeDataGridEx;

public class ColumnReflectionFactoryTests
{
    static ColumnReflectionFactoryTests()
    {
        AppBuilder.Configure<Application>()
                  .UseHeadless(new AvaloniaHeadlessPlatformOptions())
                  .SetupWithoutStarting();
    }

    private class TestModel
    {
        public string? Name { get; set; }
        public bool IsActive { get; set; }
    }

    [Fact]
    public void CreateTextColumn_ReturnsColumn()
    {
        var column = new TreeDataGridTextColumn
        {
            Header = "Name",
            Binding = new Binding("Name")
        };

        var result = column.Create(typeof(TestModel));

        Assert.NotNull(result);
    }

    [Fact]
    public void CreateCheckBoxColumn_ReturnsColumn()
    {
        var column = new TreeDataGridCheckBoxColumn
        {
            Header = "IsActive",
            Binding = new Binding("IsActive")
        };

        var result = column.Create(typeof(TestModel));

        Assert.NotNull(result);
    }
}

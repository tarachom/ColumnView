using Gtk;

namespace Program;

class Program
{
    private static void Main()
    {
        var app = Application.New("ua.org.accounting.test", Gio.ApplicationFlags.FlagsNone);
        app.OnActivate += (_, _) =>
        {
            var window = Window.New();
            window.SetDefaultSize(300, 500);
            window.Application = app;

            ColumnView grid = ColumnView.New(MultiSelection.New(StringList.New([.. Enumerable.Range(0, 500).Select(i => $"Text {i + 1}")])));

            SignalListItemFactory factory = SignalListItemFactory.New();
            factory.OnSetup += (_, args) =>
            {
                ListItem listItem = (ListItem)args.Object;
                listItem.Child = Label.New(null);
            };
            factory.OnBind += (_, args) =>
            {
                ListItem listItem = (ListItem)args.Object;
                if (listItem.Item is StringObject strobj && listItem.Child is Label label)
                    label.SetText(strobj.GetString());
            };
            ColumnViewColumn column = ColumnViewColumn.New("Text", factory);
            grid.AppendColumn(column);

            ScrolledWindow scroll = ScrolledWindow.New();
            scroll.SetChild(grid);

            window.Child = scroll;
            window.Show();
        };

        app.RunWithSynchronizationContext(null);
    }
}
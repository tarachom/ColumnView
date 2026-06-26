using Gtk;
using System.Runtime.InteropServices;

namespace Program;

class Program
{
    private static void Main()
    {
        SetMsysDirectory(@"D:\Debug\msys64\ucrt64\bin");

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
            scroll.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scroll.SetChild(grid);

            window.Child = scroll;
            window.Show();
        };

        app.RunWithSynchronizationContext(null);
    }

    public static void SetMsysDirectory(string path)
    {
        //Для Windows реєструється шлях до бібліотек Gtk
        if (OperatingSystem.IsWindows())
            if (Directory.Exists(path))
            {
                if (!NativeMethods.SetDllDirectory(path))
                    Console.WriteLine("Warning: Failed to set DLL directory.");
            }
            else
                Console.WriteLine($"Warning: MSYS2 path not found at {path}");
    }
}

internal static partial class NativeMethods
{
    [LibraryImport("kernel32.dll", EntryPoint = "SetDllDirectoryW", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetDllDirectory(string lpPathName);
}
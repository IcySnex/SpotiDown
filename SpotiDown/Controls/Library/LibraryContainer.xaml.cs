using Microsoft.UI.Xaml.Controls;
using SpotiDown.Enums;
using SpotiDown.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SpotiDown.Controls;

public sealed partial class LibraryContainer : UserControl, ILibrary
{
    public LibraryContainer(Models.LibraryContainer Container)
    {
        InitializeComponent();
        ChildContainer = Container;

        if (!(ChildContainer.Children is null))
            foreach (ILibrary Child in ChildContainer.Children)
                this.Container.Items.Add(Child);
    }


    public void UpdateList(string Filter, SortingType Sorting, bool Performance = true)
    {
        if (ChildContainer.Children is null || ChildContainer.Children.Count == 0)
            return;

        IEnumerable<ILibrary> Filtered = ChildContainer.Children.Where(Library => Library.NameContains(Filter));

        if (Performance && Container.Items.SequenceEqual(Filtered))
            return;

        Container.Items.Clear();
        switch (Sorting)
        {
            case SortingType.Default:
                foreach (ILibrary Entry in Filtered)
                    Container.Items.Add(Entry);
                break;
            case SortingType.Default_Inv:
                foreach (ILibrary Entry in Filtered.Reverse())
                    Container.Items.Add(Entry);
                break;
            case SortingType.Title:
                foreach (ILibrary Entry in Filtered.OrderBy(r => r.ContainerName))
                    Container.Items.Add(Entry);
                break;
        }
    }


    public Models.LibraryContainer ChildContainer;


    bool ILibrary.IsContainer => true;

    string ILibrary.ContainerName { get => ChildContainer.Name; set => ChildContainer.Name = value; }

    public bool NameContains(string Input)
    {
        if (ChildContainer.Name.Contains(Input, System.StringComparison.InvariantCultureIgnoreCase))
        {
            Container.Items.Clear();
            if (!(ChildContainer.Children is null))
                foreach (ILibrary Child in ChildContainer.Children)
                    Container.Items.Add(Child);
            return true;
        }

        return ChildContainer.Children is null || ChildContainer.Children.Count == 0 ? false : ChildContainer.Children.Where(Library => Library.NameContains(Input)).Any(c => c.ContainerName.Contains(Input, System.StringComparison.InvariantCultureIgnoreCase));
    }
}
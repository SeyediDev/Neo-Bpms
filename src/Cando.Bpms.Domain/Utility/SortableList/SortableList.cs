namespace Neo.Bpms.Domain.Utility.SortableList;

public class SortableList<T> : List<T>
{
    private string _propertyName;
    private bool _ascending;

    public void Sort(string propertyName, bool ascending)
    {
        //Flip the properties if the parameters are the same
        if (_propertyName == propertyName && _ascending == ascending)
            _ascending = !ascending;
        //Else, new properties are set with the new values
        else
        {
            _propertyName = propertyName;
            _ascending = ascending;
        }

        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
        PropertyDescriptor propertyDesc = properties.Find(propertyName, true);

        // Apply and set the sort, if items to sort
        PropertyComparer<T> pc = new(propertyDesc,
            _ascending ? ListSortDirection.Ascending : ListSortDirection.Descending);
        Sort(pc);
    }

    //public SortableList<User> BuildList()
    //{
    //    SortableList<Activity> list = new SortableList<Activity>();
    //    list.Sort("Id", true);
    //    return list;
    //}

}

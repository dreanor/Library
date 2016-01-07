# Library
Assume you have a Location class.
```c#
public class Location
{
    public Location(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; }

    public int Y { get; }
}
```

The following example shows how you can Register and Resolve a instance.
```c#
public void Stuff()
{
    IDiProvider diProvider = new DiProvider();
    diProvider.RegisterInstance(new Location(1, 5));

    Location location = diProvider.Resolve<Location>();
}
```

What you can register  |
--- |
RegisterType |
RegisterTypeAsSingleton |
RegisterInstance |
RegisterTypeIfNotSet |
RegisterTypeAsSingletonIfNotSet |
RegisterInstanceIfNotSet |

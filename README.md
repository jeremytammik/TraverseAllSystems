# TraverseAllSystems

C# .NET Revit API add-in to extract graph structure of all MEP systems.

For more information, please refer
to [The Building Coder](http://thebuildingcoder.typepad.com) discussion
on [traversing and exporting all MEP system graphs](http://thebuildingcoder.typepad.com/blog/2016/06/traversing-and-exporting-all-mep-system-graphs.html).


## JSON Structure

```
{
  id: -1,
  name: 'Root',
  children: [
  {
    id: 0,
    name: 'Mechanical System',
    children: [
    {
      id: 0_1,
      name: 'Child 0_1',
      type: 'window',
      otherField: 'something...',
      children: [
      {
        id: 0_1_1,
        name: 'Grandchild 0_1_1'
      }]
    }, {
      id: 0_2,
      name: 'Child 0_2',
      children: [
      {
        id: 0_2_1,
        name: 'Grandchild 0_2_1'
      }]
    }]
  }, {
    id: 2,
    name: 'Electrical System',
    children: [
    {
      id: 2_1,
      name: 'Child 2_1',
      children: [{
        id: 2_1_1,
        name: 'Grandchild 2_1_1'
      }]
    },
    {
      id: 2_2,
      name: 'Child 2_2',
      children: [{
        id: 2_2_1,
        name: 'Grandchild 2_2_1'
      }]
    }]
  },
  {
    id: 3,
    name: 'Piping System',
    children: [
    {
      id: 3_1,
      name: 'Child 3_1',
      children: [{
        id: 3_1_1,
        name: 'Grandchild 3_1_1'
      }]
    },
    {
      id: 3_2,
      name: 'Child 3_2',
      children: [{
        id: 3_2_1,
        name: 'Grandchild 3_2_1'
      }]
    }]
  }]
}
```

## Author

Jeremy Tammik,
[The Building Coder](http://thebuildingcoder.typepad.com) and
[The 3D Web Coder](http://the3dwebcoder.typepad.com),
[ADN](http://www.autodesk.com/adn)
[Open](http://www.autodesk.com/adnopen),
[Autodesk Inc.](http://www.autodesk.com)


## License

This sample is licensed under the terms of the [MIT License](http://opensource.org/licenses/MIT).
Please see the [LICENSE](LICENSE) file for full details.

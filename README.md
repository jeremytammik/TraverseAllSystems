# TraverseAllSystems

C# .NET Revit API add-in to extract graph structure of all MEP systems.

For more information, please refer
to [The Building Coder](http://thebuildingcoder.typepad.com) discussion
on [traversing and exporting all MEP system graphs](http://thebuildingcoder.typepad.com/blog/2016/06/traversing-and-exporting-all-mep-system-graphs.html).

## Options

- Use element id or UniqueId for to identify node
- Store JSON graph bottom-up or top-down

The two bottom-up and top-down JSON storage structures both comply with
the [jsTree JSON spec](https://www.jstree.com/docs/json).

### Bottom-Up JSON Structure

```
[
  { "id" : "ajson1", "parent" : "#", "text" : "Simple root node" },
  { "id" : "ajson2", "parent" : "#", "text" : "Root node 2" },
  { "id" : "ajson3", "parent" : "ajson2", "text" : "Child 1" },
  { "id" : "ajson4", "parent" : "ajson2", "text" : "Child 2" },
]
```

### Top-Down JSON Structure

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

## Test

Here is a link to the [test jsTree](https://jeremytammik.github.io/TraverseAllSystems/test) populated from the USC Revit test model.


## Author

Jeremy Tammik,
[The Building Coder](http://thebuildingcoder.typepad.com) and
[The 3D Web Coder](http://the3dwebcoder.typepad.com),
[Forge](http://forge.autodesk.com) [Platform](https://developer.autodesk.com) Development,
[ADN](http://www.autodesk.com/adn)
[Open](http://www.autodesk.com/adnopen),
[Autodesk Inc.](http://www.autodesk.com)


## License

This sample is licensed under the terms of the [MIT License](http://opensource.org/licenses/MIT).
Please see the [LICENSE](LICENSE) file for full details.

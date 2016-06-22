#region Namespaces
using System.Linq;
using System.Collections.Generic;
//using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
//using Autodesk.Revit.UI.Selection;
#endregion

namespace TraverseAllSystems
{
  [Transaction( TransactionMode.ReadOnly )]
  public class Command : IExternalCommand
  {
    /// <summary>
    /// Return true to include this system in the 
    /// exported system graphs.
    /// </summary>
    static bool IsDesirableSystemPredicate(MEPSystem s)
    {
      return s is MechanicalSystem || s is PipingSystem
        && !s.Name.Equals( "unassigned" )
        && 1 < s.Elements.Size;
    }

    /// <summary>
    /// Create a and return the path of a random temporary directory.
    /// </summary>
    static string GetTemporaryDirectory()
    {
      string tempDirectory = Path.Combine( 
        Path.GetTempPath(), Path.GetRandomFileName() );

      Directory.CreateDirectory( tempDirectory );

      return tempDirectory;
    }

    public Result Execute(
      ExternalCommandData commandData,
      ref string message,
      ElementSet elements )
    {
      UIApplication uiapp = commandData.Application;
      UIDocument uidoc = uiapp.ActiveUIDocument;
      Application app = uiapp.Application;
      Document doc = uidoc.Document;

      FilteredElementCollector allSystems
        = new FilteredElementCollector( doc )
          .OfClass( typeof( MEPSystem ) );

      int nAllSystems = allSystems.Count<Element>();

      IEnumerable<MEPSystem> desirableSystems 
        = allSystems
          .Cast<MEPSystem>()
          .Where<MEPSystem>( s => IsDesirableSystemPredicate( s ) );

      int nDesirableSystems = desirableSystems.Count<Element>();

      string outputFolder = GetTemporaryDirectory();

      int n = 0;

      foreach( MEPSystem system in desirableSystems )
      {
        Debug.Print( system.Name );

        FamilyInstance root = system.BaseEquipment;

        // Traverse the system and dump the traversal into an XML file
        TraversalTree tree = new TraversalTree( system );

        if( tree.Traverse() )
        {
          string filename = system.Id.IntegerValue.ToString();

          filename = Path.ChangeExtension(
            Path.Combine( outputFolder, filename ), "xml" );

          tree.DumpIntoXML( filename );
          //Process.Start( fileName );

          ++n;
        }
      }

      string main = string.Format( 
        "{0} XML files generated in {1} ({2} total systems, {3} desirable):",
        n, outputFolder, nAllSystems, nDesirableSystems );

      List<string> system_list = desirableSystems
        .Select<Element, string>( e =>
          string.Format( "{0}({1})", e.Id, e.Name ) )
        .ToList<string>();

      system_list.Sort();

      string detail = string.Join( ", ", system_list.ToArray<string>() );

      TaskDialog dlg = new TaskDialog( n.ToString() + " Systems" );

      dlg.MainInstruction = main;
      dlg.MainContent = detail;

      dlg.Show();

      return Result.Succeeded;
    }
  }
}

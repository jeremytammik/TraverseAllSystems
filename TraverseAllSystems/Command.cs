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
//using Autodesk.Revit.UI.Selection;
#endregion

namespace TraverseAllSystems
{
  [Transaction( TransactionMode.ReadOnly )]
  public class Command : IExternalCommand
  {
    /// <summary>
    /// Create a and return the path of a random temporary directory.
    /// </summary>
    public string GetTemporaryDirectory()
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

      FilteredElementCollector systems
        = new FilteredElementCollector( doc )
          .OfClass( typeof( MEPSystem ) );

      string outputFolder = GetTemporaryDirectory();

      foreach( MEPSystem system in systems )
      {
        Debug.Print( system.Name );

        FamilyInstance root = system.BaseEquipment;

        // Traverse the system and dump the traversal into an XML file
        TraversalTree tree = new TraversalTree( system );

        if( null != tree.Traverse() )
        {
          string filename = system.Id.IntegerValue.ToString();

          filename = Path.ChangeExtension(
            Path.Combine( outputFolder, filename ), "xml" );

          tree.DumpIntoXML( filename );
          //Process.Start( fileName );
        }
      }

      int n = systems.Count<Element>();

      //string system_names = string.Join( ", ", 
      //  systems
      //    .Select<Element, string>( e => e.Name )
      //    .ToArray<string>() );


      string main = string.Format( 
        "{0} XML files generated in {1}:",
        n, outputFolder );

      List<string> system_list = systems
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

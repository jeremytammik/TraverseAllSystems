#region Namespaces
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
#endregion

namespace TraverseAllSystems
{
  [Transaction( TransactionMode.Manual )]
  public class Command : IExternalCommand
  {
    /// <summary>
    /// Return true to include this system in the 
    /// exported system graphs.
    /// </summary>
    static bool IsDesirableSystemPredicate( MEPSystem s )
    {
      return 1 < s.Elements.Size
        && !s.Name.Equals( "unassigned" )
        && ( ( s is MechanicalSystem 
            && ( (MechanicalSystem) s ).IsWellConnected )
          || ( s is PipingSystem 
            && ( (PipingSystem) s ).IsWellConnected ) );
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
        = allSystems.Cast<MEPSystem>().Where<MEPSystem>(
          s => IsDesirableSystemPredicate( s ) );

      int nDesirableSystems = desirableSystems
        .Count<Element>();

      // Check for shared parameter
      // to store graph information.

      // Determine element from which to retrieve 
      // shared parameter definition.

      Element json_storage_element 
        = Options.StoreSeparateJsonGraphOnEachSystem
          ? desirableSystems.First<MEPSystem>()
          : new FilteredElementCollector( doc )
            .OfClass( typeof( ProjectInfo ) )
            .FirstElement();

      Definition def = SharedParameterMgr.GetDefinition( 
        json_storage_element );

      if( null == def )
      {
        //message = "Please initialise the MEP graph "
        //  + "storage shared parameter before "
        //  + "launching this command.";

        //return Result.Failed;

        SharedParameterMgr.Create( doc );

        def = SharedParameterMgr.GetDefinition( 
          json_storage_element );

        if( null == def )
        {
          message = "Error creating the "
            + "storage shared parameter.";

          return Result.Failed;
        }
      }

      string outputFolder = GetTemporaryDirectory();

      string json;
      int nXmlFiles = 0;
      int nJsonGraphs = 0;
      int nJsonBytes = 0;

      // Collect one JSON string per system.

      List<string> json_collector = new List<string>();

      using( Transaction t = new Transaction( doc ) )
      {
        t.Start( "Determine MEP Graph Structure and Store in JSON Shared Parameter" );

        foreach( MEPSystem system in desirableSystems )
        {
          Debug.Print( system.Name );

          // Debug test -- limit to HWS systems.
          //if( !system.Name.StartsWith( "HWS" ) ) { continue; }

          FamilyInstance root = system.BaseEquipment;

          // Traverse the system and dump the 
          // traversal graph into an XML file

          TraversalTree tree = new TraversalTree( system );

          if( tree.Traverse() )
          {
            string filename = system.Id.IntegerValue.ToString();

            filename = Path.ChangeExtension(
              Path.Combine( outputFolder, filename ), "xml" );

            tree.DumpIntoXML( filename );

            // Uncomment to preview the 
            // resulting XML structure

            //Process.Start( fileName );

            json = Options.StoreJsonGraphBottomUp
              ? tree.DumpToJsonBottomUp()
              : tree.DumpToJsonTopDown();

            Debug.Assert( 2 < json.Length, 
              "expected valid non-empty JSON graph data" );

            Debug.Print( json );

            json_collector.Add( json );

            if( Options.StoreSeparateJsonGraphOnEachSystem )
            {
              Parameter p = system.get_Parameter( def );
              p.Set( json );
            }

            nJsonBytes += json.Length;
            ++nJsonGraphs;
            ++nXmlFiles;
          }
        }
        t.Commit();
      }

      string main = string.Format(
        "{0} XML files and {1} JSON graphs ({2} bytes) "
        + "generated in {3} ({4} total systems, {5} desirable):",
        nXmlFiles, nJsonGraphs, nJsonBytes, 
        outputFolder, nAllSystems, nDesirableSystems );

      List<string> system_list = desirableSystems
        .Select<Element, string>( e =>
          string.Format( "{0}({1})", e.Id, e.Name ) )
        .ToList<string>();

      system_list.Sort();

      string detail = string.Join( ", ",
        system_list.ToArray<string>() );

      TaskDialog dlg = new TaskDialog( 
        nXmlFiles.ToString() + " Systems" );

      dlg.MainInstruction = main;
      dlg.MainContent = detail;

      dlg.Show();

      string json_systems = string.Join( ",", json_collector );

      const string _json_format_to_store_systrms_in_root
        = "{{"
        + "\"id\" : {0}, "
        + "\"text\" : \"{1}\", "
        + "\"children\" : [{2}]}}";

      json = string.Format(
        _json_format_to_store_systrms_in_root,
        -1, doc.Title, json_systems );

      Debug.Print( json );

      if( Options.StoreEntireJsonGraphOnProjectInfo )
      {
        Parameter p = json_storage_element
          .get_Parameter( def );

        p.Set( json );
      }

      return Result.Succeeded;
    }
  }
}

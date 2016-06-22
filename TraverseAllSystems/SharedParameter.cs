#region Namespaces
using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
#endregion // Namespaces

namespace TraverseAllSystems
{
  /// <summary>
  /// Shared parameters to keep store MEP system 
  /// graph structure in JSON strings.
  /// </summary>
  class SharedParameter
  {
    /// <summary>
    /// Define the user visible shared parameter name.
    /// </summary>
    const string _shared_param_name = "MepSystemGraphJson";

    /// <summary>
    /// Shared parameter definitions.
    /// </summary>
    Definition _shared_param_definition = null;

    Document _doc = null;
    List<ElementId> _ids = null;

    /// <summary>
    /// Return the parameter definition from
    /// the given element and parameter name.
    /// </summary>
    static Definition GetDefinition(
      Element e,
      string parameter_name )
    {
      IList<Parameter> ps = e.GetParameters( parameter_name );

      int n = ps.Count;

      Debug.Assert( 1 >= n,
        "expected maximum one shared parameters "
        + "named " + parameter_name );

      Definition d = ( 0 == n )
        ? null
        : ps[0].Definition;

      return d;
    }

    /// <summary>
    /// Initialise the shared parameter definitions
    /// from a given sample element.
    /// </summary>
    public SharedParameter( Element e )
    {
      _shared_param_definition = GetDefinition(
        e, _shared_param_name );

      if( IsValid )
      {
        _doc = e.Document;
        _ids = new List<ElementId>();
      }
    }

    /// <summary>
    /// Check whether the parameter definition 
    /// was successfully initialised.
    /// </summary>
    public bool IsValid
    {
      get
      {
        return null != _shared_param_definition;
      }
    }

    static Definition CreateNewDefinition(
      DefinitionGroup group,
      string parameter_name,
      ParameterType parameter_type )
    {
      return group.Definitions.Create(
        new ExternalDefinitionCreationOptions(
          parameter_name, parameter_type ) );
    }

    /// <summary>
    /// Create the shared parameters to keep track
    /// of the CNC fabrication export history.
    /// </summary>
    public static void Create( Document doc )
    {
      /// <summary>
      /// Shared parameters filename; used only in case
      /// none is set and we need to create the export
      /// history shared parameters.
      /// </summary>
      const string _shared_parameters_filename
        = "shared_parameters.txt";

      const string _definition_group_name 
        = "TraverseAllSystems";

      Application app = doc.Application;

      // Retrieve shared parameter file name

      string sharedParamsFileName
        = app.SharedParametersFilename;

      if( null == sharedParamsFileName
        || 0 == sharedParamsFileName.Length )
      {
        string path = Path.GetTempPath();

        path = Path.Combine( path,
          _shared_parameters_filename );

        StreamWriter stream;
        stream = new StreamWriter( path );
        stream.Close();

        app.SharedParametersFilename = path;

        sharedParamsFileName
          = app.SharedParametersFilename;
      }

      // Retrieve shared parameter file object

      DefinitionFile f
        = app.OpenSharedParameterFile();

      using( Transaction t = new Transaction( doc ) )
      {
        t.Start( "Create TraverseAllSystems "
          + "Shared Parameters" );

        // Create the category set for binding

        CategorySet catSet = app.Create.NewCategorySet();

        Category cat = doc.Settings.Categories.get_Item(
          BuiltInCategory.OST_DuctSystem );

        catSet.Insert( cat );

        cat = doc.Settings.Categories.get_Item(
          BuiltInCategory.OST_PipingSystem );

        catSet.Insert( cat );

        Binding binding = app.Create.NewInstanceBinding(
          catSet );

        // Retrieve or create shared parameter group

        DefinitionGroup group
          = f.Groups.get_Item( _definition_group_name )
          ?? f.Groups.Create( _definition_group_name );

        // Retrieve or create the three parameters;
        // we could check if they are already bound, 
        // but it looks like Insert will just ignore 
        // them in that case.

        Definition definition
          = group.Definitions.get_Item( _shared_param_name )
            ?? CreateNewDefinition( group, 
              _shared_param_name, ParameterType.Text );

        doc.ParameterBindings.Insert( definition, binding,
          BuiltInParameterGroup.PG_GENERAL );

        t.Commit();
      }
    }
  }
}

using UnityEngine;
using System.Collections;
using System.IO;

public class DataBinding {

	private Hashtable _datastore = new Hashtable();
	public string name { get; private set; }
	public string path { get; private set; }

	public ArrayList libs {
		get {
			return (ArrayList)_datastore["libs"];
		}
	}

	public ArrayList frameworks {
		get {
			return (ArrayList)_datastore ["frameworks"];
		}
	}

	public ArrayList headerpaths {
		get {
			return (ArrayList)_datastore["headerpaths"];
		}
	}

	public ArrayList files {
		get {
			return (ArrayList)_datastore["files"];
		}
	}

	public ArrayList folders {
		get {
			return (ArrayList)_datastore["folders"];
		}
	}

	public ArrayList excludes {
		get {
			return (ArrayList)_datastore["excludes"];
		}
	}

	public ArrayList compiler_flags {
		get {
			return (ArrayList)_datastore["compiler_flags"];
		}
	}

	public ArrayList linker_flags {
		get {
			return (ArrayList)_datastore["linker_flags"];
		}
	}
	public ArrayList plist {
		get {
			return (ArrayList)_datastore["plist"];
		}
	}

	public ArrayList bitcode {
		get {
			return (ArrayList)_datastore["bitcode"];
		}
	}

	public ArrayList code_sign_identity {
		get {
			return (ArrayList)_datastore["code_sign_identity"];
		}
	}

	public ArrayList provisioning_profile_specifier {
		get {
			return (ArrayList)_datastore["provisioning_profile_specifier"];
		}
	}

	public ArrayList framework_search_paths {
		get {
			return (ArrayList)_datastore["framework_search_paths"];
		}
	}

	public ArrayList other_ldflags {
		get {
			return (ArrayList)_datastore["other_ldflags"];
		}
	}

	public DataBinding( string filename )
	{	
		FileInfo projectFileInfo = new FileInfo (filename);
		if (!projectFileInfo.Exists) {
			Debug.LogWarning ("File does not exist.");
			return;
		}
		name = System.IO.Path.GetFileNameWithoutExtension (filename);
		path = System.IO.Path.GetDirectoryName (filename);
		string contents = projectFileInfo.OpenText ().ReadToEnd ();
		_datastore = (Hashtable)XUPorterJSON.MiniJSON.jsonDecode (contents);
		if (_datastore == null || _datastore.Count == 0) {
			throw new UnityException ("Parse error in file " + System.IO.Path.GetFileName (filename) + "! Check for typos such as unbalanced quotation marks, etc.");
		}
	}
}
	

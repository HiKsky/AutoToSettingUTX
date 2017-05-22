using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEditor.Callbacks;
using System.Collections.Generic;
using System.IO;
#endif

public class AutoSettingXcode : MonoBehaviour {

	#if UNITY_IOS || UNITY_EDITOR  
	[PostProcessBuild(999)]  
	public static void OnPostprocessBuild(BuildTarget BuildTarget, string path)  
	{  
		if (BuildTarget == BuildTarget.iOS)  
		{  
			string projPath = PBXProject.GetPBXProjectPath(path);  
			PBXProject proj = new PBXProject();  
			proj.ReadFromString(File.ReadAllText(projPath));  

			// 获取当前项目名字  
			string target = proj.TargetGuidByName(PBXProject.GetUnityTargetName());
			string[] files = Directory.GetFiles (Application.dataPath + "/Editor/", "*.projmods", SearchOption.AllDirectories);
			foreach (string file in files) {
				ApplyDataBinding (file, path, proj, target);
			}
			// 保存工程  
			proj.WriteToFile (projPath);  
		}  
	}  

	public static void ApplyDataBinding(string pbxmod,string rootpath,PBXProject proj,string target)
	{
		DataBinding dataBinding = new DataBinding(pbxmod);
		ApplyData(dataBinding.libs,(valueOne,valueTwo)=>{
			proj.AddFileToBuild (target,proj.AddFile("usr/lib/"+valueOne, "Frameworks/"+valueOne, PBXSourceTree.Sdk));
		});
		ApplyData(dataBinding.frameworks,(valueOne,valueTwo)=>{
			proj.AddFrameworkToProject (target, valueOne, false);  
		});
		ApplyData(dataBinding.plist,(valueOne,valueTwo)=>{
			 //修改plist  

			string plistPath = rootpath + "/Info.plist";  
			 PlistDocument plist = new PlistDocument();  
			 plist.ReadFromString(File.ReadAllText(plistPath));  
			 PlistElementDict rootDict = plist.root; 
			if(valueTwo == "True" || valueTwo == "False")
			{
				rootDict.SetBoolean(valueOne,bool.Parse(valueTwo)); 
				plist.WriteToFile (plistPath);  
				return;
			}
			if(ChargeIsNumber(valueTwo))
			{
				rootDict.SetInteger(valueOne,int.Parse(valueTwo));
				plist.WriteToFile (plistPath);  
				return;
			}
			 // 语音听写需要开启的权限
			 rootDict.SetString(valueOne, valueTwo);  
			 //保存plist  
			 plist.WriteToFile (plistPath); 
		});
		Debug.Log("修改PLIST成功!");
		ApplyData (dataBinding.bitcode, (valueOne, ValueTwo) => {
			proj.SetBuildProperty (target, "ENABLE_BITCODE", valueOne);  
		});
		Debug.Log("修改ENABLE_BITCODE成功!");
		ApplyData (dataBinding.code_sign_identity,(valueOne,ValueTwo)=>{
			proj.SetBuildProperty (target, "CODE_SIGN_IDENTITY", valueOne +":"+ ValueTwo);  
		});
		Debug.Log("修改CODE_SIGN_IDENTITY成功!");
		ApplyData (dataBinding.provisioning_profile_specifier,(valueOne,ValueTwo)=>{
			proj.SetBuildProperty (target, "PROVISIONING_PROFILE_SPECIFIER", valueOne);  
		});
		Debug.Log("修改PROVISIONING_PROFILE_SPECIFIER成功!");
		ApplyData (dataBinding.framework_search_paths,(valueOne,ValueTwo)=>{
			proj.SetBuildProperty (target, "FRAMEWORK_SEARCH_PATHS", valueOne);  
		});
		Debug.Log("修改FRAMEWORK_SEARCH_PATHS成功!");
                //设置框架路径时，无法实现在Xcode中依次添加各个路径，添加一个可以，添加第二个覆盖第一个
		ApplyData (dataBinding.other_ldflags,(valueOne,ValueTwo)=>{
			proj.SetBuildProperty (target, "OTHER_LDFLAGS", valueOne);  
		}); 
		Debug.Log("修改OTHER_LDFLAGS成功!");
	}


	public static void ApplyData(ArrayList data,Action<string,string> callBack)
	{
		if(data.Count == 0)return;
		foreach( string item in data) 
		{  
			string[] resources = item.Split (':');
			if(resources.Length == 1)callBack (resources[0],string.Empty);
			if(resources.Length == 2)callBack (resources[0],resources[1]);
		}  
	}

	public static bool ChargeIsNumber(string message)
	{
			System.Text.RegularExpressions.Regex rex=
			new System.Text.RegularExpressions.Regex(@"^\d+$");
			if (rex.IsMatch(message))
			{
				return true;
			}
			else
			{
				return false;
			}
	}
    #endif 
}

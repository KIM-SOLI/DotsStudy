using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;


namespace ScriptMaker
{

    public static class PathStrings
    {
        public static readonly string AuthoringAndComponentDataPath = "AuthoringNComponentDataTemplate.txt";
        public static readonly string AuthoringPath = "AuthoringTemplate.txt";
        public static readonly string AspectPath = "AspectTemplate.txt";
        public static readonly string ComponentDataPath = "ComponentDataTemplate.txt";
        public static readonly string SystemDataPath = "SystemTemplate.txt";
        public static readonly string SystemAndJobmDataPath = "SystemAndJobTemplate.txt";
        public static readonly string JobDataPath = "JobTemplate.txt";
        public static readonly string PackTemplate = "PackTemplate.txt";
        public static readonly string TemplatePath = "ScriptMaker/CustomScriptTemplates";

        internal static string RemoveOrInsertNamespace(string content, string rootNamespace)
        {
            string text = "#ROOTNAMESPACEBEGIN#";
            string text2 = "#ROOTNAMESPACEEND#";
            if (!content.Contains(text) || !content.Contains(text2))
            {
                return content;
            }

            if (string.IsNullOrEmpty(rootNamespace))
            {
                content = Regex.Replace(content, "((\\r\\n)|\\n)[ \\t]*" + text + "[ \\t]*", string.Empty);
                content = Regex.Replace(content, "((\\r\\n)|\\n)[ \\t]*" + text2 + "[ \\t]*", string.Empty);
                return content;
            }

            string separator = (content.Contains("\r\n") ? "\r\n" : "\n");
            List<string> list = new List<string>(content.Split(new string[3] { "\r\n", "\r", "\n" }, StringSplitOptions.None));
            int i;
            for (i = 0; i < list.Count && !list[i].Contains(text); i++)
            {
            }

            string text3 = list[i];
            string text4 = text3.Substring(0, text3.IndexOf("#"));
            list[i] = "namespace " + rootNamespace;
            list.Insert(i + 1, "{");
            for (i += 2; i < list.Count; i++)
            {
                string text5 = list[i];
                if (!string.IsNullOrEmpty(text5) && text5.Trim().Length != 0)
                {
                    if (text5.Contains(text2))
                    {
                        list[i] = "}";
                        break;
                    }

                    list[i] = text4 + text5;
                }
            }

            return string.Join(separator, list.ToArray());
        }
    }

    public class AuthoringAndComponentDataScriptMaker : EndNameEditAction
    {

        [MenuItem("Assets/ECS/Create/Authoring N Component", priority = 1)]
        public static void CreateUI()
        {
            var path = Path.Combine(Application.dataPath, PathStrings.TemplatePath, PathStrings.AuthoringAndComponentDataPath);

            var file = File.ReadAllText(path);
            var asset = new TextAsset(file);
            //file.Replace("[CustomUIForm]", )
            Debug.Log(file);

            Texture2D csIcon =
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;

            var endNameEditAction =
                ScriptableObject.CreateInstance<AuthoringAndComponentDataScriptMaker>();

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                endNameEditAction, "New Authoring.cs", csIcon, path);



        }

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            var text = File.ReadAllText(resourceFile);

            var className = Path.GetFileNameWithoutExtension(pathName);

            className = className.Replace(" ", "");
            text = text.Replace("[ScriptName]", className);
            var encoding = new UTF8Encoding(true, false);

            var rootNamespace = CompilationPipeline.GetAssemblyRootNamespaceFromScriptPath(pathName);
            text = PathStrings.RemoveOrInsertNamespace(text, rootNamespace);

            pathName = Path.Combine(Path.GetDirectoryName(pathName), $"{className}Authoring.cs");
            File.WriteAllText(pathName, text, encoding);
            
            AssetDatabase.ImportAsset(pathName);
            var asset = AssetDatabase.LoadAssetAtPath<MonoScript>(pathName);
            ProjectWindowUtil.ShowCreatedAsset(asset);
        }
    }


    public class AuthoringScriptMaker : EndNameEditAction
    {

        [MenuItem("Assets/ECS/Create/Authoring", priority = 2)]
        public static void CreateUI()
        {
            var path = Path.Combine(Application.dataPath, PathStrings.TemplatePath, PathStrings.AuthoringPath);

            var file = File.ReadAllText(path);
            var asset = new TextAsset(file);
            //file.Replace("[CustomUIForm]", )
            Debug.Log(file);

            Texture2D csIcon =
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;

            var endNameEditAction =
                ScriptableObject.CreateInstance<AuthoringScriptMaker>();

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                endNameEditAction, "New Authoring.cs", csIcon, path);



        }

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
         

            var text = File.ReadAllText(resourceFile);

            var className = Path.GetFileNameWithoutExtension(pathName);

            className = className.Replace(" ", "");
            text = text.Replace("[ScriptName]", className);
            var encoding = new UTF8Encoding(true, false);

            var rootNamespace = CompilationPipeline.GetAssemblyRootNamespaceFromScriptPath(pathName);
            text = PathStrings.RemoveOrInsertNamespace(text, rootNamespace);

            pathName = Path.Combine(Path.GetDirectoryName(pathName), $"{className}Authoring.cs");
            File.WriteAllText(pathName, text, encoding);

            AssetDatabase.ImportAsset(pathName);
            var asset = AssetDatabase.LoadAssetAtPath<MonoScript>(pathName);
            ProjectWindowUtil.ShowCreatedAsset(asset);
        }
    }

    public class ComponentDataScriptMaker : EndNameEditAction
    {

        [MenuItem("Assets/ECS/Create/Component", priority = 3)]
        public static void CreateUI()
        {
            var path = Path.Combine(Application.dataPath, PathStrings.TemplatePath, PathStrings.ComponentDataPath);

            var file = File.ReadAllText(path);
            var asset = new TextAsset(file);
            //file.Replace("[CustomUIForm]", )
            Debug.Log(file);

            Texture2D csIcon =
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;

            var endNameEditAction =
                ScriptableObject.CreateInstance<ComponentDataScriptMaker>();

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                endNameEditAction, "New ComponentData.cs", csIcon, path);



        }

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            var text = File.ReadAllText(resourceFile);

            var className = Path.GetFileNameWithoutExtension(pathName);

            className = className.Replace(" ", "");
            text = text.Replace("[ScriptName]", className);
            var encoding = new UTF8Encoding(true, false);

            var rootNamespace = CompilationPipeline.GetAssemblyRootNamespaceFromScriptPath(pathName);
            text = PathStrings.RemoveOrInsertNamespace(text, rootNamespace);

            pathName = Path.Combine(Path.GetDirectoryName(pathName), $"{className}ComponentData.cs");
            File.WriteAllText(pathName, text, encoding);

            AssetDatabase.ImportAsset(pathName);
            var asset = AssetDatabase.LoadAssetAtPath<MonoScript>(pathName);
            ProjectWindowUtil.ShowCreatedAsset(asset);
        }
    }


    public class AspectScriptMaker : EndNameEditAction
    {

        [MenuItem("Assets/ECS/Create/Aspect", priority = 4)]
        public static void CreateUI()
        {
            var path = Path.Combine(Application.dataPath, PathStrings.TemplatePath, PathStrings.AspectPath);

            var file = File.ReadAllText(path);
            var asset = new TextAsset(file);
            //file.Replace("[CustomUIForm]", )
            Debug.Log(file);

            Texture2D csIcon =
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;

            var endNameEditAction =
                ScriptableObject.CreateInstance<AspectScriptMaker>();

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                endNameEditAction, "New Aspect.cs", csIcon, path);



        }

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            var text = File.ReadAllText(resourceFile);

            var className = Path.GetFileNameWithoutExtension(pathName);

            className = className.Replace(" ", "");
            text = text.Replace("[ScriptName]", className);
            var encoding = new UTF8Encoding(true, false);

            var rootNamespace = CompilationPipeline.GetAssemblyRootNamespaceFromScriptPath(pathName);
            text = PathStrings.RemoveOrInsertNamespace(text, rootNamespace);

            pathName = Path.Combine(Path.GetDirectoryName(pathName), $"{className}Aspect.cs");
            File.WriteAllText(pathName, text, encoding);

            AssetDatabase.ImportAsset(pathName);
            var asset = AssetDatabase.LoadAssetAtPath<MonoScript>(pathName);
            ProjectWindowUtil.ShowCreatedAsset(asset);
        }
    }



    public class SystemScriptMaker : EndNameEditAction
    {

        [MenuItem("Assets/ECS/Create/System", priority = 5)]
        public static void CreateUI()
        {
            var path = Path.Combine(Application.dataPath, PathStrings.TemplatePath, PathStrings.SystemDataPath);

            var file = File.ReadAllText(path);
            var asset = new TextAsset(file);
            //file.Replace("[CustomUIForm]", )
            Debug.Log(file);

            Texture2D csIcon =
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;

            var endNameEditAction =
                ScriptableObject.CreateInstance<SystemScriptMaker>();

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                endNameEditAction, "New System.cs", csIcon, path);



        }

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            var text = File.ReadAllText(resourceFile);

            var className = Path.GetFileNameWithoutExtension(pathName);

            className = className.Replace(" ", "");
            text = text.Replace("[ScriptName]", className);
            var encoding = new UTF8Encoding(true, false);

            var rootNamespace = CompilationPipeline.GetAssemblyRootNamespaceFromScriptPath(pathName);
            text = PathStrings.RemoveOrInsertNamespace(text, rootNamespace);

            pathName = Path.Combine(Path.GetDirectoryName(pathName), $"{className}System.cs");
            File.WriteAllText(pathName, text, encoding);

            AssetDatabase.ImportAsset(pathName);
            var asset = AssetDatabase.LoadAssetAtPath<MonoScript>(pathName);
            ProjectWindowUtil.ShowCreatedAsset(asset);
        }
    }

    public class SystemAndJobScriptMaker : EndNameEditAction
    {

        [MenuItem("Assets/ECS/Create/System N Job", priority = 7)]
        public static void CreateUI()
        {
            var path = Path.Combine(Application.dataPath, PathStrings.TemplatePath, PathStrings.SystemAndJobmDataPath);

            var file = File.ReadAllText(path);
            var asset = new TextAsset(file);
            //file.Replace("[CustomUIForm]", )
            Debug.Log(file);

            Texture2D csIcon =
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;

            var endNameEditAction =
                ScriptableObject.CreateInstance<SystemAndJobScriptMaker>();

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                endNameEditAction, "New System.cs", csIcon, path);



        }

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            var text = File.ReadAllText(resourceFile);

            var className = Path.GetFileNameWithoutExtension(pathName);

            className = className.Replace(" ", "");
            text = text.Replace("[ScriptName]", className);
            var encoding = new UTF8Encoding(true, false);

            var rootNamespace = CompilationPipeline.GetAssemblyRootNamespaceFromScriptPath(pathName);
            text = PathStrings.RemoveOrInsertNamespace(text, rootNamespace);

            pathName = Path.Combine(Path.GetDirectoryName(pathName), $"{className}System.cs");
            File.WriteAllText(pathName, text, encoding);

            AssetDatabase.ImportAsset(pathName);
            var asset = AssetDatabase.LoadAssetAtPath<MonoScript>(pathName);
            ProjectWindowUtil.ShowCreatedAsset(asset);
        }
    }



    public class JobScriptMaker : EndNameEditAction
    {

        [MenuItem("Assets/ECS/Create/Job", priority = 6)]
        public static void CreateUI()
        {
            var path = Path.Combine(Application.dataPath, PathStrings.TemplatePath, PathStrings.JobDataPath);

            var file = File.ReadAllText(path);
            var asset = new TextAsset(file);
            //file.Replace("[CustomUIForm]", )
            Debug.Log(file);

            Texture2D csIcon =
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;

            var endNameEditAction =
                ScriptableObject.CreateInstance<JobScriptMaker>();

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                endNameEditAction, "New Job.cs", csIcon, path);



        }

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            var text = File.ReadAllText(resourceFile);

            var className = Path.GetFileNameWithoutExtension(pathName);

            className = className.Replace(" ", "");
            text = text.Replace("[ScriptName]", className);
            var encoding = new UTF8Encoding(true, false);

            var rootNamespace = CompilationPipeline.GetAssemblyRootNamespaceFromScriptPath(pathName);
            text = PathStrings.RemoveOrInsertNamespace(text, rootNamespace);

            pathName = Path.Combine(Path.GetDirectoryName(pathName), $"{className}Job.cs");
            File.WriteAllText(pathName, text, encoding);

            AssetDatabase.ImportAsset(pathName);
            var asset = AssetDatabase.LoadAssetAtPath<MonoScript>(pathName);
            ProjectWindowUtil.ShowCreatedAsset(asset);
        }
    }



    public class PackScriptMaker : EndNameEditAction
    {

        [MenuItem("Assets/ECS/Create/Pack", priority = 6)]
        public static void CreateUI()
        {
            var path = Path.Combine(Application.dataPath, PathStrings.TemplatePath, PathStrings.PackTemplate);
            Texture2D csIcon = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;
            var endNameEditAction =CreateInstance<PackScriptMaker>();
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                endNameEditAction, "New Authoring.cs", csIcon, path);
        }

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            var text = File.ReadAllText(resourceFile);

            var className = Path.GetFileNameWithoutExtension(pathName);

            className = className.Replace(" ", "");
            text = text.Replace("[ScriptName]", className);
            var encoding = new UTF8Encoding(true, false);

            var rootNamespace = CompilationPipeline.GetAssemblyRootNamespaceFromScriptPath(pathName);
            text = PathStrings.RemoveOrInsertNamespace(text, rootNamespace);

            pathName = Path.Combine(Path.GetDirectoryName(pathName), $"{className}Authoring.cs");
            File.WriteAllText(pathName, text, encoding);

            AssetDatabase.ImportAsset(pathName);
            var asset = AssetDatabase.LoadAssetAtPath<MonoScript>(pathName);
            ProjectWindowUtil.ShowCreatedAsset(asset);
        }
    }
}
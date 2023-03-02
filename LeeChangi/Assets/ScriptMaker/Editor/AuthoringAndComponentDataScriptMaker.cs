using System.IO;
using System.Text;
using UnityEditor;
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
        public static readonly string SystemDataPath = "SystemAndTemplate.txt";
        public static readonly string SystemAndJobmDataPath = "SystemAndJobTemplate.txt";
        public static readonly string JobDataPath = "JobTemplate.txt";
        public static readonly string PackTemplate = "PackTemplate.txt";
        public static readonly string TemplatePath = "ScriptMaker/CustomScriptTemplates";


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

            pathName = Path.Combine(Path.GetDirectoryName(pathName), $"{className}Authoring.cs");
            File.WriteAllText(pathName, text, encoding);

            AssetDatabase.ImportAsset(pathName);
            var asset = AssetDatabase.LoadAssetAtPath<MonoScript>(pathName);
            ProjectWindowUtil.ShowCreatedAsset(asset);
        }
    }
}
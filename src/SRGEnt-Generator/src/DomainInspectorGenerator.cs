using SRGEnt.Generator.DataTypes;
using Microsoft.CodeAnalysis;

namespace SRGEnt.Generator
{
    public class DomainInspectorGenerator
    {
        public static void GenerateDomainInspector(GeneratorExecutionContext context, Domain domain)
        {
            var domainSymbolName = domain.DomainName;
            var entityName = domain.Entity.EntityTypeName;

            var domainInspectorBody = $@"
#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace SRGEnt.Generated
{{
    public class {domainSymbolName}Inspector : EditorWindow
    {{
        private static {domainSymbolName}Inspector _wnd;

        [MenuItem(""SRGEnt/Inspectors/{domainSymbolName}Inspector"")]
        public static void Show{domainSymbolName}Editor()
        {{
            // This method is called when the user selects the menu item in the Editor
            _wnd = GetWindow<{domainSymbolName}Inspector>();
            _wnd.titleContent = new GUIContent(""{domainSymbolName}Inspector"");
        }}


        public void CreateGUI()
        {{
            var container = FindObjectOfType<{domain.DomainName}Container>();
            if (container != null)
            {{
                var domain = container.{domain.DomainName};
                var list = new ScrollView();
                foreach (var entry in domain.EntitiesByUid)
                {{
                    var entity = entry.Value;

                    var box = new Box();
                    box.style.borderTopWidth = 1;
                    box.style.borderRightWidth = 1;
                    box.style.borderBottomWidth = 1;
                    box.style.borderLeftWidth = 1;
                    box.style.borderTopColor = new StyleColor(Color.gray);
                    box.style.borderRightColor = new StyleColor(Color.gray);
                    box.style.borderBottomColor = new StyleColor(Color.gray);
                    box.style.borderLeftColor = new StyleColor(Color.gray);
                    box.style.marginTop = 1;
                    box.style.marginBottom = 1;
                    
                    var entityVisualElement = new {domain.Entity.EntityTypeName}VisualElement(entity, domain);
                    box.Add(entityVisualElement);
                    list.Add(box);
                }}
                rootVisualElement.Add(list);
            }}
            else
            {{
                rootVisualElement.Add(new Label(""There are no {domain.DomainName}Containers present at this time""));
            }}
        }}
    }}

}}
#endif
";
            
            FormattedFileWriter.WriteSourceFile(context,domainInspectorBody,$"{domainSymbolName}Inspector");
        }
    }
}
using System.Text;
using SRGEnt.Generator.DataTypes;
using SRGEnt.Generator.DataTypes.Utils;
using Microsoft.CodeAnalysis;

namespace SRGEnt.Generator.Unity
{
    public class UnityInspectorVisualElementGenerator
    {
        public static void GenerateUnityInspectorVisualElement(GeneratorExecutionContext context, Domain domain)
        {
            var entityName = domain.Entity.EntityTypeName;

            var propertyBlockBuilder = new StringBuilder();

            for (var i = 0; i < domain.Entity.Components.Count; i++)
            {
                var component = domain.Entity.Components[i];
                propertyBlockBuilder.AppendLine(GenerateEntityPropertyBlock(component));
            }

            var visualElementBody = $@"{GeneratorConstants.GeneratorHeader}
#if UNITY_EDITOR
using SRGEnt.Generated;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SRGEnt.Generated
{{
    public class {entityName}VisualElement : VisualElement
    {{
        private readonly {domain.DomainName} _domain;
    
        private readonly Serializable{domain.Entity.EntityTypeName} _serializableEntity;
        private readonly SerializedObject _serializedObject;
        private readonly IVisualElementScheduledItem _refreshTask;

        private bool _foldoutState; 

        public {domain.Entity.EntityTypeName}VisualElement({domain.Entity.EntityTypeName} entity, {domain.DomainName} domain)
        {{
            _domain = domain;
        
            _serializableEntity = ScriptableObject.CreateInstance<Serializable{domain.Entity.EntityTypeName}>();
            _serializableEntity.Populate(entity);
            _serializedObject = new SerializedObject(_serializableEntity);

            BuildEntityFoldout();
            _refreshTask = schedule.Execute(state =>
            {{
                ReserializeEntity();
            }});
            _refreshTask.Every(500);
        }}

        private void BuildEntityFoldout()
        {{
            var entityFoldout = new Foldout
            {{
                text = $""{entityName} [UId - {{_serializableEntity.UId.ToString()}}]""
            }};
        
            entityFoldout.RegisterValueChangedCallback((e) =>
            {{
                if (e.target != entityFoldout) return;

                if (e.newValue == false)
                {{
                    _refreshTask?.Pause();
                    _foldoutState = false;
                }}
                else
                {{
                    _foldoutState = true;
                    _refreshTask?.Resume();
                }}
            }});

            BuildEntityIdPropertyInspector(entityFoldout);
            entityFoldout.value = _foldoutState;

            contentContainer.Add(entityFoldout);
        }}

        private void Suicide()
        {{
            this.parent.parent.Remove(this.parent);
            _refreshTask.Pause();
            _serializedObject.Dispose();
            Object.Destroy(_serializableEntity);
        }}

        private void ReserializeEntity()
        {{
            if (_domain.EntitiesByUid.TryGetValue(_serializableEntity.UId, out var entity))
            {{
                _serializableEntity.Populate(entity);
                RefreshView();
            }}
            else
            {{
                Suicide();
            }}
        }}

        private void RefreshView()
        {{
            contentContainer.Clear();
            contentContainer.MarkDirtyRepaint();
            BuildEntityFoldout();
        }}

        private void BuildEntityIdPropertyInspector(Foldout entityFoldout)
        {{
{propertyBlockBuilder}
        }}

    }}
}}
#endif";
            FormattedFileWriter.WriteSourceFile(context, visualElementBody, $"{entityName}VisualElement");
        }

        private static string GenerateEntityPropertyBlock(Component component)
        {
            var camelCaseComponentName = component.Name.ToCamelCase();
            if (component.IsFlag)
            {
                return $@"
            var {camelCaseComponentName}PropertyContainer = new VisualElement();
            {camelCaseComponentName}PropertyContainer.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
            var {camelCaseComponentName}Component = new PropertyField(_serializedObject.FindProperty(""{component.Name}""));
            {camelCaseComponentName}Component.Bind(_serializedObject);
            {camelCaseComponentName}Component.RegisterValueChangeCallback((changeEvent) =>
            {{
                if (_domain.EntitiesByUid.TryGetValue(_serializableEntity.UId, out var entity))
                {{
                    entity.{component.Name} = _serializableEntity.{component.Name};
                }}
                else
                {{
                    Suicide();
                }}
            }});
            entityFoldout.contentContainer.Add({camelCaseComponentName}Component);";
            }
            else
            {
                return $@"          var {camelCaseComponentName}PropertyContainer = new VisualElement();
            if (_serializableEntity.Has{component.Name})
            {{
                {camelCaseComponentName}PropertyContainer.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
                var {camelCaseComponentName}Component = new PropertyField(_serializedObject.FindProperty(""{component.Name}""));
                {camelCaseComponentName}Component.Bind(_serializedObject);
                {camelCaseComponentName}Component.style.flexGrow = new StyleFloat(1);
                {camelCaseComponentName}Component.RegisterValueChangeCallback((changeEvent) =>
                {{
                    if (_domain.EntitiesByUid.TryGetValue(_serializableEntity.UId, out var entity))
                    {{
                        entity.{component.Name} = _serializableEntity.{component.Name};
                    }}
                    else
                    {{
                        Suicide();
                    }}
                }});

                var removeComponentButton = new Button
                {{
                    text = ""Remove {component.Name}""
                }};
                removeComponentButton.clicked += () =>
                {{
                    if (_domain.EntitiesByUid.TryGetValue(_serializableEntity.UId, out var entity))
                    {{
                        entity.Remove{component.Name}();
                        ReserializeEntity();
                    }}
                    else
                    {{
                        Suicide();
                    }}
                }};

                {camelCaseComponentName}PropertyContainer.Add({camelCaseComponentName}Component);
                {camelCaseComponentName}PropertyContainer.Add(removeComponentButton);
            }}
            else
            {{
                var addComponentButton = new Button
                {{
                    text = ""Add {component.Name}""
                }};
                addComponentButton.clicked += () =>
                {{
                    if (_domain.EntitiesByUid.TryGetValue(_serializableEntity.UId, out var entity))
                    {{
                        entity.{component.Name} = default;
                        ReserializeEntity();
                    }}
                    else
                    {{
                        Suicide();
                    }}
                }};
                {camelCaseComponentName}PropertyContainer.Add(addComponentButton);
            }}
            entityFoldout.contentContainer.Add({camelCaseComponentName}PropertyContainer);";
            }
        }
    }
}
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace Viscera
{ 

public class VisceraWindow : EditorWindow
{
    [MenuItem("Window/Viscera")]
	static void ShowWindow()
    {
        var w = EditorWindow.GetWindow<VisceraWindow>("Viscera");
        w.minSize = new Vector2(300, 240);
        w.Show();
    }

    private List<ViseraTabPage> _pages = new List<ViseraTabPage>();
    private int _pageIndex = 0;

    void OnEnable()
    {
        AddPage();
    }

    void OnGUI()
    {
        Helper.WindowWidth = position.width;

        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        bool middleDown = false;
        if(Event.current.type == EventType.MouseUp)
        {
            if(Event.current.button == 2) middleDown = true;
        }
        float windowWidth = Helper.WindowWidth;
        float sumWidth = 0;
        for(int i=0;i<_pages.Count;++i)
        {
            var p = _pages[i];
            var title = new GUIContent(p.target == null ? "Blank" : p.target.name);
            var w = EditorStyles.toolbarButton.CalcSize(title).x;
            if(sumWidth + w * 0.7f > windowWidth)
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                sumWidth = 0;
            }
            sumWidth += w;
            var b = GUILayout.Toggle(i == _pageIndex, title, EditorStyles.toolbarButton);
            if(b) _pageIndex = i;
        }
        
        if(middleDown & _pages.Count > 1)
        {
            _pages.RemoveAt(_pageIndex);
            if(_pageIndex >= _pages.Count) _pageIndex = _pages.Count -1;
        }
        var titlePlus = new GUIContent("[+]");
        var wPlus = EditorStyles.toolbarButton.CalcSize(titlePlus).x;
        if(sumWidth + wPlus * 0.7f > windowWidth)
        {
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            sumWidth = 0;
        }
        if(GUILayout.Button(titlePlus, EditorStyles.toolbarButton))
        {
            ViseraTabPage found = null;
            for(int i=0;i<_pages.Count;++i)
            {
                var p = _pages[i];
                if(p.target == Selection.activeObject)
                {
                    found = p;
                    break;
                }
            }
            var page = AddPage();
            if(found == null) page.Select();
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        _pages[_pageIndex].OnGUI();
    }

    void Update()
    {
        Helper.MemberNameWidth = 0;
        Helper.MemberTypeWidth = 0;

        _pages[_pageIndex].Update();

        Repaint();
    }

    ViseraTabPage AddPage()
    { 
        var page = new ViseraTabPage();
        _pages.Add(page);
        _pageIndex = _pages.Count - 1;
        return page;
    }
}

public class ViseraTabPage
{
    public Object target;

    public List<Entity> hierarchy = new List<Entity>();

    private int _truncateIndex;
    private Entity _currentEntity;
    
    public void Select()
    {
        target = Selection.activeObject;
        if(target != null)
        {
            hierarchy.Clear();
            if(target is GameObject)
            {
                hierarchy.Add(new GameObjectEntity(target as GameObject));
            }
            else
            {
                hierarchy.Add(new UnityObjectEntity(target as Object));
            }
        }
    }

    public void OnGUI()
    {
        if(target == null)
        {
            OnGUI_EmptyTarget();
            return;
        }
        
        EditorGUILayout.BeginHorizontal(S.BreadcrumbBar, GUILayout.ExpandWidth(true));
        if(GUILayout.Button("ROOT", S.BreadcrumbLeft))
        {
            GUI.FocusControl("");
            _truncateIndex = 1;
        }
        
        float windowWidth = Helper.WindowWidth-36;
        float sumWidth = 0;
        for(int i=1;i<hierarchy.Count;++i)
        {
            var entity = hierarchy[i];
            var guiContent = new GUIContent(entity.Name);
            var style = sumWidth > 0 ? S.BreadcrumbMid : S.BreadcrumbLeft;
            if(GUILayout.Button(guiContent, S.BreadcrumbMid))
            {
                GUI.FocusControl("");
                _truncateIndex = i+1;
            }
            var w = S.BreadcrumbMid.CalcSize(guiContent).x;
            sumWidth += w;
            if(sumWidth >= windowWidth)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(S.BreadcrumbBar, GUILayout.ExpandWidth(true));
                sumWidth = sumWidth - windowWidth;
                GUILayout.Space(sumWidth - w);
                if(GUILayout.Button(guiContent, S.BreadcrumbMid))
                {
                    GUI.FocusControl("");
                    _truncateIndex = i+1;
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        if(_currentEntity != null)
            _currentEntity.DoGUI();
    }

    public void Update()
    {
        if(target == null) return;
        if(_truncateIndex == 0)_truncateIndex = hierarchy.Count;
        for (int i = 1; i < _truncateIndex; ++i)
        {
            var entity = hierarchy[i];
            if(!entity.CheckValue())
            {
                _truncateIndex = i;
                break;
            }
        }
        if(_truncateIndex > 0 && _truncateIndex < hierarchy.Count)
        {
            hierarchy.RemoveRange(_truncateIndex, hierarchy.Count - _truncateIndex);
        }
        else if(Helper.GoInsideEntity != null)
        {
            hierarchy.Add(Helper.GoInsideEntity);
            Helper.GoInsideEntity = null;
        }
        _truncateIndex = 0;
        _currentEntity = hierarchy[hierarchy.Count-1];
        _currentEntity.Update();
    }

    void OnGUI_EmptyTarget()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(30);
        EditorGUILayout.BeginVertical();
        GUILayout.Space(30);
        EditorGUILayout.BeginVertical("GroupBox");
        GUILayout.Label("Selected Game Object", "OL Titlemid");
        EditorGUILayout.Space();
        EditorGUILayout.ObjectField(Selection.activeObject, typeof(Object), true);
        EditorGUILayout.EndVertical();
        if(GUILayout.Button(S.GoInsideContent, GUILayout.Height(40)))
        {
            Select();
        }
        EditorGUILayout.EndVertical();
        GUILayout.Space(30);
        EditorGUILayout.EndHorizontal();
    }
}
}
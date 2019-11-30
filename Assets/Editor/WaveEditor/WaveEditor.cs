using AirTowerDefence.EnemySpawn;
using AirTowerDefence.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AirTowerDefence.EditorTool
{
    public class WaveEditor
    {
        public static void BeginWaveEditing(GameObject spawnpoint, WavesManager wavesmanager)
        {
            EditorSceneManager.SaveOpenScenes();

            if (spawnpoint == null)
            {
                Debug.Log("Need a spawnpoint to edit");
                return;
            }

            if (wavesmanager == null)
            {
                Debug.Log($"Must have one component <{nameof(WavesManager)}> attached");
                return;
            }

            //Pre-setup for wave editor scene.
            GameObject spawnobjectcopy = WaveEditorHelper.BuildBareMeshCopy(spawnpoint);
            spawnobjectcopy.name = "Spawn-Preview";

            spawnobjectcopy.AddComponent<ImmutableGameObject>();

            spawnobjectcopy.transform.position = spawnpoint.transform.position;
            spawnobjectcopy.transform.rotation = spawnpoint.transform.rotation;

            HideAllObjectsExcept(spawnobjectcopy);
            Selection.activeGameObject = spawnobjectcopy;
            SceneView.lastActiveSceneView.FrameSelected();

            var editorgui = new WaveEditorSceneContext(spawnobjectcopy, wavesmanager.WavesContainer, () =>
            {
                RestoreOriginalScene();
                GameObject.DestroyImmediate(spawnobjectcopy);
            });
        }

        private static void HideAllObjectsExcept(params GameObject[] exceptions)
        {
            GameObject[] allobjects = GameObject.FindObjectsOfType(typeof(GameObject)).Cast<GameObject>().ToArray();

            foreach (var go in allobjects)
            {
                go.SetActive(false);
            }

            foreach (var gameobj in exceptions)
            {
                foreach (var transform in gameobj.GetComponentsInChildren<Transform>())
                {
                    transform.gameObject.SetActive(true);
                }
            }

        }

        private static void RestoreOriginalScene()
        {
            GameObject[] allobjects = Resources.FindObjectsOfTypeAll(typeof(GameObject)).Cast<GameObject>().ToArray();

            foreach (var go in allobjects)
            {
                go.SetActive(true);
            }
        }

        public sealed class WaveEditorSceneContext
        {
            private Action ExitEditingAction;
            private WavesContainer _WavesContainer;
            private IWaveEditorState[] _WaveEditorStates;

            private IWaveEditorState _WaveEditorActiveState;

            private GUISkin _EditorGUISkin;

            const float BottomBarHeight = 35f;
            const float OffsetFromScreenBorder = 10f;

            private GameObject _Spawnpoint;
            private Wave _SelectedWave;
            private Spawn _SelectedSpawn;

            public WaveEditorSceneContext(GameObject spawnpoint, WavesContainer wavescontainer, Action exiteditingaction)
            {
                _Spawnpoint = spawnpoint;
                ExitEditingAction = exiteditingaction;
                _WavesContainer = wavescontainer;

                _EditorGUISkin = Resources.Load<GUISkin>("WaveEditorSkin");
                SetDefaultButtonGraphicsForCustomStyles();

                _WaveEditorStates = new IWaveEditorState[]
                {
            new OverviewScene(this),
            new WaveEditingScene(this),
            new SpawnEditingScene(this)
                };

                SwitchView<OverviewScene>();

                SceneView.duringSceneGui += RenderSceneGUI;
            }

            private void SetDefaultButtonGraphicsForCustomStyles()
            {
                GUIStyle DefaultBtnStyle = _EditorGUISkin.button;

                foreach (var style in _EditorGUISkin.customStyles)
                {
                    style.margin = DefaultBtnStyle.margin;
                    style.padding = DefaultBtnStyle.padding;
                    style.border = DefaultBtnStyle.border;

                    style.normal.background = DefaultBtnStyle.normal.background;
                    style.normal.textColor = DefaultBtnStyle.normal.textColor;

                    style.hover.background = DefaultBtnStyle.hover.background;
                    style.hover.textColor = DefaultBtnStyle.hover.textColor;

                    style.active.background = DefaultBtnStyle.active.background;
                    style.active.textColor = DefaultBtnStyle.active.textColor;

                    style.focused.background = DefaultBtnStyle.focused.background;
                    style.focused.textColor = DefaultBtnStyle.focused.textColor;

                    style.onNormal.background = DefaultBtnStyle.onNormal.background;
                    style.onNormal.textColor = DefaultBtnStyle.onNormal.textColor;

                    style.onHover.background = DefaultBtnStyle.onHover.background;
                    style.onHover.textColor = DefaultBtnStyle.onHover.textColor;

                    style.onActive.background = DefaultBtnStyle.onActive.background;
                    style.onActive.textColor = DefaultBtnStyle.onActive.textColor;

                    style.onFocused.background = DefaultBtnStyle.onFocused.background;
                    style.onFocused.textColor = DefaultBtnStyle.onFocused.textColor;
                }
            }

            private void RenderSceneGUI(SceneView sceneview)
            {
                Handles.BeginGUI();

                _WaveEditorActiveState.RenderSceneGUI();

                Handles.EndGUI();
            }

            private void ExitToOriginalScene()
            {
                SceneView.duringSceneGui -= RenderSceneGUI;
                ExitEditingAction?.Invoke();
            }

            private void SwitchView<T>(Wave selectwave = null, Spawn selectspawn = null) where T : IWaveEditorState
            {
                _WaveEditorActiveState = _WaveEditorStates.Single(x => x is T);

                _SelectedWave = selectwave;
                _SelectedSpawn = selectspawn;
            }

            private class OverviewScene : IWaveEditorState
            {
                WaveEditorSceneContext _Context;

                Rect ViewNavigationSection;
                Rect CreateAndEditSection;
                Rect DeleteButtonSection;

                GUIStyle CreateNewButtonStyle;
                GUIStyle ReturnButtonStyle;
                GUIStyle WaveSpawnNavButtonStyle;
                GUIStyle DeleteButtonStyle;

                const int Columns = 3;
                int Rows => WaveEditorHelper.CalculateRows(Columns, _Context._WavesContainer.Waves.Count);

                public OverviewScene(WaveEditorSceneContext context)
                {
                    _Context = context;

                    CreateNewButtonStyle = _Context._EditorGUISkin.GetStyle("CreateNewButton");
                    ReturnButtonStyle = _Context._EditorGUISkin.GetStyle("ReturnButton");
                    WaveSpawnNavButtonStyle = _Context._EditorGUISkin.GetStyle("WaveSpawnNavButton");
                    DeleteButtonStyle = _Context._EditorGUISkin.GetStyle("DeleteButton");

                }

                public void RenderSceneGUI()
                {
                    UpdateSectionArea();

                    GUILayout.BeginArea(ViewNavigationSection);
                    {
                        RenderNavigationSection();
                    }
                    GUILayout.EndArea();

                    GUILayout.BeginArea(DeleteButtonSection);
                    {
                        RenderDeleteButtonSection();
                    }
                    GUILayout.EndArea();

                    GUILayout.BeginArea(CreateAndEditSection);
                    {
                        RenderCreateAndEditSection();
                    }
                    GUILayout.EndArea();
                }

                private void UpdateSectionArea()
                {
                    //NavigationButton
                    ViewNavigationSection.width = ReturnButtonStyle.fixedWidth;
                    ViewNavigationSection.height = ReturnButtonStyle.fixedHeight;
                    ViewNavigationSection.y = OffsetFromScreenBorder;
                    ViewNavigationSection.x = OffsetFromScreenBorder;

                    float BottomMargin = _Context._EditorGUISkin.button.margin.bottom;
                    float HorizontalMargin = _Context._EditorGUISkin.button.margin.horizontal;

                    CreateAndEditSection.width = CreateNewButtonStyle.fixedWidth + ((WaveSpawnNavButtonStyle.fixedWidth + HorizontalMargin) * Columns);
                    CreateAndEditSection.height = (CreateNewButtonStyle.fixedHeight + (WaveSpawnNavButtonStyle.fixedHeight + (BottomMargin)) * Rows);

                    CreateAndEditSection.x = OffsetFromScreenBorder;
                    CreateAndEditSection.y = (Screen.height - CreateAndEditSection.height) - (BottomBarHeight + OffsetFromScreenBorder);

                    DeleteButtonSection.width = DeleteButtonStyle.fixedWidth;
                    DeleteButtonSection.height = DeleteButtonStyle.fixedHeight;
                    DeleteButtonSection.y = (Screen.height - DeleteButtonSection.height) - (BottomBarHeight + OffsetFromScreenBorder);
                    DeleteButtonSection.x = (Screen.width / 2) - (DeleteButtonStyle.fixedWidth / 2);
                }

                private void RenderNavigationSection()
                {
                    if (GUILayout.Button("Return to original scene", ReturnButtonStyle))
                    {
                        _Context.ExitToOriginalScene();
                    }
                }

                private void RenderCreateAndEditSection()
                {
                    GUILayout.BeginVertical();
                    {
                        RenderButton_CreateNewWave();


                        RenderButtons_EditWave();

                    }
                    GUILayout.EndVertical();
                }

                private void RenderDeleteButtonSection()
                {
                    if (GUILayout.Button("Clear all waves/spawns", DeleteButtonStyle))
                    {
                        _Context._WavesContainer.Waves.Clear();
                    }
                }

                private void RenderButtons_EditWave()
                {

                    //Increase I with Columns each time
                    for (int i = 0; i < _Context._WavesContainer.Waves.Count; i += Columns)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            for (int z = 0; z < Columns; z++)
                            {
                                //Prevents from trying to draw another button in an incomplete row if there are none left to be drawn
                                if (i + z >= _Context._WavesContainer.Waves.Count)
                                {
                                    break;
                                }

                                if (GUILayout.Button($"Wave{i + z}", WaveSpawnNavButtonStyle))
                                {
                                    _Context.SwitchView<WaveEditingScene>(_Context._WavesContainer.Waves[i + z]);
                                }
                            }
                        }

                        GUILayout.EndHorizontal();
                    }
                }

                private void RenderButton_CreateNewWave()
                {
                    if (GUILayout.Button("Create New Wave", CreateNewButtonStyle))
                    {
                        CreateNewEmptyWave();
                    }
                }

                private void CreateNewEmptyWave()
                {
                    var wave = new Wave();

                    _Context._WavesContainer.Waves.Add(wave);
                }
            }

            private class WaveEditingScene : IWaveEditorState
            {
                WaveEditorSceneContext _Context;

                Rect ViewNavigationSection;
                Rect CreateAndEditSection;
                Rect DeleteButtonSection;

                GUIStyle CreateNewButtonStyle;
                GUIStyle ReturnButtonStyle;
                GUIStyle WaveSpawnNavButtonStyle;
                GUIStyle DeleteButtonStyle;

                const int Columns = 3;
                int Rows => WaveEditorHelper.CalculateRows(Columns, _Context._SelectedWave.Spawns.Count);

                public WaveEditingScene(WaveEditorSceneContext context)
                {
                    _Context = context;

                    CreateNewButtonStyle = _Context._EditorGUISkin.GetStyle("CreateNewButton");
                    ReturnButtonStyle = _Context._EditorGUISkin.GetStyle("ReturnButton");
                    WaveSpawnNavButtonStyle = _Context._EditorGUISkin.GetStyle("WaveSpawnNavButton");
                    DeleteButtonStyle = _Context._EditorGUISkin.GetStyle("DeleteButton");
                }

                public void RenderSceneGUI()
                {
                    UpdateSectionArea();

                    GUILayout.BeginArea(ViewNavigationSection);
                    {
                        RenderNavigationSection();
                    }
                    GUILayout.EndArea();

                    GUILayout.BeginArea(DeleteButtonSection);
                    {
                        RenderDeleteButtonSection();
                    }
                    GUILayout.EndArea();

                    GUILayout.BeginArea(CreateAndEditSection);
                    {
                        RenderCreateAndEditSection();
                    }
                    GUILayout.EndArea();
                }

                private void UpdateSectionArea()
                {
                    //NavigationButton
                    ViewNavigationSection.width = ReturnButtonStyle.fixedWidth;
                    ViewNavigationSection.height = ReturnButtonStyle.fixedHeight;
                    ViewNavigationSection.y = OffsetFromScreenBorder;
                    ViewNavigationSection.x = OffsetFromScreenBorder;

                    float BottomMargin = _Context._EditorGUISkin.button.margin.bottom;
                    float HorizontalMargin = _Context._EditorGUISkin.button.margin.horizontal;

                    CreateAndEditSection.width = CreateNewButtonStyle.fixedWidth + ((WaveSpawnNavButtonStyle.fixedWidth + HorizontalMargin) * Columns);
                    CreateAndEditSection.height = (CreateNewButtonStyle.fixedHeight + (WaveSpawnNavButtonStyle.fixedHeight + (BottomMargin)) * Rows);

                    CreateAndEditSection.x = OffsetFromScreenBorder;
                    CreateAndEditSection.y = (Screen.height - CreateAndEditSection.height) - (BottomBarHeight + OffsetFromScreenBorder);

                    DeleteButtonSection.width = DeleteButtonStyle.fixedWidth;
                    DeleteButtonSection.height = DeleteButtonStyle.fixedHeight;
                    DeleteButtonSection.y = (Screen.height - DeleteButtonSection.height) - (BottomBarHeight + OffsetFromScreenBorder);
                    DeleteButtonSection.x = (Screen.width / 2) - (DeleteButtonStyle.fixedWidth / 2);
                }

                private void RenderNavigationSection()
                {
                    if (GUILayout.Button("Return to overview", ReturnButtonStyle))
                    {
                        _Context.SwitchView<OverviewScene>();
                    }
                }

                private void RenderDeleteButtonSection()
                {
                    if (GUILayout.Button("Delete this wave", DeleteButtonStyle))
                    {
                        DeleteThisWave();
                    }
                }

                private void DeleteThisWave()
                {
                    _Context._WavesContainer.Waves.Remove(_Context._SelectedWave);
                    _Context.SwitchView<OverviewScene>();
                }

                private void RenderCreateAndEditSection()
                {
                    GUILayout.BeginVertical();
                    {
                        RenderButton_CreateNewSpawnData();

                        RenderButtons_EditSpawn();
                    }
                    GUILayout.EndVertical();

                }

                private void RenderButton_CreateNewSpawnData()
                {
                    if (GUILayout.Button("Create new spawn", CreateNewButtonStyle))
                    {
                        CreateNewEmptySpawn();
                    }
                }

                private void RenderButtons_EditSpawn()
                {
                    if (_Context._SelectedWave == null || _Context._SelectedWave.Spawns.Count == 0)
                    {
                        return;
                    }

                    //Increase I with Columns each time
                    for (int i = 0; i < _Context._SelectedWave.Spawns.Count; i += Columns)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            for (int z = 0; z < Columns; z++)
                            {
                                //Prevents from trying to draw another button in an incomplete row if there are none left to be drawn
                                if (i + z >= _Context._SelectedWave.Spawns.Count)
                                {
                                    break;
                                }

                                if (GUILayout.Button($"Spawn{i + z}", WaveSpawnNavButtonStyle))
                                {
                                    _Context.SwitchView<SpawnEditingScene>(_Context._SelectedWave, _Context._SelectedWave.Spawns[i + z]);
                                }

                                //Don't show text field if last one
                                if (i + z == _Context._SelectedWave.Spawns.Count - 1)
                                {
                                    break;
                                }


                                if (int.TryParse(GUILayout.TextField(_Context._SelectedWave.Spawns[i + z].TimeToNext.ToString(), _Context._EditorGUISkin.GetStyle("TextField")), out int res))
                                {
                                    _Context._SelectedWave.Spawns[i + z].TimeToNext = res;
                                }
                            }
                        }

                        GUILayout.EndHorizontal();
                    }
                }

                private void CreateNewEmptySpawn()
                {
                    var spawn = new Spawn();

                    _Context._SelectedWave.Spawns.Add(spawn);
                }
            }

            private class SpawnEditingScene : IWaveEditorState
            {
                WaveEditorSceneContext _Context;

                GameObject[] _CreepPrefabs;

                Rect ViewNavigationSection;
                Rect CreepDisplaySection;
                Rect SaveButtonSection;
                Rect DeleteButtonSection;

                GUIStyle ReturnButtonStyle;
                GUIStyle CreepButtonStyle;
                GUIStyle SaveButtonStyle;
                GUIStyle DeleteButtonStyle;

                const int Columns = 3;
                int Rows => WaveEditorHelper.CalculateRows(Columns, _CreepPrefabs.Length);

                bool _PreviewRendered = false;
                List<GameObject> _PreviewObjects = new List<GameObject>();

                const string PreviewObjectSuffix = "-Preview";

                readonly Vector3 SpawnPosition;

                public SpawnEditingScene(WaveEditorSceneContext context)
                {
                    _Context = context;

                    _CreepPrefabs = Resources.LoadAll("CreepPrefabs").Cast<GameObject>().ToArray();

                    ReturnButtonStyle = _Context._EditorGUISkin.GetStyle("ReturnButton");
                    CreepButtonStyle = _Context._EditorGUISkin.GetStyle("CreepButton");
                    SaveButtonStyle = _Context._EditorGUISkin.GetStyle("SaveButton");
                    DeleteButtonStyle = _Context._EditorGUISkin.GetStyle("DeleteButton");

                    SpawnPosition = _Context._Spawnpoint.transform.position;
                }

                private Texture GetCreepIcon(string name)
                {
                    return Resources.Load<Texture>($"CreepIcons/{name}");
                }

                public void RenderSceneGUI()
                {
                    UpdateSectionArea();

                    //Remove any objects that have been destroyed
                    if (_PreviewObjects.Any(x => x == null))
                    {
                        _PreviewObjects.RemoveAll(x => x == null);
                    }

                    if (!_PreviewRendered)
                    {
                        SetupSpawnPreview();
                    }



                    if (_PreviewObjects.Any(x => x != null && x.transform.hasChanged == true))
                    {
                        SaveCurrentSpawnData();
                    }

                    GUILayout.BeginArea(ViewNavigationSection);
                    {
                        RenderNavigationSection();
                    }
                    GUILayout.EndArea();

                    GUILayout.BeginArea(DeleteButtonSection);
                    {
                        RenderDeleteButtonSection();
                    }
                    GUILayout.EndArea();

                    GUILayout.BeginArea(CreepDisplaySection);
                    {
                        GUILayout.BeginVertical();
                        RenderCreepSelectionSection();
                        GUILayout.EndVertical();
                    }
                    GUILayout.EndArea();

                }

                private void UpdateSectionArea()
                {
                    ViewNavigationSection.width = ReturnButtonStyle.fixedWidth;
                    ViewNavigationSection.height = ReturnButtonStyle.fixedHeight;
                    ViewNavigationSection.y = OffsetFromScreenBorder;
                    ViewNavigationSection.x = OffsetFromScreenBorder;

                    float VerticalMargin = _Context._EditorGUISkin.button.margin.vertical;
                    float HorizontalMargin = _Context._EditorGUISkin.button.margin.horizontal;

                    CreepDisplaySection.width = (CreepButtonStyle.fixedWidth + HorizontalMargin) * Columns;
                    CreepDisplaySection.height = (CreepButtonStyle.fixedHeight + VerticalMargin) * Rows;

                    CreepDisplaySection.x = Screen.width - ((CreepButtonStyle.fixedWidth + HorizontalMargin) * Columns) - OffsetFromScreenBorder;
                    CreepDisplaySection.y = (Screen.height - CreepDisplaySection.height) - (BottomBarHeight + OffsetFromScreenBorder);

                    //TODO: remove
                    SaveButtonSection.width = SaveButtonStyle.fixedWidth;
                    SaveButtonSection.height = SaveButtonStyle.fixedHeight;

                    SaveButtonSection.x = OffsetFromScreenBorder;
                    SaveButtonSection.y = (Screen.height - SaveButtonSection.height) - (BottomBarHeight + OffsetFromScreenBorder);

                    DeleteButtonSection.width = DeleteButtonStyle.fixedWidth;
                    DeleteButtonSection.height = DeleteButtonStyle.fixedHeight;
                    DeleteButtonSection.y = (Screen.height - DeleteButtonSection.height) - (BottomBarHeight + OffsetFromScreenBorder);
                    DeleteButtonSection.x = (Screen.width / 2) - (DeleteButtonStyle.fixedWidth / 2);


                }

                private void RenderNavigationSection()
                {
                    if (GUILayout.Button("Return to wave editing", ReturnButtonStyle))
                    {
                        ClearSpawnPreview();
                        _Context.SwitchView<WaveEditingScene>(_Context._SelectedWave);
                    }
                }

                private void RenderDeleteButtonSection()
                {
                    if (GUILayout.Button("Delete this spawn", DeleteButtonStyle))
                    {
                        DeleteThisSpawn();
                    }
                }

                private void RenderCreepSelectionSection()
                {
                    RenderButtons_CreepTypes();
                }

                private void RenderButtons_CreepTypes()
                {

                    //Increase I with Columns each time
                    for (int i = 0; i < _CreepPrefabs.Length; i += Columns)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            for (int z = 0; z < Columns; z++)
                            {
                                //Prevents from trying to draw another button in an incomplete row if there are none left to be drawn
                                if (i + z >= _CreepPrefabs.Length)
                                {
                                    break;
                                }

                                var c = new GUIContent();
                                var image = GetCreepIcon(_CreepPrefabs[i + z].name);

                                //Checks if icon exists for this creep
                                if (image != null)
                                {
                                    c.tooltip = _CreepPrefabs[i + z].name;
                                    c.image = image;
                                }
                                else
                                {
                                    c.text = _CreepPrefabs[i + z].name;
                                }

                                if (GUILayout.Button(c, CreepButtonStyle))
                                {
                                    SpawnEditorPreviewCreep(_CreepPrefabs[i + z] as GameObject);
                                }
                            }
                        }

                        GUILayout.EndHorizontal();
                    }
                }

                private void SpawnEditorPreviewCreep(GameObject prefab)
                {
                    var go = WaveEditorHelper.BuildBareMeshCopy(prefab);
                    go.name = AddPreviewSuffix(go.name);

                    go.transform.position = new Vector3(SpawnPosition.x, go.transform.position.y, SpawnPosition.z);

                    _PreviewObjects.Add(go);
                    SaveCurrentSpawnData();
                }

                private void SaveCurrentSpawnData()
                {
                    _Context._SelectedSpawn.SpawnData.Clear();

                    foreach (var creep in _PreviewObjects)
                    {
                        if (creep == null)
                        {
                            continue;
                        }

                        var spawntransform = _Context._Spawnpoint.transform;

                        var relativecreeppos = spawntransform.InverseTransformPoint(creep.transform.position);

                        SpawnData data;
                        data.CreepIdentifier = RemovePreviewSuffix(creep.name);
                        data.RelativePosition = relativecreeppos;

                        _Context._SelectedSpawn.SpawnData.Add(data);
                    }
                }

                private void DeleteThisSpawn()
                {
                    ClearSpawnPreview();

                    _Context._SelectedWave.Spawns.Remove(_Context._SelectedSpawn);
                    _Context.SwitchView<WaveEditingScene>(_Context._SelectedWave);
                }

                private void SetupSpawnPreview()
                {
                    if (!_PreviewRendered)
                    {
                        foreach (var datapoint in _Context._SelectedSpawn.SpawnData)
                        {
                            var preview = WaveEditorHelper.BuildBareMeshCopy(_CreepPrefabs.Single(x => x.name == datapoint.CreepIdentifier));
                            preview.name = AddPreviewSuffix(preview.name);

                            var spawntransform = _Context._Spawnpoint.transform;

                            var relativecreeppos = spawntransform.TransformPoint(datapoint.RelativePosition);

                            preview.transform.position = relativecreeppos;

                            _PreviewObjects.Add(preview);
                        }

                        _PreviewRendered = true;
                    }
                }

                private void ClearSpawnPreview()
                {
                    foreach (var go in _PreviewObjects)
                    {
                        GameObject.DestroyImmediate(go);
                    }

                    _PreviewObjects.Clear();

                    _PreviewRendered = false;
                }

                private string AddPreviewSuffix(string orginalname)
                {
                    return string.Concat(orginalname, PreviewObjectSuffix);
                }
                private string RemovePreviewSuffix(string orginalname)
                {
                    if (orginalname.Contains(PreviewObjectSuffix))
                    {
                        return orginalname.Split(PreviewObjectSuffix[0])[0];
                    }

                    return orginalname;
                }
            }
        }
    }
}
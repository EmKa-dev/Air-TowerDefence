using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class WaveEditor
{
    public const string _SpawnObjectTag = "Spawnpoint";

    [MenuItem("Editor/Wave editor")]
    public static void BeginWaveEditing()
    {
        //Save current scene
        EditorSceneManager.SaveOpenScenes();

        GameObject originalspawnobject = GameObject.FindGameObjectWithTag(_SpawnObjectTag);

        if (originalspawnobject == null)
        {
            Debug.Log("Couldn't find any object tagged 'Spawnpoint'");
            return;
        }

        WavesManager originalwavesmanager = GetWavesManager();

        if (originalwavesmanager == null)
        {
            Debug.Log($"Must have one component <{nameof(WavesManager)}> attached to active gameobject");
            return;
        }

        //Pre-setup for wave editor scene.
        GameObject SpawnObjectCopy = WaveEditorHelper.BuildUnmovableMeshCopy(originalspawnobject, "Spawn", originalspawnobject.tag);

        SpawnObjectCopy.transform.position = originalspawnobject.transform.position;

        HideAllObjectsExcept(SpawnObjectCopy, originalwavesmanager.gameObject);
        Selection.activeGameObject = SpawnObjectCopy;
        SceneView.lastActiveSceneView.FrameSelected();

        var editorgui = new WaveEditorSceneContext(originalwavesmanager.WavesContainer, () =>
        {
            RestoreOriginalScene();
            GameObject.DestroyImmediate(SpawnObjectCopy);
        });
    }

    private static void HideAllObjectsExcept(params GameObject[] gameobjects)
    {
        GameObject[] allobjects = GameObject.FindObjectsOfType(typeof(GameObject)).Cast<GameObject>().ToArray();

        foreach (var go in allobjects)
        {
            go.SetActive(false);
        }

        foreach (var gameobj in gameobjects)
        {
            gameobj.SetActive(true);
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

    private static WavesManager GetWavesManager()
    {
        return GameObject.FindObjectOfType<WavesManager>();
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

        private Wave _SelectedWave;
        private Spawn _SelectedSpawn;

        public WaveEditorSceneContext(WavesContainer wavescontainer, Action exiteditingaction)
        {
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

                _Context._WavesContainer.AddWave(wave);
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

            public SpawnEditingScene(WaveEditorSceneContext context)
            {
                _CreepPrefabs = Resources.LoadAll("CreepPrefabs").Cast<GameObject>().ToArray();

                _Context = context;

                ReturnButtonStyle = _Context._EditorGUISkin.GetStyle("ReturnButton");
                CreepButtonStyle = _Context._EditorGUISkin.GetStyle("CreepButton");
                SaveButtonStyle = _Context._EditorGUISkin.GetStyle("SaveButton");
                DeleteButtonStyle = _Context._EditorGUISkin.GetStyle("DeleteButton");
            }

            private Texture GetCreepIcon(string name)
            {
                return Resources.Load<Texture>($"CreepIcons/{name}");
            }

            public void RenderSceneGUI()
            {
                UpdateSectionArea();

                if (!_PreviewRendered)
                {
                    SetupSpawnPreview();
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

                GUILayout.BeginArea(SaveButtonSection);
                {
                    RenderSaveButtonSection();
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

            private void RenderSaveButtonSection()
            {
                if (GUILayout.Button("Save current spawn", SaveButtonStyle))
                {
                    SaveCurrentSpawnData();
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
                                var go = PrefabUtility.InstantiatePrefab(_CreepPrefabs[i + z]) as GameObject;
                                go.transform.position = GameObject.FindGameObjectWithTag(WaveEditor._SpawnObjectTag).transform.position;
                                PrefabUtility.UnpackPrefabInstance(go, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                            }
                        }
                    }

                    GUILayout.EndHorizontal();
                }
            }

            private void SaveCurrentSpawnData()
            {
                _Context._SelectedSpawn.SpawnData.Clear();

                //TODO so something about this tag search
                var gos = GameObject.FindGameObjectsWithTag("TCreep");
                foreach (var creep in gos)
                {
                    var spawntransform = GameObject.FindGameObjectWithTag(WaveEditor._SpawnObjectTag).transform;

                    var relativecreeppos = spawntransform.InverseTransformPoint(creep.transform.position);
                    _Context._SelectedSpawn.SpawnData.Add(new SpawnData() { CreepIdentifier = creep.name, RelativePosition = relativecreeppos });
                }
            }

            private void DeleteThisSpawn()
            {
                _Context._SelectedWave.Spawns.Remove(_Context._SelectedSpawn);
                ClearSpawnPreview();
                _Context.SwitchView<WaveEditingScene>(_Context._SelectedWave);

            }
            private void SetupSpawnPreview()
            {
                if (_Context._SelectedSpawn != null && !_PreviewRendered)
                {
                    foreach (var spawn in _Context._SelectedSpawn.SpawnData)
                    {

                        var creep = PrefabUtility.InstantiatePrefab(Resources.Load($"CreepPrefabs/{spawn.CreepIdentifier}")) as GameObject;
                        PrefabUtility.UnpackPrefabInstance(creep, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

                        var spawntransform = GameObject.FindGameObjectWithTag(WaveEditor._SpawnObjectTag).transform;

                        var relativecreeppos = spawntransform.TransformPoint(spawn.RelativePosition);
                        creep.transform.position = relativecreeppos;
                    }

                    _PreviewRendered = true;
                }
            }

            private void ClearSpawnPreview()
            {

                var gos = GameObject.FindGameObjectsWithTag("TCreep");

                //Remove the gos used to create spawndata
                foreach (var go in gos)
                {
                    GameObject.DestroyImmediate(go);
                }

                _PreviewRendered = false;
            }
        }
    }
}
/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using UnityEngine;
using UnityEditor;
using Opsive.UltimateCharacterController.Editor.Inspectors.Utility;

namespace Opsive.UltimateCharacterController.Editor.Managers
{
    /// <summary>
    /// Draws a list of all of the available integrations.
    /// </summary>
    [ManagerMenuItem("Integrations", 10)]
    public class IntegrationsManager : Manager
    {
        private const int c_IntegrationCellWidth = 300;
        private const int c_IntegrationCellHeight = 100;
        private const int c_IntegrationCellSpacing = 5;

        /// <summary>
        /// Stores the information about the integration asset.
        /// </summary>
        private class AssetIntegration
        {
            private const int c_IconSize = 78;

            private GUIStyle m_IntegrationAssetTitle;
            private GUIStyle IntegrationAssetTitle
            {
                get
                {
                    if (m_IntegrationAssetTitle == null) {
                        m_IntegrationAssetTitle = new GUIStyle(InspectorUtility.CenterBoldLabel);
                        m_IntegrationAssetTitle.fontSize = 14;
                        m_IntegrationAssetTitle.alignment = TextAnchor.MiddleLeft;
                    }
                    return m_IntegrationAssetTitle;
                }
            }

            private int m_ID;
            private string m_Name;
            private string m_IntegrationURL;
            private Texture2D m_Icon;

            private WWW m_IconRequest;

            /// <summary>
            /// Constructor for the AssetIntegration class.
            /// </summary>
            public AssetIntegration(int id, string name, string iconURL, string integrationURL)
            {
                m_ID = id;
                m_Name = name;
                m_IntegrationURL = integrationURL;

                // Start loading the icon as soon as the url is retrieved.
                m_IconRequest = new WWW(iconURL);
            }

            /// <summary>
            /// Draws the integration details at the specified position.
            /// </summary>
            public void DrawIntegration(Vector2 position)
            {
                // Show the integration details. Keep loading the icon until it is done.
                if (m_Icon == null) {
                    if (m_IconRequest != null && m_IconRequest.isDone) {
                        if (string.IsNullOrEmpty(m_IconRequest.error)) {
                            m_Icon = m_IconRequest.texture;
                        } else {
                            m_IconRequest = null;
                        }
                    }
                } else {
                    // Draw the icon, name, and integration/Asset Store link.
                    GUI.DrawTexture(new Rect(position.x, position.y, c_IconSize, c_IconSize), m_Icon);

                    var rect = new Rect(position.x + c_IconSize + 10, position.y + 3, 200, 18);
                    EditorGUI.LabelField(rect, m_Name, IntegrationAssetTitle);

                    if (GUI.Button(new Rect(rect.x, rect.y + 23, 80, 18), "Integration")) {
                        Application.OpenURL(m_IntegrationURL);
                    }

                    if (GUI.Button(new Rect(rect.x, rect.y + 47, 80, 18), "Asset Store")) {
                        Application.OpenURL("https://opsive.com/asset/UltimateCharacterController/IntegrationRedirect.php?asset=" + m_ID);
                    }
                }
            }
        }

        private Vector2 m_ScrollPosition;
        private WWW m_IntegrationsReqest;
        private AssetIntegration[] m_Integrations;

        /// <summary>
        /// Draws the Manager.
        /// </summary>
        public override void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Integrations can also be found on the");
            GUILayout.Space(-3);
            if (GUILayout.Button("integrations page.", InspectorUtility.LinkStyle, GUILayout.Width(106))) {
                Application.OpenURL(" https://opsive.com/integrations/?pid=923");
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            // Draw the integrations once they are loaded.
            if (m_Integrations != null && m_Integrations.Length > 0) {
                var lastRect = GUILayoutUtility.GetLastRect();
                // Multiple integrations can be drawn on a single row depending on the width of the window.
                var cellsPerRow = (int)(Screen.width - m_MainManagerWindow.MenuWidth - 2) / (c_IntegrationCellWidth + c_IntegrationCellSpacing);
                m_ScrollPosition = GUI.BeginScrollView(new Rect(0, lastRect.y, Screen.width - m_MainManagerWindow.MenuWidth - 2, Screen.height - 64), m_ScrollPosition, 
                                            new Rect(0, 0, Screen.width - m_MainManagerWindow.MenuWidth - 25, 
                                                    (m_Integrations.Length / cellsPerRow) * (c_IntegrationCellHeight + c_IntegrationCellSpacing)));
                var position = new Vector2(0, 20);
                // Draw each integration.
                for (int i = 0; i < m_Integrations.Length; ++i) {
                    position.x = (i % cellsPerRow) * c_IntegrationCellWidth;
                    m_Integrations[i].DrawIntegration(position + (new Vector2(0, c_IntegrationCellHeight + c_IntegrationCellSpacing) * (i / cellsPerRow)));
                }
                GUI.EndScrollView();
            } else {
                if (m_IntegrationsReqest == null) {
                    if (Event.current.type == EventType.Repaint) {
                        m_IntegrationsReqest = new WWW("https://opsive.com/asset/UltimateCharacterController/IntegrationsList.txt");
                    }
                } else {
                    // Load all of the integrations as soon as the request is complete.
                    if (m_IntegrationsReqest.isDone) {
                        if (string.IsNullOrEmpty(m_IntegrationsReqest.error)) {
                            var splitIntegrations = m_IntegrationsReqest.text.Split('\n');
                            m_Integrations = new AssetIntegration[splitIntegrations.Length];
                            var count = 0;
                            for (int i = 0; i < splitIntegrations.Length; ++i) {
                                if (string.IsNullOrEmpty(splitIntegrations[i])) {
                                    continue;
                                }

                                // The data must contain info on the integration name, id, icon, and integraiton url.
                                var integrationData = splitIntegrations[i].Split(',');
                                if (integrationData.Length < 4) {
                                    continue;
                                }

                                m_Integrations[count] = new AssetIntegration(int.Parse(integrationData[0].Trim()), integrationData[1].Trim(), integrationData[2].Trim(), integrationData[3].Trim());
                                count++;
                            }

                            if (count != m_Integrations.Length) {
                                System.Array.Resize(ref m_Integrations, count);
                            }
                        } else {
                            EditorGUILayout.LabelField("Error: Unable to retrieve integrations.");
                        }
                    } else {
                        EditorGUILayout.LabelField("Loading most recent list of integrations...");
                    }
                }
            }
        }
    }
}
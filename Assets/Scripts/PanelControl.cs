using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelControl : MonoBehaviour
{
    
    public static PanelControl PanelInstanic;
    public PanelCollection panelCollection;
    private void Awake()
    {
        PanelInstanic = this;
    }
    /// <summary>
    /// Change the panel and closed others.
    /// </summary>
    /// <param name="panelGameObject">Which panel you want to close.Must be <see cref="PanelControl"/> 's GameObject</param>
    public void ChangePanel(GameObject panelGameObject,bool isShowMasterUI=true)
    {
        panelCollection.CommandPanel.SetActive(false);
        panelCollection.TilemapSpawnPanel.SetActive(false);
        panelCollection.UpdatePanel.SetActive(false);
        panelCollection.NetworkPanel.SetActive(false);
        panelCollection.MasterPanel.SetActive(isShowMasterUI);
        panelGameObject.SetActive(true);
    }
    public void ChangePanelWithoutClosedMaster(GameObject panelGameObject) => ChangePanel(panelGameObject, true);
    public void ChangePanelClosedMaster(GameObject panelGameObject) => ChangePanel(panelGameObject, false);
    public void ClosePanel(GameObject panelGameObject) => panelGameObject.SetActive(false);

    [System.Serializable]
    public struct PanelCollection
    {
        /// <summary>
        /// Main UI panel
        /// </summary>
        public GameObject MasterPanel;
        /// <summary>
        /// <seealso cref="NetworkControl"/>,<seealso cref="NetworkRxDiscover"/>
        /// </summary>
        public GameObject NetworkPanel;
        /// <summary>
        /// <seealso cref="CheckUpdate"/>
        /// </summary>
        public GameObject UpdatePanel;
        /// <summary>
        /// <seealso cref="TilemapSpawn"/>
        /// </summary>
        public GameObject TilemapSpawnPanel;
        /// <summary>
        /// <seealso cref="CommandSystem"/>
        /// </summary>
        public GameObject CommandPanel;
    }
}

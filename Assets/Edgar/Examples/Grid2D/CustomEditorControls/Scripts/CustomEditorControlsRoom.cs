using System.Collections.Generic;
using UnityEngine;

namespace Edgar.Unity.Examples.CustomEditorControls
{
    #region codeBlock:2d_customEditorControls_customRoom

    /// <summary>
    /// Simple custom room to demonstrate custom editor controls.
    /// </summary>
    public class CustomEditorControlsRoom : RoomBase
    {
        /// <summary>
        /// Each room has a type that determines the display name and icon.
        /// </summary>
        public enum RoomType
        {
            Basic,
            Exit,
            Shop,
            Boss,
            Spawn
        }

        /// <summary>
        /// Type of the room.
        /// </summary>
        public RoomType Type = RoomType.Basic;

        public override List<GameObject> GetRoomTemplates()
        {
            // Not implemented in this example
            throw new System.NotImplementedException();
        }

        public override string GetDisplayName()
        {
            return Type.ToString();
        }

        #region hide

        #region codeBlock:2d_customEditorControls_roomControl

        /// <summary>
        /// Simple example of a custom editor control for the CustomEditorControlsRoom room type.
        /// </summary>
        [CustomRoomControl(typeof(CustomEditorControlsRoom))]
        private class Control : RoomControl
        {
        
            static Control()
            {
            
            }
           
        }

        #endregion

        #endregion
    }

    #endregion
}
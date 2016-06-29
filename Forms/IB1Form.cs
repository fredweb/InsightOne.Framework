//   insightOne.SB1.Framework.Framework
// ****************************************************************************
// 
//   Copyright 2015 (c) Provider
//   Autor:     George Santos
// 
// ****************************************************************************

using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace insightOne.SB1.Framework.Forms
{
    public interface IB1Form : IDisposable
    {
        string FormID { get; }

        string FormType { get; }

        bool IsInitialized { get; }

        bool IsSystemForm { get; }

        bool IsModal { get; }

        string ResourceName { get; }

        #region API Events...

        void OnItemEvent(string FormUID, ref ItemEvent pVal, out bool BubbleEvent);

        void OnFormDataEvent(ref BusinessObjectInfo BusinessObjectInfo, out bool BubbleEvent);

        void OnMenuEvent(ref MenuEvent pVal, out bool BubbleEvent);

        void OnRightClickEvent(ref ContextMenuInfo eventInfo, out bool BubbleEvent);

        #endregion

        void Load(Form sboForm);

        void Select();

        void Show();
    }
}

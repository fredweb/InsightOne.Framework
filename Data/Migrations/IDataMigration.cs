﻿//   insightOne.SB1.Framework.Framework
// ****************************************************************************
// 
//   Copyright 2015 (c) Provider
//   Autor:     George Santos
// 
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace insightOne.SB1.Framework.Data.Migrations
{
    public interface IDataMigration
    {
        void CreateOrUpgrade();
    }
}
